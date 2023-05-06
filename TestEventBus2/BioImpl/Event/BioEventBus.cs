using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface;
using TestEventBus2.Interface.Event;
using TestEventBus2.Interface.Event.Message;

namespace TestEventBus2.BioImpl.Event
{
    public class BioEventBus : IEventBus
    {
        ///重点是三存RAM  怎么融入到非DI 容器的 静态写法里面


        #region Singleton

        private static BioEventBus _eventBus = null;

        public static object lockBioEventBus = new object();

        public static BioEventBus Instance
        {
            get
            {
                if (_eventBus == null)
                {
                    lock (lockBioEventBus)
                    {
                        if (_eventBus == null)
                        {
                            _eventBus = new BioEventBus();
                        }
                    }
                }
                return _eventBus;
            }

        }
        #endregion

        #region ctor

        private BioEventBus()
        {

        }
        #endregion




        #region Propery
        /// <summary>
        /// 注册banding关系
        /// </summary>
        public IRegistryEventTypes EventHandlerTypes { get { return RegistryEventTypes.Instance; } }

        private EventWaitHandle waiter = new AutoResetEvent(false);

        /// <summary>
        /// 自己实现类、自己维护消息队列--消息体-实质是 --》EventMessage<T> --》EventMessage<IEvent>
        /// </summary>

        private ConcurrentQueue<string> MessageQueue = new ConcurrentQueue<string>();

        #endregion

        public bool Publish_Enqueue<TEventMessage>(TEventMessage eventMessage) where TEventMessage : IEventMessage
        {

            if (EventHandlerTypes.GetEventHandlerTypes().ContainsKey(eventMessage.EventName))
            {
                MessageQueue.Enqueue(eventMessage.ToJson());
                //set begin customer
                this.waiter.Set();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async void Publish_Dequeue(Func<string, Task<bool>> func)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (!MessageQueue.IsEmpty)
                    {
                        if (MessageQueue.TryDequeue(out string evt))
                        { 
                            //todo 同步调异步 不等待
                            func(evt);
                        }
                    }
                    else
                    {
                        this.waiter.WaitOne();
                    }

                }

            });
        }

    }
}
