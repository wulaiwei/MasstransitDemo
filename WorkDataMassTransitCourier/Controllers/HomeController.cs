using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Scheduling;
using WorkDataMassTransitCourier.StateMachine.Contract;

namespace WorkDataMassTransitCourier.Controllers
{
    public class ScheduleNotification
    {
        public DateTime DeliveryTime { get; }
        public string EmailAddress { get; }
        public string Body { get; }
    }

    /// <summary>
    /// HomeController
    /// </summary>
    public class HomeController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<SubmitOrder> _requestClient;

        public HomeController(IPublishEndpoint publishEndpoint, IRequestClient<SubmitOrder> requestClient)
        {
            _publishEndpoint = publishEndpoint;
            _requestClient = requestClient;
        }

        [HttpGet]
        [Route("home")]
        public IActionResult Home()
        {
            return Ok("true");
        }


        [HttpGet]
        [Route("send/saveOrder")]
        public async Task<IActionResult> SaveOrder()
        {
            var data = await _requestClient.GetResponse<SubmitOrderResponse>(new SubmitOrder()
            {
                ProductKey = $"{Guid.NewGuid()}",
                Quantity = 1,
                OrderId = Guid.NewGuid()
            });
            //await _publishEndpoint.Publish<SubmitOrder>(new SubmitOrder()
            //{
            //    ProductKey = $"{Guid.NewGuid()}",
            //    Quantity = 1,
            //    OrderId = Guid.NewGuid()
            //}
            //);

            return Ok("123");
        }

        /// <summary>
        /// SendOrder
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("send/schedule")]
        public async Task<IActionResult> Schedule()
        {
            var request = new ScheduleNotification
            {

            };
            
            await _publishEndpoint.ScheduleSend(new Uri("rabbitmq://localhost/schedule_test_queue"), DateTime.Now.AddSeconds(20), request);
            return Ok(DateTime.Now.ToLongTimeString());
        }
    }
}