namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="IPropertySymbol"/>.
    /// </summary>
    public class QualifiedProperty : QualifiedMember<IPropertySymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedProperty"/> class.
        /// </summary>
        /// <param name="containingType">The containing type</param>
        /// <param name="name">The name</param>
        public QualifiedProperty(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}
