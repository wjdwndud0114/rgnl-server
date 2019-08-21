using rgnl_server.Models.Resources.Validations;
using FluentValidation.Attributes;

namespace rgnl_server.Models.Resources
{
    [Validator(typeof(CredentialsResourceValidator))]
    public class CredentialsResource
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}