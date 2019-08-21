using FluentValidation;

namespace rgnl_server.Models.Resources.Validations
{
    public class RegistrationResourceValidator : AbstractValidator<RegistrationResource>
    {
        public RegistrationResourceValidator()
        {
            RuleFor(resource => resource.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(resource => resource.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(resource => resource.FirstName).NotEmpty().WithMessage("FirstName cannot be empty");
            RuleFor(resource => resource.LastName).NotEmpty().WithMessage("LastName cannot be empty");
        }
    }
}