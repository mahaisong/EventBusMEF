using Unity;

namespace MEFIInterface
{
    public interface ILogService
    {
        void Log(string message);

        void Init(UnityContainer unityContainer);
    }
}