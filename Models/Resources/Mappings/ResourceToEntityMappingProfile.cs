using AutoMapper;
using rgnl_server.Models.Entities;
using rgnl_server.Models.Resources;
using Profile = AutoMapper.Profile;

namespace rgnl_server.Models.Mapping
{
    public class ResourceToEntityMappingProfile : Profile
    {
        public ResourceToEntityMappingProfile()
        {
            CreateMap<RegistrationResource, AppUser>()
                .ForMember(appUser => appUser.UserName, map => map.MapFrom(resource => resource.Email));
        }
    }
}