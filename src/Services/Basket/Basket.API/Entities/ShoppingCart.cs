﻿using System.Collections.Generic;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public ShoppingCart(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public decimal TotalPrice
        {
            get
            {
                decimal total = 0;
                foreach (ShoppingCartItem item in Items)
                {
                    total = (item.Price * item.Quantity) + total;
                }
                return total;
            }
        }
    }
}
