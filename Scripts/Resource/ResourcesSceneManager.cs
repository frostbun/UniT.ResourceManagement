#nullable enable
namespace UniT.ResourceManagement.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class ResourcesSceneManager : ISceneManager
    {
        #region Constructor

        private readonly ILogger logger;

        private readonly Dictionary<string, AsyncOperation> loadingScenes = new();

        [Preserve]
        public ResourcesSceneManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        async UniTask ISceneManager.LoadAsync(string name, LoadSceneMode mode, bool activateOnLoad, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(name, mode)!;
            if (activateOnLoad)
            {
                await asyncOperation.ToUniTask(progress: progress, cancellationToken: cancellationToken);
            }
            else
            {
                this.loadingScenes[name]            = asyncOperation;
                asyncOperation.allowSceneActivation = false;
                while (asyncOperation.progress < .9f)
                {
                    await UniTask.Yield(cancellationToken);
                    progress?.Report(asyncOperation.progress * 10 / 9);
                }
            }
            this.logger.Debug($"Loaded {name}, mode: {mode}, activateOnLoad: {activateOnLoad}");
        }

        async UniTask ISceneManager.ActivateAsync(string name, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            if (!this.loadingScenes.Remove(name, out var asyncOperation))
            {
                throw new InvalidOperationException($"Scene {name} not loaded");
            }
            asyncOperation.allowSceneActivation = true;
            await asyncOperation.ToUniTask(progress: progress, cancellationToken: cancellationToken);
            this.logger.Debug($"Activated {name}");
        }

        async UniTask ISceneManager.UnloadAsync(string name, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            await SceneManager.UnloadSceneAsync(name).ToUniTask(progress: progress, cancellationToken: cancellationToken);
            this.logger.Debug($"Unloaded {name}");
        }
    }
}