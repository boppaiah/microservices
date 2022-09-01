using Basket.API.Entities;
using Basket.API.GrpcService;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly DiscountGrpcService _discountGrpcService;
        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _discountGrpcService = discountGrpcService;
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

    }
}
