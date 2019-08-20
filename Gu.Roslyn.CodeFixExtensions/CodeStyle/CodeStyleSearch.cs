namespace Gu.Roslyn.CodeFixExtensions
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Specify how <see cref="CompilationStyleWalker{T}"/> walks <see cref="SyntaxTree"/>.
    /// Ignore members as an optimization.
    /// </summary>
    [Flags]
    public enum CodeStyleSearch
    {
        /// <summary>
        /// Walk all nodes.
        /// </summary>
        All = 0,

        /// <summary>
        /// Don't walk static members.
        /// </summary>
        IgnoreStaticMembers,

        /// <summary>
        /// Don't walk instance members.
        /// </summary>
        IgnoreInstanceMembers,

        /// <summary>
        /// Don't walk public members.
        /// </summary>
        IgnorePublicMembers,

        /// <summary>
        /// Don't walk internal members.
        /// </summary>
        IgnoreInternalMembers,

        /// <summary>
        /// Don't walk protected members.
        /// </summary>
        IgnoreProtectedMembers,

        /// <summary>
        /// Don't walk private members.
        /// </summary>
        IgnorePrivateMembers,

        /// <summary>
        /// Don't walk abstract members.
        /// </summary>
        IgnoreAbstractMembers,

        /// <summary>
        /// Don't walk virtual members.
        /// </summary>
        IgnoreVirtualMembers,

        /// <summary>
        /// Don't walk sealed members.
        /// </summary>
        IgnoreSealedMembers,

        /// <summary>
        /// Don't walk <see cref="FieldDeclarationSyntax"/>.
        /// </summary>
        IgnoreFieldDeclarations,

        /// <summary>
        /// Don't walk <see cref="ConstructorDeclarationSyntax"/>
        /// </summary>
        IgnoreConstructorDeclarations,

        /// <summary>
        /// Ignore <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        IgnorePropertyDeclarations,

        /// <summary>
        /// Ignore <see cref="EventDeclarationSyntax"/> and <see cref="EventFieldDeclarationSyntax"/>.
        /// </summary>
        IgnoreEventDeclarations,

        /// <summary>
        /// Ignore <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        IgnoreMethodDeclarations,

        /// <summary>
        /// Ignore nested <see cref="TypeDeclarationSyntax"/>.
        /// </summary>
        IgnoreNestedTypeDeclarations,
    }
}
