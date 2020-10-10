namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class FieldDeclarationComparer : IComparer<FieldDeclarationSyntax>
    {
        /// <summary> The default instance. </summary>
        public static readonly FieldDeclarationComparer Default = new FieldDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
        public static int Compare(FieldDeclarationSyntax? x, FieldDeclarationSyntax? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
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

            var compare = MemberDeclarationComparer.CompareAccessibility(x.Modifiers, y.Modifiers, Accessibility.Private);
            if (compare != 0)
            {
                return compare;
            }

            compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareReadOnly(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareBackingProperty(x, y);
            if (compare != 0)
            {
                return compare;
            }

            return MemberDeclarationComparer.CompareSpanStart(x, y);
        }

        /// <inheritdoc />
        int IComparer<FieldDeclarationSyntax>.Compare(FieldDeclarationSyntax? x, FieldDeclarationSyntax? y) => Compare(x, y);

        private static bool IsInitializedWith(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (y.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword) &&
                x.Declaration.Variables.TryLast(out var variable) &&
                variable.Initializer is { Value: { } value } initializer &&
                !(value is LiteralExpressionSyntax))
            {
                using var walker = IdentifierNameWalker.Borrow(initializer);
                foreach (var identifierName in walker.IdentifierNames)
                {
                    if (y.Declaration.TryFindVariable(identifierName.Identifier.ValueText, out _))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int CompareReadOnly(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            return Index(x).CompareTo(Index(y));

            static int Index(FieldDeclarationSyntax field)
            {
                if (field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    return 0;
                }

                return 1;
            }
        }

        private static int CompareBackingProperty(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (TryGetSetter(x, out var xSetter) &&
                TryGetSetter(y, out var ySetter))
            {
                return xSetter.SpanStart.CompareTo(ySetter.SpanStart);
            }

            return 0;
        }

        /// <summary>
        /// Try get single setter assigning the field.
        /// </summary>
        /// <param name="field">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="setter">The single setter accessor assigning the field.</param>
        /// <returns>True if a single setter was found.</returns>
        private static bool TryGetSetter(FieldDeclarationSyntax field, [NotNullWhen(true)] out AccessorDeclarationSyntax? setter)
        {
            setter = null;
            if (field is { Declaration: { Variables: { Count: 1 } variables } } &&
                variables[0].Identifier is { ValueText: { } name })
            {
                using var walker = SpecificIdentifierNameWalker.Borrow(field.Parent, name);
                foreach (var identifierName in walker.IdentifierNames)
                {
                    var node = identifierName.Parent is MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax _ } memberAccess
                        ? (ExpressionSyntax)memberAccess
                        : identifierName;

                    if (IsAssigning() &&
                        node.TryFirstAncestor<AccessorDeclarationSyntax>(out var accessor) &&
                        accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        setter = accessor;
                    }

                    bool IsAssigning()
                    {
                        return node switch
                        {
                            { Parent: AssignmentExpressionSyntax { Right: IdentifierNameSyntax { Identifier: { ValueText: "value" } } } } => true,
                            { Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Arguments: { Count: 2 }, Parent: InvocationExpressionSyntax invocation } } }
                                when invocation.TryGetMethodName(out var methodName) &&
                                     methodName == "SetValue"
                                => true,
                            { Parent: ArgumentSyntax { RefKindKeyword: { ValueText: "ref" }, Parent: ArgumentListSyntax { Arguments: { } arguments } } }
                                when arguments.Count >= 2
                                     => true,
                            _ => false,
                        };
                    }
                }
            }

            return setter != null;
        }
    }
}
