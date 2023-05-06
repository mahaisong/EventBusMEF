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
    public class RegistryEventTypes : IRegistryEventTypes
    {
        #region Singleton

        private static RegistryEventTypes _registryEventTypes = null;

        public static object lockRegistryEventTypes = new object();

        public static RegistryEventTypes Instance
        {
            get
            {
                if (_registryEventTypes == null)
                {
                    lock (lockRegistryEventTypes)
                    {
                        if (_registryEventTypes == null)
                        {
                            _registryEventTypes = new RegistryEventTypes();
                        }
                    }
                }
                return _registryEventTypes;
            }

        }
        #endregion

        public RegistryEventTypes()
        {
            this.GetEventHandlerTypes();

        }
        private static object _lockObj = new object();
        //private List<string> _eventNames;

        //public IEnumerable<string> GetEventNames()
        //{
        //    if (this._eventNames == null)
        //    {//锁一下，加载
        //        lock (RegistryEventTypes._lockObj)
        //        {
        //            if (this._eventNames == null)
        //            {
        //                this._eventNames = new List<string>();
        //                //所有程序集
        //                foreach (Assembly assembly in this.GetAssemblies())
        //                {
        //                    IEnumerable<Type> source = ((IEnumerable<Type>)assembly.GetTypes()).
        //                        Where<Type>(
        //                        (Func<Type, bool>)(w => !w.IsInterface && typeof(IEvent).IsAssignableFrom(w))
        //                        );//lamabda表达式 Func<Type, bool >   不是 IsInterface 类型是 是 IEvent的子类
        //                    if (source.Count<Type>() > 0)
        //                    {
        //                        foreach (Type type in source)
        //                        {
        //                            //todo name  是 type的名字
        //                            this._eventNames.Add(type.Name); //type名 叫做eventname名
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return (IEnumerable<string>)this._eventNames;
        //}

        /// <summary>
        /// 扫描的目录前缀
        /// </summary>
        public static readonly List<string> IncludeAssemblyNamePrefixs = new List<string>()
    {
      "BioFoundry.", "Console.", "test."
    };

        /// <summary>
        /// 所有的 event name的属性  + hanldertype eventtype
        /// </summary>
        private ConcurrentDictionary<string, (Type handlerType, Type eventType)> _eventHandlerTypes;

       
        private void AssemblyAnalyseAdd(Assembly assembly)
        {
            foreach (Type type in ((IEnumerable<Type>)assembly.GetTypes()).Where<Type>(
                                   (Func<Type, bool>)(w => !w.IsInterface && !w.IsAbstract &&
                                   ((IEnumerable<Type>)w.GetInterfaces()).Any<Type>((Func<Type, bool>)
                                   (x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                   ))))
            //不是接口 不是抽象类  类型重载的接口s里面有没有 是泛型，且是IEventHandler的
            {
                foreach (Type c in ((IEnumerable<Type>)type.GetInterfaces()).Where<Type>(
                    (Func<Type, bool>)(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    ).SelectMany<Type, Type>(
                    (Func<Type, IEnumerable<Type>>)(i => (IEnumerable<Type>)i.GetGenericArguments())
                    ).ToArray<Type>())
                {//type重载的接口s里面  是 泛型类 且是IEventHandler的  子type 要求子type 提供 N个泛型类型的参数
                    if (typeof(IEvent).IsAssignableFrom(c))
                    {//同时 这个泛型参数 的父级 还需要是Ievent的
                        string name = c.Name;
                        //name  就是 ievent的name  也就是放入 _eventHandlerTypes 里面的 string 索引
                        //todo name  是 Ievent 的name 属性的名字
                        if (!this._eventHandlerTypes.TryAdd(name, (type, c)))
                            throw new Exception("检测到重复的消息处理 EventHandler, message: " + name + ", handler name: " + type.Name);
                    }
                }
            }
        }


        /// <summary>
        /// get 及 初始化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ConcurrentDictionary<string, (Type handlerType, Type eventType)> GetEventHandlerTypes()
        {
            if (this._eventHandlerTypes == null)
            {
                lock (RegistryEventTypes._lockObj)
                {
                    if (this._eventHandlerTypes == null)
                    {
                        this._eventHandlerTypes = new ConcurrentDictionary<string, (Type, Type)>();

                        //1.自动扫描所有程序集
                        foreach (Assembly assembly in this.GetAssemblies())
                        {
                            AssemblyAnalyseAdd(assembly);
                        }

                        //2.通过MEF加载---属于driver自己注册 --SubscribeFromDriver
                    }
                }
            }
            return this._eventHandlerTypes;
        }
        /// <summary>
        /// 同一个应用程序域下所有程序集----这是第一种方式， 还可以提供第2种方式 MEF的方式。 2种方式组合加载所有程序集
        /// 
        /// </summary>
        /// <returns></returns>
        private Assembly[] GetAssemblies() => ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>)(w =>
        {
            string name = w.GetName().Name.ToLower();
            return RegistryEventTypes.IncludeAssemblyNamePrefixs.Any<string>((Func<string, bool>)(m => name.StartsWith(m)));
        })).ToArray<Assembly>();



        #region init- Subscribe


        //private void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : IEvent
        //{  }

        //private void Subscribe<TEvent>(Type eventHandlerType) where TEvent : IEvent
        //{ }


        /// <summary>
        ///default action common subscribe
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventHandlerFunc"></param>
        public void Subscribe<TEvent, TEventHandler>() where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventType = typeof(TEvent);
            var eventHandlerType = typeof(TEventHandler);
            if (null == this._eventHandlerTypes)
            {
                GetEventHandlerTypes();//初始化
            }
            //存在则覆盖
            if (_eventHandlerTypes.ContainsKey(eventType.Name))
            {
                var typeHandlerPairs = _eventHandlerTypes[eventType.Name];
                typeHandlerPairs.eventType = eventType;
                typeHandlerPairs.handlerType = eventHandlerType;
            }
            else
            {//不存在 则插入
                _eventHandlerTypes.TryAdd(eventType.Name, (eventHandlerType, eventType));
            }
        }

        /// <summary>
        /// Driver专用
        /// </summary>
        public void SubscribeFromDriver(Assembly assembly)
        {
            AssemblyAnalyseAdd(assembly); 
        }

        #endregion

    }
}
