using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using MediatR;
using Task3.Application.Common.Interfaces.MediatR;

namespace Task3.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> :
    IResultPipelineBehavior<TRequest, TResponse>
        where TRequest : IResultRequest<TResponse>
        where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResponse>> Handle(TRequest request,
        CancellationToken ct,
        RequestHandlerDelegate<Result<TResponse>> next)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var validationResults = await ValidateRequestAsync(request,
            _validators, ct);

        var failures = GetValidationFailures(validationResults);

        if (failures.Any())
        {
            var ex = new ValidationException(failures);
            var response = new Result<TResponse>(ex);

            return response;
        }

        return await next();
    }

    private async Task<ValidationResult[]> ValidateRequestAsync(
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators,
        CancellationToken ct = default)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, ct)));

        return validationResults;
    }

    private ValidationFailure[] GetValidationFailures(ValidationResult[] results)
    {
        return results
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToArray();
    }
}
