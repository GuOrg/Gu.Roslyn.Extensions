namespace Gu.Roslyn.AnalyzerExtensions
{
    public static class ScopeExt
    {
        public static bool IsEither(this Scope scope, Scope scope1, Scope scope2) => scope == scope1 || scope == scope2;
    }
}