#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class ExternalAssetsManager : IExternalAssetsManager
    {
        #region Constructor

        private readonly ILogger logger;

        private readonly Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        [Preserve]
        public ExternalAssetsManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        #if UNIT_UNITASK
        UniTask<Texture2D> IExternalAssetsManager.DownloadTextureAsync(string url, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return this.cache.TryAddAsync(url, async () =>
            {
                using var request = new UnityWebRequest(url);
                using var downloadHandler = new DownloadHandlerTexture();
                request.downloadHandler = downloadHandler;
                await request.SendWebRequest().ToUniTask(progress: progress, cancellationToken: cancellationToken);
                return downloadHandler.texture;
            }).ContinueWith(isDownloaded =>
            {
                this.logger.Debug(isDownloaded ? $"Downloaded texture from {url}" : $"Using cached texture from {url}");
                return this.cache[url];
            });
        }
        #else
        IEnumerator IExternalAssetsManager.DownloadTextureAsync(string url, Action<Texture2D> callback, IProgress<float>? progress)
        {
            return this.cache.TryAddAsync(
                url,
                DownloadTextureAsync,
                isDownloaded =>
                {
                    this.logger.Debug(isDownloaded ? $"Downloaded texture from {url}" : $"Using cached texture from {url}");
                    callback(this.cache[url]);
                }
            );

            IEnumerator DownloadTextureAsync(Action<Texture2D> callback)
            {
                using var request         = new UnityWebRequest(url);
                using var downloadHandler = new DownloadHandlerTexture();
                request.downloadHandler = downloadHandler;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    progress?.Report(operation.progress);
                    yield return null;
                }
                callback(downloadHandler.texture);
            }
        }
        #endif

        void IExternalAssetsManager.Unload(string key)
        {
            if (this.cache.Remove(key))
            {
                this.logger.Debug($"Unloaded {key}");
            }
            else
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
            }
        }

        #endregion

        #region Finalizer

        private void Dispose()
        {
            this.cache.Clear();
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            this.logger.Debug("Disposed");
        }

        ~ExternalAssetsManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        #endregion
    }
}