namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.BaseMethodDeclarationSyntaxExtTests;

using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class TryFindParameter
{
    [TestCase(0, "int v1")]
    [TestCase(1, "int v2")]
    [TestCase(2, "int v3")]
    public static void Ordinal(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    namespace N
    {
        internal class C
        {
            public void M()
            {
                Meh(1, 2, 3);
            }

            internal void Meh(int v1, int v2, int v3)
            {
            }
        }
    }
}");
        var argument = syntaxTree.FindInvocation("Meh(1, 2, 3)")
                                 .ArgumentList.Arguments[index];
        var method = syntaxTree.FindMethodDeclaration("internal void Meh(int v1, int v2, int v3)");

        Assert.AreEqual(true, BaseMethodDeclarationSyntaxExt.TryFindParameter(method, argument, out var parameter));
        Assert.AreEqual(expected, parameter.ToString());
    }

    [TestCase(0, "int v1")]
    [TestCase(1, "int v2")]
    [TestCase(2, "int v3")]
    public static void NamedAtOrdinalPositions(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    namespace N
    {
        internal class C
        {
            public void M()
            {
                Meh(v1: 1, v2: 2, v3: 3);
            }

            internal void Meh(int v1, int v2, int v3)
            {
            }
        }
    }
}");
        var argument = syntaxTree.FindInvocation("Meh(v1: 1, v2: 2, v3: 3)")
                                 .ArgumentList.Arguments[index];
        var method = syntaxTree.FindMethodDeclaration("internal void Meh(int v1, int v2, int v3)");

        Assert.AreEqual(true, BaseMethodDeclarationSyntaxExt.TryFindParameter(method, argument, out var parameter));
        Assert.AreEqual(expected, parameter.ToString());
    }

    [TestCase(0, "int v2")]
    [TestCase(1, "int v1")]
    [TestCase(2, "int v3")]
    public static void Named(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    namespace N
    {
        internal class C
        {
            public void M()
            {
                Meh(v2: 2, v1: 1, v3: 3);
            }

            internal void Meh(int v1, int v2, int v3)
            {
            }
        }
    }
}");
        var argument = syntaxTree.FindInvocation("Meh(v2: 2, v1: 1, v3: 3)")
                                 .ArgumentList.Arguments[index];
        var method = syntaxTree.FindMethodDeclaration("internal void Meh(int v1, int v2, int v3)");

        Assert.AreEqual(true, BaseMethodDeclarationSyntaxExt.TryFindParameter(method, argument, out var parameter));
        Assert.AreEqual(expected, parameter.ToString());
    }

    [TestCase(0, "params int[] values")]
    [TestCase(1, "params int[] values")]
    [TestCase(2, "params int[] values")]
    public static void Params(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    namespace N
    {
        internal class C
        {
            public void M()
            {
                Meh(1, 2, 3);
            }

            internal void Meh(params int[] values)
            {
            }
        }
    }
}");
        var argument = syntaxTree.FindInvocation("Meh(1, 2, 3)")
                                 .ArgumentList.Arguments[index];
        var method = syntaxTree.FindMethodDeclaration("internal void Meh(params int[] values)");

        Assert.AreEqual(
            true,
            BaseMethodDeclarationSyntaxExt.TryFindParameter(
                method,
                argument,
                out var parameter));
        Assert.AreEqual(expected, parameter.ToString());
    }
}
