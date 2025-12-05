using FluentValidation;
using GastosPersonales.Application.DTOs.User;

namespace GastosPersonales.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Nombre requerido");
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().WithMessage("Correo invÃ¡lido");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("ContraseÃ±a mÃ­nima 6 caracteres");
        }
    }
}
