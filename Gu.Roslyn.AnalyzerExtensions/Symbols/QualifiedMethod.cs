namespace Gu.Roslyn.AnalyzerExtensions
{
    using Microsoft.CodeAnalysis;

    public class QualifiedMethod : QualifiedMember<IMethodSymbol>
    {
        public QualifiedMethod(QualifiedType containingType, string name)
            : base(containingType, name)
        {
        }
    }
}
