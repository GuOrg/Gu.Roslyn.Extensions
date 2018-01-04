namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class NamesTests
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
            Assert.AreEqual(false, Names.UsesUnderscore(semanticModel, CancellationToken.None));
            Assert.AreEqual(false, Names.UsesUnderscore(syntaxTree.FindConstructorDeclaration("Foo"), semanticModel, CancellationToken.None));
        }
    }
}
