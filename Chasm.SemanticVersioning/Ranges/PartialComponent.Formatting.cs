using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Ranges
{
    public readonly partial struct PartialComponent : ISpanBuildable
    {
        [Pure] internal int CalculateLength()
        {
            if (_value == -1) return 0;
            uint val = Math.Max((uint)_value, 0u); // wildcards take up one character, just like zero
            return SpanBuilder.CalculateLength(val);
        }
        internal void BuildString(ref SpanBuilder sb)
        {
            if (_value == -1) return;
            if (_value > -1) sb.Append((uint)_value);
            else sb.Append((char)-_value);
        }
        [Pure] int ISpanBuildable.CalculateLength() => CalculateLength();
        void ISpanBuildable.BuildString(ref SpanBuilder sb) => BuildString(ref sb);

        /// <summary>
        ///   <para>Returns the string representation of this partial version component.</para>
        /// </summary>
        /// <returns>The string representation of this partial version component.</returns>
        [Pure] public override string ToString()
        {
            if (_value > -1) return ((uint)_value).ToString();
            return _value switch
            {
                -'x' => "x",
                -'X' => "X",
                -'*' => "*",
                _ => "",
            };
        }

    }
}
