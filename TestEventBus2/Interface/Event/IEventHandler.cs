using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEventBus2.Interface.Event
{
    public interface IEventHandler<in T> where T : IEvent
    { 
        Task<bool> HandleAsync(T @event);
    }
}