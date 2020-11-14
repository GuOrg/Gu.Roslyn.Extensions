namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Walks the current member but saves all targets for deferred walking.
    /// </summary>
    /// <typeparam name="T">The inheriting type.</typeparam>
    public abstract class RecursiveWalker<T> : ExecutionWalker<T>
            where T : RecursiveWalker<T>
    {
        private readonly List<Target<SyntaxNode, ISymbol, SyntaxNode>> targets = new List<Target<SyntaxNode, ISymbol, SyntaxNode>>();

        /// <summary>
        /// Gets the recursive <see cref="Target{TSource,TSymbol,TTarget}"/> found when walking the first level.
        /// </summary>
        public IReadOnlyList<Target<SyntaxNode, ISymbol, SyntaxNode>> Targets => this.targets;

        /// <inheritdoc />
        protected override void Clear()
        {
            this.targets.Clear();
            base.Clear();
        }

        /// <inheritdoc />
        protected override bool TryGetTargetSymbol<TSource, TSymbol, TDeclaration>(TSource node, out Target<TSource, TSymbol, TDeclaration> target, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        {
            if (base.TryGetTargetSymbol(node, out target, caller, line) &&
                IsRecursiveTarget(target))
            {
                this.targets.Add(Target.Create<SyntaxNode, ISymbol, SyntaxNode>(node, target.Symbol, target.Declaration));
            }

            return false;

            static bool IsRecursiveTarget(Target<TSource, TSymbol, TDeclaration> t)
            {
                return t switch
                {
                    { Declaration: null } => false,
                    { Symbol: IFieldSymbol _ } => false,
                    { Symbol: IPropertySymbol property } when property.IsAutoProperty() => false,
                    _ => true,
                };
            }
        }
    }
}
