using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;

namespace MEFTestLib1
{
    public class Test1Event:IEvent
    {
        public String Name { get; set; }

    }
}
