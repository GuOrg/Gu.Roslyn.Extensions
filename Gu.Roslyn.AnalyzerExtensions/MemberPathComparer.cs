namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class MemberPathComparer : IEqualityComparer<ExpressionSyntax>
    {
        /// <summary> The default instance. </summary>
        internal static readonly MemberPathComparer Default = new();

        private MemberPathComparer()
        {
        }

        /// <summary> Compares equality by <see cref="MemberPath.Equals(ExpressionSyntax,ExpressionSyntax)"/>. </summary>
        /// <param name="x">The first instance.</param>
        /// <param name="y">The other instance.</param>
        /// <returns>True if the instances are found equal.</returns>
        public static bool Equals(ExpressionSyntax x, ExpressionSyntax y) => MemberPath.Equals(x, y);

        //// ReSharper disable once UnusedMember.Global
        //// ReSharper disable UnusedParameter.Global
#pragma warning disable CA1707,CS1591,IDE1006,SA1313,SA1600 // Missing XML comment for publicly visible type or member
        [Obsolete("Should only be called with arguments of type IEventSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<ExpressionSyntax>.Equals(ExpressionSyntax? x, ExpressionSyntax? y) => MemberPath.Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(ExpressionSyntax obj)
        {
            if (MemberPath.TryGetMemberName(obj, out var name))
            {
                return name.GetHashCode();
            }

            return 0;
        }
    }
}
