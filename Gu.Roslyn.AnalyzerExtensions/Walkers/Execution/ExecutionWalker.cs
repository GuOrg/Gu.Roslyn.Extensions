﻿namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Walks code as it is executed.
/// </summary>
/// <typeparam name="T">The inheriting type.</typeparam>
public abstract class ExecutionWalker<T> : PooledWalker<T>
    where T : ExecutionWalker<T>
{
    /// <summary>
    /// Gets or sets if the walker should walk declarations of invoked methods etc.
    /// </summary>
    protected SearchScope SearchScope { get; set; }

    /// <summary>
    /// Gets the <see cref="SemanticModel"/>.
    /// </summary>
    protected SemanticModel SemanticModel => this.Recursion.SemanticModel;

    /// <summary>
    /// Gets the containing <see cref="INamedTypeSymbol"/> where the recursion started.
    /// </summary>
    protected INamedTypeSymbol ContainingType => this.Recursion.ContainingType;

    /// <summary>
    /// Gets the <see cref="CancellationToken"/>.
    /// </summary>
    protected CancellationToken CancellationToken => this.Recursion.CancellationToken;

#pragma warning disable IDISP008 // Don't assign member with injected and created disposables.
    /// <summary>
    /// Gets the <see cref="Recursion"/>.
    /// </summary>
    protected Recursion Recursion { get; private set; } = null!;
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables.

    /// <inheritdoc />
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        this.VisitTypeDeclaration(node);
    }

    /// <inheritdoc />
    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        this.VisitTypeDeclaration(node);
    }

    /// <inheritdoc />
    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (this.SearchScope != SearchScope.Member &&
            node.Initializer is null &&
            this.SemanticModel.TryGetSymbol(node, this.CancellationToken, out var ctor) &&
            ctor.ContainingType is { BaseType: { } baseType } &&
            Constructor.TryFindDefault(baseType, Search.Recursive, out var defaultCtor) &&
            defaultCtor.TrySingleDeclaration(this.CancellationToken, out ConstructorDeclarationSyntax? defaultCtorDeclaration))
        {
            this.Visit(defaultCtorDeclaration);
        }

        base.VisitConstructorDeclaration(node);
    }

    /// <inheritdoc />
    public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
    {
        base.VisitConstructorInitializer(node);
        switch (this.SearchScope)
        {
            case SearchScope.Member:
                break;
            case SearchScope.Instance:
            case SearchScope.Type:
            case SearchScope.Recursive:
                if (this.Recursion.Target(node) is { Declaration: { } declaration })
                {
                    this.Visit(declaration);
                }

                break;
            default:
                throw new InvalidOperationException($"Not handling member {this.SearchScope}.");
        }
    }

    /// <inheritdoc />
    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (this.SearchScope == SearchScope.Member)
        {
            base.VisitObjectCreationExpression(node);
            return;
        }

        if (this.Recursion.Target(node.Type) is { Symbol: { } containingType, Declaration: { } containingTypeDeclaration } &&
            ShouldVisit(containingType))
        {
            using (var walker = TypeDeclarationWalker.Borrow(containingTypeDeclaration))
            {
                foreach (var memberInitializer in walker.Initializers)
                {
                    this.Visit(memberInitializer);
                }
            }

            this.Visit(node.Type);
            if (node.ArgumentList is { } argumentList)
            {
                this.VisitArgumentList(argumentList);
            }

            if (this.Recursion.Target(node) is { Symbol: { }, Declaration: { } declaration })
            {
                this.Visit(declaration);
            }

            if (node.Initializer is { } objectInitializer)
            {
                this.VisitInitializerExpression(objectInitializer);
            }
        }

        bool ShouldVisit(ITypeSymbol type)
        {
            return this.SearchScope switch
            {
                SearchScope.Recursive => true,
                _ => TypeSymbolComparer.Equal(type, this.ContainingType),
            };
        }
    }

    /// <inheritdoc />
    public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (this.SearchScope == SearchScope.Member)
        {
            base.VisitImplicitObjectCreationExpression(node);
            return;
        }

        if (this.Recursion.TargetType(node) is { Symbol: { } containingType, Declaration: { } containingTypeDeclaration } &&
            ShouldVisit(containingType))
        {
            using (var walker = TypeDeclarationWalker.Borrow(containingTypeDeclaration))
            {
                foreach (var memberInitializer in walker.Initializers)
                {
                    this.Visit(memberInitializer);
                }
            }

            if (node.ArgumentList is { } argumentList)
            {
                this.VisitArgumentList(argumentList);
            }

            if (this.Recursion.Target(node) is { Symbol: { }, Declaration: { } declaration })
            {
                this.Visit(declaration);
            }

            if (node.Initializer is { } objectInitializer)
            {
                this.VisitInitializerExpression(objectInitializer);
            }
        }

        bool ShouldVisit(ITypeSymbol type)
        {
            return this.SearchScope switch
            {
                SearchScope.Recursive => true,
                _ => TypeSymbolComparer.Equal(type, this.ContainingType),
            };
        }
    }

    /// <inheritdoc />
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        base.VisitInvocationExpression(node);
        if (this.TryGetTargetSymbol<InvocationExpressionSyntax, IMethodSymbol, MethodDeclarationSyntax>(node, out var target) &&
            target.Declaration is { } declaration)
        {
            this.Visit(declaration);
        }
    }

    /// <inheritdoc />
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        base.VisitIdentifierName(node);
        if (this.TryGetTargetSymbol<IdentifierNameSyntax, IPropertySymbol, PropertyDeclarationSyntax>(node, out var target) &&
            target.Declaration is { } declaration)
        {
            if (this.IsPropertyGetAndSet(node))
            {
                this.Visit(declaration.Getter());
                this.Visit(declaration.Setter());
            }
            else if (this.IsPropertySet(node))
            {
                this.Visit(declaration.Setter());
            }
            else if (target.Symbol.TryGetGetMethodDeclaration(this.CancellationToken, out var getter))
            {
                this.Visit(getter);
            }
        }
    }

    /// <inheritdoc />
    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        if (node is { Left: { } left, Right: { } right })
        {
            this.Visit(right);
            this.Visit(left);
        }
    }

