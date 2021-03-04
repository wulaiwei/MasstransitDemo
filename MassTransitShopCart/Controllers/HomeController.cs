using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransitShopCart.Models;
using WorkData.Abp.Caching;
using MassTransitShopCart.Models.ViewModel;
using MassTransitShopCart.Models.Input;

namespace MassTransitShopCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache<ProductStockCache> _distributedCache;
        private readonly ShopConext _shopConext;
        private readonly IStackExchangeRedisHelper _stackExchangeRedisHelper;

        public HomeController(ILogger<HomeController> logger, IDistributedCache<ProductStockCache> distributedCache, ShopConext shopCartConext, IStackExchangeRedisHelper stackExchangeRedisHelper)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _shopConext = shopCartConext;
            _stackExchangeRedisHelper = stackExchangeRedisHelper;
        }

        public IActionResult Index()
        {
            var data = _shopConext.Products.ToList();
            return View(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
