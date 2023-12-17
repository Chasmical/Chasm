namespace Chasm.SemanticVersioning
{
    public sealed class SemanticVersion
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string[] PreReleases { get; }
        public string[] BuildMetadata { get; }
    }
}
