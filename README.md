# Gu.Roslyn.Extensions
Extensions for analyzers &amp; code fixes.

[![Build status](https://ci.appveyor.com/api/projects/status/ipk8pqx4n4m7y8u8/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-extensions/branch/master)

- [Gu.Roslyn.Extensions](#guroslynextensions)
- [Pooled](#pooled)
  - [PooledSet<T>](#pooledset-t)
  - [StringBuilderPool](#stringbuilderpool)
- [StyleCopComparers](#stylecopcomparers)
- [Symbols](#symbols)
  - [QualifiedType](#qualifiedtype)
  - [QualifiedMember](#qualifiedmember)
  - [SymbolExt](#symbolext)
    - [TrySingleDeclaration](#trysingledeclaration)
  - [TypeSymbolExt](#typesymbolext)
    - [Is](#is)
    - [TryFindMember](#tryfindmember)
    - [TryFindMemberRecursive](#tryfindmemberrecursive)
- [Syntax](#syntax)
  - [ArgumentListSyntaxExt](#argumentlistsyntaxext)
    - [TryGetMatchingArgument](#trygetmatchingargument)
  - [ArgumentSyntaxExt](#argumentsyntaxext)
    - [TryGetStringValue](#trygetstringvalue)
    - [TryGetMatchingParameter](#trygetmatchingparameter)
  - [BasePropertyDeclarationSyntaxExt](#basepropertydeclarationsyntaxext)
    - [TryGetGetter and TryGetSetter](#trygetgetter-and-trygetsetter)
    - [IsGetOnly](#isgetonly)
    - [IsAutoProperty](#isautoproperty)
  - [SyntaxNodeExt](#syntaxnodeext)
    - [IsExecutedBefore](#isexecutedbefore)
  - [TypeSyntaxExt](#typesyntaxext)
    - [TryFindMember](#tryfindmember)
- [Walkers](#walkers)
  - [ExecutionWalker<T> : PooledWalker<T>](#executionwalker-t--pooledwalker-t)
    - [PooledWalker<T>](#pooledwalker-t)
    - [Cache<TKey, TValue>](#cache-tkey--tvalue)
    - [EnumarebleExt](#enumarebleext)
- [FixAll](#fixall)
  - [DocumentEditorCodeFixProvider](#documenteditorcodefixprovider)
- [CodeStyle](#codestyle)
  - [UnderscoreFields](#underscorefields)
  - [UsingDirectivesInsideNamespace](#usingdirectivesinsidenamespace)
  - [BackingFieldsAdjacent](#backingfieldsadjacent)
- [DocumentEditorExt](#documenteditorext)
  - [AddUsing](#addusing)
  - [AddField](#addfield)
  - [AddProperty](#addproperty)
  - [AddMethod](#addmethod)
- [Simplify](#simplify)
  - [WithSimplifiedNames<T>](#withsimplifiednames-t)
- [Trivia](#trivia)
  - [WithTriviaFrom](#withtriviafrom)
  - [WithLeadingTriviaFrom](#withleadingtriviafrom)
  - [WithTrailingTriviaFrom](#withtrailingtriviafrom)

# Pooled

## PooledSet<T>
```cs
using (var set = PooledSet<int>.Borrow())
{
}
```

Or when used recursively:
```cs
// set can be null here if so a new set its returned.
using (var current = set.IncrementUsage())
{
}

## StringBuilderPool
```cs
var text = StringBuilderPool.Borrow()
                            .AppendLine("a")
                            .AppendLine()
                            .AppendLine("b")
                            .Return();
```

# StyleCopComparers

Comparers that compare member declarations with stylecop order.

# Symbols

## QualifiedType
For comparing with `ITypeSymbol`

```cs
public static readonly QualifiedType Object = new QualifiedType("System.Object", "object");
```

## QualifiedMember
Same as `QualifiedType` but for members.

## SymbolExt

Extension methods for `ISymbol`.

### TrySingleDeclaration

Get the declaration if it exists. If the symbol is from a binary reference the declaration will not exist.

## TypeSymbolExt

Extension methods for `ItypeSymbol`.

### Is

For checking if `foo is Type`

### TryFindMember

Find members by name or predicate.

### TryFindMemberRecursive

Find members by name or predicate in type or base types.

# Syntax

## ArgumentListSyntaxExt

### TryGetMatchingArgument

Find the argument that matches the parameter.

## ArgumentSyntaxExt

### TryGetStringValue

Try get the constant string value of the argument.

### TryGetMatchingParameter

Try get the matching parameter

## BasePropertyDeclarationSyntaxExt

### TryGetGetter and TryGetSetter
Get the getter or setter if exists.

### IsGetOnly

Check if the property is `public int Value { get; }`

### IsAutoProperty

Check if the property is an auto property.

## SyntaxNodeExt

### IsExecutedBefore

Check if a node is executed before another node.

## TypeSyntaxExt

### TryFindMember

Helper methods for finding members by name or predicate.

# Walkers

## ExecutionWalker<T> : PooledWalker<T>

Base type for a walker that walks code in execution order. Use the `Search` enum to specify if walk should be recursive walking invoked methods etc.
Remember to clear locals in the `Clear` method.

```cs
internal sealed class AssignmentExecutionWalker : ExecutionWalker<AssignmentExecutionWalker>
{
    private readonly List<AssignmentExpressionSyntax> assignments = new List<AssignmentExpressionSyntax>();

    private AssignmentExecutionWalker()
    {
    }

    /// <summary>
    /// Gets a list with all <see cref="AssignmentExpressionSyntax"/> in the scope.
    /// </summary>
    public IReadOnlyList<AssignmentExpressionSyntax> Assignments => this.assignments;

    public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        this.assignments.Add(node);
        base.VisitAssignmentExpression(node);
    }

    internal static AssignmentExecutionWalker Borrow(SyntaxNode node, Search search, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        var walker = Borrow(() => new AssignmentExecutionWalker());
        walker.SemanticModel = semanticModel;
        walker.CancellationToken = cancellationToken;
        walker.Search = search;
        walker.Visit(node);
        return walker;
    }


    protected override void Clear()
    {
        this.assignments.Clear();
        base.Clear();
    }
}
```

### PooledWalker<T>

A pooled walker for reuse.
Remember to clear locals in the `Clear` method.

```cs
internal sealed class IdentifierNameWalker : PooledWalker<IdentifierNameWalker>
{
    private readonly List<IdentifierNameSyntax> identifierNames = new List<IdentifierNameSyntax>();

    private IdentifierNameWalker()
    {
    }

    public IReadOnlyList<IdentifierNameSyntax> IdentifierNames => this.identifierNames;

    public static IdentifierNameWalker Borrow(SyntaxNode node) => BorrowAndVisit(node, () => new IdentifierNameWalker());

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        this.identifierNames.Add(node);
        base.VisitIdentifierName(node);
    }

    protected override void Clear()
    {
        this.identifierNames.Clear();
    }
}
```

### Cache<TKey, TValue>

For caching expensive calls

```cs
public override void Initialize(AnalysisContext context)
{
    context.CacheToCompilationEnd<SyntaxTree, SemanticModel>();
}
```

### EnumarebleExt

Extension methods for enumarebls 
- TrySingle
- TryLast
- TryElementAt

# FixAll
## DocumentEditorCodeFixProvider
A fix all provider that use document editor for batch fixes.

```cs
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseParameterCodeFixProvider))]
[Shared]
internal class UseParameterCodeFixProvider : DocumentEditorCodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(GU0014PreferParameter.DiagnosticId);

    /// <inheritdoc/>
    protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                        .ConfigureAwait(false);

        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Properties.TryGetValue("Name", out var name))
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is MemberAccessExpressionSyntax memberAccess)
                {
                    context.RegisterCodeFix(
                        "Prefer parameter.",
                        (editor, _) => editor.ReplaceNode(
                            memberAccess,
                            SyntaxFactory.IdentifierName(name)
                                            .WithLeadingTriviaFrom(memberAccess)),
                        "Prefer parameter.",
                        diagnostic);
                }
                else if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is IdentifierNameSyntax identifierName)
                {
                    context.RegisterCodeFix(
                        "Prefer parameter.",
                        (editor, _) => editor.ReplaceNode(
                            identifierName,
                            identifierName.WithIdentifier(SyntaxFactory.Identifier(name))
                                            .WithLeadingTriviaFrom(identifierName)),
                        "Prefer parameter.",
                        diagnostic);
                }
            }
        }
    }
}
```

# CodeStyle

Helpers for determinining the code style used in the project.

## UnderscoreFields

## UsingDirectivesInsideNamespace

## BackingFieldsAdjacent 
Figures out if backing field is placed like stylecop wants it or adjacent to the property.

# DocumentEditorExt

Helpers for adding members sorted according to how StyleCop wants it.

## AddUsing

Adds the using sorted and figures out if it shoul add outside or insside the namespace from the current document then project.

## AddField 

Adds the field at the position StyleCop wants it.

## AddProperty 

Adds the property at the position StyleCop wants it.

## AddMethod 

Adds the method at the position StyleCop wants it.

# Simplify

## WithSimplifiedNames<T> 
Uses a syntax rewriter that adds `Simplifier.Annotation` to all `QualifiedNameSyntax`

# Trivia

Helpers for copying triviat from other nodes.

## WithTriviaFrom
Copy trivia from a node.

## WithLeadingTriviaFrom

Copy leading trivia from a node.

## WithTrailingTriviaFrom

Copy trailing trivia from a node.