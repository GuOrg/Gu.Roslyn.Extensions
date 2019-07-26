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
namespace N
{
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"
namespace N
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
namespace N
{
}";

                var otherCode = @"
using System;

namespace N
{
}";
                var sln = CodeFactory.CreateSolution(new[] { testCode, otherCode });
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"using System;

namespace N
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
namespace N
{
    using System.Collections;
}";
                var sln = CodeFactory.CreateSolution(testCode);
                var editor = await DocumentEditor.CreateAsync(sln.Projects.First().Documents.First()).ConfigureAwait(false);

                var expected = @"
namespace N
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
namespace N
{
}";
                var sln = CodeFactory.CreateSolution(testCode, MetadataReferences.FromAttributes());
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
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
namespace N
{
    using System.Text;
}";
                var sln = CodeFactory.CreateSolution(testCode, MetadataReferences.FromAttributes());
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
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
namespace N.Extensions
{
    class C { }
}
";
                var testCode = @"
namespace N
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.First();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
{
    using N.Extensions;
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("N.Extensions.C");
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
namespace N
{
    class C { }
}
";
                var testCode = @"
namespace N.Extensions
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N.Extensions
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("N.C");
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
namespace N
{
    class C1 { }
}
";
                var testCode = @"
namespace N
{
    class C2 { }
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
{
    class C2 { }
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("N.C1");
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
namespace N
{
    class C<T> { }
}
";
                var testCode = @"
namespace N
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("N.C`1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }

            [Test]
            public async Task TypeInSameNamespaceWhenEmptyNamespace()
            {
                var classCode = @"
namespace N
{
    class C1 { }
}
";
                var testCode = @"
namespace N
{
}";
                var sln = CodeFactory.CreateSolution(new[] { classCode, testCode });
                var document = sln.Projects.First().Documents.Last();
                var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

                var expected = @"
namespace N
{
}";
                var type = editor.SemanticModel.Compilation.ObjectType.ContainingAssembly.GetTypeByMetadataName("N.C1");
                _ = editor.AddUsing(type);
                CodeAssert.AreEqual(expected, editor.GetChangedDocument());
            }
        }
    }
}
