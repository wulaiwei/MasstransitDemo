using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransitShopCart.Models;
using MassTransitShopCart.Models.Input;
using MassTransitShopCart.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkData.Abp.Caching;

namespace MassTransitShopCart.Controllers
{
    /// <summary>
    /// MainController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IDistributedCache<ProductStockCache> _distributedCache;
        private readonly ShopConext _shopConext;
        private readonly IStackExchangeRedisHelper _stackExchangeRedisHelper;

        /// <summary>
        /// MainController
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="distributedCache"></param>
        /// <param name="shopCartConext"></param>
        /// <param name="stackExchangeRedisHelper"></param>
        public MainController(ILogger<MainController> logger, IDistributedCache<ProductStockCache> distributedCache, ShopConext shopCartConext, IStackExchangeRedisHelper stackExchangeRedisHelper)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _shopConext = shopCartConext;
            _stackExchangeRedisHelper = stackExchangeRedisHelper;
        }

        /// <summary>
        /// AddOrder
        /// </summary>
        /// <param name="addOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addOrder")]
        public bool AddOrder(AddOrder addOrder)
        {
            var database = _stackExchangeRedisHelper.GetDatabase();
            var key = $"ShoppingCartLockAdd_{addOrder.ProductId}";
            if (database.LockTake(key, key, TimeSpan.FromSeconds(10)))
            {
                var result = true;
                try
                {
                    var order = new Order
                    {
                        OrderAmount = addOrder.Amount,
                        ProductId = addOrder.ProductId,
                        UserName = addOrder.UserName
                    };
                    _shopConext.Orders.Add(order);
                    _shopConext.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = false;
                    _logger.LogError($"{ex.Message}");
                }
                finally
                {
                    database.LockRelease(key, key);
                }
                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
