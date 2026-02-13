using FluentValidation;

namespace Enclave.Echelon.Core.Validators;

/// <summary>
/// Validator for password word input.
/// </summary>
public class PasswordValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordValidator"/> class.
    /// </summary>
    public PasswordValidator()
    {
        RuleFor(word => word)
            .NotNull()
            .WithMessage("Word cannot be null.")
            .NotEmpty()
            .WithMessage("Word cannot be empty.")
            .Must(word => !string.IsNullOrWhiteSpace(word))
            .WithMessage("Word cannot be whitespace only.")
            .Matches("^[a-zA-Z]+$")
            .WithMessage("Word must contain only letters.");
    }
}



