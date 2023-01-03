using AutoMapper;
using Basket.API.Entities;
using EventBus.Messages.Events;
using StackExchange.Redis;

namespace Basket.API.Mapper
{
    public class BasketCheckoutProfile : Profile
        {
            //map domain object to application objects
            public BasketCheckoutProfile()
            {
                CreateMap<BasketCheckout, BaseCheckoutEvent>().ReverseMap();
            }
        }
}