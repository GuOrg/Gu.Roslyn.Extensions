﻿namespace Gu.Roslyn.CodeFixExtensions.Tests;

using System.Linq;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

public static class ParseTests
{
    [TestCase("private readonly int value = 1;")]
    public static void FieldDeclaration(string code)
    {
        var declaration = Parse.FieldDeclaration(code);
        Assert.AreEqual(code, declaration.ToFullString());
        var expected = (FieldDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(code);
        RoslynAssert.Ast(expected, declaration);
    }

    [TestCase("public C(){}")]
    public static void ConstructorDeclaration(string code)
    {
        var declaration = Parse.ConstructorDeclaration(code);
        Assert.AreEqual(code, declaration.ToFullString());
        var expected = (ConstructorDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(code);
        RoslynAssert.Ast(expected, declaration);
    }

    [TestCase("public int C { get; }")]
    public static void PropertyDeclaration(string code)
    {
        var declaration = Parse.PropertyDeclaration(code);
        Assert.AreEqual(code, declaration.ToFullString());
        var expected = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(code);
        RoslynAssert.Ast(expected, declaration);
    }

    [TestCase("public int C() => 1;")]
    public static void MethodDeclaration(string code)
    {
        var declaration = Parse.MethodDeclaration(code);
        Assert.AreEqual(code, declaration.ToFullString());
        var expected = (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(code);
        RoslynAssert.Ast(expected, declaration);
    }

    [TestCase("<summary> Text </summary>")]
    [TestCase("<param name=\"cancellationToken\">The <see cref=\"CancellationToken\"/> that the task will observe.</param>")]
    public static void XmlElementSyntaxSingleLine(string code)
    {
        var node = Parse.XmlElementSyntax(code, string.Empty);
        Assert.AreEqual(code, node.ToFullString());
        var expected = SyntaxFactory.ParseLeadingTrivia("/// " + code)
                                    .Single(x => x.HasStructure)
                                    .GetStructure()
                                    .ChildNodes()
                                    .OfType<XmlElementSyntax>()
                                    .Single();
        RoslynAssert.Ast(expected, node);
    }

    [TestCase("<summary> Line 1\r\nLine2 </summary>", "<summary> Line 1\r\n/// Line2 </summary>")]
    [TestCase("<summary>\r\nLine 1\r\n</summary>", "<summary>\r\n/// Line 1\r\n/// </summary>")]
    [TestCase("<summary>\r\nLine 1\r\nLine2\r\n</summary>", "<summary>\r\n/// Line 1\r\n/// Line2\r\n/// </summary>")]
    public static void XmlElementSyntaxMultiLine(string code, string expected)
    {
        var node = Parse.XmlElementSyntax(code, string.Empty);
        Assert.AreEqual(expected, node.ToFullString());
    }

    [TestCase("<see cref=\"Value\"/>")]
    public static void XmlEleXmlEmptyElement(string code)
    {
        var node = Parse.XmlEmptyElementSyntax(code);
        Assert.AreEqual(code, node.ToFullString());
    }

    [TestCase("/// <summary> Text </summary>")]
    [TestCase("/// <summary> Line 1\r\n/// Line2 </summary>")]
    [TestCase("/// <summary>\r\n/// Line 1\r\n/// </summary>")]
    [TestCase("/// <summary>\r\n/// Line 1\r\n/// Line2\r\n/// </summary>")]
    [TestCase("/// <param name=\"cancellationToken\">The <see cref=\"CancellationToken\"/> that the task will observe.</param>")]
    [TestCase("/// <summary>\r\n        /// Initializes a new instance of the <see cref=\"C\"/> class.\r\n        /// </summary>")]
    [TestCase("/// <summary> Initializes a new instance of the <see cref=\"C\"/> class. </summary>")]
    public static void DocumentationCommentTriviaSyntax(string code)
    {
        var node = Parse.DocumentationCommentTriviaSyntax(code);
        Assert.AreEqual(code + "\r\n", node.ToFullString());
    }

    [TestCase("/// <summary> Text </summary>")]
    public static void LeadingTrivia(string code)
    {
        var node = Parse.LeadingTrivia(code);
        Assert.AreEqual(code + "\r\n", node.ToFullString());
    }

    [TestCase("[TemplatePart(Name = \"PART_Border\", Type = typeof(Border))]")]
    public static void AttributeList(string code)
    {
        var node = Parse.AttributeList(code);
        Assert.AreEqual(code + "\r\n", node.ToFullString());
    }
}
