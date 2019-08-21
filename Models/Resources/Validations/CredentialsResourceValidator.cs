using FluentValidation;

namespace rgnl_server.Models.Resources.Validations
{
    public class CredentialsResourceValidator : AbstractValidator<CredentialsResource>
    {
        public CredentialsResourceValidator()
        {
            RuleFor(resource => resource.UserName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(resource => resource.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(resource => resource.Password).Length(8, 32).WithMessage("Password must be between 8 and 32 characters");
        }
    }
}