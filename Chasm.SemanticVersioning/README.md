# Chasm.SemanticVersioning

A semantic versioning library focused on functionality and performance.

## To-do List

- `SemanticVersion` parsing;
  - Strict SemVer 2.0.0 parsing;
  - Loose parsing with options;
  - Option to allow remove empty pre-releases/build metadata;
  - Option to allow older syntax `1.2.3beta5`;
  - Option to parse only a part of the source text;
- Advanced `SemanticVersion` formatting (`M.m.p-rrr+ddd`);
- `node-semver` version ranges:
  - Class `PartialVersion`;
    - Basic implementation;
    - Parsing and formatting partial versions;
  - Classes `VersionRange`, `ComparatorSet`, `Comparator`;
    - Basic implementation;
    - Primitive version comparators;
    - Advanced version comparators;
    - Parsing of version ranges and its components;
