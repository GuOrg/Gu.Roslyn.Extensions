namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class MemberPath
    {
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

            using (var xWalker = IdentifierNameWalker.Borrow(x))
            using (var yWalker = IdentifierNameWalker.Borrow(y))
            {
                if (xWalker.IdentifierNames.Count != yWalker.IdentifierNames.Count)
                {
                    return false;
                }

                for (var i = 0; i < xWalker.IdentifierNames.Count; i++)
                {
                    if (xWalker.IdentifierNames[i].Identifier.ValueText != yWalker.IdentifierNames[i].Identifier.ValueText)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool TryGetMemberName(ExpressionSyntax expression, out string name)
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

        private static bool IsRoot(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    return memberAccess.Expression is InstanceExpressionSyntax;
                case IdentifierNameSyntax identifierName:
                    return !IdentifierTypeWalker.IsLocalOrParameter(identifierName);
                default:
                    return false;
            }
        }
    }
}
