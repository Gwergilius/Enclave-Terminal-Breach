using System.Runtime.CompilerServices;

namespace Enclave.Echelon.Core.Extensions;

/// <summary>
/// Extension methods for FluentValidation validators.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates the instance and throws an exception if validation fails.
    /// </summary>
    /// <typeparam name="T">The type of the instance to validate.</typeparam>
    /// <param name="validator">The validator to use.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated (captured from the call site when omitted).</param>
    /// <exception cref="ArgumentNullException">Thrown when validator or instance is null.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    /// <example>
    /// <code>
    /// var validator = new PasswordValidator();
    /// validator.ValidateAndThrowArgumentException(word);
    /// </code>
    /// </example>
    public static void ValidateAndThrowArgumentException<T>(
        this IValidator<T> validator,
        T instance,
        [CallerArgumentExpression(nameof(instance))] string parameterName = "value")
    {
        ArgumentNullException.ThrowIfNull(validator);
        if (instance is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        var result = validator.Validate(instance);

        if (result.IsValid)
        {
            return;
        }

        var firstError = result.Errors[0];

        // Determine which exception to throw based on the error
        if (firstError.ErrorMessage.Contains("null", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentNullException(parameterName, firstError.ErrorMessage);
        }

        throw new ArgumentException(firstError.ErrorMessage, parameterName);
    }
}
