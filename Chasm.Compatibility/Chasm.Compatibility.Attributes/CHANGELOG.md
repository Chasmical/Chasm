# Chasm.Compatibility.Attributes Changelog

### v2.2.0
- ✨ Added polyfills for attributes introduced in .NET 9, and some trim warning and dynamic dependency attributes;
- 🧩 Added `net9.0` target. Now targets: `net9.0`, `net8.0`, `net7.0`, `net6.0`, `net5.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netcoreapp1.1`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.1`, `netstandard1.0`, `net45`, `net40`, `net35`;
- ✨ Added `[ParamCollection]` polyfill;
- ✨ Added `[OverloadResolutionPriority]` polyfill;
- ✨ Added `[FeatureGuard]` polyfill;
- ✨ Added `[FeatureSwitchDefinition]` polyfill;
- ✨ Added `[DynamicDependency]` polyfill;
- ✨ Added `[DynamicallyAccessedMembers]` polyfill;
- ✨ Added `enum DynamicallyAccessedMemberTypes` polyfill;
- ✨ Added `[RequiresAssemblyFiles]` polyfill;
- ✨ Added `[RequiresDynamicCode]` polyfill;
- ✨ Added `[RequiresUnreferencedCode]` polyfill;

### v2.1.2
- 🐛 Fixed `InAttribute` type-forwarding for .NET Framework targets;

### v2.1.1
- ✨ Added `CollectionBuilderAttribute` polyfill;

### v2.1.0
- 🧩 Added `netstandard1.1` and `net45` targets. Now targets: `net8.0`, `net7.0`, `net6.0`, `net5.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netcoreapp1.1`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.1`, `netstandard1.0`, `net45`, `net40`, `net35`;
- ✨ Added `CallerArgumentExpressionAttribute` polyfill;
- ✨ Added `CallerFilePathAttribute` polyfill;
- ✨ Added `CallerLineNumberAttribute` polyfill;
- ✨ Added `CallerMemberNameAttribute` polyfill;
- ✨ Added `UnconditionalSuppressMessageAttribute` polyfill;
- ✨ Added `CompilerFeatureRequiredAttribute` polyfill;
- ✨ Added `InAttribute` polyfill;
- ✨ Added `RequiresLocationAttribute` polyfill;
- ✨ Added `RequiresPreviewFeaturesAttribute` polyfill;
- ✨ Added `StackTraceHiddenAttribute` polyfill;

### v2.0.0
- 🧩 Targets: `net8.0`, `net7.0`, `net6.0`, `net5.0`, `netcoreapp3.0`, `netcoreapp2.0`, `netcoreapp1.1`, `netcoreapp1.0`, `netstandard2.1`, `netstandard2.0`, `netstandard1.0`, `net40`, `net35`;
- ✨ Added `ConstantExpectedAttribute` polyfill;
- ✨ Added `ExcludeFromCodeCoverageAttribute` polyfill;
- ✨ Added `ExperimentalAttribute` polyfill;
- ✨ Added `SetsRequiredMembersAttribute` polyfill;
- ✨ Added `StringSyntaxAttribute` polyfill;
- ✨ Added `UnscopedRefAttribute` polyfill;
- ✨ Added `AsyncMethodBuilderAttribute` polyfill;
- ✨ Added `InterpolatedStringHandlerAttribute` polyfill;
- ✨ Added `InterpolatedStringHandlerArgumentAttribute` polyfill;
- ✨ Added `IsExternalInit` polyfill;
- ✨ Added `ModuleInitializerAttribute` polyfill;
- ✨ Added `RequiredMemberAttribute` polyfill;
- ✨ Added `SkipLocalsInitAttribute` polyfill;
- ✨ Added `AllowNullAttribute` polyfill;
- ✨ Added `DisallowNullAttribute` polyfill;
- ✨ Added `DoesNotReturnAttribute` polyfill;
- ✨ Added `DoesNotReturnIfAttribute` polyfill;
- ✨ Added `MaybeNullAttribute` polyfill;
- ✨ Added `MaybeNullWhenAttribute` polyfill;
- ✨ Added `MemberNotNullAttribute` polyfill;
- ✨ Added `MemberNotNullWhenAttribute` polyfill;
- ✨ Added `NotNullAttribute` polyfill;
- ✨ Added `NotNullIfNotNullAttribute` polyfill;
- ✨ Added `NotNullWhenAttribute` polyfill;
