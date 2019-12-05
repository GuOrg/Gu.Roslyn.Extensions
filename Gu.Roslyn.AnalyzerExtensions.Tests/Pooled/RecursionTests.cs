namespace Gu.Roslyn.AnalyzerExtensions.Tests.Pooled
{
    using System.Threading;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public static class RecursionTests
    {
        private static readonly SymbolDisplayFormat Format =
            new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
#pragma warning disable SA1118 // Parameter should not span multiple lines
                memberOptions:
                SymbolDisplayMemberOptions.IncludeParameters |
                SymbolDisplayMemberOptions.IncludeContainingType |
                SymbolDisplayMemberOptions.IncludeExplicitInterface,
                parameterOptions:
                SymbolDisplayParameterOptions.IncludeExtensionThis |
                SymbolDisplayParameterOptions.IncludeParamsRefOut |
                SymbolDisplayParameterOptions.IncludeType |
                SymbolDisplayParameterOptions.IncludeName,
                miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays |
                SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName |
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
#pragma warning restore SA1118 // Parameter should not span multiple lines

        [Test]
        public static void Argument()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P => M(1);

        public int M(int n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            Assert.AreEqual(node, target.Source);
            Assert.AreEqual("int n", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public int M(int n) => n;", target.TargetNode.ToString());
        }

        [Test]
        public static void ArgumentGeneric()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C
    {
        public int P => this.M<int>(1);

        public T M<T>(T n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            Assert.AreEqual(node, target.Source);
            Assert.AreEqual("T n", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public T M<T>(T n) => n;", target.TargetNode.ToString());
        }

        [Test]
        public static void ArgumentOverloaded()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public class C1
    {
        public int P => M(1);

        public virtual int M(int n) => n;
    }

    public class C2
    {
        public override int M(int x) => x;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C2"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            Assert.AreEqual(node, target.Source);
            Assert.AreEqual("int x", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public override int M(int x) => x;", target.TargetNode.ToString());
        }

        [Test]
        public static void ArgumentOverloadedGeneric()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public abstract class C1<T>
    {
        public abstract T M(T n);
    }

    public class C2 : C1<int>
    {
        public int P => M(1);

        public override int M(int x) => x;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C2"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindArgument("1");
            var target = recursion.Target(node).Value;
            Assert.AreEqual(node, target.Source);
            Assert.AreEqual("int x", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public override int M(int x) => x;", target.TargetNode.ToString());
        }

        [Test]
        public static void ExtensionMethod()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public static class C
    {
        public static int P => 1.M();

        public static int M(this int n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindInvocation("1.M()");
            var target = recursion.Target(node).Value;
            Assert.AreEqual("1", target.Source.ToString());
            Assert.AreEqual("int n", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public static int M(this int n) => n;", target.TargetNode.ToString());
        }

        [Test]
        public static void ExtensionMethodConditional()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace N
{
    public static class C
    {
        public static int M1(int? i) => i?.M2() ?? 0;

        public static int M2(this int n) => n;
    }
}");
            var compilation = CSharpCompilation.Create("test", new[] { tree });
            var semanticModel = compilation.GetSemanticModel(tree);
            Assert.AreEqual(true, semanticModel.TryGetNamedType(tree.FindClassDeclaration("C"), CancellationToken.None, out var type));
            using var recursion = Recursion.Borrow(type, semanticModel, CancellationToken.None);
            var node = tree.FindInvocation("i?.M2()");
            var target = recursion.Target(node).Value;
            Assert.AreEqual("i", target.Source.ToString());
            Assert.AreEqual("int n", target.Symbol.ToDisplayString(Format));
            Assert.AreEqual("public static int M2(this int n) => n;", target.TargetNode.ToString());
        }
    }
}
