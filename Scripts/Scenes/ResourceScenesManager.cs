#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public sealed class ResourceScenesManager : IScenesManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public ResourceScenesManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        UniTask IScenesManager.LoadAsync(string name, LoadSceneMode mode, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return SceneManager.LoadSceneAsync(name, mode)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(() => this.logger.Debug($"Loaded {name}, mode: {mode}"));
        }

        UniTask IScenesManager.UnloadAsync(string name, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return SceneManager.UnloadSceneAsync(name)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(() => this.logger.Debug($"Unloaded {name}"));
        }
    }
}