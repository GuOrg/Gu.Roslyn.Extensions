namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="IPropertySymbol"/>.
    /// </summary>
    public class QualifiedField : QualifiedMember<IFieldSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedField"/> class.
        /// </summary>
        /// <param name="containingType">The containing type</param>
        /// <param name="name">The name</param>
        public QualifiedField(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}