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
        /// <param name="containingType">The containing type.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        public QualifiedOverload(QualifiedType containingType, string name, ImmutableArray<QualifiedParameter> parameters)
            : base(containingType, name)
        {
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public ImmutableArray<QualifiedParameter> Parameters { get; }

        /// <inheritdoc />
        protected override bool Equals(IMethodSymbol symbol)
        {
            return base.Equals(symbol) &&
#pragma warning disable CA1062 // Validate arguments of public methods
                   this.Equals(symbol.Parameters);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        private bool Equals(ImmutableArray<IParameterSymbol> parameters)
        {
            if (parameters.Length == this.Parameters.Length)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] != this.Parameters[i])
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
