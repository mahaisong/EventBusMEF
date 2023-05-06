using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;

namespace TestEventBus2
{
    /// <summary>
    /// 主流程注入ROM容器--以及单例-对生产者、消费者的线程持有
    /// </summary>
    public static class EventBusExtensions
    {

        /// <summary>
        /// 将统一的初始化+publish实现类 注入
        /// </summary>
        public static IEventBus AddEventBus(this IEventBus eventBus)
        {

            return eventBus;

        }
        /// <summary>
        /// 看具体的eventbus实现类 是否需要消费者功能--这里没有使用MQ中间件解耦，使用的是内存MQ，只有一次注入，应该必须 注入Consumer消费者
        /// </summary>
        public static IEventBus AddConsumer(this IEventBus eventBus)
        {
            return eventBus;
        }

    }
}
