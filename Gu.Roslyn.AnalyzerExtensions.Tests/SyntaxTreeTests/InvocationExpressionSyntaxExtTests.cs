namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests
{
    using System.Reflection;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class InvocationExpressionSyntaxExtTests
    {
        [TestCase("Method1()", "Method1")]
        [TestCase("this.Method1()", "Method1")]
        [TestCase("new Foo()?.Method1()", "Method1")]
        [TestCase("this.Method2<int>()", "Method2")]
        public void TryGetInvokedMethodName(string code, string expected)
        {
            var testCode = @"
namespace N
{
    public class Foo
    {
        public Foo()
        {
            var i = Method1();
            i = this.Method1();
            i = new Foo()?.Method1() ?? 0;
            i = Method2<int>();
            i = this.Method2<int>();
        }
        private int Method1() => 1;
        private int Method2<T>() => 2;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var invocation = syntaxTree.FindInvocation(code);
            Assert.AreEqual(true, invocation.TryGetMethodName(out var name));
            Assert.AreEqual(expected, name);
        }

        [Test]
        public void TryGetTargetAssemblyGetType()
        {
            var testCode = @"
namespace N
{
    using System.Reflection;

    public class Foo
    {
        public Foo(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation("GetType");
            var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
            Assert.AreEqual(true, invocation.TryGetTarget(method, semanticModel, CancellationToken.None, out var target));
            Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
        }

        [Test]
        public void TryGetTargetAssemblyGetTypeWithParameterByName()
        {
            var testCode = @"
namespace N
{
    using System.Reflection;

    public class Foo
    {
        public Foo(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation("GetType");
            var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
            Assert.AreEqual(true, invocation.TryGetTarget(method, QualifiedParameter.Create("name"), semanticModel, CancellationToken.None, out var target, out var arg));
            Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
            Assert.AreEqual("\"System.Int32\"", arg.ToString());
        }

        [Test]
        public void TryGetTargetAssemblyGetTypeWithParameterByType()
        {
            var testCode = @"
namespace N
{
    using System.Reflection;

    public class Foo
    {
        public Foo(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation("GetType");
            var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
            Assert.AreEqual(true, invocation.TryGetTarget(method, QualifiedParameter.Create(QualifiedType.FromType(typeof(string))), semanticModel, CancellationToken.None, out var target, out var arg));
            Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
            Assert.AreEqual("\"System.Int32\"", arg.ToString());
        }

        [Test]
        public void TryGetTargetAssemblyGetTypeWithParameterByNameAndType()
        {
            var testCode = @"
namespace N
{
    using System.Reflection;

    public class Foo
    {
        public Foo(Assembly assembly)
        {
            assembly.GetType(""System.Int32"");
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(testCode);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocation = syntaxTree.FindInvocation("GetType");
            var method = new QualifiedMethod(new QualifiedType(typeof(Assembly).FullName), "GetType");
            Assert.AreEqual(true, invocation.TryGetTarget(method, new QualifiedParameter("name", QualifiedType.FromType(typeof(string))), semanticModel, CancellationToken.None, out var target, out var arg));
            Assert.AreEqual("System.Reflection.Assembly.GetType(string)", target.ToString());
            Assert.AreEqual("\"System.Int32\"", arg.ToString());
        }
    }
}
