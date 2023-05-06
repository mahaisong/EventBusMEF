using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;

namespace TestEventBus2.Interface
{
    public interface IRegistryEventTypes
    {
        ConcurrentDictionary<string, (Type handlerType, Type eventType)> GetEventHandlerTypes();

        //IEnumerable<string> GetEventNames();

        void Subscribe<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;

        void SubscribeFromDriver(Assembly assembly);
    }
}
