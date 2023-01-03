using MediatR;

namespace Ordering.Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommand : IRequest
    {
        public int Id { get; set; }

    }
}