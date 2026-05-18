#if UNIT_ADDRESSABLES
#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public sealed class AddressableScenesManager : IScenesManager
    {
        #region Constructor

        private readonly ILogger logger;

        private readonly Dictionary<string, SceneInstance> loadedScenes = new();

        [Preserve]
        public AddressableScenesManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        UniTask IScenesManager.LoadAsync(string name, LoadSceneMode mode, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadSceneAsync(name, mode)
                .ToUniTask(progress, cancellationToken)
                .ContinueWith(scene =>
                {
                    if (mode is LoadSceneMode.Single) this.loadedScenes.Clear();
                    this.loadedScenes.Add(name, scene);
                    this.logger.Debug($"Loaded {name}, mode: {mode}");
                });
        }

        UniTask IScenesManager.UnloadAsync(string name, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (!this.loadedScenes.Remove(name, out var scene))
            {
                this.logger.Warning($"{name} not loaded");
                return UniTask.CompletedTask;
            }
            return Addressables.UnloadSceneAsync(scene)
                .ToUniTask(progress, cancellationToken)
                .ContinueWith(_ => this.logger.Debug($"Unloaded {name}"));
        }
    }
}
#endif