namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Linq;
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierNameExecutionWalkerTests
    {
        [TestCase(Scope.Member, "Foo")]
        [TestCase(Scope.Instance, "text, Foo")]
        [TestCase(Scope.Type, "text, Foo")]
        [TestCase(Scope.Recursive, "text, Foo")]
        public void StaticInitializers(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public sealed class Foo
    {
        public static readonly Foo Default = new Foo();
        
        private static readonly string text = ""abc"";
        
        public string Text { get; set; } = text;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindExpression("new Foo()");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.IdentifierNames));
            }
        }

        [TestCase(Scope.Member, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Instance, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Type, "ValuePropertyKey, DependencyProperty")]
        [TestCase(Scope.Recursive, "ValuePropertyKey, DependencyProperty")]
        public void DependencyPropertyRegisterReadOnly(Scope scope, string expected)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    using System.Windows;
    using System.Windows.Controls;

    public class FooControl : Control
    {
        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Value),
            typeof(int),
            typeof(FooControl), 
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
        public void TryFindWhenProperty(Scope scope)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class C
    {
        public int P { get; }

        public void M()
        {
            var p = this.P;
        }
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { syntaxTree }, MetadataReferences.FromAttributes());
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var node = syntaxTree.FindMethodDeclaration("M");
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, scope, semanticModel, CancellationToken.None))
            {
                CollectionAssert.AreEqual(new[] { "var", "P" }, walker.IdentifierNames.Select(x => x.Identifier.ValueText));

                Assert.AreEqual(true, walker.TryFind("P", out var match));
                Assert.AreEqual("P", match.Identifier.ValueText);

                Assert.AreEqual(false, walker.TryFind("missing", out _));
            }
        }
    }
}
