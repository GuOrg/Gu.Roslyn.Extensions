namespace Gu.Roslyn.AnalyzerExtensions
{
    public static class KnownSymbol
    {
        private static QualifiedType Create(string qualifiedName)
        {
            return new QualifiedType(qualifiedName);
        }

        public static class System
        {
            public static readonly QualifiedType Void = new QualifiedType("System.Void", "void");
            public static readonly QualifiedType Object = new QualifiedType("System.Object", "object");
            public static readonly QualifiedType Boolean = new QualifiedType("System.Boolean", "bool");
            public static readonly QualifiedType String = new QualifiedType("System.String", "string");

            public static class Linq
            {
                internal static readonly QualifiedType Expression = Create("System.Linq.Expressions.Expression");
            }
        }
    }
}
