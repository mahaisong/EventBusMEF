using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.BioImpl.Event;
using TestEventBus2.BioImpl.Event.Message;
using TestEventBus2.Interface.Event;
using TestEventBus2.Interface.Event.Message;
using TestEventBus2.Interface.PC;

namespace TestEventBus2.BioImpl.PC
{
    public class BioPublisher : IPublisher
    {

        #region Singleton

        private static BioPublisher _bioPublisher = null;

        public static object lockBioPublisher = new object();

        public static BioPublisher Instance
        {
            get
            {
                if (_bioPublisher == null)
                {
                    lock (lockBioPublisher)
                    {
                        if (_bioPublisher == null)
                        {
                            _bioPublisher = new BioPublisher();
                        }
                    }
                }
                return _bioPublisher;
            }

        }
        #endregion

        #region ctor

        private BioPublisher()
        {
       
        }
        #endregion

      
        public bool Publish(IEvent @event)
        {
            BioEventMessage<IEvent> eventMessage = new BioEventMessage<IEvent>();
            eventMessage.Body = @event; //赋值 body 属性 ;
            eventMessage.EventName = @event.GetType().Name;
            //todo name  发布时 采用的是 type 的name  
            return this.Publish((IEventMessage)eventMessage);
        }

        public bool Publish(IEventMessage eventMessage)
        { 
            try
            {
                if (!BioEventBus.Instance.Publish_Enqueue(eventMessage))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //发布异常
                return false;
            }

            return true;
        }
        public void Dispose() { }
    }
}
