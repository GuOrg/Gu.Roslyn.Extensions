namespace Gu.Roslyn.CodeFixExtensions
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <inheritdoc />
    public abstract class DocumentEditorCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override FixAllProvider? GetFixAllProvider() => this.FixAllProvider();

        /// <inheritdoc />
        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context) => this.RegisterCodeFixesAsync(new DocumentEditorCodeFixContext(context));

        /// <summary>
        /// The <see cref="DocumentEditorFixAllProvider"/> for the desired scope <see cref="DocumentEditorFixAllProvider.Document"/>.
        /// </summary>
        /// <returns>A <see cref="DocumentEditorFixAllProvider"/>. </returns>
        protected virtual DocumentEditorFixAllProvider FixAllProvider() => DocumentEditorFixAllProvider.Document;

        /// <summary>
        /// Computes one or more fixes for the specified <see cref="DocumentEditorCodeFixContext" />.
        /// </summary>
        /// <param name="context">
        /// A <see cref="DocumentEditorCodeFixContext" /> containing context information about the diagnostics to fix.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context);
    }
}
