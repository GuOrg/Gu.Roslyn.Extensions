namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <inheritdoc />
public sealed class MethodDeclarationComparer : IComparer<MethodDeclarationSyntax>
{
    /// <summary> The default instance. </summary>
    public static readonly MethodDeclarationComparer Default = new();

    /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
    /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
    /// <param name="x">The first node to compare.</param>
    /// <param name="y">The second node to compare.</param>
    public static int Compare(MethodDeclarationSyntax? x, MethodDeclarationSyntax? y)
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

        if (x.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            x.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            IsInWpfContext(x) &&
            TryGetWpfSortIndex(x, out var xIndex) &&
            TryGetWpfSortIndex(y, out var yIndex))
        {
            return xIndex.CompareTo(yIndex);
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

        return MemberDeclarationComparer.CompareSpanStart(x, y);

        static bool IsInWpfContext(SyntaxNode? node)
        {
            return node switch
            {
                CompilationUnitSyntax compilationUnit => HasSystemWindows(compilationUnit.Usings),
                NamespaceDeclarationSyntax namespaceDeclaration => HasSystemWindows(namespaceDeclaration.Usings) || IsInWpfContext(node.Parent),
                null => false,
                _ => IsInWpfContext(node.Parent),
            };

            static bool HasSystemWindows(SyntaxList<UsingDirectiveSyntax> usings)
            {
                foreach (var @using in usings)
                {
                    if (@using.Name is QualifiedNameSyntax { Left: IdentifierNameSyntax { Identifier.ValueText: "System" }, Right: IdentifierNameSyntax { Identifier.ValueText: "Windows" } })
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    /// <inheritdoc />
    int IComparer<MethodDeclarationSyntax>.Compare(MethodDeclarationSyntax? x, MethodDeclarationSyntax? y) => Compare(x, y);

    private static Accessibility Accessibility(MethodDeclarationSyntax declaration)
    {
        if (declaration.ExplicitInterfaceSpecifier is { })
        {
            return Microsoft.CodeAnalysis.Accessibility.Public;
        }

        return declaration.Modifiers.Accessibility(Microsoft.CodeAnalysis.Accessibility.Private);
    }

    private static bool TryGetWpfSortIndex(MethodDeclarationSyntax method, out long result)
    {
        long callbackStart = int.MaxValue;
        result = method switch
        {
            { Identifier.ValueText: { } name, ParameterList.Parameters.Count: 1 }
                when name.StartsWith("Get", StringComparison.Ordinal) &&
                     FindInvocation(method, "GetValue") is { ArgumentList.Arguments: { Count: 1 } arguments } &&
                     arguments[0].Expression is IdentifierNameSyntax fieldName &&
                     FindDeclaration(fieldName) is { } field
                => field.SpanStart,
            { ReturnType: PredefinedTypeSyntax { Keyword.ValueText: "void" }, Identifier.ValueText: { } name, ParameterList.Parameters: { Count: 2 } parameters }
                when
                name.StartsWith("Set", StringComparison.Ordinal) && FindInvocation(method, "SetValue") is { ArgumentList.Arguments: { Count: 2 } arguments } &&
                arguments[0].Expression is IdentifierNameSyntax fieldName && arguments[1].Expression is IdentifierNameSyntax arg1 &&
                arg1.Identifier.ValueText == parameters[1].Identifier.ValueText && FindDeclaration(fieldName) is { } field
                => field.Declaration.SpanStart,
            { ReturnType: PredefinedTypeSyntax { Keyword.ValueText: "void" }, ParameterList.Parameters: { Count: 2 } parameters }
                when parameters[0].Type is IdentifierNameSyntax { Identifier.ValueText: "DependencyObject" } &&
                     parameters[1].Type is IdentifierNameSyntax { Identifier.ValueText: "DependencyPropertyChangedEventArgs" } &&
                     FindCallbackInvocation() is { } usage
                => callbackStart + usage.SpanStart,
            { ReturnType: PredefinedTypeSyntax { Keyword.ValueText: "object" }, ParameterList.Parameters: { Count: 2 } parameters }
                when parameters[0].Type is IdentifierNameSyntax { Identifier.ValueText: "DependencyObject" } &&
                     parameters[1].Type is PredefinedTypeSyntax { Keyword.ValueText: "object" } &&
                     FindCallbackInvocation() is { } usage
                => callbackStart + usage.SpanStart,
            { ReturnType: PredefinedTypeSyntax { Keyword.ValueText: "bool" }, ParameterList.Parameters: { Count: 1 } parameters }
                when parameters[0].Type is PredefinedTypeSyntax { Keyword.ValueText: "object" } &&
                     FindCallbackInvocation() is { } usage
                => callbackStart + usage.SpanStart,
            _ => -1,
        };

        return result != -1;

        static InvocationExpressionSyntax? FindInvocation(SyntaxNode scope, string name)
        {
            InvocationExpressionSyntax? match = null;
            using var walker = InvocationWalker.Borrow(scope);
            foreach (var invocation in walker.Invocations)
            {
                if (invocation.TryGetMethodName(out var candidate) &&
                    candidate == name)
                {
                    if (match is null)
                    {
                        match = invocation;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return match;
        }

        FieldDeclarationSyntax? FindDeclaration(ExpressionSyntax expression)
        {
            return expression switch
            {
                IdentifierNameSyntax identifierName
                    when FindField(identifierName.Identifier.ValueText) is { Declaration.Variables: { Count: 1 } variables } field &&
                         variables[0] is { Initializer.Value: { } value }
                    =>
                    value switch
                    {
                        InvocationExpressionSyntax _ => field,
                        MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax key }
                            when key != expression
                            => FindDeclaration(key),
                        _ => null,
                    },
                _ => null,
            };

            FieldDeclarationSyntax? FindField(string name)
            {
                return method.Parent switch
                {
                    ClassDeclarationSyntax declaration => declaration.FindField(name),
                    StructDeclarationSyntax declaration => declaration.FindField(name),
                    _ => null,
                };
            }
        }

        IdentifierNameSyntax? FindCallbackInvocation()
        {
            if (method.Parent is null)
            {
                return null;
            }

            using var walker = SpecificIdentifierNameWalker.Borrow(method.Parent, method.Identifier.ValueText);
            if (walker.IdentifierNames.TrySingle(out var candidate))
            {
                if (ContainingArgument() is { } argument &&
                    argument.FirstAncestorOrSelf<InvocationExpressionSyntax>() is { } invocation &&
                    invocation.MethodName() is { } methodName &&
                    methodName.StartsWith("Register", StringComparison.Ordinal))
                {
                    return candidate;
                }

                ArgumentSyntax? ContainingArgument()
                {
                    return candidate switch
                    {
                        { Parent: ArgumentSyntax containing } => containing,
                        { Parent: InvocationExpressionSyntax parent }
                            when parent.FirstAncestorOrSelf<ArgumentSyntax>() is { } containing
                            => containing,
                        _ => null,
                    };
                }
            }

            return null;
        }
    }
}
