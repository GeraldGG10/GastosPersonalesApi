using FluentValidation;
using GastosPersonales.Application.DTOs.User;

namespace GastosPersonales.Application.Validators
{
    // Validador para la solicitud de registro de usuario usando FluentValidation 
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Nombre requerido");
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().WithMessage("Correo invalido");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Contraseña minima 6 caracteres");
        }
    }
}
