using Kyrnel;

namespace Kyrnel
{
    public class PlatformManager : Singleton<PlatformManager>
    {
        private IPlatformBridge _bridge;
    }
}
