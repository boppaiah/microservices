using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : MediatR.IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();

            }
            catch(System.Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError($"An exception {ex} has occured for the request {requestName} on {request}");
                throw;
            }
        }
    }
}
