
using FitnessProfileService.Common.Responses;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FitnessProfileService.Features.Common;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var validators = _validators.ToList();

        if (validators.Count == 0)
            return await next();

        var failures = new List<ValidationFailure>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(
                new ValidationContext<TRequest>(request),
                cancellationToken);

            failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
        {
            var errors = failures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(x => x.ErrorMessage).ToArray());

            return RequestResponseFactory
                .ValidationFailure<TResponse>(errors);
        }

        return await next();
    }
}
