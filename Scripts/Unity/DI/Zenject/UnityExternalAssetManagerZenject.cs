#nullable enable
namespace UniT.ResourceManagement.Unity.DI
{
    using Zenject;

    public static class UnityExternalAssetManagerZenject
    {
        public static void BindUnityExternalAssetManager(this DiContainer container)
        {
            container.BindInterfacesTo<UnityExternalAssetManager>().AsSingle();
        }
    }
}