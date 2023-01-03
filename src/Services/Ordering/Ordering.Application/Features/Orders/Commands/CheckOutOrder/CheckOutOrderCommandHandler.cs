using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckOutOrder
{
    public class CheckOutOrderCommandHandler : IRequestHandler<CheckOutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepositiry;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckOutOrderCommandHandler> _logger;

        public CheckOutOrderCommandHandler(IOrderRepository orderRepositiry,
            IMapper mapper, IEmailService emailService,
            ILogger<CheckOutOrderCommandHandler> logger)
        {
            _orderRepositiry = orderRepositiry;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> Handle(CheckOutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepositiry.AddAsync(orderEntity);
            _logger.LogInformation($"Order succesfully created. OrderID: {newOrder.Id}");

            await SendEmail(newOrder);

            return newOrder.Id;

        }

        private async Task SendEmail(Order order)
        {
            var email = new Email()
            {
                Body = "New order",
                Subject = "New order placed!",
                To = "boppaiah38@yahoo.in"

            };
            try
            {
                await _emailService.SendEmail(email);
            }
            catch(System.Exception ex)
            {
                _logger.LogError($"Unable to send email to {email.To} for orderId: {order.Id} due to {ex}");
            }
        }
    }
}

