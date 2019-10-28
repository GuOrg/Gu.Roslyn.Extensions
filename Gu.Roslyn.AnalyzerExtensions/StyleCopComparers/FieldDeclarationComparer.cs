namespace Gu.Roslyn.AnalyzerExtensions.StyleCopComparers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class FieldDeclarationComparer : IComparer<FieldDeclarationSyntax>
    {
        /// <summary> The default instance. </summary>
        public static readonly FieldDeclarationComparer Default = new FieldDeclarationComparer();

        /// <summary>Compares two nodes and returns a value indicating whether one is less than, equal to, or greater than the other according to StyleCop.</summary>
        /// <returns>A signed integer that indicates if the node should be before the other according to StyleCop.</returns>
        /// <param name="x">The first node to compare.</param>
        /// <param name="y">The second node to compare.</param>
        public static int Compare(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
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

            var compare = MemberDeclarationComparer.CompareAccessibility(x.Modifiers, y.Modifiers, Accessibility.Private);
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

            return MemberDeclarationComparer.CompareSpanStart(x, y);
        }

        /// <inheritdoc />
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
                        if (y.Declaration.TryFindVariable(identifierName.Identifier.ValueText, out _))
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

            static int Index(FieldDeclarationSyntax field)
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

        /// <inheritdoc />
        internal sealed class SetterAssignmentWalker : PooledWalker<SetterAssignmentWalker>
        {
            private readonly List<AssignmentExpressionSyntax> assignments = new List<AssignmentExpressionSyntax>();

            private SetterAssignmentWalker()
            {
            }

            /// <inheritdoc />
            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                this.assignments.Add(node);
                base.VisitAssignmentExpression(node);
            }

            /// <summary>
            /// Try get single setter assigning the field.
            /// </summary>
            /// <param name="field">The <see cref="FieldDeclarationSyntax"/>.</param>
            /// <param name="setter">The single setter accessor assigning the field.</param>
            /// <returns>True if a single setter was found.</returns>
            internal static bool TryGetSetter(FieldDeclarationSyntax field, [NotNullWhen(true)]out AccessorDeclarationSyntax? setter)
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

            /// <inheritdoc />
            protected override void Clear()
            {
                this.assignments.Clear();
            }
        }
    }
}
