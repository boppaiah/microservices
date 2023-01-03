/*
 * collect all the validation and run the validate method
 * if there is an validation error then throw the validation exception
 */
using FluentValidation;
using MediatR;
using ValidationException = Ordering.Application.Exception.ValidationException;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : MediatR.IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validationRequest;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validationRequest)
        {
            _validationRequest = validationRequest;
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if(_validationRequest.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                //choose all the validators coming in from the request and run the validate method one by one
                var validationResult = await Task.WhenAll(_validationRequest.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResult
                                .SelectMany(r => r.Errors)
                                .Where(o => o != null)
                                .ToList();
                if(failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
