using MEFIInterface;
using System.ComponentModel.Composition;
using TestModelLib;
using Unity;

namespace MEFTestLib1
{
    [Export(typeof(ILogService))]
    [ExportMetadata("Name", "LIB1")]
    public class Class1 : ILogService
    {
        public void Init(UnityContainer unityContainer)
        {
            //放入静态类中
            MEFTestLib1.unityContainer = unityContainer;
            unityContainer.RegisterInstance<TestSubModel1>(new TestSubModel1() { aProperty = 1111 });
            //StaticRAm.Ram.Add(typeof(TestSubModel1)); 
            //注入到eventbus中
            TestEventBus2.BioImpl.Event.BioEventBus.Instance.EventHandlerTypes.SubscribeFromDriver(this.GetType().Assembly);
        }

        void ILogService.Log(string message)
        {
           var testModel1= MEFTestLib1.unityContainer.Resolve<TestModel1>();
           Console.WriteLine("driver程序1：11111" + message+ "testModel1:"+ testModel1.Id);
            testModel1.GOGOGO("driver程序1");
        }
    }
    public static class MEFTestLib1
    {
        public static UnityContainer unityContainer { get; set; }

    }

    public class TestSubModel1
    {
        public int aProperty { get; set; }
    }
}