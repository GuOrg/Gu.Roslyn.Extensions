namespace Gu.Roslyn.AnalyzerExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <inheritdoc />
    public sealed class MemberPathComparer : IEqualityComparer<ExpressionSyntax>
    {
        /// <summary> The default instance. </summary>
        internal static readonly MemberPathComparer Default = new MemberPathComparer();

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
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
        [Obsolete("Should only be called with arguments of type IEventSymbol.", error: true)]
        public static new bool Equals(object _, object __) => throw new InvalidOperationException("This is hidden so that it is not called by accident.");
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        //// ReSharper restore UnusedParameter.Global

        /// <inheritdoc />
        bool IEqualityComparer<ExpressionSyntax>.Equals(ExpressionSyntax x, ExpressionSyntax y) => MemberPath.Equals(x, y);

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
