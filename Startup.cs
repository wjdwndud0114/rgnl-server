using AutoMapper;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rgnl_server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Serialization;
using rgnl_server.Auth;
using rgnl_server.Extensions;
using rgnl_server.Helpers;
using rgnl_server.Hubs;
using rgnl_server.Interfaces.Repositories;
using rgnl_server.Models;
using rgnl_server.Models.Entities;
using rgnl_server.Repositories;

namespace rgnl_server
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA323sqsfhqGbMRdRj1PVkH";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        private IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), optionsBuilder => optionsBuilder.MigrationsAssembly("rgnl-server")));

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register ConfigurationBuilder instance of FacebookAuthSettings
            services.Configure<FacebookAuthSettings>(Configuration.GetSection(nameof(FacebookAuthSettings)));

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Jwt wire up, get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Strings.Roles.Consumer, policy =>
                    policy.RequireClaim(ClaimTypes.Role, Constants.Strings.Roles.Consumer));
                options.AddPolicy(Constants.Strings.Roles.Producer, policy =>
                    policy.RequireClaim(ClaimTypes.Role, Constants.Strings.Roles.Producer));
                options.AddPolicy(Constants.Strings.Roles.Admin, policy =>
                    policy.RequireClaim(ClaimTypes.Role, Constants.Strings.Roles.Admin));
            });

            var identityBuilder = services.AddIdentityCore<AppUser>(options =>
            {
                // configure identity options
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            });
            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole<int>), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "Allow All",
                    policyBuilder => policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials()
                    );
            });

            services.AddOData();
            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<AppUser>(nameof(AppUser));

            return builder.GetEdmModel();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler(
                builder => {
                    builder.Run( async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        });
                });

            app.UseSignalR(options => { options.MapHub<PostHub>("/hub"); });

            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(
                builder =>
                {
                    builder.MapODataServiceRoute("odata", "odata", GetEdmModel());
                    builder.Expand().Select().OrderBy().Filter().MaxTop(null);
                });

            CreateRoles(services).Wait();
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            if (!await roleManager.RoleExistsAsync("admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (!await roleManager.RoleExistsAsync("producer"))
            {
                await roleManager.CreateAsync(new IdentityRole("producer"));
            }
            if (!await roleManager.RoleExistsAsync("consumer"))
            {
                await roleManager.CreateAsync(new IdentityRole("consumer"));
            }

            var adminUser = await userManager.FindByNameAsync("admin@admin.com");
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    FirstName = "Kevin",
                    LastName = "Jeong",
                    Email = "admin@admin.com",
                };
                await userManager.CreateAsync(adminUser, "Test1234");
                await userManager.AddToRoleAsync(adminUser, Constants.Strings.Roles.Admin);
            }

            var govUser = await userManager.FindByNameAsync("gov@gov.com");
            if (govUser == null)
            {
                govUser = new AppUser
                {
                    FirstName = "Barry",
                    LastName = "Obama",
                    Email = "gov@gov.com"
                };
                await userManager.CreateAsync(govUser, "Test1234");
                await userManager.AddToRoleAsync(govUser, Constants.Strings.Roles.Producer);
            }

            var normalUser = await userManager.FindByNameAsync("gov@gov.com");
            if (normalUser == null)
            {
                normalUser = new AppUser
                {
                    FirstName = "Ryan",
                    LastName = "Smith",
                    Email = "user@user.com"
                };
                await userManager.CreateAsync(normalUser, "Test1234");
                await userManager.AddToRoleAsync(normalUser, Constants.Strings.Roles.Consumer);
            }
        }
    }
}
