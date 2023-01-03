using Basket.API.Entities;
using Basket.API.GrpcService;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _discountGrpcService = discountGrpcService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _basketRepository.GetBasket(userName);
            //if the user is trying to get the basket for the first time 
            //we create a new shopping cart for the user else we return the
            //existing shopping cart back to the user.
            if (basket == null)
            {
                return Ok(new ShoppingCart(userName));
            }
            return Ok(basket);

        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {

            foreach (var item in shoppingCart.Items)
            {
                //      1. Communicate with DiscoutnGrpc
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                //      2. Then calculate latest prices of products into the shopping cart
                item.Price -= coupon.Amount;
            }

            var basket = await _basketRepository.UpdateBasket(shoppingCart);
            return Ok(basket);

        }

        //[HttpDelete("{userName}", Name = "DeleteBasket")]
        [Route("[action]/{userName}", Name = "DeleteBasket")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _basketRepository.DeleteBasket(userName);
            return Ok();

        }

        [Route("[action]", Name = "Checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout(BasketCheckout basketCheckout)
        {
            //get the basket for the given customer 
            var basket = await _basketRepository.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();
            
            //Create an event to send to the queue
            //1. map the basket checkout to an event
            var eventMessage = _mapper.Map<BaseCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            
            //2. publish the event to rabbit mq using mass transit
            await _publishEndpoint.Publish(eventMessage);
            
            //remove the basket 
            await _basketRepository.DeleteBasket(basketCheckout.UserName);
            return Accepted();


        }

    }
}
