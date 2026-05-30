#nullable enable
namespace UniT.ResourceManagement.Unity.DI
{
    using UniT.DI;

    public static class UnityExternalAssetManagerInternalDI
    {
        public static void AddUnityExternalAssetManager(this DependencyContainer container)
        {
            container.AddInterfaces<UnityExternalAssetManager>();
        }
    }
}