using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.Interface.Event;

namespace MEFTestLib2
{
    public class Test2Event:IEvent
    {
        public ExpandoObject DicParam { get; set; }

    }
}
