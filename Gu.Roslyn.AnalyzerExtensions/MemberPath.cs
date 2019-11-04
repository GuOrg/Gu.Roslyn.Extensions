#pragma warning disable CA1034 // Nested types should not be visible
namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper for working with member paths like C.P1?.P2.
    /// </summary>
    public static class MemberPath
    {
        /// <summary> Compares equality by path. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equals(ExpressionSyntax x, ExpressionSyntax y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null ||
                y is null)
            {
                return false;
            }

            using (var xWalker = PathWalker.Borrow(x))
            {
                using (var yWalker = PathWalker.Borrow(y))
                {
                    return Equals(xWalker, yWalker);
                }
            }
        }

        /// <summary> Compares equality by path. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equals(PathWalker x, PathWalker y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            var xs = x.Tokens;
            var ys = y.Tokens;
            if (xs.Count == 0 ||
                xs.Count != ys.Count)
            {
                return false;
            }

            for (var i = 0; i < xs.Count; i++)
            {
                if (xs[i].ValueText != ys[i].ValueText)
                {
                    return false;
                }
            }

            return true;
        }

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
        [Obsolete("Should only be called with arguments of type IAssemblySymbol.", error: true)]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

        /// <summary>
        /// Tries to find C in this.C.P1.P2.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="token">The root member.</param>
        /// <returns>True if root was found.</returns>
        public static bool TryFindRoot(ExpressionSyntax expression, out SyntaxToken token)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.TryFirst(out token);
            }
        }

        /// <summary>
        /// Tries to find Baz in this.C.P1.P2.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="token">The leaf member.</param>
        /// <returns>True if leaf was found.</returns>
        public static bool TryFindLast(ExpressionSyntax expression, out SyntaxToken token)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.TryLast(out token);
            }
        }

        /// <summary>
        /// Tries to the single member in the path.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="token">The single member.</param>
        /// <returns>True if the path was only one member this.C or C for example.</returns>
        public static bool TrySingle(ExpressionSyntax expression, out SyntaxToken token)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.TrySingle(out token);
            }
        }

        /// <summary>
        /// Checks that the path is empty, this is true for this. and base. calls.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <returns>True if the path is empty, this is true for this. and base. calls.</returns>
        public static bool IsEmpty(ExpressionSyntax expression)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.Tokens.Count == 0;
            }
        }

        /// <summary>
        /// Try get the member name of the expression.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <returns>True if a name was found.</returns>
        public static bool TryGetMemberName(this ExpressionSyntax expression, [NotNullWhen(true)]out string? name)
        {
            name = null;
            switch (expression)
            {
                case IdentifierNameSyntax identifierName:
                    name = identifierName.Identifier.ValueText;
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    name = memberAccess.Name.Identifier.ValueText;
                    break;
                case MemberBindingExpressionSyntax memberBinding:
                    name = memberBinding.Name.Identifier.ValueText;
                    break;
                case ConditionalAccessExpressionSyntax conditionalAccess:
                    TryGetMemberName(conditionalAccess.WhenNotNull, out name);
                    break;
            }

            return name != null;
        }

        /// <summary>
        /// Get a <see cref="PathWalker"/> for <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A <see cref="PathWalker"/> for <paramref name="expression"/>.</returns>
        public static PathWalker Get(ExpressionSyntax expression) => PathWalker.Borrow(expression);

        /// <inheritdoc />
        public sealed class PathWalker : PooledWalker<PathWalker>
        {
            private readonly List<SyntaxToken> tokens = new List<SyntaxToken>();

            private PathWalker()
            {
            }

            /// <summary>
            /// Gets the <see cref="IdentifierNameSyntax"/> found in the path.
            /// </summary>
            public IReadOnlyList<SyntaxToken> Tokens => this.tokens;

            /// <summary>
            /// <see cref="PooledWalker{T}.Borrow"/>.
            /// </summary>
            /// <param name="node">The path to walk.</param>
            /// <returns>A walker.</returns>
            public static PathWalker Borrow(ExpressionSyntax node)
            {
                if (node is null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                if (node.Parent is ConditionalAccessExpressionSyntax conditionalAccess)
                {
                    return Borrow(conditionalAccess);
                }

                var walker = BorrowAndVisit(node, () => new PathWalker());
                return walker;
            }

            /// <inheritdoc />
            public override void Visit(SyntaxNode node)
            {
                switch (node)
                {
                    case BinaryExpressionSyntax binary
                        when binary.IsKind(SyntaxKind.AsExpression):
                        base.Visit(binary.Left);
                        return;
                    case CastExpressionSyntax cast:
                        base.Visit(cast.Expression);
                        return;
                    case InvocationExpressionSyntax { Expression: IdentifierNameSyntax _ }:
                    case InvocationExpressionSyntax { Expression: PredefinedTypeSyntax _ }:
                        return;
                    case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Expression: { } expression } }:
                        this.Visit(expression);
                        return;
                }

                switch (node.Kind())
                {
                    case SyntaxKind.ConditionalAccessExpression:
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.MemberBindingExpression:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.ParenthesizedExpression:
                        base.Visit(node);
                        return;
                }
            }

            /// <inheritdoc />
            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                if (node is null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                this.tokens.Add(node.Identifier);
            }

            /// <inheritdoc />
            public override void VisitPredefinedType(PredefinedTypeSyntax node)
            {
                if (node is null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                this.tokens.Add(node.Keyword);
            }

            /// <summary>
            /// Get the first token if any.
            /// </summary>
            /// <param name="token">The <see cref="SyntaxToken"/>.</param>
            /// <returns>True if found.</returns>
            public bool TryFirst(out SyntaxToken token) => this.tokens.TryFirst(out token);

            /// <summary>
            /// Get the single token if any.
            /// </summary>
            /// <param name="token">The <see cref="SyntaxToken"/>.</param>
            /// <returns>True if found.</returns>
            public bool TrySingle(out SyntaxToken token) => this.tokens.TrySingle(out token);

            /// <summary>
            /// Get the last token if any.
            /// </summary>
            /// <param name="token">The <see cref="SyntaxToken"/>.</param>
            /// <returns>True if found.</returns>
            public bool TryLast(out SyntaxToken token) => this.tokens.TryLast(out token);

            /// <summary>
            /// Check if this path starts with the other or is equal.
            /// </summary>
            /// <param name="other">The other path.</param>
            /// <returns>True if this path starts with the other or is equal.</returns>
            public bool StartsWith(PathWalker other)
            {
                if (other is null)
                {
                    throw new ArgumentNullException(nameof(other));
                }

                if (other.tokens.Count == 0 ||
                    other.tokens.Count > this.tokens.Count)
                {
                    return false;
                }

                for (var i = 0; i < other.tokens.Count; i++)
                {
                    if (this.tokens[i].ValueText != other.tokens[i].ValueText)
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Check if this path starts with the other or is equal.
            /// </summary>
            /// <param name="other">The other path.</param>
            /// <returns>True if this path starts with the other or is equal.</returns>
            public bool StartsWith(ExpressionSyntax other)
            {
                using (var walker = Borrow(other))
                {
                    return this.StartsWith(walker);
                }
            }

            /// <inheritdoc />
            protected override void Clear()
            {
                this.tokens.Clear();
            }
        }
    }
}
