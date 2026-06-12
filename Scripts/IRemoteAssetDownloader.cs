#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

    public interface IRemoteAssetDownloader
    {
        public UniTask DownloadAsync(object key, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask DownloadAllAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask DownloadAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : notnull => this.DownloadAsync(typeof(T).GetKey(), progress, cancellationToken);
    }
}