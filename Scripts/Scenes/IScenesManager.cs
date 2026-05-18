#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine.SceneManagement;

    public interface IScenesManager
    {
        public UniTask LoadAsync(string name, LoadSceneMode mode = LoadSceneMode.Single, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask UnloadAsync(string name, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
    }
}