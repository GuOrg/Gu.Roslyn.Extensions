namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="IEventSymbol"/>.
    /// </summary>
    public class QualifiedEvent : QualifiedMember<IEventSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedEvent"/> class.
        /// </summary>
        /// <param name="containingType">The containing type.</param>
        /// <param name="name">The name.</param>
        public QualifiedEvent(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}
