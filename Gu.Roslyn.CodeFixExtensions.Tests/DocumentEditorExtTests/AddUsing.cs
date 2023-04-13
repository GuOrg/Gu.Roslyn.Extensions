namespace Gu.Roslyn.CodeFixExtensions.Tests.DocumentEditorExtTests;

using System.Linq;
using System.Threading.Tasks;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;

public static class AddUsing
{
    [Test]
    public static async Task SystemWhenEmpty()
    {
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(code);
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
    public static async Task SystemWhenEmptyOutside()
    {
        var code = @"
namespace N
{
}";

        var otherCode = @"
using System;

namespace N
{
}";
        var sln = CodeFactory.CreateSolution(new[] { code, otherCode });
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
    public static async Task SystemWhenSystemCollectionsExists()
    {
        var code = @"
namespace N
{
    using System.Collections;
}";
        var sln = CodeFactory.CreateSolution(code);
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
    public static async Task StringBuilderType()
    {
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(code);
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
    public static async Task ListOfStringBuilderType()
    {
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(code);
        var document = sln.Projects.First().Documents.First();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N
{
    using System.Collections.Generic;
    using System.Text;
}";

        var type = editor.SemanticModel.GetSpeculativeTypeInfo(
            0,
            SyntaxFactory.ParseTypeName("System.Collections.Generic.List<System.Text.StringBuilder>"),
            SpeculativeBindingOption.BindAsTypeOrNamespace).Type;
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task StringBuilderTypeWhenUsingExists()
    {
        var code = @"
namespace N
{
    using System.Text;
}";
        var sln = CodeFactory.CreateSolution(code);
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
    public static async Task TypeInNestedNamespace()
    {
        var classCode = @"
namespace N.Extensions
{
    class C { }
}
";
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.First();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N
{
    using N.Extensions;
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("N.Extensions.C");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInNestedDeepNamespace()
    {
        var classCode = @"
namespace A.B.C.Extensions
{
    class C { }
}
";
        var code = @"
namespace A.B.C
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.First();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace A.B.C
{
    using A.B.C.Extensions;
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("A.B.C.Extensions.C");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInContainingNamespace()
    {
        var classCode = @"
namespace N
{
    class C { }
}
";
        var code = @"
namespace N.Extensions
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N.Extensions
{
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("N.C");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInContainingDeepNamespace()
    {
        var classCode = @"
namespace A.B.C
{
    class C1 { }
}
";
        var code = @"
namespace A.B.C.Extensions
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace A.B.C.Extensions
{
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("A.B.C.C1");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInSameNamespace()
    {
        var classCode = @"
namespace N
{
    class C1 { }
}
";
        var code = @"
namespace N
{
    class C2 { }
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N
{
    class C2 { }
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("N.C1");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInSameDeepNamespace()
    {
        var classCode = @"
namespace A.B.C
{
    class C1 { }
}
";
        var code = @"
namespace A.B.C
{
    class C2 { }
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace A.B.C
{
    class C2 { }
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("A.B.C.C1");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task GenericTypeInSameNamespace()
    {
        var classCode = @"
namespace N
{
    class C<T> { }
}
";
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N
{
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("N.C`1");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }

    [Test]
    public static async Task TypeInSameNamespaceWhenEmptyNamespace()
    {
        var classCode = @"
namespace N
{
    class C1 { }
}
";
        var code = @"
namespace N
{
}";
        var sln = CodeFactory.CreateSolution(new[] { classCode, code });
        var document = sln.Projects.First().Documents.Last();
        var editor = await DocumentEditor.CreateAsync(document).ConfigureAwait(false);

        var expected = @"
namespace N
{
}";
        var type = editor.SemanticModel.Compilation.GetTypeByMetadataName("N.C1");
        _ = editor.AddUsing(type);
        CodeAssert.AreEqual(expected, editor.GetChangedDocument());
    }
}
