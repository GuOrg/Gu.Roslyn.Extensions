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
