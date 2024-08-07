#if UNIT_ADDRESSABLES
#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.Scripting;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AddressableAssetsManager : AssetsManager
    {
        private readonly string? scope;

        [Preserve]
        public AddressableAssetsManager(ILoggerManager loggerManager, string? scope = null) : base(loggerManager)
        {
            this.scope = scope.NullIfWhitespace();
        }

        private string GetScopedKey(string key) => this.scope is null ? key : $"{this.scope}/{key}";

        protected override Object? Load<T>(string key)
        {
            return Addressables.LoadAssetAsync<T>(this.GetScopedKey(key)).WaitForCompletion();
        }

        protected override void Unload(Object asset)
        {
            Addressables.Release(asset);
        }

        #if UNIT_UNITASK
        protected override UniTask<Object?> LoadAsync<T>(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadAssetAsync<T>(this.GetScopedKey(key))
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(asset => (Object?)asset);
        }
        #else
        protected override IEnumerator LoadAsync<T>(string key, Action<Object?> callback, IProgress<float>? progress)
        {
            var operation = Addressables.LoadAssetAsync<T>(this.GetScopedKey(key));
            while (!operation.IsDone)
            {
                progress?.Report(operation.PercentComplete);
                yield return null;
            }
            callback(operation.Result);
        }
        #endif
    }
}
#endif