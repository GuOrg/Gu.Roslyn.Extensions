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

Extension methods for symbols.

### TrySingleDeclaration

Get the declaration if it exists. If the symbol is from a binary reference the declaration will not exist.



