namespace Gu.Roslyn.CodeFixExtensions
{
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
        /// Add the using directive and respect if the convention is inside or outside of namespace and sorted with system first.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddUsing(this DocumentEditor editor, UsingDirectiveSyntax usingDirective)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (usingDirective is null)
            {
                throw new System.ArgumentNullException(nameof(usingDirective));
            }

            if (editor.OriginalRoot is CompilationUnitSyntax compilationUnit)
            {
                editor.ReplaceNode(
                    compilationUnit,
                    root => root.AddUsing(usingDirective, editor.SemanticModel));
            }

            return editor;
        }

        /// <summary>
        /// Add the using directive and respect if the convention is inside or outside of namespace and sorted with system first.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="type">The <see cref="ITypeSymbol"/> that needs to be brought into scope.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddUsing(this DocumentEditor editor, ITypeSymbol type)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (type is null)
            {
                throw new System.ArgumentNullException(nameof(type));
            }

            if (editor.OriginalRoot is CompilationUnitSyntax compilationUnit)
            {
                editor.ReplaceNode(
                    compilationUnit,
                    root => root.AddUsing(type, editor.SemanticModel));
            }

            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="field">The <see cref="FieldDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddField(this DocumentEditor editor, TypeDeclarationSyntax containingType, FieldDeclarationSyntax field)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (field is null)
            {
                throw new System.ArgumentNullException(nameof(field));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((TypeDeclarationSyntax)node, field));
            return editor;
        }

        /// <summary>
        /// Add the backing field and figure out placement.
        /// StyleCop ordering is the default but it also checks for if field adjacent to property is used.
        /// The property is unchanged by this call.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/>.</returns>
        public static FieldDeclarationSyntax AddBackingField(this DocumentEditor editor, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (propertyDeclaration is null)
            {
                throw new System.ArgumentNullException(nameof(propertyDeclaration));
            }

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
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/>.</param>
        /// <returns>A <see cref="FieldDeclarationSyntax"/>.</returns>
        public static FieldDeclarationSyntax CreateBackingField(this DocumentEditor editor, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (propertyDeclaration is null)
            {
                throw new System.ArgumentNullException(nameof(propertyDeclaration));
            }

            var property = editor.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
            var name = editor.SemanticModel.UnderscoreFields() == CodeStyleResult.Yes
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
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="event">The <see cref="EventDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddEvent(this DocumentEditor editor, ClassDeclarationSyntax containingType, EventDeclarationSyntax @event)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((ClassDeclarationSyntax)node, @event));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="event">The <see cref="EventFieldDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddEvent(this DocumentEditor editor, ClassDeclarationSyntax containingType, EventFieldDeclarationSyntax @event)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (@event is null)
            {
                throw new System.ArgumentNullException(nameof(@event));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((ClassDeclarationSyntax)node, @event));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="property">The <see cref="BasePropertyDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddProperty(this DocumentEditor editor, TypeDeclarationSyntax containingType, BasePropertyDeclarationSyntax property)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((TypeDeclarationSyntax)node, property));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="method">The <see cref="MethodDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddMethod(this DocumentEditor editor, TypeDeclarationSyntax containingType, MethodDeclarationSyntax method)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (method is null)
            {
                throw new System.ArgumentNullException(nameof(method));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((TypeDeclarationSyntax)node, method));
            return editor;
        }

        /// <summary>
        /// Add the field and respect StyleCop ordering.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="containingType">The containing type.</param>
        /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
        /// <returns>The <paramref name="editor"/>.</returns>
        public static DocumentEditor AddMember(this DocumentEditor editor, TypeDeclarationSyntax containingType, MemberDeclarationSyntax member)
        {
            if (editor is null)
            {
                throw new System.ArgumentNullException(nameof(editor));
            }

            if (containingType is null)
            {
                throw new System.ArgumentNullException(nameof(containingType));
            }

            if (member is null)
            {
                throw new System.ArgumentNullException(nameof(member));
            }

            editor.ReplaceNode(
                containingType,
                (node, generator) => generator.AddSorted((TypeDeclarationSyntax)node, member));
            return editor;
        }

        private static TypeDeclarationSyntax AddBackingField(this DocumentEditor editor, TypeDeclarationSyntax type, FieldDeclarationSyntax backingField, string propertyName)
        {
            if (type.TryFindProperty(propertyName, out var property))
            {
                if (editor.SemanticModel.BackingFieldsAdjacent(out var newLine) == CodeStyleResult.Yes)
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

            return editor.Generator.AddSorted(type, backingField);
        }
    }
}
