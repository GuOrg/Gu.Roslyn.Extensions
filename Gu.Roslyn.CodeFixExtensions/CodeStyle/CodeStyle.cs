namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeStyle;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Options;

#pragma warning disable CA1724 // Type names should not match namespaces
    /// <summary>
    /// Helper for figuring out if the code uses underscore prefix in field names.
    /// </summary>
    public static class CodeStyle
#pragma warning restore CA1724 // Type names should not match namespaces
    {
        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyFieldAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> QualifyFieldAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyFieldAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyFieldAccessWalker.CheckAsync(document, cancellationToken)
                                                 .ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyFieldAccess if present.
        /// 2. Walk current <see cref="Document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static Task<CodeStyleResult> QualifyFieldAccessAsync(this DocumentEditor editor, CancellationToken cancellationToken)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.OriginalDocument.QualifyFieldAccessAsync(cancellationToken);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyEventAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> QualifyEventAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyEventAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyEventAccessWalker.CheckAsync(document, cancellationToken)
                                                 .ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyEventAccess if present.
        /// 2. Walk current <see cref="Document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static Task<CodeStyleResult> QualifyEventAccessAsync(this DocumentEditor editor, CancellationToken cancellationToken)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.OriginalDocument.QualifyEventAccessAsync(cancellationToken);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyPropertyAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> QualifyPropertyAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyPropertyAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyPropertyAccessWalker.CheckAsync(document, cancellationToken)
                                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyPropertyAccess if present.
        /// 2. Walk current <see cref="Document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static Task<CodeStyleResult> QualifyPropertyAccessAsync(this DocumentEditor editor, CancellationToken cancellationToken)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.OriginalDocument.QualifyPropertyAccessAsync(cancellationToken);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyMethodAccess if present.
        /// 2. Walk current <paramref name="document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> QualifyMethodAccessAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (await FindInEditorConfigAsync(document, CodeStyleOptions.QualifyMethodAccess, cancellationToken).ConfigureAwait(false) is CodeStyleOption<bool> option)
            {
                return option.Value ? CodeStyleResult.Yes : CodeStyleResult.No;
            }

            return await QualifyMethodAccessWalker.CheckAsync(document, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if field access should be prefixed with this.
        /// 1. Check CodeStyleOptions.QualifyPropertyAccess if present.
        /// 2. Walk current <see cref="Document"/>.
        /// 3. Walk current project.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static Task<CodeStyleResult> QualifyMethodAccessAsync(this DocumentEditor editor, CancellationToken cancellationToken)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.OriginalDocument.QualifyMethodAccessAsync(cancellationToken);
        }

        /// <summary>
        /// Figuring out if field names should be prefixed with _.
        /// 1. Walk current <paramref name="document"/>.
        /// 2. Walk current project.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static async Task<CodeStyleResult> UnderscoreFieldsAsync(this Document document, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return await UnderscoreFieldWalker.CheckAsync(document, cancellationToken)
                                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UnderscoreFields(DocumentEditor editor)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.SemanticModel.UnderscoreFields();
        }

        /// <summary>
        /// Figuring out if the code uses underscore prefix in field names.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UnderscoreFields(this SemanticModel semanticModel)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return UnderscoreFieldWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation);
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UsingDirectivesInsideNamespace(SemanticModel semanticModel)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return UsingDirectivesInsideNamespaceWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation);
        }

        /// <summary>
        /// Figuring out if the code uses using directives inside namespaces.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult UsingDirectivesInsideNamespace(DocumentEditor editor)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return UsingDirectivesInsideNamespace(editor.SemanticModel);
        }

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="semanticModel">The <see cref="SemanticModel"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult BackingFieldsAdjacent(this SemanticModel semanticModel, out bool newLineBetween)
        {
            if (semanticModel is null)
            {
                throw new ArgumentNullException(nameof(semanticModel));
            }

            return BackingFieldsAdjacentWalker.Check(semanticModel.SyntaxTree, semanticModel.Compilation, out newLineBetween);
        }

        /// <summary>
        /// Figuring out if backing fields are adjacent to their properties.
        /// </summary>
        /// <param name="editor">The <see cref="DocumentEditor"/>.</param>
        /// <param name="newLineBetween">If there is a new line between the field and the property.</param>
        /// <returns>True if the code is found to prefix field names with underscore.</returns>
        public static CodeStyleResult BackingFieldsAdjacent(this DocumentEditor editor, out bool newLineBetween)
        {
            if (editor is null)
            {
                throw new ArgumentNullException(nameof(editor));
            }

            return editor.SemanticModel.BackingFieldsAdjacent(out newLineBetween);
        }

        /// <summary>
        /// Find the <see cref="CodeStyleOption{T}"/> for <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <param name="key">The <see cref="PerLanguageOption{T}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns><see cref="CodeStyleOption{T}"/> or null if not found.</returns>
        public static async Task<CodeStyleOption<T>?> FindInEditorConfigAsync<T>(Document document, PerLanguageOption<CodeStyleOption<T>> key, CancellationToken cancellationToken)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var optionSet = await document.GetOptionsAsync(cancellationToken).ConfigureAwait(false);
            if (optionSet.GetOption(key, document.Project.Language) is CodeStyleOption<T> option &&
                !ReferenceEquals(option, key.DefaultValue))
            {
                return option;
            }

            return null;
        }
    }
}
