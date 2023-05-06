using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;
using static System.Net.Mime.MediaTypeNames;

namespace MEFTestLib1
{
    public class Test1ventHandler : IEventHandler<Test1Event>
    {
        public Task<bool> HandleAsync(Test1Event @event)
        {
            Console.WriteLine("Test1ventHandler" + @event.Name);
            return Task.FromResult(true);
        }
    }
}
