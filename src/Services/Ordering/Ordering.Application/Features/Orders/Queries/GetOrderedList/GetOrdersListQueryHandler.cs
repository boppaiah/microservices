using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Features.Orders.Queries.GetOrderedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Queries
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrdersVm>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        /*
         * Use case : Get the orders related to the customer
         */
        public async Task<List<OrdersVm>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var orderedList = await _orderRepository.GetOrdersByName(request.UserName);

            //map it back to overdVm
            return _mapper.Map<List<OrdersVm>>(orderedList);

        }
    }
}
