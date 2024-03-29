﻿namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.ExpressionSyntaxExtTests;

using System;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class Types
{
    [TestCase("1", typeof(int))]
    [TestCase("1", typeof(int?))]
    [TestCase("1", typeof(double))]
    [TestCase("1", typeof(object))]
    [TestCase("1", typeof(IComparable))]
    [TestCase("1", typeof(IComparable<int>))]
    public static void IsAssignableTo(string text, Type type)
    {
        var code = @"
namespace N
{
    class C
    {
        C()
        {
            1;
        }
    }
}".AssertReplace("1", text);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var expression = syntaxTree.FindExpression(text);
        Assert.AreEqual(true, expression.IsAssignableTo(QualifiedType.FromType(type), semanticModel));
    }

    [TestCase("1", "System.Int32")]
    public static void IsAssignableTo(string text, string fullname)
    {
        var code = @"
namespace N
{
    class C
    {
        C()
        {
            1;
        }
    }
}".AssertReplace("1", text);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var expression = syntaxTree.FindExpression(text);
        Assert.AreEqual(true, expression.IsAssignableTo(new QualifiedType(fullname), semanticModel));
    }

    [TestCase("1", typeof(int))]
    [TestCase("1.0", typeof(double))]
    public static void IsSameType(string text, Type type)
    {
        var code = @"
namespace N
{
    class C
    {
        C()
        {
            1;
        }
    }
}".AssertReplace("1", text);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var expression = syntaxTree.FindExpression(text);
        Assert.AreEqual(true, expression.IsSameType(new QualifiedType(type.FullName), semanticModel));
        Assert.AreEqual(true, expression.IsSameType(QualifiedType.FromType(type), semanticModel));
    }
}
