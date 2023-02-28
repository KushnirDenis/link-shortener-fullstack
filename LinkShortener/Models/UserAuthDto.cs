using FluentValidation;

namespace LinkShortener.Models;

public class UserAuthDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserAuthDtoValidator : AbstractValidator<UserAuthDto>
{
    public UserAuthDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Почта имеет неверный формат");

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithMessage("Пароль не может быть короче шести символов");
    }
}