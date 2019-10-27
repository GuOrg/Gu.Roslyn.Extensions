using Gu.Roslyn.Asserts;

[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: TransitiveMetadataReferences(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation))]
[assembly: MetadataReferences(
    typeof(System.Linq.Enumerable),
    typeof(System.Linq.Expressions.Expression),
    typeof(System.Collections.Generic.IEnumerable<>),
    typeof(System.Net.WebClient),
    typeof(System.Data.Common.DbConnection),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(NUnit.Framework.Assert))]
