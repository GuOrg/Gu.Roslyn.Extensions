namespace Gu.Roslyn.AnalyzerExtensions
{
    internal static class KnownSymbol
    {
        internal static readonly QualifiedType Expression = Create("System.Linq.Expressions.Expression");

        private static QualifiedType Create(string qualifiedName)
        {
            return new QualifiedType(qualifiedName);
        }
    }
}