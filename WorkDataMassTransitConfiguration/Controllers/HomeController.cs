using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WorkDataMassTransitConfiguration.Consumers;

namespace WorkDataMassTransitConfiguration.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<HomeController> _logger;
        private readonly IRequestClient<CheckOrderStatus> _client;

        public HomeController(ILogger<HomeController> logger, IPublishEndpoint publishEndpoint,
            IRequestClient<CheckOrderStatus> client)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _client = client;
        }

        [HttpGet]
        [Route("/home/get")]
        public async Task<IActionResult> Get()
        {
//            await _publishEndpoint.Publish(new InsertOrder
//            {
//                ProductName = $"测试商品更新{Guid.NewGuid()}"
//            });
            var s = await _client.GetResponse<CheckOrderStatusResponse>(new CheckOrderStatus
            {
                ProductName = "c测试小"
            });
//            await _publishEndpoint.Publish(new UpdateOrder
//            {
//                ProductName = $"测试商品更新{Guid.NewGuid()}"
//            });
            return Ok(s.Message);
        }
    }
}