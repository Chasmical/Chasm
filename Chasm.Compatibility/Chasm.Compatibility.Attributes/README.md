# Chasm.Compatibility.Attributes

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Compatibility.Attributes)](https://www.nuget.org/packages/Chasm.Compatibility.Attributes/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../../LICENSE)



## Compiler-interpreted attributes

These attributes are interpreted by the compiler, and are generally not used directly in the compiled assembly.

- `[In]` (Core 1.0+ / Standard 1.1+ / Framework 1.1+) - `ref readonly` method return type;
- `[CallerFilePath]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[CallerLineNumber]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[CallerMemberName]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[AsyncMethodBuilder]` (Core 1.1+ / Standard 2.0+);
- `[CallerArgumentExpression]` (Core 3.0+);
- `[ModuleInitializer]` (.NET 5+) - module initializers are a CLI spec feature and work on older targets as well;
- `[SkipLocalsInit]` (.NET 5+) - simply removes the `.locals init` flag from metadata;
- `IsExternalInit` metadata class (.NET 5+) - used for `{ init; }` properties and `record` types;
- `[InterpolatedStringHandler]` (.NET 6+) - allows using your own string interpolation handlers;
- `[InterpolatedStringHandlerArgument]` (.NET 6+);
- `[CompilerFeatureRequired]` (.NET 7+) - allows declaring `required` members and `ref struct` types;
- `[RequiredMember]` (.NET 7+) - allows declaring `required` members;
- `[RequiresLocation]` (.NET 8+) - `ref readonly` parameters in function pointers.
- `[CollectionBuilder]` (.NET 8+) - used for creating custom collections from spans.



## Nullable analysis attributes

Attributes to assist with C# 8.0's `#nullable` context and nullable reference type analysis.

- `[AllowNull]` (Core 3.0+ / Standard 2.1+);
- `[DisallowNull]`;
- `[DoesNotReturn]`;
- `[DoesNotReturnIf]`;
- `[MaybeNull]`;
- `[MaybeNullWhen]`;
- `[NotNull]`;
- `[NotNullIfNotNull]`;
- `[NotNullWhen]`;
- `[MemberNotNull]` (.NET 5+);
- `[MemberNotNullWhen]` (.NET 5+).



## Code analysis attributes

Attributes for code analyzers, IDE suggestions, syntax highlighting, and other tools.

- `[ExcludeFromCodeCoverage]` (Core 2.0+ / Standard 2.0+ / Framework 4.0+);
- `[UnconditionalSuppressMessage]` (.NET 5+);
- `[RequiresPreviewFeatures]` (.NET 6+);
- `[StringSyntax]` (.NET 7+);
- `[ConstantExpected]` (.NET 7+);
- `[SetsRequiredMembers]` (.NET 7+);
- `[UnscopedRef]` (.NET 7+);
- `[Experimental]` (.NET 8+).

Note: `ExcludeFromCodeCoverageAttribute.Justification` property is not shimmed, since it was added only in .NET 5.



## Attributes used at runtime

- `[StackTraceHidden]` (.NET 6+) - omits the method/type from `StackTrace.ToString()`.



## To-do List

Future .NET versions:

- [ ] `[FeatureGuard]` (.NET 9+);
- [ ] `[FeatureSwitchDefinition]` (.NET 9+);
- [ ] `[OverloadResolutionPriority]` (.NET 9+);
- [ ] `[ParamCollection]` (.NET 9+);

I'm not sure how the attributes below work, so I'll put it off for now:

- [ ] `[DisableRuntimeMarshalling]`;
- [ ] `[SuppressGCTransition]`;
- [ ] `[UnmanagedCallersOnly]`;
- [ ] `[UnsafeAccessor]`;
- [ ] `[InlineArray]`;
- [ ] `[DynamicallyAccessedMembers]`;
- [ ] `enum DynamicallyAccessedMemberTypes`;
- [ ] `[DynamicDependency]`;
- [ ] `[RequiresAssemblyFiles]`;
- [ ] `[RequiresDynamicCode]`;
- [ ] `[RequiresUnreferencedCode]`;



## Not needed

The following attributes are not provided by this package:

- Attributes for C++ and Visual Basic compilers;
- `[RefSafetyRules]` - generated automatically;
- `[ScopedRef]` - generated automatically, for `scoped ref` parameters;
- `[NullableContext]` - generated automatically, on types and methods in `#nullable` context;
- `[Nullable]` - generated automatically, on types, members and parameters in `#nullable` context;
- `[TupleElementNames]` - provided in the `ValueTuple` package;
- `[EnumeratorCancellation]` - that would require adding `TaskAsyncEnumerableExtensions` as well;



## Can't be added

- `[PreserveBaseOverrides]` - requires runtime support of covariant return types;


