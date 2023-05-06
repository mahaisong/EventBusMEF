using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;
using static System.Net.Mime.MediaTypeNames;

namespace MEFTestLib2
{
    public class Test2ventHandler : IEventHandler<Test2Event>
    {
        public Task<bool> HandleAsync(Test2Event @event)
        {
            Console.WriteLine("Test2ventHandler 动态参数属性" + JsonConvert.SerializeObject(@event.DicParam));
             
            var k = @event.DicParam.ToDictionary(k => k.Key, k => k.Value);
            Console.WriteLine("Test2ventHandler 参数A的值" + k["A"]);
            Console.WriteLine("Test2ventHandler 参数A的值" + k["C"]);
            return Task.FromResult(true);
        }
    }
}
