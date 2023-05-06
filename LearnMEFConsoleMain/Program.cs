using MediatR;
using MEFIInterface;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestModelLib;
using Unity;
using TestEventBus2;
using TestEventBus2.BioImpl.Event;
using TestEventBus2.BioImpl.PC;
using System.Diagnostics.Metrics;
using TestEventBus2.Interface.Event;
using System.Dynamic;
using System;

namespace LearnMEFConsoleMain
{
    internal class Program
    {
        //[Import(typeof(ILogService))]
        //public ILogService CurrentLogService { get; set; }

        /// <summary>
        /// 实际操作的 IOperation 对象
        /// 和一个代表元数据的 IOperationData 对象。
        /// </summary>
        [ImportMany]IEnumerable<Lazy<ILogService, IOperationData>> operations;



        static void Main(string[] args)
        {
            Console.WriteLine("程序启动!");

            Program pro = new Program();
            //初始化eventbus
            BioEventBus.Instance.AddBioEventBus(); 
            //IOC容器
            var container = new UnityContainer();
            pro.Run(container);
        }



        void Run(UnityContainer container)
        { 
            //MEF 加载dll driver
            AggregateCatalog  agg=new AggregateCatalog(); 
            var catalog1 = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "MEFTestLib1.dll");
            agg.Catalogs.Add(catalog1);
            var catalog2 = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "MEFTestLib2.dll");
            agg.Catalogs.Add(catalog2);
             
            //IOC容器
            container.RegisterInstance<TestModel1>(new TestModel1() {  Id=999999999});

            //MEF
            var mefcontainer = new CompositionContainer(agg);

            mefcontainer.ComposeParts(this);

            string name = "LIB2";   //LIB1
            foreach (Lazy<ILogService, IOperationData> i in operations)
            {
                //if (i.Metadata.Name.Equals(name))
                //{

                //1. 对i.value 进行反射，得到methodinfo。   判断方法名称， 执行invoke 调用。
                //2. 【me】dll文件的初始化，将执行--event与eventhandler, 构造event,------XXXXXX----因为event不可能被主程序构造的。   
                //3.【me】{最好是声明--消息式的}  INotificationHandler



                //建议直接使用挂载方式。 暴露一堆的string 为key 

                i.Value.Init(container);
                i.Value.Log($"主程序：输出MEF import的 子 driver的 类：{i.Metadata.Name}");

                //通过Metadata 元数据-打标签，得到相应的 实现
                //通过 i.Value.Log   使用方法

                //}



            }

            List<string> publishEvents = new List<string>()
            {  "Test1Event","Test2Event"};

            //通过string--Test1Event  得到事件的字符串 

            foreach (var item in publishEvents)
            {
                if (BioEventBus.Instance.EventHandlerTypes.GetEventHandlerTypes().TryGetValue(item, out var typeHandlerPairs))
                {

                    //判断当前程序集中是否有--没有则加载dll文件对应的程序集 
                    Assembly assembly = Assembly.LoadFrom(typeHandlerPairs.eventType.Assembly.Location);
                    //typeHandlerPairs.eventType 空构造函数  创建new对象
                    var eventObj = Activator.CreateInstance(typeHandlerPairs.eventType);
                    //属性获取  属性赋值 
                    //0.老办法：通过set prop value反射塞到属性中--不过还要从页面变量 搞一次 找到属性，慢
                    //1.采用固定属性 统一dic  event收到后再进行执行时通过key从dic里面抽取值----一样，也需要找到固定属性，查找多次，慢
                    //2.可以声明固定ExpandoObject 快速替代1 方案。 
                    //---ExpandoObject  的优点： 完全可以不用考虑是什么event， 只传递1个eventname的string字符串 + 动态的参数即可。
                    //具体处理是eventhandler的责任。

                    //例子：ExpandoObject

                    if (item== "Test2Event")
                    {
                        dynamic keyValuePairs = new ExpandoObject(); 
                        keyValuePairs.A = "1";
                        keyValuePairs.B = "2";
                        keyValuePairs.C = "3";
                        keyValuePairs.D = "4";

                        var property = typeHandlerPairs.eventType.GetProperty("DicParam");
                        property.SetValue(eventObj, keyValuePairs);  
                    }


                    //发布事件
                    BioPublisher.Instance.Publish((IEvent)eventObj);


                }
            }
             

            Console.Read(); 

        }
    }
    

     
}