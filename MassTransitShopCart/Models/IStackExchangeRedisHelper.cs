using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransitShopCart.Models
{
    /// <summary>
    /// IStackExchangeRedisHelper
    /// </summary>
    public interface IStackExchangeRedisHelper
    {
        /// <summary>
        /// 获取db
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase();
    }

    /// <summary>
    /// StackExchangeRedisHelper
    /// </summary>
    public class StackExchangeRedisHelper : IStackExchangeRedisHelper
    {
        private readonly string _conStr;
        private readonly object _locker = new object();
        private static ConnectionMultiplexer _instance;

        /// <summary>
        /// StackExchangeRedisHelper
        /// </summary>
        public StackExchangeRedisHelper()
        {
            _conStr = "119.27.185.32:6379, password=workdata123!@#";
        }

        /// <summary>
        /// 使用一个静态属性来返回已连接的实例，如下列中所示。这样，一旦 ConnectionMultiplexer 断开连接，便可以初始化新的连接实例。
        /// </summary>
        private ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (_locker)
                {
                    if (_instance == null || !_instance.IsConnected)
                    {
                        _instance = ConnectionMultiplexer.Connect(_conStr);
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 获取db
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return Instance.GetDatabase();
        }
    }
}
