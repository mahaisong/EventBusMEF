using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.BioImpl.PC;

namespace TestEventBus2.BioImpl.Event
{
    public static class BioEventBusExtensions
    {
         
        public static BioEventBus AddBioEventBus(this BioEventBus  bioEventBus)
        {
            bioEventBus.AddEventBus();

            //增加消费
            var consumer= BioConsumer.Instance;
            return bioEventBus;
        }
         
         
    }
}
