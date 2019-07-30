namespace Gu.Roslyn.AnalyzerExtensions.Tests.SyntaxTreeTests.MemberDeclarationSyntaxExtensionsTests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class TryGetTarget
    {
        [Test]
        public void TryGetTargetProperty()
        {
            var code = @"
namespace N
{
    using System.Reflection;

    public class C
    {
        public object M(string text) => text.Length;
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var identifierNameSyntax = syntaxTree.FindMemberAccessExpression("text.Length");
            var method = new QualifiedProperty(new QualifiedType(typeof(string).FullName), "Length");
            Assert.AreEqual(true, identifierNameSyntax.TryGetTarget(method, semanticModel, CancellationToken.None, out var target));
            Assert.AreEqual("string.Length", target.ToString());
        }
    }
}
