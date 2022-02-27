namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class CompilationUintSyntaxExtensionsTests
    {
        [Test]
        public static void SystemWhenEmpty()
        {
            var code = @"
namespace N
{
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N
{
usingSystem;}";
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

            var updated = compilationUnit.AddUsing(usingDirective, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void SystemWhenEmptyFileScopedNamespace()
        {
            var code = @"namespace N;
";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"namespace N;
usingSystem;";
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

            var updated = compilationUnit.AddUsing(usingDirective, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void AddSystem()
        {
            var code = @"
namespace N
{

    using System.Text;
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N
{
usingSystem;

    using System.Text;
}";
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

            var updated = compilationUnit.AddUsing(usingDirective, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void AddSystemFileScopedNamespace()
        {
            var code = @"
namespace N;

using System.Text;";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N;
usingSystem;

using System.Text;";
            var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);

            var updated = compilationUnit.AddUsing(usingDirective, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void StringBuilderType()
        {
            var code = @"
namespace N
{
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N
{
usingSystem.Text;}";
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);
            var type = compilation.GetTypeByMetadataName("System.Text.StringBuilder");
            var updated = compilationUnit.AddUsing(type, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void StringBuilderTypeWhenAlreadyHasUsing()
        {
            var code = @"
namespace N
{
    using System.Text;
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N
{
    using System.Text;
}";
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);
            var type = compilation.GetTypeByMetadataName("System.Text.StringBuilder");
            var updated = compilationUnit.AddUsing(type, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }

        [Test]
        public static void StringBuilderTypeWhenAlreadyHasUsingFileScopedNamespace()
        {
            var code = @"
namespace N;

using System.Text;
";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, Settings.Default.MetadataReferences);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var expected = @"
namespace N;

using System.Text;
";
            var compilationUnit = syntaxTree.GetCompilationUnitRoot(CancellationToken.None);
            var type = compilation.GetTypeByMetadataName("System.Text.StringBuilder");
            var updated = compilationUnit.AddUsing(type, semanticModel);
            CodeAssert.AreEqual(expected, updated.ToFullString());
        }
    }
}
