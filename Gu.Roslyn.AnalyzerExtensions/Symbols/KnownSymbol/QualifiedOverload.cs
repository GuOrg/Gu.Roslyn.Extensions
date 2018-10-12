namespace Gu.Roslyn.AnalyzerExtensions
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// For comparison with roslyn <see cref="IPropertySymbol"/>.
    /// </summary>
    public class QualifiedOverload : QualifiedMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedOverload"/> class.
        /// </summary>
        /// <param name="containingType">The containing type</param>
        /// <param name="name">The name</param>
        /// <param name="parameters">The parameters.</param>
        public QualifiedOverload(QualifiedType containingType, string name, ImmutableArray<QualifiedParameter> parameters)
            : base(containingType, name)
        {
            this.Parameters = parameters;
        }

        /// <summary>
        /// Get the parameters.
        /// </summary>
        public ImmutableArray<QualifiedParameter> Parameters { get; }

        /// <inheritdoc />
        protected override bool Equals(IMethodSymbol symbol)
        {
            return this.Equals(symbol.Parameters) &&
                   base.Equals(symbol);
        }

        private bool Equals(ImmutableArray<IParameterSymbol> parameters)
        {
            if (parameters.Length == this.Parameters.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (this.Parameters[i].Name is string name &&
                        parameter.Name != name)
                    {
                        return false;
                    }

                    if (this.Parameters[i].Type is QualifiedType type &&
                        parameter.Type != type)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
