namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// A silly optimization for pooling syntax walkers.
    /// </summary>
    /// <typeparam name="T">The inheriting type.</typeparam>
    public abstract class PooledWalker<T> : CSharpSyntaxWalker, IDisposable
        where T : PooledWalker<T>
    {
        private static readonly ConcurrentQueue<PooledWalker<T>> Cache = new ConcurrentQueue<PooledWalker<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledWalker{T}"/> class.
        /// </summary>
        /// <param name="depth">Where the walker should stop.</param>
        protected PooledWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
            : base(depth)
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Debug.Assert(!Cache.Contains(this), "!Cache.Contains(this)");
            this.Clear();
            Cache.Enqueue(this);
        }

        /// <summary>
        /// Returns a walker that have visited <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <see cref="SyntaxNode"/>.</param>
        /// <param name="create">The factory for creating a walker if not found in cache.</param>
        /// <returns>The walker that have visited <paramref name="node"/>.</returns>
        protected static T BorrowAndVisit(SyntaxNode node, Func<T> create)
        {
            var walker = Borrow(create);
            walker.Visit(node);
            return walker;
        }

        /// <summary>
        /// Returns a walker from cache or a new instance.
        /// Returned to cache on dispose.
        /// </summary>
        /// <param name="create">The factory for creating a walker if not found in cache.</param>
        /// <returns>The walker.</returns>
        protected static T Borrow(Func<T> create)
        {
            if (!Cache.TryDequeue(out var walker))
            {
                walker = create();
            }

            return (T)walker;
        }

        /// <summary>
        /// Remember to clear all lists etc so that they don't spill over to the next use.
        /// </summary>
        protected abstract void Clear();

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        [Conditional("DEBUG")]
        protected void ThrowIfDisposed()
        {
            if (Cache.Contains(this))
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
