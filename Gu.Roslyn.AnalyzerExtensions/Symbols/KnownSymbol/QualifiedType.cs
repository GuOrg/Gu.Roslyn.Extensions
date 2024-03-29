﻿#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1720 // Identifier contains type name
#pragma warning disable CA1724 // Type names should not match namespaces
namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// For comparison with <see cref="ITypeSymbol"/>.
/// </summary>
[global::System.Diagnostics.DebuggerDisplay("{this.FullName}")]
#pragma warning disable GU0025 // Seal type with overridden equality.
public class QualifiedType
#pragma warning restore GU0025 // Seal type with overridden equality.
{
    /// <summary>
    /// A map type.FullName - alias
    /// System.Boolean - bool.
    /// </summary>
    protected static readonly IReadOnlyDictionary<string, string> TypeAliasMap = new Dictionary<string, string>
    {
        { typeof(bool).FullName!, "bool" },
        { typeof(byte).FullName!, "byte" },
        { typeof(sbyte).FullName!, "sbyte" },
        { typeof(char).FullName!, "char" },
        { typeof(decimal).FullName!, "decimal" },
        { typeof(double).FullName!, "double" },
        { typeof(float).FullName!, "float" },
        { typeof(int).FullName!, "int" },
        { typeof(uint).FullName!, "uint" },
        { typeof(long).FullName!, "long" },
        { typeof(ulong).FullName!, "ulong" },
        { typeof(object).FullName!, "object" },
        { typeof(short).FullName!, "short" },
        { typeof(ushort).FullName!, "ushort" },
        { typeof(string).FullName!, "string" },
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="QualifiedType"/> class.
    /// </summary>
    /// <param name="fullName">For example 'System.String'.</param>
    /// <param name="alias">For example 'string'.</param>
    public QualifiedType(string fullName, string? alias = null)
        : this(
              fullName ?? throw new ArgumentNullException(nameof(fullName)),
              NamespaceParts.Create(fullName),
              fullName.Substring(fullName.LastIndexOf('.') + 1),
              alias)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QualifiedType"/> class.
    /// </summary>
    /// <param name="fullName">For example 'System.String'.</param>
    /// <param name="namespace">The namespace parts.</param>
    /// <param name="type">The type metadata name.</param>
    /// <param name="alias">For example 'string'.</param>
    protected QualifiedType(string fullName, NamespaceParts @namespace, string type, string? alias = null)
    {
        this.FullName = fullName;
        this.Namespace = @namespace;
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Alias = alias ?? (type.EndsWith("Attribute", StringComparison.Ordinal)
            ? type.Substring(0, type.Length - 9)
            : null);
    }

    /// <summary>
    /// Gets the fully qualified name of the type.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Gets the namespace.
    /// </summary>
    public NamespaceParts Namespace { get; }

    /// <summary>
    /// Gets the type name.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the type alias, can be null.
    /// </summary>
    public string? Alias { get; }

#pragma warning disable CA1062 // Validate arguments of public methods

    /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="ITypeSymbol"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if found equal.</returns>
    public static bool operator ==(ITypeSymbol? left, QualifiedType right) => right.Equals(left);

    /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="ITypeSymbol"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if not found equal.</returns>
    public static bool operator !=(ITypeSymbol? left, QualifiedType right) => !right.Equals(left);

    /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="BaseTypeSyntax"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if found equal.</returns>
    public static bool operator ==(BaseTypeSyntax? left, QualifiedType right) => right.Equals(left);

    /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="BaseTypeSyntax"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if not found equal.</returns>
    public static bool operator !=(BaseTypeSyntax? left, QualifiedType right) => !right.Equals(left);

    /// <summary> Check if <paramref name="left"/> is the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="TypeSyntax"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if found equal.</returns>
    public static bool operator ==(TypeSyntax? left, QualifiedType right) => right.Equals(left);

    /// <summary> Check if <paramref name="left"/> is not the type described by <paramref name="right"/>. </summary>
    /// <param name="left">The <see cref="TypeSyntax"/>.</param>
    /// <param name="right">The <see cref="QualifiedType"/>.</param>
    /// <returns>True if not found equal.</returns>
    public static bool operator !=(TypeSyntax? left, QualifiedType right) => !right.Equals(left);

#pragma warning restore CA1062 // Validate arguments of public methods

    /// <summary>
    /// Create a <see cref="QualifiedType"/> from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type to use FullName.</param>
    /// <returns>A <see cref="QualifiedType"/>.</returns>
    public static QualifiedType FromType(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.FullName is null)
        {
            throw new ArgumentException("FullName is null", nameof(type));
        }

        if (type.IsArray)
        {
            return new QualifiedArrayType(FromType(type.GetElementType()!));
        }

        if (type.IsConstructedGenericType)
        {
            return new QualifiedGenericType(type.FullName, type.GenericTypeArguments.Select(FromType).ToImmutableArray());
        }

        return TypeAliasMap.TryGetValue(type.FullName, out var alias)
            ? new QualifiedType(type.FullName, alias)
            : new QualifiedType(type.FullName);
    }

    /// <summary>
    /// Calls compilation.GetTypeByMetadataName(this.FullName).
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/>.</param>
    /// <returns>The <see cref="INamedTypeSymbol"/>.</returns>
    public virtual ITypeSymbol? GetTypeSymbol(Compilation compilation)
    {
        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        return compilation.GetTypeByMetadataName(this.FullName);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj switch
        {
            QualifiedType t => this.Equals(t),
            ITypeSymbol t => this.Equals(t),
            TypeSyntax t => this.Equals(t),
            BaseTypeSyntax t => this.Equals(t),
            _ => false,
        };
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.FullName.GetHashCode();
    }

    /// <summary>
    /// Check if equal.
    /// </summary>
    /// <param name="other">The other instance.</param>
    /// <returns>True if equal.</returns>
    protected virtual bool Equals(QualifiedType other)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
        return string.Equals(this.FullName, other.FullName, StringComparison.Ordinal);
#pragma warning restore CA1062 // Validate arguments of public methods
    }

    /// <summary>
    /// Check if equal.
    /// </summary>
    /// <param name="type">The <see cref="ITypeSymbol"/>.</param>
    /// <returns>True if equal.</returns>
    protected virtual bool Equals(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        return this.NameEquals(type.MetadataName) &&
               type.ContainingNamespace == this.Namespace;
    }

    /// <summary>
    /// Check if equal.
    /// </summary>
    /// <param name="type">The <see cref="TypeSyntax"/>.</param>
    /// <returns>True if equal.</returns>
    protected virtual bool Equals(TypeSyntax? type)
    {
        if (type is null)
        {
            return false;
        }

        return type switch
        {
            PredefinedTypeSyntax { Keyword: { } keyword } => keyword.ValueText == this.Alias,
            ArrayTypeSyntax array => this is QualifiedArrayType { ElementType: { } elementType } &&
                                     elementType.Equals(array.ElementType),
            NullableTypeSyntax { ElementType: { } elementType } => this is QualifiedGenericType { Type: "Nullable`1", TypeArguments: { Length: 1 } typeArguments } &&
                                                                   typeArguments.TrySingle(out var typeArg) &&
                                                                   typeArg.Equals(elementType),
            GenericNameSyntax genericName => this.Type.IsParts(genericName.Identifier.ValueText, "`", genericName.Arity.ToString(CultureInfo.InvariantCulture)),
            SimpleNameSyntax simple => this.NameEquals(simple.Identifier.ValueText) ||
                                       Aliased(simple),
            QualifiedNameSyntax qualified => this.Equals(qualified.Right) &&
                       this.Namespace.Matches(qualified.Left),

            _ => false,
        };
        bool Aliased(SimpleNameSyntax name)
        {
            if (!name.Parent.IsKind(SyntaxKind.QualifiedName) &&
                !name.Parent.IsKind(SyntaxKind.UsingDirective) &&
                AliasWalker.TryGet(type.SyntaxTree, this, out var directive))
            {
                return directive.Alias?.Name.Identifier.Text == name.Identifier.Text;
            }

            return false;
        }
    }

    /// <summary>
    /// Check if equal.
    /// </summary>
    /// <param name="baseType">The <see cref="BaseTypeSyntax"/>.</param>
    /// <returns>True if equal.</returns>
    protected virtual bool Equals(BaseTypeSyntax? baseType) => this.Equals(baseType?.Type);

    /// <summary>
    /// Check if <paramref name="name"/> matches <see cref="Type"/> or <see cref="Alias"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>True if match.</returns>
    protected virtual bool NameEquals(string name)
    {
        return name == this.Type ||
               (this.Alias is { } &&
                name == this.Alias);
    }

    /// <summary>
    /// Contains types from the System namespace.
    /// </summary>
    public static class System
    {
        /// <summary> System.Void. </summary>
        public static readonly QualifiedType Void = new("System.Void", "void");

        /// <summary> System.Object. </summary>
        public static readonly QualifiedType Object = new("System.Object", "object");

        /// <summary> System.Nullable. </summary>
        public static readonly QualifiedType Nullable = new("System.Nullable");

        /// <summary> System.Nullable`1. </summary>
        public static readonly QualifiedType NullableOfT = new("System.Nullable`1");

        /// <summary> System.Boolean. </summary>
        public static readonly QualifiedType Boolean = new("System.Boolean", "bool");

        /// <summary> System.String. </summary>
        public static readonly QualifiedType String = new("System.String", "string");

        /// <summary> System.StringComparison. </summary>
        public static readonly QualifiedType StringComparison = new("System.StringComparison");

        /// <summary> System.ObsoleteAttribute. </summary>
        public static readonly QualifiedType ObsoleteAttribute = new("System.ObsoleteAttribute");

        /// <summary> System.CodeDom. </summary>
        public static class CodeDom
        {
            /// <summary> System.CodeDom.Compiler. </summary>
            public static class Compiler
            {
                /// <summary> System.Runtime.CompilerServices.GeneratedCodeAttribute. </summary>
                public static readonly QualifiedType GeneratedCodeAttribute = new("System.CodeDom.Compiler.GeneratedCodeAttribute");
            }
        }

        /// <summary> System.CodeDom. </summary>
        public static class Collections
        {
            /// <summary> System.CodeDom.Compiler. </summary>
            public static class Generic
            {
                /// <summary> System.Runtime.CompilerServices.GeneratedCodeAttribute. </summary>
                public static readonly QualifiedType IEqualityComparerOfT = new("System.Collections.Generic.IEqualityComparer`1");
            }
        }

        /// <summary> System.Linq. </summary>
        public static class Linq
        {
            /// <summary> System.Linq.Expressions.Expression. </summary>
            internal static readonly QualifiedType Expression = new("System.Linq.Expressions.Expression");
        }

        /// <summary> System.Runtime. </summary>
        public static class Runtime
        {
            /// <summary> System.Runtime.CompilerServices. </summary>
            public static class CompilerServices
            {
                /// <summary> System.Runtime.CompilerServices.RuntimeHelpers. </summary>
                public static readonly QualifiedType RuntimeHelpers = new("System.Runtime.CompilerServices.RuntimeHelpers");

                /// <summary> System.Runtime.CompilerServices.CallerMemberNameAttribute. </summary>
                public static readonly QualifiedType CallerMemberNameAttribute = new("System.Runtime.CompilerServices.CallerMemberNameAttribute");

                /// <summary> System.Runtime.CompilerServices.CompilerGeneratedAttribute. </summary>
                public static readonly QualifiedType CompilerGeneratedAttribute = new("System.Runtime.CompilerServices.CompilerGeneratedAttribute");
            }
        }
    }
}
