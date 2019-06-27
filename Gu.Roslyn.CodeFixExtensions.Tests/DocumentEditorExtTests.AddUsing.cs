namespace Gu.Roslyn.CodeFixExtensions.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;
    using NUnit.Framework;

    public partial class DocumentEditorExtTests
    {
        public class AddUsing
        {
            [Test]
            public async Task SystemWhenEmpty()
            {
                var testCode = @"
namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    using System;
}";
                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
                _ = editor.AddUsing(usingDirective);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task SystemWhenEmptyOutside()
            {
                var testCode = @"
namespace RoslynSandbox
{
}";

                var otherCode = @"
using System;

namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(new[] { testCode, otherCode });
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"using System;

namespace RoslynSandbox
{
}";
                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
                _ = editor.AddUsing(usingDirective);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task SystemWhenSystemCollectionsExists()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System.Collections;
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    using System;
    using System.Collections;
}";
                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System"));
                _ = editor.AddUsing(usingDirective);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task StringBuilderType()
            {
                var testCode = @"
namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(testCode, MetadataReferences.FromAttributes());
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    using System.Text;
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("System.Text.StringBuilder");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task StringBuilderTypeWhenUsingExists()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System.Text;
}";
                var sln = CodeFactory.CreateSolution(testCode, MetadataReferences.FromAttributes());
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    using System.Text;
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("System.Text.StringBuilder");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInNestedNamespace()
            {
                var classCode = @"
namespace RoslynSandbox.Extensions
{
    class C { }
}
";
                var testCode = @"
namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    using RoslynSandbox.Extensions;
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("RoslynSandbox.Extensions.C");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInNestedDeepNamespace()
            {
                var classCode = @"
namespace A.B.C.Extensions
{
    class C { }
}
";
                var testCode = @"
namespace A.B.C
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace A.B.C
{
    using A.B.C.Extensions;
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("A.B.C.Extensions.C");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInContainingNamespace()
            {
                var classCode = @"
namespace RoslynSandbox
{
    class C { }
}
";
                var testCode = @"
namespace RoslynSandbox.Extensions
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox.Extensions
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("RoslynSandbox.C");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInContainingDeepNamespace()
            {
                var classCode = @"
namespace A.B.C
{
    class C1 { }
}
";
                var testCode = @"
namespace A.B.C.Extensions
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace A.B.C.Extensions
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("A.B.C.C1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInSameNamespace()
            {
                var classCode = @"
namespace RoslynSandbox
{
    class C1 { }
}
";
                var testCode = @"
namespace RoslynSandbox
{
    class C2 { }
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
    class C2 { }
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("RoslynSandbox.C1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInSameDeepNamespace()
            {
                var classCode = @"
namespace A.B.C
{
    class C1 { }
}
";
                var testCode = @"
namespace A.B.C
{
    class C2 { }
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace A.B.C
{
    class C2 { }
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("A.B.C.C1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task GenericTypeInSameNamespace()
            {
                var classCode = @"
namespace RoslynSandbox
{
    class C<T> { }
}
";
                var testCode = @"
namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("RoslynSandbox.C`1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInSameNamespaceWhenEmptyNamespace()
            {
                var classCode = @"
namespace RoslynSandbox
{
    class C1 { }
}
";
                var testCode = @"
namespace RoslynSandbox
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace RoslynSandbox
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("RoslynSandbox.C1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}
