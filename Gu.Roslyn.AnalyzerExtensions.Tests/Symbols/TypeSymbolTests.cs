namespace Gu.Roslyn.AnalyzerExtensions.Tests.Symbols
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public partial class TypeSymbolTests
    {
        [TestCase("int value1, int value2")]
        [TestCase("int value1, System.IComparable value2")]
        [TestCase("int value1, System.IComparable<int> value2")]
        [TestCase("int value1, object value2")]
        public void WhenTrue(string parameters)
        {
            var code = @"
namespace RoslynSandbox
{
    public class Foo
    {
        public Foo(int value1, int value2)
        {
        }
    }
}";
            code = code.AssertReplace("int value1, int value2", parameters);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var constructorDeclaration = syntaxTree.FindConstructorDeclaration("Foo");
            var ctor = semanticModel.GetDeclaredSymbol(constructorDeclaration);
            var type1 = ctor.Parameters[0].Type;
            var type2 = ctor.Parameters[1].Type;
            var typeSyntax = constructorDeclaration.ParameterList.Parameters[1].Type;
            Assert.AreEqual(true, type1.IsRepresentationPreservingConversion(typeSyntax, semanticModel, CancellationToken.None));
        }
    }
}
