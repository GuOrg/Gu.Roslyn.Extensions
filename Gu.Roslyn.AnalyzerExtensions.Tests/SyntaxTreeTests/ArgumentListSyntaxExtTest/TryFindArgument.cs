﻿namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.ArgumentListSyntaxExtTest;

using System.Threading;

using Gu.Roslyn.Asserts;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

public static class TryFindArgument
{
    [TestCase(0, "1")]
    [TestCase(1, "2")]
    [TestCase(2, "3")]
    public static void Ordinal(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal class C
    {
        public void M()
        {
            M(1, 2, 3);
        }

        internal void M(int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
        Assert.AreEqual(expected, argument.ToString());

        Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "2")]
    [TestCase(1, "3")]
    public static void ExtensionMethodInvocation(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal static class C
    {
        public void M()
        {
            1.M(2, 3);
        }

        internal static void M(this int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        var parameter = method.Parameters[index];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out var argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());

        parameter = method.ReducedFrom.Parameters[index + 1];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "2")]
    [TestCase(1, "3")]
    public static void ExtensionMethodConditionalInvocation(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal static class C
    {
        public void M(string s)
        {
            s?.M(2, 3);
        }

        internal static void M(this string i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        var parameter = method.Parameters[index];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out var argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());

        parameter = method.ReducedFrom.Parameters[index + 1];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "1")]
    [TestCase(1, "2")]
    [TestCase(2, "3")]
    public static void ExtensionMethodStaticInvocation(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal static class C
    {
        public void M()
        {
            M(1, 2, 3);
            1.M(2, 3);
        }

        internal static void M(this int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        var parameter = method.Parameters[index];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out var argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        if (index > 0)
        {
            parameter = semanticModel.GetSymbolSafe(syntaxTree.FindInvocation("1.M(2, 3)"), CancellationToken.None).Parameters[index - 1];
            Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
            Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

            Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
        }
    }

    [TestCase(0, "1")]
    [TestCase(1, "2")]
    [TestCase(2, "3")]
    public static void ExtensionMethodQualifiedStaticInvocation(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal static class C
    {
        public void M()
        {
            C.M(1, 2, 3);
            1.M(2, 3);
        }

        internal static void M(this int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        var parameter = method.Parameters[index];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out var argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        if (index > 0)
        {
            parameter = semanticModel.GetSymbolSafe(syntaxTree.FindInvocation("1.M(2, 3)"), CancellationToken.None).Parameters[index - 1];
            Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
            Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

            Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
        }
    }

    [TestCase(0, "1")]
    [TestCase(1, "2")]
    [TestCase(2, "3")]
    public static void ExtensionMethodFullyQualifiedStaticInvocation(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal static class C
    {
        public void M()
        {
            N.C.M(1, 2, 3);
            1.M(2, 3);
        }

        internal static void M(this int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        var parameter = method.Parameters[index];
        Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out var argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
        Assert.AreEqual(expected, argument.ToString());
        Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

        if (index > 0)
        {
            parameter = semanticModel.GetSymbolSafe(syntaxTree.FindInvocation("1.M(2, 3)"), CancellationToken.None).Parameters[index - 1];
            Assert.AreEqual(true,     invocation.TryFindArgument(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
            Assert.AreEqual(expected, invocation.FindArgument(parameter)!.ToString());

            Assert.AreEqual(true,     invocation.ArgumentList.TryFind(parameter, out argument));
            Assert.AreEqual(expected, argument.ToString());
        }
    }

    [TestCase(0, "i1: 1")]
    [TestCase(1, "i2: 2")]
    [TestCase(2, "i3: 3")]
    public static void NamedAtOrdinalPositions(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    internal class C
    {
        public void M()
        {
            M(i1: 1, i2: 2, i3: 3);
        }

        internal void M(int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(i1: 1, i2: 2, i3: 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
        Assert.AreEqual(expected, argument.ToString());

        Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "i1: 1")]
    [TestCase(1, "i2: 2")]
    [TestCase(2, "i3: 3")]
    public static void Named(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        public void M()
        {
            M(i2: 2, i1: 1, i3: 3);
        }

        internal void M(int i1, int i2, int i3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(i2: 2, i1: 1, i3: 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
        Assert.AreEqual(expected, argument.ToString());

        Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "i1: 1")]
    [TestCase(1, "i2: 2")]
    [TestCase(2, "i3: 3")]
    public static void NamedOptionalWhenPassingAll(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        public void M()
        {
            M(i2: 2, i1: 1, i3: 3);
        }

        internal void M(int i1, int i2 = 2, int i3 = 3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(i2: 2, i1: 1, i3: 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[index], out var argument));
        Assert.AreEqual(expected, argument.ToString());

        Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
        Assert.AreEqual(expected, argument.ToString());
    }

    [TestCase(0, "1")]
    [TestCase(1, null)]
    [TestCase(2, "i3: 3")]
    public static void NamedOptionalWhenNotPassingAll(int index, string expected)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        public void M()
        {
            M(1, i3: 3);
        }

        internal void M(int i1, int i2 = 2, int i3 = 3)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, i3: 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(expected != null, invocation.TryFindArgument(method.Parameters[index], out var argument));
        Assert.AreEqual(expected, argument?.ToString());

        Assert.AreEqual(expected != null, invocation.ArgumentList.TryFind(method.Parameters[index], out argument));
        Assert.AreEqual(expected, argument?.ToString());
    }

    [Test]
    public static void ParamsExplicitArray()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        public void M()
        {
            M(new[] { 1, 2, 3 });
        }

        internal void M(params int[] xs)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(new[] { 1, 2, 3 })");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(true, invocation.TryFindArgument(method.Parameters[0], out var argument));
        Assert.AreEqual("new[] { 1, 2, 3 }", argument.ToString());

        Assert.AreEqual(true, invocation.ArgumentList.TryFind(method.Parameters[0], out argument));
        Assert.AreEqual("new[] { 1, 2, 3 }", argument.ToString());
    }

    [Test]
    public static void ParamsReturnsFalse()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            @"
namespace N
{
    class C
    {
        public void M()
        {
            M(1, 2, 3);
        }

        internal void M(params int[] xs)
        {
        }
    }
}");
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("M(1, 2, 3)");
        var method = semanticModel.GetSymbolSafe(invocation, CancellationToken.None);
        Assert.AreEqual(false, invocation.TryFindArgument(method.Parameters[0], out _));
        Assert.AreEqual(false, invocation.ArgumentList.TryFind(method.Parameters[0], out _));
    }
}
