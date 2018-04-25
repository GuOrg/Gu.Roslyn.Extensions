# Gu.Roslyn.Extensions
Extensions for analyzers &amp; code fixes.

[![Build status](https://ci.appveyor.com/api/projects/status/ipk8pqx4n4m7y8u8/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-roslyn-extensions/branch/master)


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

# StyleCopComparares

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

