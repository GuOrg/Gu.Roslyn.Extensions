namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests;

using System.Reflection;
using System.Threading;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public static class InvocationExpressionSyntaxExtTests
{
    [TestCase("Method1()", "Method1")]
    [TestCase("this.Method1()", "Method1")]
    [TestCase("new C()?.Method1()", "Method1")]
    [TestCase("this.Method2<int>()", "Method2")]
    public static void TryGetInvokedMethodName(string expression, string expected)
    {
        var code = @"
namespace N
{
    public class C
    {
        public C()
        {
            var i = Method1();
            i = this.Method1();
            i = new C()?.Method1() ?? 0;
            i = Method2<int>();
            i = this.Method2<int>();
        }
        private int Method1() => 1;
        private int Method2<T>() => 2;
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var invocation = syntaxTree.FindInvocation(expression);
        Assert.AreEqual(true, invocation.TryGetMethodName(out var name));
        Assert.AreEqual(expected, name);
    }

    [Test]
    public static void TryGetTargetAssemblyGetType()
    {
        var code = @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public C(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("GetType");
        var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
        Assert.AreEqual(true, invocation.TryGetTarget(method, semanticModel, CancellationToken.None, out var target));
        Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
    }

    [Test]
    public static void TryGetTargetAssemblyGetTypeWithParameterByName()
    {
        var code = @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public C(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("GetType");
        var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
        Assert.AreEqual(true, invocation.TryGetTarget(method, QualifiedParameter.Create("name"), semanticModel, CancellationToken.None, out var target, out var arg));
        Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
        Assert.AreEqual("\"System.Int32\"", arg.ToString());
    }

    [Test]
    public static void TryGetTargetAssemblyGetTypeWithParameterByType()
    {
        var code = @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public C(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("GetType");
        var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
        Assert.AreEqual(true, invocation.TryGetTarget(method, QualifiedParameter.Create(QualifiedType.FromType(typeof(string))), semanticModel, CancellationToken.None, out var target, out var arg));
        Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
        Assert.AreEqual("\"System.Int32\"", arg.ToString());
    }

    [Test]
    public static void TryGetTargetAssemblyGetTypeWithParameterByNameAndType()
    {
        var code = @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public C(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("GetType");
        var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
        Assert.AreEqual(true, invocation.TryGetTarget(method, new QualifiedParameter("name", QualifiedType.FromType(typeof(string))), semanticModel, CancellationToken.None, out var target, out var arg));
        Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
        Assert.AreEqual("\"System.Int32\"", arg.ToString());
    }

    [Test]
    public static void IsSymbol()
    {
        var code = @"
namespace N
{
    public class C
    {
        public C(object o)
        {
            _ = o.ToString();
        }
    }
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var invocation = syntaxTree.FindInvocation("o.ToString()");
        var method = new QualifiedMethod(new QualifiedType(typeof(object).FullName), "ToString");
        Assert.AreEqual(true, invocation.IsSymbol(method, semanticModel, CancellationToken.None));

        method = new QualifiedMethod(new QualifiedType(typeof(object).FullName), "Equals");
        Assert.AreEqual(false, invocation.IsSymbol(method, semanticModel, CancellationToken.None));
    }
}
