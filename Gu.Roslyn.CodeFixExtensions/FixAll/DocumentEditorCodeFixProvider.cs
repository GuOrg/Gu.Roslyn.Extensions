namespace Gu.Analyzers
{
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis.CodeFixes;

    public abstract class DocumentEditorCodeFixProvider : CodeFixProvider
    {
        protected virtual DocumentEditorFixAllProvider FixAllProvider() => DocumentEditorFixAllProvider.Document;

        public sealed override FixAllProvider GetFixAllProvider() => this.FixAllProvider();

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context) => this.RegisterCodeFixesAsync(new DocumentEditorCodeFixContext(context));

        public abstract Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context);
    }
}
