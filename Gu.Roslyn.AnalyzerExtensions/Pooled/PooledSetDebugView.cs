namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Debug view for <see cref="PooledSet{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items in the set.</typeparam>
    internal class PooledSetDebugView<T>
    {
        private readonly PooledSet<T> set;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledSetDebugView{T}"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        public PooledSetDebugView(PooledSet<T> set)
        {
            this.set = set ?? throw new ArgumentNullException(nameof(set));
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => this.set.ToArray();
    }
}
