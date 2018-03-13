namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class FieldDeclarationComparer : IComparer<FieldDeclarationSyntax>
    {
        public static readonly FieldDeclarationComparer Default = new FieldDeclarationComparer();

        public static int Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (IsInitializedWith(x, y))
            {
                return 1;
            }

            if (IsInitializedWith(y, x))
            {
                return -1;
            }

            var compare = MemberDeclarationComparer.CompareAccessability(x.Modifiers, y.Modifiers, Accessibility.Private);
            if (compare != 0)
            {
                return compare;
            }

            compare = MemberDeclarationComparer.CompareScope(x.Modifiers, y.Modifiers);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareReadOnly(x, y);
            if (compare != 0)
            {
                return compare;
            }

            compare = CompareBackingProperty(x, y);
            if (compare != 0)
            {
                return compare;
            }

            return x.SpanStart.CompareTo(y.SpanStart);
        }

        int IComparer<FieldDeclarationSyntax>.Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y) => Compare(x, y);

        private static bool IsInitializedWith(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (y.Modifiers.Any(SyntaxKind.ConstKeyword, SyntaxKind.StaticKeyword) &&
                x.Declaration.Variables.TryLast(out var variable) &&
                variable.Initializer is EqualsValueClauseSyntax initializer &&
                !(initializer.Value is LiteralExpressionSyntax))
            {
                using (var walker = IdentifierNameWalker.Borrow(initializer))
                {
                    foreach (var identifierName in walker.IdentifierNames)
                    {
                        if (y.Declaration.Variables.TryFirst(v => v.Identifier.ValueText == identifierName.Identifier.ValueText, out _))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static int CompareReadOnly(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            return Index(x).CompareTo(Index(y));

            int Index(FieldDeclarationSyntax field)
            {
                if (field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    return 0;
                }

                return 1;
            }
        }

        private static int CompareBackingProperty(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (SetterAssignmentWalker.TryGetSetter(x, out var xSetter) &&
                SetterAssignmentWalker.TryGetSetter(y, out var ySetter))
            {
                return xSetter.SpanStart.CompareTo(ySetter.SpanStart);
            }

            return 0;
        }

        internal sealed class SetterAssignmentWalker : PooledWalker<SetterAssignmentWalker>
        {
            private readonly List<AssignmentExpressionSyntax> assignments = new List<AssignmentExpressionSyntax>();

            private SetterAssignmentWalker()
            {
            }

            internal static bool TryGetSetter(FieldDeclarationSyntax field, out AccessorDeclarationSyntax setter)
            {
                setter = null;

                if (field.Declaration.Variables.TrySingle(out var variable) &&
                    field.Parent is TypeDeclarationSyntax type)
                {
                    using (var walker = Borrow(() => new SetterAssignmentWalker()))
                    {
                        foreach (var member in type.Members)
                        {
                            if (member is PropertyDeclarationSyntax property &&
                                property.TryGetSetter(out setter))
                            {
                                walker.Visit(setter);
                            }
                        }

                        if (walker.assignments.TrySingle(IsAssigning, out var assignment))
                        {
                            setter = assignment.FirstAncestor<AccessorDeclarationSyntax>();
                        }
                    }
                }

                return setter != null;

                bool IsAssigning(AssignmentExpressionSyntax assignment)
                {
                    return assignment.Right is IdentifierNameSyntax right &&
                           right.Identifier.ValueText == "value" &&
                           IsField(assignment.Left);
                }

                bool IsField(ExpressionSyntax expression)
                {
                    if (expression is IdentifierNameSyntax identifierName)
                    {
                        return identifierName.Identifier.ValueText == variable.Identifier.ValueText;
                    }

                    if (expression is MemberAccessExpressionSyntax memberAccess &&
                        memberAccess.Expression is ThisExpressionSyntax)
                    {
                        return memberAccess.Name.Identifier.ValueText == variable.Identifier.ValueText;
                    }

                    return false;
                }
            }

            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                assignments.Add(node);
                base.VisitAssignmentExpression(node);
            }

            protected override void Clear()
            {
                this.assignments.Clear();
            }
        }
    }
}
