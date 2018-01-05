namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public class QualifiedProperty : QualifiedMember<IPropertySymbol>
    {
        public QualifiedProperty(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}
