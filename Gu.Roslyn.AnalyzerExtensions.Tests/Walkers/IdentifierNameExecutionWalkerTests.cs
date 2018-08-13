namespace Gu.Roslyn.AnalyzerExtensions.Tests.Walkers
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class IdentifierNameExecutionWalkerTests
    {
        [TestCase(Scope.Member, "text, Foo")]
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
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, Scope.Recursive, semanticModel, CancellationToken.None))
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
            using (var walker = IdentifierNameExecutionWalker.Borrow(node, Scope.Type, semanticModel, CancellationToken.None))
            {
                Assert.AreEqual(expected, string.Join(", ", walker.IdentifierNames));
            }
        }
    }
}
