namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class CodeStyleTests
    {
        [Test]
        public void DefaultsToFalse()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    internal class Foo
    {
        internal Foo(int i, double d)
        {
        }

        internal void Bar()
        {
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void WhenFieldIsNamedWithUnderscore()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        int _value;
        public int Bar()  => _value = 1;
    }
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void WhenPropertyIsAssignedWithThis()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public Foo(int bar)
        {
            this.Bar = bar;
        }

        public int Bar { get; set; }
    }
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void WhenPropertyIsAssignedWithoutThis()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public Foo(int bar)
        {
            Bar = bar;
        }

        public int Bar { get; set; }
    }
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree });
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void WhenFieldIsNotNamedWithUnderscore()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        int value;
        public int Bar()  => value = 1;
    }
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void WhenUsingThis()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        public int Value { get; private set; }

        public int Bar()  => this.value = 1;
    }
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
        }

        [Test]
        public void FiguresOutFromOtherClass()
        {
            var fooCode = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Foo
    {
        private int _value;

        public int Bar()  => _value = 1;
    }
}");

            var barCode = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    class Bar
    {
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { fooCode, barCode }, MetadataReferences.FromAttributes());
            Assert.AreEqual(2, compilation.SyntaxTrees.Length);
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                Assert.AreEqual(true, CodeStyle.UnderscoreFields(semanticModel, CancellationToken.None));
            }
        }

        [Test]
        public void UsingDirectiveInsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System;
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
        }

        [Test]
        public void UsingDirectiveInsideAndOutsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;

namespace RoslynSandbox
{
    using System.Collections;
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(true, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
        }

        [Test]
        public void UsingDirectiveOutsideNamespace()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
namespace RoslynSandbox
{
}");

            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            Assert.AreEqual(false, CodeStyle.UsingDirectivesInsideNamespace(semanticModel, CancellationToken.None));
        }
    }
}
