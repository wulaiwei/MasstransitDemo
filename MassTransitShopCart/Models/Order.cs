using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitShopCart.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int OrderAmount { get; set; }

        public string UserName { get; set; }
    }
}
