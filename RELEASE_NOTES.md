#### 0.17.1
BUGFIX: handle target typed new

#### 0.16.5
* BUGFIX: Handle SuppressNullableWarningExpression

#### 0.16.4
* AMEND: Fic warning 

#### 0.16.3
* BUGFIX: Handle top level statements

#### 0.16.2
* Add missing annotation to T4 output.

#### 0.16.1
* Explicit #nullable enable needed

#### 0.16.0
* BREAKING: Update Roslyn dependency to 4.0.1

#### 0.15.7
* BUFIX: IMethodSymbol.FindArgument(parameter) when extension method invocation.

#### 0.15.6
* BUFIX: IMethodSymbol.FindParameter(name) when extension method invocation.

#### 0.15.5
* BUFIX: IsRepresentationPreservingConversion when cast reference type.

#### 0.15.3
* BUGFIX: Don't use Roslyn SymbolEqualityComparer

#### 0.15.0
* BREAKING: recursion.Target() returns syntax node for invocation as it can be a local function

#### 0.14.4
* BUGFIX: QualifiedType == BaseTypeSyntax.

#### 0.14.3
* Only compare nullability if both types are annotated reference types.

#### 0.14.2
* Only compare nullability if both types are annotated.

#### 0.14.1
* Return annotated types from GetType()

#### 0.14.0
* BREAKING: Use Roslyn 3.5.0

#### 0.13.0
* BREAKING: Roslyn 3.3.1 and netstandard 2.0

#### 0.12.9
* BUGFIX: Handle default CodeStyleOptions

#### 0.12.8
* BUGFIX: Handle using C = C

#### 0.12.6
* BUGFIX: Infinite recursion when comparing generic parameter.

#### 0.7
FEATURE: Scope
BREAKING: Rename SearchScope, was Scope
BREAKING: Move IsExecutedBefore extension method to Scope

#### 0.6.2
FEATURE: DocumentEditor.MoveBefore and MoveAfter.
BUGFIX: Simplify in doc comments.
BUGFIX: AttributeSyntaxExt.TryFindArgument when name.
BREAKING: Move TryFindArgument to AttributeSyntaxExt.
FEATURE: QualifiedType.Equals handle alias.

#### 0.6
* BREAKING: rename SyntaxTreeCache and SyntaxTreeCacheAnalyzer
* BREAKING: Remove unused CanecllationToken parameter in IsRepresentationPreservingConversion
* FEATURE: Equality
* BUGFIX: Nullcheck
* BREAKING: MemberPath.Tokens was IdentiferNames. 
* BREAKING: Cache.Begin returns transaction.
* BREAKING: return CodeStyleResult

#### 0.5.4
* FEATURE: SealRewriter
* FEATURE: SortRewriter

#### 0.5.3
* FEATURE: QuyalifyFieldAccess, QuyalifyPropertyAccess and QuyalifyMethodAccess.
* BREAKING: UnderscoreFields checks only field declarations.
* FEATURE: More walkers.
* FIX: AddMember when directives.
* BUGFIX: TryFindArgument when multiple optional.

#### 0.5.1
* BUGFIX: Ignore object initializer when figuring out underscore names.

#### 0.5.0
* BUGFIX: Handle discard symbol in TrygetSymbol.
* FEATURE: More TrygetSymbol overloads.
* BREAKING: TryGetSymbol signatures.

#### 0.4.2
* BREAKING: TryGetSymbol returns array symbol or indexer.

#### 0.3.4
* BREAKING: Move IsExecutedBefore from syntax node to ExpressionmSyntax & StatementSyntax
* FEATURE: PooledList<T> for silly zero allocation.

#### 0.3.4
* FEATURE: TryFindTarget with parameters.
* FEATURE: QualifiedOverload
* FEATURE: QualifiedArrayType
* FEATURE: QualifiedGenericType

#### 0.3.3
* BUGFIX: TryGetConstantValue when null & enum.

#### 0.3.2
* BUGFIX: Walk System.Object in TryFindMemberRecursive.

#### 0.3.1.4
* BUGFIX: Walk overridden.

#### 0.3.1
* BUGFIX: Handle trivia when pragma.

#### 0.3.1
* FEATURE: helpers for working with doc comments.

#### 0.2.1
* More overload for EnumerableExt
