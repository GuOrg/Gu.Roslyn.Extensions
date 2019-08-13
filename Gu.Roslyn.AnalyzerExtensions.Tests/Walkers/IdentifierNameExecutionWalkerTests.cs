namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class IdentifierNameExecutionWalkerTests
    {
        [TestCase(Scope.Member, "C")]
        [TestCase(Scope.Instance, "text, C")]
        [TestCase(Scope.Type, "text, C")]
        [TestCase(Scope.Recursive, "text, C")]
        public static void StaticInitializers(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public sealed class C
    {
        public static readonly C Default = new C();
        
        private static readonly string text = ""abc"";
        
        public string Text { get; set; } = text;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindExpression("new C()");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.IdentifierNames));
            }
        }

        [TestCase(Scope.Member, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Instance, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Type, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Recursive, "ValuePropertyKey, DependencyProperty")]
        public static void DependencyPropertyRegisterReadOnly(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    using System.Windows;
    using System.Windows.Controls;

    public class CControl : Control
    {
        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Value),
            typeof(int),
            typeof(CControl), 
            new PropertyMetadata(default(int)));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        public int Value
        {
            get => (int) this.GetValue(ValueProperty);
            private set => this.SetValue(ValuePropertyKey, value);
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindExpression("ValuePropertyKey.DependencyProperty");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.IdentifierNames));
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Type)]
        [TestCase(Scope.Recursive)]
        public static void TryFindWhenProperty(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P { get; }

        public void M()
        {
            var p = this.P.ToString();
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("M");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                CollectionAssert.AreEqual(new[] { "var", "P", "ToString" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));

                Assert.AreEqual(true, walker.TryFind("P", out var match));
                Assert.AreEqual("P", match.Identifier.ValueText);

                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }

        [TestCase(Scope.Member)]
        [TestCase(Scope.Instance)]
        [TestCase(Scope.Type)]
        [TestCase(Scope.Recursive)]
        public static void TryFindWhenProperty2(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P { get; }

        public void M()
        {
            this.P.ToString();
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("M");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                CollectionAssert.AreEqual(new[] { "P", "ToString" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));

                Assert.AreEqual(true, walker.TryFind("P", out var match));
                Assert.AreEqual("P", match.Identifier.ValueText);

                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }
    }
}
