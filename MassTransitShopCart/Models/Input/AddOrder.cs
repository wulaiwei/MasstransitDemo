using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitShopCart.Models.Input
{
    public class AddOrder
    {
        public int ProductId { get; set; }

        public int Amount { get; set; }

        public string UserName { get; set; }
    }
}
