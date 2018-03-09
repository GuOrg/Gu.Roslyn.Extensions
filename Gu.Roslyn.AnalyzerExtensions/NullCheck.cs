namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class NullCheck
    {
        internal static bool IsCheckedBefore(ISymbol symbol, SyntaxNode context, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //if (ifStatement.Condition is BinaryExpressionSyntax binary &&
            //    binary.IsKind(SyntaxKind.EqualsExpression))
            //{
            //    if (binary.Left.IsKind(SyntaxKind.NullLiteralExpression) &&
            //        IsSymbol(binary.Right))
            //    {
            //        return !IsAssignedBefore(ifStatement);
            //    }

            //    if (IsSymbol(binary.Left) &&
            //        binary.Right.IsKind(SyntaxKind.NullLiteralExpression))
            //    {
            //        return !IsAssignedBefore(ifStatement);
            //    }
            //}
            //else if (ifStatement.Condition is InvocationExpressionSyntax invocation)
            //{
            //    if (invocation.Expression is IdentifierNameSyntax identifierName &&
            //        invocation.ArgumentList != null &&
            //        invocation.ArgumentList.Arguments.Count == 2 &&
            //        (identifierName.Identifier.ValueText == "ReferenceEquals" ||
            //         identifierName.Identifier.ValueText == "Equals"))
            //    {
            //        if (invocation.ArgumentList.Arguments.TrySingle(x => x.Expression?.IsKind(SyntaxKind.NullLiteralExpression) == true, out _) &&
            //            invocation.ArgumentList.Arguments.TrySingle(x => IsSymbol(x.Expression), out _))
            //        {
            //            return !IsAssignedBefore(ifStatement);
            //        }
            //    }
            //    else if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            //             memberAccess.Name is IdentifierNameSyntax memberIdentifier &&
            //             invocation.ArgumentList != null &&
            //             invocation.ArgumentList.Arguments.Count == 2 &&
            //             (memberIdentifier.Identifier.ValueText == "ReferenceEquals" ||
            //              memberIdentifier.Identifier.ValueText == "Equals"))
            //    {
            //        if (invocation.ArgumentList.Arguments.TrySingle(x => x.Expression?.IsKind(SyntaxKind.NullLiteralExpression) == true, out _) &&
            //            invocation.ArgumentList.Arguments.TrySingle(x => IsSymbol(x.Expression), out _))
            //        {
            //            return !IsAssignedBefore(ifStatement);
            //        }
            //    }
            //}
        }

        private sealed class NullCheckWalker : PooledWalker<NullCheckWalker>
        {
            private NullCheckWalker()
            {
            }

            public static NullCheckWalker Borrow(SyntaxNode scope) => BorrowAndVisit(scope, () => new NullCheckWalker());

            protected override void Clear()
            {
            }
        }
    }
}
