namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyDeclarationComparer : IComparer<BasePropertyDeclarationSyntax>
    {
        public static readonly PropertyDeclarationComparer Default = new PropertyDeclarationComparer();

        public static int Compare(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x is IndexerDeclarationSyntax &&
                !(y is IndexerDeclarationSyntax))
            {
                return 1;
            }

            if (!(x is IndexerDeclarationSyntax) &&
                y is IndexerDeclarationSyntax)
            {
                return -1;
            }

            if (x is PropertyDeclarationSyntax xProperty &&
                y is PropertyDeclarationSyntax yProperty)
            {
                if (IsInitializedWith(xProperty, yProperty))
                {
                    return 1;
                }

                if (IsInitializedWith(yProperty, xProperty))
                {
                    return -1;
                }
            }

            var compare = CompareAccessability(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareSetterAccessability(x, y);
            if (compare != 0)
            {
                return compare;
            }

            if (x.TryGetGetter(out _) &&
                !y.TryGetGetter(out _))
            {
                return -1;
            }

            if (!x.TryGetGetter(out _) &&
                y.TryGetGetter(out _))
            {
                return 1;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<BasePropertyDeclarationSyntax>.Compare(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y) => Compare(x, y);

        private static int CompareAccessability(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(BasePropertyDeclarationSyntax property)
            {
                if (property.ExplicitInterfaceSpecifier != null ||
                    property.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    return 0;
                }

                if (property.Modifiers.Any(SyntaxKind.ProtectedKeyword) &&
                    property.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return 1;
                }

                if (property.Modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return 2;
                }

                if (property.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    return 3;
                }

                if (property.Modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    return 4;
                }

                return 2;
            }
        }

        private static int CompareScope(SyntaxTokenList x, SyntaxTokenList y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.StaticKeyword))
                {
                    return 0;
                }

                return 1;
            }
        }

        private static bool IsInitializedWith(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            if (y.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                x.Initializer is EqualsValueClauseSyntax initializer &&
                !(initializer.Value is LiteralExpressionSyntax))
            {
                using (var walker = IdentifierNameWalker.Borrow(initializer))
                {
                    foreach (var identifierName in walker.IdentifierNames)
                    {
                        if (y.Identifier.ValueText == identifierName.Identifier.ValueText)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static int CompareSetterAccessability(BasePropertyDeclarationSyntax x, BasePropertyDeclarationSyntax y)
        {
            if (x.TryGetSetter(out var xSetter))
            {
                if (y.TryGetSetter(out var ySetter))
                {
                    return Index(xSetter.Modifiers).CompareTo(Index(ySetter.Modifiers));
                }

                return 1;
            }

            return y.TryGetSetter(out _) ? -1 : 0;

            int Index(SyntaxTokenList list)
            {
                if (list.Any(SyntaxKind.PublicKeyword))
                {
                    return 0;
                }

                if (list.Any(SyntaxKind.ProtectedKeyword) &&
                    list.Any(SyntaxKind.InternalKeyword))
                {
                    return 1;
                }

                if (list.Any(SyntaxKind.InternalKeyword))
                {
                    return 2;
                }

                if (list.Any(SyntaxKind.ProtectedKeyword))
                {
                    return 3;
                }

                if (list.Any(SyntaxKind.PrivateKeyword))
                {
                    return 4;
                }

                return 0;
            }
        }
    }
}
