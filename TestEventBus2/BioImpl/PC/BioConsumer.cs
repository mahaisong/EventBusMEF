using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestEventBus2.BioImpl.Event;
using TestEventBus2.BioImpl.Event.Message;
using TestEventBus2.Interface.Event;
using TestEventBus2.Interface.Event.Message;
using TestEventBus2.Interface.PC;
using static System.Formats.Asn1.AsnWriter;

namespace TestEventBus2.BioImpl.PC
{
    public class BioConsumer : IConsumer
    {
        #region Singleton

        private static BioConsumer _bioConsumer = null;

        public static object lockBioConsumer = new object();


        public static BioConsumer Instance
        {
            get
            {
                if (_bioConsumer == null)
                {
                    lock (lockBioConsumer)
                    {
                        if (_bioConsumer == null)
                        {
                            _bioConsumer = new BioConsumer();
                        }
                    }
                }
                return _bioConsumer;
            }

        }
        public BioConsumer()
        {
            //todo 同步调异步 不等待
            BioEventBus.Instance.Publish_Dequeue(ConsumerFunc);
        }


        #endregion


        public async Task<bool> ConsumerFunc(string message)
        {
            string eventName = GetEventName(message);

            if (BioEventBus.Instance.EventHandlerTypes.GetEventHandlerTypes().TryGetValue(eventName, out var typeHandlerPairs))
            {
                try
                {
                    //将message 反序列化为  IEventMessage
                    Type type = typeof(BioEventMessage<>);
                    Type[] typeArray = new Type[1] { typeHandlerPairs.eventType };
                    IEventMessage eventMessage = (IEventMessage)message.FromJsonToObj(type.MakeGenericType(typeArray), (JsonSerializerSettings)null);

                    //创建handler处理

                    var method = typeHandlerPairs.handlerType.GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly, Type.DefaultBinder,
                        new Type[] { eventMessage.GetBody<IEvent>().GetType() }, null);
                    if (method == null)
                    {
                        //没有 HandleAsync 方法
                    }

                    //动态创建1个瞬时的
                    var handler = Activator.CreateInstance(typeHandlerPairs.handlerType);
                    if (!await (Task<bool>) method.Invoke(handler,
                          (object[])  new IEvent[1] {   eventMessage.GetBody<IEvent>() }
                          ))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    //消费异常
                }
            }
            else
            {
                //没配置绑定
            } 
            return true;
        }


        private string GetEventName(string message)
        {
            JToken jtoken = JObject.Parse(message).GetValue("EventName", StringComparison.OrdinalIgnoreCase);
            string eventName = jtoken != null ? jtoken.Value<string>() : (string)null;
            return eventName;
        }


    }
}
