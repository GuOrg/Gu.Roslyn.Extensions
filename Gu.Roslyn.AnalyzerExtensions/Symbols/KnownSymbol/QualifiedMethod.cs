namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="IPropertySymbol"/>.
    /// </summary>
    public class QualifiedMethod : QualifiedMember<IMethodSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedMethod"/> class.
        /// </summary>
        /// <param name="containingType">The containing type.</param>
        /// <param name="name">The name.</param>
        public QualifiedMethod(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}
