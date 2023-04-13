namespace Gu.Roslyn.AnalyzerExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Helper methods for <see cref="MemberDeclarationSyntax"/>.
/// </summary>
public static class MemberDeclarationSyntaxExtensions
{
    /// <summary>
    /// Get the <see cref="DocumentationCommentTriviaSyntax"/> for the accessor if it exists.
    /// </summary>
    /// <param name="member">The <see cref="MemberDeclarationSyntax"/>.</param>
    /// <param name="comment">The returned <see cref="DocumentationCommentTriviaSyntax"/>.</param>
    /// <returns>True if a single <see cref="DocumentationCommentTriviaSyntax"/> was found.</returns>
    public static bool TryGetDocumentationComment(this MemberDeclarationSyntax member, [NotNullWhen(true)] out DocumentationCommentTriviaSyntax? comment)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (member.HasLeadingTrivia &&
            member.GetLeadingTrivia() is { } triviaList &&
            triviaList.TrySingle(x => x.HasStructure && x.GetStructure() is DocumentationCommentTriviaSyntax, out var commentTrivia) &&
            commentTrivia.GetStructure() is DocumentationCommentTriviaSyntax triviaSyntax)
        {
            comment = triviaSyntax;
            return true;
        }

        comment = null;
        return false;
    }
}
