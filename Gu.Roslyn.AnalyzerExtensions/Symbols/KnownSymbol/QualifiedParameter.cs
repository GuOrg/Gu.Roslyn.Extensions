namespace Gu.Roslyn.AnalyzerExtensions
{
    /// <summary>
    /// For comparison with <see cref="Microsoft.CodeAnalysis.IParameterSymbol"/>
    /// </summary>
    public struct QualifiedParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedParameter"/> struct.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        public QualifiedParameter(string name, QualifiedType type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public QualifiedType Type { get; }

        public QualifiedParameter Create(string name) => new QualifiedParameter(name, null);

        public QualifiedParameter Create(QualifiedType type) => new QualifiedParameter(null, type);
    }
}
