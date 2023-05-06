using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;
using TestEventBus2.Interface.Event.Message;


namespace TestEventBus2.BioImpl.Event.Message
{
    public class BioEventMessage<T> : IEventMessage where T : class, IEvent
    {

        public T Body { get; set; }

        public string EventName { get; set; }

        public TBody GetBody<TBody>() where TBody : class => (object)this.Body as TBody;

        public string CreateTimeStr { get; set; }

    }
}