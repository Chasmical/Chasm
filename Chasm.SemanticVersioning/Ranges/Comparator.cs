using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract class Comparator : ISpanBuildable
    {
        public abstract bool IsPrimitive { get; }

        [Pure] public abstract bool CanMatchPreRelease(int major, int minor, int patch);
        [Pure] public abstract bool IsSatisfiedBy(SemanticVersion? version);

        [Pure] protected static bool CanMatchPreRelease(SemanticVersion version, int major, int minor, int patch)
            => version.IsPreRelease && version.Major == major && version.Minor == minor && version.Patch == patch;
        [Pure] protected static bool CanMatchPreRelease(PartialVersion version, int major, int minor, int patch)
            => version.IsPreRelease
            && (!version.Major.IsNumeric || version.Major.GetValueOrZero() == major)
            && (!version.Minor.IsNumeric || version.Minor.GetValueOrZero() == minor)
            && (!version.Patch.IsNumeric || version.Patch.GetValueOrZero() == patch);

        /// <inheritdoc cref="ISpanBuildable.CalculateLength"/>
        [Pure] protected internal abstract int CalculateLength();
        /// <inheritdoc cref="ISpanBuildable.BuildString"/>
        protected internal abstract void BuildString(ref SpanBuilder sb);

        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        [Pure] public override string ToString()
            => SpanBuilder.Format(this);

    }
}
