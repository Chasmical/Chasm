# Chasm.Compatibility.Attributes

[![Latest NuGet version](https://img.shields.io/nuget/v/Chasm.Compatibility.Attributes)](https://www.nuget.org/packages/Chasm.Compatibility.Attributes/)
[![MIT License](https://img.shields.io/github/license/Chasmical/Chasm)](../../LICENSE)



## Compiler-interpreted attributes

These attributes are interpreted by the compiler, and are generally not used directly in the compiled assembly.

- `[CallerFilePath]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[CallerLineNumber]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[CallerMemberName]` (Core 1.0+ / Standard 1.0+ / Framework 4.5+);
- `[AsyncMethodBuilder]` (Core 1.1+ / Standard 2.0+);
- `[CallerArgumentExpression]` (Core 3.0+);
- `[ModuleInitializer]` (.NET 5+) - module initializers are a CLI spec feature and work on older targets as well;
- `[SkipLocalsInit]` (.NET 5+) - simply removes the `.locals init` flag from metadata;
- `IsExternalInit` metadata class (.NET 5+) - used for `{ init; }` properties and `record` types;
- `[InterpolatedStringHandler]` (.NET 6+) - allows using your own string interpolation handlers on older targets;
- `[InterpolatedStringHandlerArgument]` (.NET 6+);
- `[RequiredMember]` (.NET 7+) - allows declaring `required` members.



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
- `[StringSyntax]` (.NET 7+);
- `[ConstantExpected]` (.NET 7+);
- `[SetsRequiredMembers]` (.NET 7+);
- `[UnscopedRef]` (.NET 7+);
- `[Experimental]` (.NET 8+).

Note: `ExcludeFromCodeCoverageAttribute.Justification` property is not shimmed, since it was added only in .NET 5.



## To-do List

The following attributes may be added in the future. I need to investigate whether they're actually needed and whether it's possible to polyfill them.

- [ ] `[CollectionBuilder]`;
- [ ] `[CompilerFeatureRequired]`;
- [ ] `[RequiresLocation]`;
- [ ] `[RequiresPreviewFeatures]`;
- [ ] `[DisableRuntimeMarshalling]`;
- [ ] `[StackTraceHidden]`;
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
- [ ] `[UnconditionalSuppressMessage]`;
- [ ] `[FeatureGuard]`;
- [ ] `[FeatureSwitchDefinition]`;
- [ ] `[IteratorStateMachine]`;
- [ ] `[AsyncIteratorStateMachine]`;
- [ ] `[StateMachine]`;
- [ ] `[AsyncStateMachine]`;
- [ ] `[EnumeratorCancellation]`;
- [ ] `[InlineArray]`;
- [ ] `[PreserveBaseOverrides]`;
- [ ] `[ScopedRef]`;
- [ ] `[TupleElementNames]` (in `ValueTuple` package?);
- [ ] `[OverloadResolutionPriority]`;
- [ ] `[ParamCollection]`;
- [ ] `[RequiredAttribute]`;


