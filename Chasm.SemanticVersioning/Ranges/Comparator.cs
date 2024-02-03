using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public abstract class Comparator : ISpanBuildable
    {
        public abstract bool IsPrimitive { get; }

        [Pure] public abstract bool CanMatchPreRelease(int major, int minor, int patch);
        [Pure] public abstract bool IsSatisfiedBy(SemanticVersion? version);

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
