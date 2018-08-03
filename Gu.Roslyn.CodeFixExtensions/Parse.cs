namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using System.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Helpers for parsing members
    /// </summary>
    public static class Parse
    {
        /// <summary>
        /// Parse a <see cref="FieldDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="FieldDeclarationSyntax"/></returns>
        public static FieldDeclarationSyntax FieldDeclaration(string code)
        {
            return (FieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="ConstructorDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="ConstructorDeclarationSyntax"/></returns>
        public static ConstructorDeclarationSyntax ConstructorDeclaration(string code)
        {
            return (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="EventFieldDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="EventFieldDeclarationSyntax"/></returns>
        public static EventFieldDeclarationSyntax EventFieldDeclaration(string code)
        {
            return (EventFieldDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="EventDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="EventDeclarationSyntax"/></returns>
        public static EventDeclarationSyntax EventDeclaration(string code)
        {
            return (EventDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="PropertyDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="PropertyDeclarationSyntax"/></returns>
        public static PropertyDeclarationSyntax PropertyDeclaration(string code)
        {
            return (PropertyDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="MethodDeclarationSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The code text.</param>
        /// <returns>The <see cref="MethodDeclarationSyntax"/></returns>
        public static MethodDeclarationSyntax MethodDeclaration(string code)
        {
            return (MethodDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(code).Members.Single();
        }

        /// <summary>
        /// Parse a <see cref="XmlElementSyntax"/> from a string.
        /// </summary>
        /// <param name="code">The element text including start and end tags.</param>
        /// <param name="leadingWhitespace">The whitespace to prepend.</param>
        /// <returns>The <see cref="XmlElementSyntax"/></returns>
        public static XmlElementSyntax XmlElementSyntax(string code, string leadingWhitespace)
        {
            if (CSharpSyntaxTree.ParseText(ClassCode()) is CSharpSyntaxTree tree &&
                tree.TryGetRoot(out var root) &&
                root.ChildNodes().Single() is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.TryGetDocumentationComment(out var comment) &&
                comment.Content.TrySingleOfType(out XmlElementSyntax element))
            {
                return element;
            }

            throw new InvalidOperationException($"Failed parsing {code} into an XmlElementSyntax");

            string ClassCode()
            {
                leadingWhitespace = leadingWhitespace ?? string.Empty;
                var builder = StringBuilderPool.Borrow();
                if (code.IndexOf('\n') < 0)
                {
                    builder.AppendLine($"{leadingWhitespace}/// {code}");
                }
                else
                {
                    foreach (var line in code.Split('\n'))
                    {
                        builder.Append($"{leadingWhitespace}/// {line}\n");
                    }
                }

                builder.AppendLine($"{leadingWhitespace}public class Foo {{}}");
                return builder.Return();
            }
        }

        /// <summary>
        /// Parse a <see cref="DocumentationCommentTriviaSyntax"/> from a string.
        /// Lines are expected to start with ///
        /// </summary>
        /// <param name="code">
        /// The element text including start and end tags.
        /// Lines are expected to start with ///
        /// </param>
        /// <returns>The <see cref="DocumentationCommentTriviaSyntax"/></returns>
        public static DocumentationCommentTriviaSyntax DocumentationCommentTriviaSyntax(string code)
        {
            if (CSharpSyntaxTree.ParseText(ClassCode()) is CSharpSyntaxTree tree &&
                tree.TryGetRoot(out var root) &&
                root.ChildNodes().Single() is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.TryGetDocumentationComment(out var comment))
            {
                return comment;
            }

            throw new InvalidOperationException($"Failed parsing {code} into a DocumentationCommentTriviaSyntax");

            string ClassCode()
            {
                return code + "\r\npublic class Foo { }";
            }
        }
    }
}
