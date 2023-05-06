using MEFIInterface;
using System.ComponentModel.Composition;
using TestModelLib;
using Unity;

namespace MEFTestLib2
{
    [Export(typeof(ILogService))]
    [ExportMetadata("Name", "LIB2")]
    public class Class2: ILogService
    {
        public void Init(UnityContainer unityContainer)
        {
            //放入静态类中
            MEFTestLib2.unityContainer = unityContainer;
            unityContainer.RegisterInstance<TestSubModel2>(new TestSubModel2() { bProperty = 222 });
            //StaticRAm.Ram.Add(typeof(TestSubModel2));
            //注入到eventbus中
            TestEventBus2.BioImpl.Event.BioEventBus.Instance.EventHandlerTypes.SubscribeFromDriver(this.GetType().Assembly);

        }

        void ILogService.Log(string message)
        {
            var testModel2 = MEFTestLib2.unityContainer.Resolve<TestModel1>();
            Console.WriteLine("driver程序2：2222" + message + "testModel2:" + testModel2.Id);
            testModel2.GOGOGO("driver程序2");

        }
        
    }

    public static class MEFTestLib2
    {
        public static UnityContainer unityContainer { get; set; }

    }

    public class TestSubModel2
    {
        public int bProperty { get; set; }

    }
} 