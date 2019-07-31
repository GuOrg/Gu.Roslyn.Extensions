namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    /// <summary>
    /// Debug view for <see cref="PooledSet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the set.</typeparam>
    internal class PooledSetDebugView<T>
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        private readonly PooledSet<T> set;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledSetDebugView{T}"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        internal PooledSetDebugView(PooledSet<T> set)
        {
            this.set = set ?? throw new ArgumentNullException(nameof(set));
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal T[] Items => this.set.ToArray();
    }
}
