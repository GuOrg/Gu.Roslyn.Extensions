namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    [Obsolete("Move to Gu.Roslyn.Asserts")]
    public static class Dump
    {
        public static string Ast(SyntaxNode node)
        {
            var builder = new StringBuilder();
            using (var writer = new IndentedTextWriter(new StringWriter(builder)))
            {
                Write(writer, node);
                return builder.ToString();
            }
        }

        private static void Write(IndentedTextWriter writer, SyntaxNode node)
        {
            writer.WriteText("{")
                  .WriteProperty("Kind", node.Kind().ToString())
                  .WriteText(",")
                  .WriteProperty("Text", node.ToString().Replace("\r", "\\r").Replace("\n", "\\n"))
                  .WriteProperty("LeadingTrivia", node.GetLeadingTrivia())
                  .WriteProperty("ChildTokens", node.ChildTokens().ToList())
                  .WriteProperty("TrailingTrivia", node.GetTrailingTrivia());

            var childNodes = node.ChildNodes().ToArray();
            if (childNodes.Any())
            {
                writer.Indent++;
                writer.WriteLine(", \"ChildNodes\": [");
                writer.Indent++;
                foreach (var child in childNodes)
                {
                    Write(writer, child);
                    writer.WriteLine(",");
                }

                writer.Indent--;
                writer.WriteLine("]");
                writer.Indent--;
            }

            writer.Write(" }");
        }

        private static IndentedTextWriter WriteProperty(this IndentedTextWriter writer, string name, string value)
        {
            writer.Write($" \"{name}\": \"{value}\"");
            return writer;
        }

        private static IndentedTextWriter WriteProperty(this IndentedTextWriter writer, string name, IReadOnlyList<SyntaxToken> tokens)
        {
            if (tokens.Any())
            {
                writer.Write($", \"{name}\": [");
                foreach (var token in tokens)
                {
                    writer.WriteToken(token);
                    writer.Write(", ");
                }

                writer.Write("]");
            }

            return writer;
        }

        private static IndentedTextWriter WriteToken(this IndentedTextWriter writer, SyntaxToken token)
        {
            return writer.WriteText(" {")
                         .WriteProperty("Kind", token.Kind().ToString())
                         .WriteText(",")
                         .WriteProperty("Text", token.Text.Replace("\r", "\\r").Replace("\n", "\\n"))
                         .WriteText(",")
                         .WriteProperty("ValueText", token.ValueText.Replace("\r", "\\r").Replace("\n", "\\n"))
                         .WriteProperty("LeadingTrivia", token.LeadingTrivia)
                         .WriteProperty("TrailingTrivia", token.TrailingTrivia)
                         .WriteText(" }");
        }

        private static IndentedTextWriter WriteProperty(this IndentedTextWriter writer, string name, SyntaxTriviaList triviaList)
        {
            if (triviaList.Any())
            {
                writer.Write($", \"{name}\": [");
                foreach (var trivia in triviaList)
                {
                    writer.WriteTrivia(trivia);
                    writer.Write(", ");
                }

                writer.Write(" ] ");
            }

            return writer;
        }

        private static IndentedTextWriter WriteTrivia(this IndentedTextWriter writer, SyntaxTrivia trivia)
        {
            return writer.WriteText(" {")
                         .WriteProperty("Kind", trivia.Kind().ToString())
                         .WriteText(",")
                         .WriteProperty("Text", trivia.ToString().Replace("\r", "\\r").Replace("\n", "\\n"))
                         .WriteText(" }");
        }

        private static IndentedTextWriter WriteText(this IndentedTextWriter writer, string text)
        {
            writer.Write(text);
            return writer;
        }
    }
}
