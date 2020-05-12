using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WorkDataMassTransitSagaPersistence.Saga.Order.Event;

namespace WorkDataMassTransitSagaPersistence.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<SubmitOrder> _client;
        private static  Guid Key=new Guid("4FEEA13D-F373-47A9-BAB7-916A2D1C3FD1");

        public HomeController(IPublishEndpoint publishEndpoint, IRequestClient<SubmitOrder> client)
        {
            _publishEndpoint = publishEndpoint;
            _client = client;
        }

        [HttpGet]
        [Route("/send/submit")]
        public async Task<IActionResult> Send()
        {
            var data= await _client.GetResponse<SaveOrderResponse>(new SubmitOrder(Key)
            {
                OrderId = "123"
            });

            return Ok(data.Message.CompleteTime);
        }
        
        [HttpGet]
        [Route("/send/pay")]
        public async Task<IActionResult> Send(string id)
        {
            await _publishEndpoint.Publish(new PayOrder(Key)
            {
                OrderId = "123"
            });
            return Ok(true);
        }
    }
}