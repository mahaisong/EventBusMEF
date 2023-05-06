using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventBus2.Interface.Event.Message
{
    public interface IEventMessage
    {
        string EventName { get; } 

        TBody GetBody<TBody>() where TBody : class;
    }
}
