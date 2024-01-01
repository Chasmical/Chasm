# Chasm.SemanticVersioning

A semantic versioning library focused on functionality and performance.

## To-do List

### v2.0.0 milestones

- [x] `SemanticVersion` class;
- [x] `SemverPreRelease` struct;
- [x] `SemverPreRelease` parsing and formatting;
- [x] `SemanticVersion` strict parsing;
- [ ] `SemanticVersion` loose parsing;

### Extra functionality

- [ ] Use more efficient formatting (Chasm.Formatting);
- [ ] Advanced `SemanticVersion` formatting (`M.m.p-rrr+ddd`);
- [ ] `SemanticVersionBuilder` class;
- [ ] `BuildMetadataComparer` class;
- [ ] Advanced `SemverPreRelease` formatting, maybe?;
- [ ] `SemverPreRelease.ParseMultiple/Many` method;
- [ ] Option to ignore empty pre-releases/build metadata during parsing;
- [ ] Option to allow an older version syntax, like `1.2.3beta5`;

### `node-semver` version ranges

- [ ] Classes `PartialVersion`, `VersionRange`, `ComparatorSet`, `Comparator`;
- [ ] Primitive version comparators;
- [ ] Advanced version comparators;
- [ ] `PartialVersion` parsing and formatting;
- [ ] Parsing of version ranges and its components;
