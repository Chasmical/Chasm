﻿#if NOT_PUBLISHING_PACKAGE
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Chasm.Compatibility
{
    // A compilation-time test to ensure that the polyfilled API is available here
    internal class Test
    {
        public required int Number;

        public static void Run(ref readonly string t)
        {
            List<int> numbers = [0, 1, 2, 3, 4, 5, t.Length];

            byte[] buffer = [0, 0, 1, 2];
            int int32 = Unsafe.As<byte, int>(ref buffer[0]);

            (string Name, int A) tuple = ("two", 2);

            int num = numbers[^tuple.A];

            _ = HashCode.Combine(int32, tuple, num);
        }
    }
}
#endif
