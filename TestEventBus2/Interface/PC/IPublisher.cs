using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;
using TestEventBus2.Interface.Event.Message;

namespace TestEventBus2.Interface.PC
{ 
     public interface IPublisher : IDisposable
    {
        bool Publish(IEvent @event);

        bool Publish(IEventMessage eventMessage);
    }
}
