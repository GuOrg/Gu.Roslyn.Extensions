namespace Gu.Roslyn.CodeFixExtensions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Helper methods for modifying <see cref="MemberDeclarationSyntax"/>.
/// </summary>
public static partial class MemberDeclarationSyntaxExtensions
{
    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="AccessorDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static AccessorDeclarationSyntax WithAttributeListText(this AccessorDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="AccessorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="AccessorDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static AccessorDeclarationSyntax WithAttributeList(this AccessorDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="ClassDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static ClassDeclarationSyntax WithAttributeListText(this ClassDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="ClassDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="ClassDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static ClassDeclarationSyntax WithAttributeList(this ClassDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="CompilationUnitSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static CompilationUnitSyntax WithAttributeListText(this CompilationUnitSyntax member, string text)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="CompilationUnitSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="CompilationUnitSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static CompilationUnitSyntax WithAttributeList(this CompilationUnitSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="ConstructorDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static ConstructorDeclarationSyntax WithAttributeListText(this ConstructorDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="ConstructorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="ConstructorDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static ConstructorDeclarationSyntax WithAttributeList(this ConstructorDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="ConversionOperatorDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static ConversionOperatorDeclarationSyntax WithAttributeListText(this ConversionOperatorDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="ConversionOperatorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="ConversionOperatorDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static ConversionOperatorDeclarationSyntax WithAttributeList(this ConversionOperatorDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="DelegateDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static DelegateDeclarationSyntax WithAttributeListText(this DelegateDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="DelegateDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="DelegateDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static DelegateDeclarationSyntax WithAttributeList(this DelegateDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="DestructorDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static DestructorDeclarationSyntax WithAttributeListText(this DestructorDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="DestructorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="DestructorDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static DestructorDeclarationSyntax WithAttributeList(this DestructorDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="EnumDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static EnumDeclarationSyntax WithAttributeListText(this EnumDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="EnumDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="EnumDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static EnumDeclarationSyntax WithAttributeList(this EnumDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="EnumMemberDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static EnumMemberDeclarationSyntax WithAttributeListText(this EnumMemberDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="EnumMemberDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="EnumMemberDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static EnumMemberDeclarationSyntax WithAttributeList(this EnumMemberDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="EventDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static EventDeclarationSyntax WithAttributeListText(this EventDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="EventDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="EventDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static EventDeclarationSyntax WithAttributeList(this EventDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="EventFieldDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static EventFieldDeclarationSyntax WithAttributeListText(this EventFieldDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="EventFieldDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="EventFieldDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static EventFieldDeclarationSyntax WithAttributeList(this EventFieldDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="FieldDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static FieldDeclarationSyntax WithAttributeListText(this FieldDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="FieldDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="FieldDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static FieldDeclarationSyntax WithAttributeList(this FieldDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="IncompleteMemberSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static IncompleteMemberSyntax WithAttributeListText(this IncompleteMemberSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="IncompleteMemberSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="IncompleteMemberSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static IncompleteMemberSyntax WithAttributeList(this IncompleteMemberSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="IndexerDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static IndexerDeclarationSyntax WithAttributeListText(this IndexerDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="IndexerDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="IndexerDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static IndexerDeclarationSyntax WithAttributeList(this IndexerDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="InterfaceDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static InterfaceDeclarationSyntax WithAttributeListText(this InterfaceDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="InterfaceDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="InterfaceDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static InterfaceDeclarationSyntax WithAttributeList(this InterfaceDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="MethodDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static MethodDeclarationSyntax WithAttributeListText(this MethodDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="MethodDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="MethodDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static MethodDeclarationSyntax WithAttributeList(this MethodDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="OperatorDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static OperatorDeclarationSyntax WithAttributeListText(this OperatorDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="OperatorDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="OperatorDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static OperatorDeclarationSyntax WithAttributeList(this OperatorDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="ParameterSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static ParameterSyntax WithAttributeListText(this ParameterSyntax member, string text)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="ParameterSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="ParameterSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static ParameterSyntax WithAttributeList(this ParameterSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="PropertyDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static PropertyDeclarationSyntax WithAttributeListText(this PropertyDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="PropertyDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="PropertyDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static PropertyDeclarationSyntax WithAttributeList(this PropertyDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="StructDeclarationSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <param name="adjustLeadingWhitespace">If true leading whitespace is adjusted to match <paramref name="member"/>.</param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static StructDeclarationSyntax WithAttributeListText(this StructDeclarationSyntax member, string text, bool adjustLeadingWhitespace = true)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text, adjustLeadingWhitespace ? member.LeadingWhitespace() : null));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="StructDeclarationSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="StructDeclarationSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static StructDeclarationSyntax WithAttributeList(this StructDeclarationSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }

    /// <summary>
    /// Add <paramref name="text"/> as attribute list to <paramref name="member"/>.
    /// </summary>
    /// <param name="member">The <see cref="TypeParameterSyntax"/>.</param>
    /// <param name="text">
    /// The attribute text including start and end [].
    /// </param>
    /// <returns>The <paramref name="member"/> with docs in leading trivia.</returns>
    public static TypeParameterSyntax WithAttributeListText(this TypeParameterSyntax member, string text)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (text is null)
        {
            throw new System.ArgumentNullException(nameof(text));
        }

        return member.WithAttributeList(Parse.AttributeList(text));
    }

    /// <summary>
    /// Add the attribute list to the <see cref="TypeParameterSyntax"/>.
    /// </summary>
    /// <param name="member">The <see cref="TypeParameterSyntax"/>.</param>
    /// <param name="attributeList">The <see cref="AttributeListSyntax"/>.</param>
    /// <returns>The <paramref name="member"/> with <paramref name="attributeList"/> added.</returns>
    public static TypeParameterSyntax WithAttributeList(this TypeParameterSyntax member, AttributeListSyntax attributeList)
    {
        if (member is null)
        {
            throw new System.ArgumentNullException(nameof(member));
        }

        if (attributeList is null)
        {
            throw new System.ArgumentNullException(nameof(attributeList));
        }

        return member.WithAttributeLists(member.AttributeLists.Add(attributeList));
    }
}