#pragma warning disable CA1068 // CancellationToken parameters must come last
    /// <summary>
    /// Returns a walker that have visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The <see cref="SyntaxNode"/>.</param>
    /// <param name="scope">The scope to walk.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <param name="create">The factory for creating a walker if not found in cache.</param>
    /// <returns>The walker that have visited <paramref name="node"/>.</returns>
    protected static T BorrowAndVisit(SyntaxNode node, SearchScope scope, SemanticModel semanticModel, CancellationToken cancellationToken, Func<T> create)
#pragma warning restore CA1068 // CancellationToken parameters must come last
    {
        if (node is null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (node.FirstAncestorOrSelf<TypeDeclarationSyntax>() is { } containingTypeDeclaration &&
            semanticModel.TryGetNamedType(containingTypeDeclaration, cancellationToken, out var containingType))
        {
            var walker = Borrow(create);

            // Not pretty below here, throwing is perhaps nicer, dunno.
            walker.SearchScope = scope == SearchScope.Member && node is TypeDeclarationSyntax ? SearchScope.Type : scope;
            walker.Recursion = Recursion.Borrow(containingType, semanticModel, cancellationToken);
            walker.Visit(node);
            walker.SearchScope = scope;
            return walker;
        }

        return Borrow(create);
    }

    /// <summary>
    /// Returns a walker that have visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The <see cref="SyntaxNode"/>.</param>
    /// <param name="scope">The scope to walk.</param>
    /// <param name="containingType">The <see cref="INamedTypeSymbol"/> where walking starts.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
    /// <param name="create">The factory for creating a walker if not found in cache.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The walker that have visited <paramref name="node"/>.</returns>
    protected static T BorrowAndVisit(SyntaxNode node, SearchScope scope, INamedTypeSymbol containingType, SemanticModel semanticModel, Func<T> create, CancellationToken cancellationToken)
    {
        var walker = Borrow(create);

        // Not pretty below here, throwing is perhaps nicer, dunno.
        walker.SearchScope = scope == SearchScope.Member && node is TypeDeclarationSyntax ? SearchScope.Type : scope;
        walker.Recursion = Recursion.Borrow(containingType, semanticModel, cancellationToken);
        walker.Visit(node);
        walker.SearchScope = scope;
        return walker;
    }

    /// <summary>
    /// Returns a walker that have visited <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The <see cref="SyntaxNode"/>.</param>
    /// <param name="scope">The scope to walk.</param>
    /// <param name="recursion">The <see cref="Recursion"/>.</param>
    /// <param name="create">The factory for creating a walker if not found in cache.</param>
    /// <returns>The walker that have visited <paramref name="node"/>.</returns>
    protected static T BorrowAndVisit(SyntaxNode node, SearchScope scope, Recursion recursion, Func<T> create)
    {
        if (recursion is null)
        {
            return Borrow(create);
        }

        var walker = Borrow(create);
        walker.SearchScope = scope;
        walker.Recursion = recursion;
        walker.Visit(node);
        walker.Recursion = null!;
        return walker;
    }

    /// <summary>
    /// Returns a walker that have visited <paramref name="node"/>.
    /// </summary>
    /// <typeparam name="TParent">The parent type.</typeparam>
    /// <param name="node">The <see cref="SyntaxNode"/>.</param>
    /// <param name="scope">The scope to walk.</param>
    /// <param name="parent">The parent <see cref="ExecutionWalker{TParent}"/>.</param>
    /// <param name="create">The factory for creating a walker if not found in cache.</param>
    /// <returns>The walker that have visited <paramref name="node"/>.</returns>
    protected static T BorrowAndVisit<TParent>(SyntaxNode node, SearchScope scope, ExecutionWalker<TParent> parent, Func<T> create)
        where TParent : ExecutionWalker<TParent>
    {
        if (parent is null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (create is null)
        {
            throw new ArgumentNullException(nameof(create));
        }

        var walker = Borrow(create);
        walker.SearchScope = scope;
        walker.Recursion = parent.Recursion;
        walker.Visit(node);
        walker.Recursion = null!;
        return walker;
    }

    /// <summary>
    /// Called by <see cref="VisitClassDeclaration"/> and <see cref="VisitStructDeclaration"/>
    /// Walks the members in the following order:
    /// 1. Field and property initializers in document order.
    /// 2. Non-private constructors in document order
    /// 3. Non-private members.
    /// 4. Nested types if scope is recursive.
    /// </summary>
    /// <param name="node">The <see cref="TypeDeclarationSyntax"/>.</param>
    protected virtual void VisitTypeDeclaration(TypeDeclarationSyntax node)
    {
        if (node is null)
        {
            return;
        }

        using var walker = TypeDeclarationWalker.Borrow(node);
        foreach (var initializer in walker.Initializers)
        {
            this.Visit(initializer);
        }

        foreach (var ctor in walker.Ctors)
        {
            if (this.SearchScope == SearchScope.Instance &&
                ctor.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                continue;
            }

            this.Visit(ctor);
        }

        foreach (var member in walker.Members)
        {
            this.Visit(member);
        }

        if (this.SearchScope == SearchScope.Recursive)
        {
            foreach (var type in walker.Types)
            {
                this.Visit(type);
            }
        }
    }

    /// <summary>
    /// Check if the current context is a property set.
    /// </summary>
    /// <param name="node">The current context.</param>
    /// <returns>True if <paramref name="node"/> is found to be a property set.</returns>
    protected virtual bool IsPropertySet(IdentifierNameSyntax node)
    {
        return node switch
        {
            { Parent: AssignmentExpressionSyntax assignment } => assignment.Left == node,
            { Parent: MemberAccessExpressionSyntax { Expression: InstanceExpressionSyntax _, Parent: AssignmentExpressionSyntax assignment } } => assignment.Left.Contains(node),
            _ => this.SearchScope switch
            {
                SearchScope.Member => false,
                SearchScope.Instance => false,
                SearchScope.Type => false,
                SearchScope.Recursive => node.TryFirstAncestor(out AssignmentExpressionSyntax? assignment) &&
                                         assignment.Left.Contains(node),
                _ => throw new InvalidOperationException($"Unhandled scope: {this.SearchScope}"),
            },
        };
    }

    /// <summary>
    /// Check if the current context is a property set.
    /// </summary>
    /// <param name="node">The current context.</param>
    /// <returns>True if <paramref name="node"/> is found to be a property set.</returns>
    protected virtual bool IsPropertyGetAndSet(IdentifierNameSyntax node)
    {
        return node switch
        {
            { Parent: PrefixUnaryExpressionSyntax _ } => true,
            { Parent: PostfixUnaryExpressionSyntax _ } => true,
            { Parent: MemberAccessExpressionSyntax { Expression: InstanceExpressionSyntax { }, Parent: PrefixUnaryExpressionSyntax _ } } => true,
            { Parent: MemberAccessExpressionSyntax { Expression: InstanceExpressionSyntax { }, Parent: PostfixUnaryExpressionSyntax _ } } => true,
            _ => this.SearchScope switch
            {
                SearchScope.Member => false,
                SearchScope.Instance => false,
                SearchScope.Type => false,
                SearchScope.Recursive => node.TryFirstAncestor(out PrefixUnaryExpressionSyntax? _) ||
                                         node.TryFirstAncestor(out PostfixUnaryExpressionSyntax? _),
                _ => throw new InvalidOperationException($"Unhandled scope: {this.SearchScope}"),
            },
        };
    }

    /// <summary>
    /// Try getting the target symbol for the node. Check if visited and that the symbol matches <see cref="SearchScope"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of <paramref name="node"/>.</typeparam>
    /// <typeparam name="TSymbol">The expected type.</typeparam>
    /// <typeparam name="TDeclaration">The expected declaration type.</typeparam>
    /// <param name="node">The <typeparamref name="TSource"/>.</param>
    /// <param name="target">The symbol and declaration if a match.</param>
    /// <param name="caller">The invoking method.</param>
    /// <param name="line">Line number in <paramref name="caller"/>.</param>
    /// <returns>True if a symbol was found.</returns>
    protected virtual bool TryGetTargetSymbol<TSource, TSymbol, TDeclaration>(TSource node, out Target<TSource, TSymbol, TDeclaration> target, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        where TSource : SyntaxNode
        where TSymbol : class, ISymbol
        where TDeclaration : SyntaxNode
    {
        target = default;
        if (node is null ||
            this.SearchScope == SearchScope.Member)
        {
            return false;
        }

        if (node.TryFirstAncestor(out InvocationExpressionSyntax? invocation) &&
            invocation.TryGetMethodName(out var name) &&
            name == "nameof")
        {
            return false;
        }

        if (this.SearchScope == SearchScope.Instance &&
            node.Parent is MemberAccessExpressionSyntax { Expression: InstanceExpressionSyntax expression } &&
            expression.IsEither(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
        {
            return false;
        }

        if (this.Recursion.Target<TSource, TSymbol, TDeclaration>(node, caller, line) is { Symbol: { } symbol, Declaration: { } } t)
        {
            if (this.SearchScope == SearchScope.Instance &&
                symbol.IsStatic)
            {
                return false;
            }

            if (this.SearchScope.IsEither(SearchScope.Instance, SearchScope.Type) &&
                this.ContainingType?.IsAssignableTo(symbol.ContainingType, this.SemanticModel.Compilation) != true)
            {
                return false;
            }

            target = t;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override void Clear()
    {
        this.Recursion?.Dispose();
        this.Recursion = null!;
    }

    private sealed class TypeDeclarationWalker : PooledWalker<TypeDeclarationWalker>
    {
#pragma warning disable SA1401 // Fields must be private
        internal readonly List<EqualsValueClauseSyntax> Initializers = new();
        internal readonly List<ConstructorDeclarationSyntax> Ctors = new();
        internal readonly List<MemberDeclarationSyntax> Members = new();
        internal readonly List<TypeDeclarationSyntax> Types = new();
#pragma warning restore SA1401 // Fields must be private

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.Declaration is { Variables: { } variables } &&
                variables.TryLast(out var variable) &&
                variable.Initializer is { } equalsValueClause)
            {
                this.Initializers.Add(equalsValueClause);
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node.Initializer is { } equalsValueClause)
            {
                this.Initializers.Add(equalsValueClause);
            }

            if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                this.Members.Add(node);
            }
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                if (this.Ctors.Count > 0 &&
                    node.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    this.Ctors.Insert(0, node);
                }
                else
                {
                    this.Ctors.Add(node);
                }
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (!node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                this.Members.Add(node);
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            this.Types.Add(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            this.Types.Add(node);
        }

        internal static TypeDeclarationWalker Borrow(TypeDeclarationSyntax typeDeclaration)
        {
            var walker = Borrow(() => new TypeDeclarationWalker());
            foreach (var member in typeDeclaration.Members)
            {
                walker.Visit(member);
            }

            return walker;
        }

        protected override void Clear()
        {
            this.Initializers.Clear();
            this.Ctors.Clear();
            this.Members.Clear();
            this.Types.Clear();
        }
    }
}
