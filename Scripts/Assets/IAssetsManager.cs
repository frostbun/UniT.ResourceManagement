#nullable enable
namespace UniT.ResourceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using Object = UnityEngine.Object;

    public interface IAssetsManager : IDisposable
    {
        public UniTask<bool> ContainsAsync<T>(object key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object;

        public UniTask<T> LoadAsync<T>(object key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object;

        public UniTask<IReadOnlyCollection<T>> LoadAllAsync<T>(object key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object;

        public void Unload(object key);

        public void UnloadAll(object key);

        #region Implicit Key

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<bool> ContainsAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object => this.ContainsAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<T> LoadAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object => this.LoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask<IReadOnlyCollection<T>> LoadAllAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : Object => this.LoadAllAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unload<T>() => this.Unload(typeof(T).GetKey());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnloadAll<T>() => this.UnloadAll(typeof(T).GetKey());

        #endregion
    }
}