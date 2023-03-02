using FluentValidation;
using LinkShortener.Localization;
using Microsoft.Extensions.Localization;

namespace LinkShortener.Models;

public class UserAuthDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserAuthDtoValidator : AbstractValidator<UserAuthDto>
{
    public UserAuthDtoValidator(IStringLocalizer<ErrorMessages> localizer)
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(localizer["WrongMailFormat"]);

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithMessage(localizer["ShortPassword"]);
    }
}