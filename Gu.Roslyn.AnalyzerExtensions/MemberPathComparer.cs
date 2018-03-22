namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class MemberPathComparer : IEqualityComparer<ExpressionSyntax>
    {
        internal static MemberPathComparer Default = new MemberPathComparer();

        private MemberPathComparer()
        {
        }

        /// <inheritdoc />
        public bool Equals(ExpressionSyntax x, ExpressionSyntax y)
        {
            return MemberPath.Equals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(ExpressionSyntax obj)
        {
            if (MemberPath.TryGetMemberName(obj, out var name, out _))
            {
                return name.GetHashCode();
            }

            return 0;
        }
    }
}
