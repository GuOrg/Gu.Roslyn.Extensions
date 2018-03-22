namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class MemberPath
    {
        //internal static bool TryGet(IdentifierNameSyntax expression, out ExpressionSyntax path)
        //{
        //    ExpressionSyntax node = expression;
        //    while (TryGetMemberName(node, out _, out path))
        //    {
        //        node = path;
        //    }

        //    return !IdentifierTypeWalker.IsLocalOrParameter(path);
        //}

        public static bool Intersects(ExpressionSyntax path, ExpressionSyntax part)
        {
            if (Equals(path, part))
            {
                return true;
            }

            return false;
        }

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

            if (TryGetMemberName(x, out var xn, out var subx))
            {
                if (TryGetMemberName(y, out var yn, out var suby))
                {
                    if (xn != yn)
                    {
                        return false;
                    }

                    if (IsRoot(x))
                    {
                        return IsRoot(y);
                    }

                    return Equals(subx, suby);
                }

                return false;
            }

            return false;
        }

        public static bool TryGetMemberName(ExpressionSyntax expression, out string name, out ExpressionSyntax subExpression)
        {
            name = null;
            subExpression = null;
            switch (expression)
            {
                case IdentifierNameSyntax identifierName:
                    name = identifierName.Identifier.ValueText;
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    name = memberAccess.Name.Identifier.ValueText;
                    subExpression = memberAccess.Expression;
                    break;
                case MemberBindingExpressionSyntax memberBinding:
                    name = memberBinding.Name.Identifier.ValueText;
                    break;
                case ConditionalAccessExpressionSyntax conditionalAccess:
                    TryGetMemberName(conditionalAccess.WhenNotNull, out name, out _);
                    subExpression = conditionalAccess.Expression;
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
                    return !IdentifierNameWalker.IsLocalOrParameter(identifierName);
                default:
                    return false;
            }
        }
    }
}
