namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyDeclarationComparer : IComparer<PropertyDeclarationSyntax>
    {
        public static readonly PropertyDeclarationComparer Default = new PropertyDeclarationComparer();

        public static int Compare(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
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

            if (IsInitializedWith(x, y))
            {
                return 1;
            }

            if (IsInitializedWith(y, x))
            {
                return -1;
            }

            var compare = CompareAccessability(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
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

        int IComparer<PropertyDeclarationSyntax>.Compare(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y) => Compare(x, y);

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

        private static int CompareAccessability(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            return MemberDeclarationComparer.CompareAccessability(Accessibility(x), Accessibility(y));
        }

        private static int CompareSetterAccessability(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            if (x.TryGetSetter(out var xSetter))
            {
                if (y.TryGetSetter(out var ySetter))
                {
                    return MemberDeclarationComparer.CompareAccessability(
                        ySetter.Modifiers.Accessibility(Accessibility(y)),
                        xSetter.Modifiers.Accessibility(Accessibility(x)));
                }

                return 1;
            }

            return y.TryGetSetter(out _) ? -1 : 0;
        }

        private static Accessibility Accessibility(PropertyDeclarationSyntax method)
        {
            if (method.ExplicitInterfaceSpecifier != null)
            {
                return Microsoft.CodeAnalysis.Accessibility.Public;
            }

            return method.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
        }
    }
}
