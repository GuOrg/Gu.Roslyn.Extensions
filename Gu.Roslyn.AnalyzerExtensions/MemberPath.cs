namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helper fow working with member paths like foo.Bar?.Baz
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
            using (var yWalker = PathWalker.Borrow(y))
            {
                var xPath = xWalker.IdentifierNames;
                var yPath = yWalker.IdentifierNames;
                if (xPath.Count == 0 ||
                    xPath.Count != yPath.Count)
                {
                    return false;
                }

                for (var i = 0; i < xPath.Count; i++)
                {
                    if (xPath[i].Identifier.ValueText != yPath[i].Identifier.ValueText)
                    {
                        return false;
                    }
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
        /// Tries to find foo in this.foo.Bar.Baz
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
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
        /// Tries to find Baz in this.foo.Bar.Baz
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
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
        /// Tries to the single member in the path
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
        /// <param name="member">The single member.</param>
        /// <returns>True if the path was only one member this.foo or foo for example.</returns>
        public static bool TrySingle(ExpressionSyntax expression, out IdentifierNameSyntax member)
        {
            using (var walker = PathWalker.Borrow(expression))
            {
                return walker.IdentifierNames.TrySingle(out member);
            }
        }

        /// <summary>
        /// Try get the member name of the expression
        /// </summary>
        /// <param name="expression">The <see cref="ExpressionSyntax"/></param>
        /// <param name="name">The name.</param>
        /// <returns>True if a name was found</returns>
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
            /// <see cref="PooledWalker{T}.Borrow"/>
            /// </summary>
            /// <param name="node">The path to walk.</param>
            /// <returns>A walker</returns>
            public static PathWalker Borrow(ExpressionSyntax node)
            {
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
                    case InvocationExpressionSyntax invocation when invocation.Expression is IdentifierNameSyntax identifierName:
                        base.Visit(identifierName);
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

            /// <inheritdoc />
            protected override void Clear()
            {
                this.identifierNames.Clear();
            }
        }
    }
}
