using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exception;
using Ordering.Application.Features.Orders.Commands.Delete;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepositiry;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepository orderRepositiry,
            IMapper mapper,
            IEmailService emailService,
            ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepositiry = orderRepositiry;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepositiry.GetByIdAsync(request.Id);
            if(orderToDelete == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            var orderEntity = _mapper.Map<Order>(orderToDelete);
            await _orderRepositiry.DeleteAsync(orderEntity);

            _logger.LogInformation($"Order deleted succesfully for OrderID: {orderEntity.Id}");

            return Unit.Value;
        }

    }
}
