using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public readonly partial struct PartialComponent : ISpanBuildable
    {
        [Pure] internal int CalculateLength()
        {
            if ((int)_value != -1)
            {
                // wildcards take up one character, just like zero
                uint num = (uint)GetValueOrZero();
                return SpanBuilder.CalculateLength(num);
            }
            return 0;
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            uint value = _value;
            if ((int)value != -1)
            {
                if ((int)value > -1) sb.Append(value);
                else sb.Append((char)-(int)value);
            }
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this partial version component.</para>
        /// </summary>
        /// <returns>The string representation of this partial version component.</returns>
        [Pure] public override string ToString()
        {
            uint value = _value;
            if ((int)value > -1) return value.ToString();

            // 'x': -120, 'X': -88, '*': -42, omitted: -1

            if ((int)value <= -88)
                return (int)value == -88 ? "X" : "x";
            return (int)value != -1 ? "*" : "";
        }

    }
}
