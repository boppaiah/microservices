using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exception;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepositiry;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepositiry,
            IMapper mapper,
            IEmailService emailService,
            ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderRepositiry = orderRepositiry;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Order> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepositiry.GetByIdAsync(request.Id);
            if(orderToDelete == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            var orderEntity = _mapper.Map<Order>(orderToDelete);
            var updatedOrder = await _orderRepositiry.UpdateAsync(orderEntity);

            _logger.LogInformation($"Order updated succesfully for OrderID: {updatedOrder.Id}");

            return updatedOrder;
        }
    }
}
