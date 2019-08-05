namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class PropertyDeclarationComparer : IComparer<PropertyDeclarationSyntax>
    {
        /// <summary> The default instance. </summary>
        public static readonly PropertyDeclarationComparer Default = new PropertyDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
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

            var compare = MemberDeclarationComparer.CompareAccessibility(Accessibility(x), Accessibility(y));
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

            if (x.IsGetOnly())
            {
                if (!y.IsGetOnly())
                {
                    return -1;
                }
            }
            else if (y.IsGetOnly())
            {
                return 1;
            }

            return MemberDeclarationComparer.CompareSpanStart(x.SpanStart, y.SpanStart);
        }

        /// <inheritdoc />
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

        private static int CompareSetterAccessability(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            if (x.TryGetSetter(out var xSetter))
            {
                if (y.TryGetSetter(out var ySetter))
                {
                    return MemberDeclarationComparer.CompareAccessibility(
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
