using FluentValidation;

namespace LinkShortener.Auth.Models;

public class UserAuthDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDtoValidator : AbstractValidator<UserAuthDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Почта имеет неверный формат");

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithMessage("Пароль не может быть короче шести символов");
    }
}