namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// Helper methods for adding members sorted according to how StyleCop wants it.
    /// </summary>
    public static partial class DocumentEditorExt
    {
        /// <summary>
        /// Add the using directive and respect if the convenion is inside or outside of namespace and sorted with system first.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddUsing(this DocumentEditor editor, UsingDirectiveSyntax usingDirective)
        {
            editor.ReplaceNode(
                editor.OriginalRoot,
                (root, _) => AddUsing(root as CompilationUnitSyntax, editor.SemanticModel, usingDirective));

            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="field">The <see cref="FieldDeclarationSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddField(this DocumentEditor editor, TypeDeclarationSyntax containingType, FieldDeclarationSyntax field)
        {
            editor.ReplaceNode(containingType, (node, generator) => AddSorted(generator, (TypeDeclarationSyntax)node, field));
            return editor;
        }

        /// <summary>
        /// Add the backing field and figure out placement.
        /// StyleCop ordering is the default but it also checks for if field adjacent to property is used.
        /// The property is unchanged by this call.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="propertyDeclaration">The <see cref="FieldDeclarationSyntax"/></param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/></returns>
        public static FieldDeclarationSyntax AddBackingField(this DocumentEditor editor, PropertyDeclarationSyntax propertyDeclaration)
        {
            var property = editor.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
            var type = (TypeDeclarationSyntax)propertyDeclaration.Parent;
            var backingField = CreateBackingField(editor, propertyDeclaration);
            editor.ReplaceNode(
                type,
                (node, generator) => AddBackingField(editor, (TypeDeclarationSyntax)node, backingField, property.Name));
            return backingField;
        }

        /// <summary>
        /// Create a backing <see cref="FieldDeclarationSyntax"/> for the <paramref name="propertyDeclaration"/>
        /// Handles name collisions and reserved keywords.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="propertyDeclaration">The <see cref="FieldDeclarationSyntax"/></param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/></returns>
        public static FieldDeclarationSyntax CreateBackingField(this DocumentEditor editor, PropertyDeclarationSyntax propertyDeclaration)
        {
            var property = editor.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
            var name = editor.SemanticModel.UnderscoreFields()
                ? $"_{property.Name.ToFirstCharLower()}"
                : property.Name.ToFirstCharLower();
            while (property.ContainingType.MemberNames.Any(x => x == name))
            {
                name += "_";
            }

            if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
                SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None)
            {
                name = "@" + name;
            }

            return (FieldDeclarationSyntax)editor.Generator.FieldDeclaration(
                name,
                accessibility: Accessibility.Private,
                modifiers: DeclarationModifiers.None,
                type: propertyDeclaration.Type,
                initializer: propertyDeclaration.Initializer?.Value);
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="event">The <see cref="EventDeclarationSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddEvent(this DocumentEditor editor, ClassDeclarationSyntax containingType, EventDeclarationSyntax @event)
        {
            editor.ReplaceNode(containingType, (node, generator) => AddSorted(generator, (ClassDeclarationSyntax)node, @event));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="event">The <see cref="EventFieldDeclarationSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddEvent(this DocumentEditor editor, ClassDeclarationSyntax containingType, EventFieldDeclarationSyntax @event)
        {
            editor.ReplaceNode(containingType, (node, generator) => AddSorted(generator, (ClassDeclarationSyntax)node, @event));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddProperty(this DocumentEditor editor, TypeDeclarationSyntax containingType, BasePropertyDeclarationSyntax property)
        {
            editor.ReplaceNode(containingType, (node, generator) => AddSorted(generator, (TypeDeclarationSyntax)node, property));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/></param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="method">The <see cref="MethodDeclarationSyntax"/></param>
        /// <returns>The <paramref name="editor"/></returns>
        public static DocumentEditor AddMethod(this DocumentEditor editor, TypeDeclarationSyntax containingType, MethodDeclarationSyntax method)
        {
            editor.ReplaceNode(containingType, (node, generator) => AddSorted(generator, (TypeDeclarationSyntax)node, method));
            return editor;
        }

        private static SyntaxNode AddUsing(CompilationUnitSyntax root, SemanticModel semanticModel, UsingDirectiveSyntax usingDirective)
        {
            if (root == null)
            {
                return null;
            }

            using (var walker = UsingDirectiveWalker.Borrow(root))
            {
                if (walker.UsingDirectives.Count == 0)
                {
                    if (walker.NamespaceDeclarations.TryFirst(out var namespaceDeclaration))
                    {
                        if (CodeStyle.UsingDirectivesInsideNamespace(semanticModel))
                        {
                            return root.ReplaceNode(namespaceDeclaration, namespaceDeclaration.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                        }

                        return root.ReplaceNode(root, root.WithUsings(SyntaxFactory.SingletonList(usingDirective)));
                    }

                    return root;
                }

                UsingDirectiveSyntax previous = null;
                foreach (var directive in walker.UsingDirectives)
                {
                    var compare = UsingDirectiveComparer.Compare(directive, usingDirective);
                    if (compare == 0)
                    {
                        return root;
                    }

                    if (compare > 0)
                    {
                        return root.InsertNodesBefore(directive, new[] { usingDirective });
                    }

                    previous = directive;
                }

                return root.InsertNodesAfter(previous, new[] { usingDirective });
            }
        }

        private static TypeDeclarationSyntax AddBackingField(this DocumentEditor editor, TypeDeclarationSyntax type, FieldDeclarationSyntax backingField, string propertyName)
        {
            if (type.TryFindProperty(propertyName, out var property))
            {
                if (editor.SemanticModel.BackingFieldsAdjacent(out var newLine))
                {
                    if (newLine ||
                        !property.GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia))
                    {
                        return type.InsertNodesBefore(property, new[] { backingField });
                    }

                    return type.InsertNodesBefore(property, new[] { backingField.WithTrailingTrivia(null) });
                }

                var index = type.Members.IndexOf(property);
                for (var i = index + 1; i < type.Members.Count; i++)
                {
                    if (type.Members[i] is PropertyDeclarationSyntax other)
                    {
                        if (other.TryGetBackingField(out var otherField))
                        {
                            return type.InsertNodesBefore(otherField, new[] { backingField });
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                for (var i = index - 1; i >= 0; i--)
                {
                    if (type.Members[i] is PropertyDeclarationSyntax other)
                    {
                        if (other.TryGetBackingField(out var otherField))
                        {
                            return type.InsertNodesAfter(otherField, new[] { backingField });
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return AddSorted(editor.Generator, type, backingField);
        }

        private static TypeDeclarationSyntax AddSorted(SyntaxGenerator generator, TypeDeclarationSyntax containingType, MemberDeclarationSyntax memberDeclaration)
        {
            var memberIndex = MemberIndex(memberDeclaration);
            for (var i = 0; i < containingType.Members.Count; i++)
            {
                var member = containingType.Members[i];
                if (memberIndex < MemberIndex(member))
                {
                    return (TypeDeclarationSyntax)generator.InsertMembers(containingType, i, memberDeclaration);
                }
            }

            return (TypeDeclarationSyntax)generator.AddMembers(containingType, memberDeclaration);
        }

        private static int MemberIndex(MemberDeclarationSyntax member)
        {
            int ModifierOffset(SyntaxTokenList modifiers)
            {
                if (modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return 0;
                }

                if (modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    if (modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                    {
                        return 1;
                    }

                    return 2;
                }

                if (modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    return 3;
                }

                return 4;
            }

            int AccessOffset(Accessibility accessibility)
            {
                const int step = 5;
                switch (accessibility)
                {
                    case Accessibility.Public:
                        return 0 * step;
                    case Accessibility.Internal:
                        return 1 * step;
                    case Accessibility.ProtectedAndInternal:
                        return 2 * step;
                    case Accessibility.ProtectedOrInternal:
                        return 3 * step;
                    case Accessibility.Protected:
                        return 4 * step;
                    case Accessibility.Private:
                        return 5 * step;
                    case Accessibility.NotApplicable:
                        return int.MinValue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
                }
            }

            Accessibility Accessability(SyntaxTokenList modifiers)
            {
                if (modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    return Accessibility.Public;
                }

                if (modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return Accessibility.Internal;
                }

                if (modifiers.Any(SyntaxKind.ProtectedKeyword) &&
                    modifiers.Any(SyntaxKind.InternalKeyword))
                {
                    return Accessibility.ProtectedAndInternal;
                }

                if (modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    return Accessibility.Protected;
                }

                if (modifiers.Any(SyntaxKind.PrivateKeyword))
                {
                    return Accessibility.Private;
                }

                return Accessibility.Private;
            }

            int TypeOffset(SyntaxKind kind)
            {
                const int step = 5 * 6;
                switch (kind)
                {
                    case SyntaxKind.FieldDeclaration:
                        return 0 * step;
                    case SyntaxKind.ConstructorDeclaration:
                        return 1 * step;
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                        return 2 * step;
                    case SyntaxKind.PropertyDeclaration:
                        return 3 * step;
                    case SyntaxKind.MethodDeclaration:
                        return 4 * step;
                    default:
                        return int.MinValue;
                }
            }

            var mfs = member.Modifiers();
            return TypeOffset(member.Kind()) + AccessOffset(Accessability(mfs)) + ModifierOffset(mfs);
        }

        private static SyntaxTokenList Modifiers(this MemberDeclarationSyntax member)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field:
                    return field.Modifiers;
                case BasePropertyDeclarationSyntax prop:
                    return prop.Modifiers;
                case BaseMethodDeclarationSyntax method:
                    return method.Modifiers;
                case TypeDeclarationSyntax type:
                    return type.Modifiers;
                default:
                    return default(SyntaxTokenList);
            }
        }

        private sealed class UsingDirectiveWalker : PooledWalker<UsingDirectiveWalker>
        {
            private readonly List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();
            private readonly List<NamespaceDeclarationSyntax> namespaceDeclarations = new List<NamespaceDeclarationSyntax>();

            public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives => this.usingDirectives;

            public IReadOnlyList<NamespaceDeclarationSyntax> NamespaceDeclarations => this.namespaceDeclarations;

            public static UsingDirectiveWalker Borrow(CompilationUnitSyntax compilationUnit) => BorrowAndVisit(compilationUnit, () => new UsingDirectiveWalker());

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                this.usingDirectives.Add(node);
            }

            public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                this.namespaceDeclarations.Add(node);
                base.VisitNamespaceDeclaration(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                // Stop walking here
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // Stop walking here
            }

            protected override void Clear()
            {
                this.usingDirectives.Clear();
                this.namespaceDeclarations.Clear();
            }
        }
    }
}
