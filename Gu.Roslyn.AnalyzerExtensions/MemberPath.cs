namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
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
            var xs = x.IdentifierNames;
            var ys = y.IdentifierNames;
            if (xs.Count == 0 ||
                xs.Count != ys.Count)
            {
                return false;
            }

            for (var i = 0; i < xs.Count; i++)
            {
                if (xs[i].Identifier.ValueText != ys[i].Identifier.ValueText)
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
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

        /// <summary>
        /// Tries to find C in this.C.P1.P2.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="member">The root member.</param>
        /// <returns>True if root was found.</returns>
        public static bool TryFindRoot(ExpressionSyntax expression, out IdentifierNameSyntax member)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.IdentifierNames.TryFirst(out member);
            }
        }

        /// <summary>
        /// Tries to find Baz in this.C.P1.P2.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="member">The leaf member.</param>
        /// <returns>True if leaf was found.</returns>
        public static bool TryFindLast(ExpressionSyntax expression, out IdentifierNameSyntax member)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.IdentifierNames.TryLast(out member);
            }
        }

        /// <summary>
        /// Tries to the single member in the path.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="member">The single member.</param>
        /// <returns>True if the path was only one member this.C or C for example.</returns>
        public static bool TrySingle(ExpressionSyntax expression, out IdentifierNameSyntax member)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.IdentifierNames.TrySingle(out member);
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
                return walker.IdentifierNames.Count == 0;
            }
        }

        /// <summary>
        /// Try get the member name of the expression.
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/>.</param>
        /// <param name="name">The name.</param>
        /// <returns>True if a name was found.</returns>
        public static bool TryGetMemberName(this ExpressionSyntax expression, out string name)
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
            private readonly List<IdentifierNameSyntax> identifierNames = new List<IdentifierNameSyntax>();

            private PathWalker()
            {
            }

            /// <summary>
            /// Gets the <see cref="IdentifierNameSyntax"/> found in the path.
            /// </summary>
            public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

            /// <summary>
            /// <see cref="PooledWalker{T}.Borrow"/>.
            /// </summary>
            /// <param name="node">The path to walk.</param>
            /// <returns>A walker.</returns>
            public static PathWalker Borrow(ExpressionSyntax node)
            {
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
                    case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AsExpression):
                        base.Visit(binary.Left);
                        return;
                    case CastExpressionSyntax cast:
                        base.Visit(cast.Expression);
                        return;
                    case InvocationExpressionSyntax invocation when invocation.Expression is IdentifierNameSyntax:
                        return;
                    case InvocationExpressionSyntax invocation when invocation.Expression is MemberAccessExpressionSyntax memberAccess:
                        this.Visit(memberAccess.Expression);
                        return;
                }

                switch (node.Kind())
                {
                    case SyntaxKind.ConditionalAccessExpression:
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.MemberBindingExpression:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.ParenthesizedExpression:
                        base.Visit(node);
                        return;
                }
            }

            /// <inheritdoc />
            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                this.identifierNames.Add(node);
            }

            /// <summary>
            /// Check if this path starts with the other or is equal.
            /// </summary>
            /// <param name="other">The other path.</param>
            /// <returns>True if this path starts with the other or is equal.</returns>
            public bool StartsWith(PathWalker other)
            {
                if (other.identifierNames.Count == 0 ||
                    other.identifierNames.Count > this.identifierNames.Count)
                {
                    return false;
                }

                for (var i = 0; i < other.identifierNames.Count; i++)
                {
                    if (this.identifierNames[i].Identifier.ValueText != other.identifierNames[i].Identifier.ValueText)
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
                this.identifierNames.Clear();
            }
        }
    }
}
