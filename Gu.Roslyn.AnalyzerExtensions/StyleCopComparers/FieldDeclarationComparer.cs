namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System;
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
                if (x.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                    y.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    var byMember = CompareBackingMember(x, y);
                    if (byMember != 0)
                    {
                        return byMember;
                    }
                }

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

            compare = CompareBackingMember(x, y);
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

        private static int CompareBackingMember(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (TryGetMember(x, out var xSetter) &&
                TryGetMember(y, out var ySetter))
            {
                return xSetter.SpanStart.CompareTo(ySetter.SpanStart);
            }

            return 0;
        }

        /// <summary>
        /// Try get single result assigning the field.
        /// </summary>
        /// <param name="field">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <param name="result">The single result accessor assigning the field.</param>
        /// <returns>True if a single result was found.</returns>
        private static bool TryGetMember(FieldDeclarationSyntax field, [NotNullWhen(true)] out MemberDeclarationSyntax? result)
        {
            result = null;
            if (field is { Declaration: { Variables: { Count: 1 } variables } } &&
                variables[0].Identifier is { ValueText: { } name })
            {
                using var walker = SpecificIdentifierNameWalker.Borrow(field.Parent, name);
                foreach (var identifierName in walker.IdentifierNames)
                {
                    var node = identifierName.Parent is MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax _ } memberAccess
                        ? (ExpressionSyntax)memberAccess
                        : identifierName;

                    if (Member(node) is { } member)
                    {
                        if (ReferenceEquals(member, result))
                        {
                            continue;
                        }
                        else if (result is null)
                        {
                            result = member;
                        }
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
                }
            }

            return result != null;

            static MemberDeclarationSyntax? Member(ExpressionSyntax usage)
            {
                if (IsAssigning() &&
                    usage.TryFirstAncestor<AccessorDeclarationSyntax>(out var accessor) &&
                    accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                {
                    return accessor.FirstAncestor<PropertyDeclarationSyntax>();
                }

                if (usage.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Arguments: { Count: 2 }, Parent: InvocationExpressionSyntax setValue } } &&
                    setValue.TryGetMethodName(out var methodName) &&
                    methodName == "SetValue")
                {
                    if (usage.TryFirstAncestor<MethodDeclarationSyntax>(out var set) &&
                        set.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                        set.Identifier.ValueText.StartsWith("Set", StringComparison.Ordinal) &&
                        set.ParameterList.Parameters.Count == 1)
                    {
                        return set;
                    }

                    if (usage.TryFirstAncestor(out accessor) &&
                        accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        return accessor.FirstAncestor<PropertyDeclarationSyntax>();
                    }
                }

                if (usage.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Arguments: { Count: 1 }, Parent: InvocationExpressionSyntax getValue } } &&
                    getValue.TryGetMethodName(out methodName) &&
                    methodName == "GetValue")
                {
                    if (usage.TryFirstAncestor<MethodDeclarationSyntax>(out var method) &&
                        method.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                        method.Identifier.ValueText.StartsWith("Get", StringComparison.Ordinal) &&
                        method.ParameterList.Parameters.Count == 1)
                    {
                        return method;
                    }

                    if (usage.TryFirstAncestor(out accessor) &&
                        accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        return accessor.FirstAncestor<PropertyDeclarationSyntax>();
                    }
                }

                return null;

                bool IsAssigning()
                {
                    return usage switch
                    {
                        { Parent: AssignmentExpressionSyntax { Right: IdentifierNameSyntax { Identifier: { ValueText: "value" } } } } => true,
                        { Parent: ArgumentSyntax { RefKindKeyword: { ValueText: "ref" }, Parent: ArgumentListSyntax { Arguments: { } arguments } } }
                            when arguments.Count >= 2
                            => true,
                        _ => false,
                    };
                }
            }
        }
    }
}
