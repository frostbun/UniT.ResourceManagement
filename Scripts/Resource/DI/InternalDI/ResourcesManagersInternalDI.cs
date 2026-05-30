#nullable enable
namespace UniT.ResourceManagement.Resources.DI
{
    using UniT.DI;

    public static class ResourcesManagersInternalDI
    {
        public static void AddResourcesAssetManager(this DependencyContainer container, string? scope = null)
        {
            container.AddInterfaces<ResourcesAssetManager>(scope);
        }

        public static void AddResourcesSceneManager(this DependencyContainer container)
        {
            container.AddInterfaces<ResourcesSceneManager>();
        }
    }
}