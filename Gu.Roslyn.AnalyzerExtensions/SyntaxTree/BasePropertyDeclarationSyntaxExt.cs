namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class BasePropertyDeclarationSyntaxExt
    {
        public static bool TryGetGetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.GetAccessorDeclaration), out result) == true;
        }

        public static bool TryGetSetter(this BasePropertyDeclarationSyntax property, out AccessorDeclarationSyntax result)
        {
            result = null;
            return property?.AccessorList?.Accessors.TryFirst(x => x.IsKind(SyntaxKind.SetAccessorDeclaration), out result) == true;
        }

        public static bool TryGetBackingField(this PropertyDeclarationSyntax property, out FieldDeclarationSyntax backingField)
        {
            if (TrySingleReturned(property, out var returned) &&
                property.Parent is TypeDeclarationSyntax type)
            {
                switch (returned)
                {
                    case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                        return type.TryFindField(memberAccess.Name.Identifier.ValueText, out backingField);
                    case IdentifierNameSyntax identifierName:
                        return type.TryFindField(identifierName.Identifier.ValueText, out backingField);
                }
            }

            backingField = null;
            return false;
        }

        public static bool TrySingleReturned(this PropertyDeclarationSyntax property, out ExpressionSyntax result)
        {
            result = null;
            if (property == null)
            {
                return false;
            }

            var expressionBody = property.ExpressionBody;
            if (expressionBody != null)
            {
                result = expressionBody.Expression;
                return result != null;
            }

            if (property.TryGetGetter(out var getter))
            {
                expressionBody = getter.ExpressionBody;
                if (expressionBody != null)
                {
                    result = expressionBody.Expression;
                    return result != null;
                }

                var body = getter.Body;
                if (body == null ||
                    body.Statements.Count == 0)
                {
                    return false;
                }

                if (body.Statements.TrySingle(out var statement) &&
                    statement is ReturnStatementSyntax returnStatement)
                {
                    result = returnStatement.Expression;
                    return result != null;
                }
            }

            return false;
        }
    }
}
