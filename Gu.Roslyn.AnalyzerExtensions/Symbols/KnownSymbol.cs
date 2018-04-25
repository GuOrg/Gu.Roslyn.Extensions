namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary> Known symbols </summary>
    public static class KnownSymbol
    {
        private static QualifiedType Create(string qualifiedName)
        {
            return new QualifiedType(qualifiedName);
        }

        /// <summary> System </summary>
        public static class System
        {
            /// <summary> System.Void </summary>
            public static readonly QualifiedType Void = new QualifiedType("System.Void", "void");

            /// <summary> System.Object </summary>
            public static readonly QualifiedType Object = new QualifiedType("System.Object", "object");

            /// <summary> System.Boolean </summary>
            public static readonly QualifiedType Boolean = new QualifiedType("System.Boolean", "bool");

            /// <summary> System.String </summary>
            public static readonly QualifiedType String = new QualifiedType("System.String", "string");

            /// <summary> System.Linq </summary>
            public static class Linq
            {
                /// <summary> System.Linq.Expressions.Expression </summary>
                internal static readonly QualifiedType Expression = Create("System.Linq.Expressions.Expression");
            }
        }
    }
}
