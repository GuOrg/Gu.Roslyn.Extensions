namespace Gu.Roslyn.CodeFixExtensions.Tests.CodeStyleTests.UnderscoreFields
{
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class DocumentEditor
    {
        [Test]
        public static void WhenUnknown()
        {
            var sln = CodeFactory.CreateSolution(@"
namespace N
{
    internal class C
    {
        internal C(int i, double d)
        {
        }

        internal void M()
        {
        }
    }
}");
            var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(sln.Projects.Single().Documents.Single()).Result;
            Assert.AreEqual(CodeStyleResult.NotFound, CodeStyle.UnderscoreFields(editor));
        }

        [TestCase("private int _f", CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f", CodeStyleResult.No)]
        [TestCase("private readonly int f", CodeStyleResult.No)]
        public static void FiguresOutFromDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(new[]
            {
                @"
namespace N
{
    class C1
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration),
            });
            foreach (var document in sln.Projects.Single().Documents)
            {
                var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(document).Result;
                Assert.AreEqual(expected, CodeStyle.UnderscoreFields(editor));
            }
        }

        [TestCase("private int _f", CodeStyleResult.Yes)]
        [TestCase("private readonly int _f = 1", CodeStyleResult.Yes)]
        [TestCase("private int f", CodeStyleResult.No)]
        [TestCase("private readonly int f", CodeStyleResult.No)]
        public static void FiguresOutFromOtherDocument(string declaration, CodeStyleResult expected)
        {
            var sln = CodeFactory.CreateSolution(new[]
            {
                @"
namespace N
{
    class C1
    {
        private int _f;
    }
}".AssertReplace("private int _f", declaration),
                @"
namespace N
{
    class C2
    {
    }
}",
            });
            foreach (var document in sln.Projects.Single().Documents)
            {
                var editor = Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(document).Result;
                Assert.AreEqual(expected, CodeStyle.UnderscoreFields(editor));
            }
        }

        [Test]
        public static void Repro()
        {
            var editor = CreateDocumentEditor(@"
namespace Vanguard_MVVM.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        IChildDataContext _childDataContext;
        readonly string _title;
        MainWindowViewModel()
        {
            _title = ""MVVM Attempt"";
        }

        public IChildDataContext ChildDataContext
        {
            get { return _childDataContext; }

            private set
            {
                if (Equals(value, _childDataContext)) return;
                ↓_childDataContext = value;
                NotifyPropertyChanged(nameof(ChildDataContext));
            }
        }

        public static MainWindowViewModel Instance { get; } = new MainWindowViewModel();

        public string Title => ChildDataContext?.Title == null ? _title : string.Concat(_title, "" - "", ChildDataContext?.Title);


        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}");
            Assert.AreEqual(CodeStyleResult.Yes, CodeStyle.UnderscoreFields(editor));
        }

        private static Microsoft.CodeAnalysis.Editing.DocumentEditor CreateDocumentEditor(string code)
        {
            return Microsoft.CodeAnalysis.Editing.DocumentEditor.CreateAsync(CodeFactory.CreateSolution(code).Projects.Single().Documents.Single()).Result;
        }
    }
}
