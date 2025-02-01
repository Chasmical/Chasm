namespace Chasm.SemanticVersioning.Benchmarks
{
    public static class RangeSamples
    {
        public static readonly string[] Sample1 =
        [
            // Simple primitive comparators
            ">=0.1.0",
            "<4.0.0-0",
            "=4.3.0-alpha.7",
        ];
        public static readonly string[] Sample2 =
        [
            // A pair of primitives and advanced comparators
            ">=1.0.0 <1.3.0-0",
            "^5.0.0-beta.7",
            "1.2.x - 3.4.*",
        ];
        public static readonly string[] Sample3 =
        [
            // Two comparator sets
            "=2.4.0 || >=2.5.0",
            "^5.2.0 || ~1.2.x",
            ">=0.1.0 <0.3.0-0 || 0.5 - 0.6.x",
        ];
        public static readonly string[] Sample4 =
        [
            // A lot of different comparator sets
            ">=1.2.0 <=1.5.0 || >1.7.0-alpha.5 <2.0.0-0 || >=3.0.0-beta.4 || >=1.4.0 ^1.3.0",
            "1.x 2.0 - 3.0.5 <=3.0.2 || 1.2 - 1.4.5-beta || 5.6 - 5.7 || ~3.2",
            "^2.0.0-beta.5 <2.5.0 || ~1.2.* >1.2.4 || ^5.0.0 || ~3.4.x",
        ];

        public static readonly string[] SimplifiedSample2 =
        [
            ">=1.0.0 <1.3.0-0",
            "^5.0.0-beta.7",
            "1.2 - 3.4.0",
        ];
        public static readonly string[] SimplifiedSample3 =
        [
            "=2.4.0 || >=2.5.0",
            "^5.2.0 || ~1.2.5",
            ">=0.1.0 <0.3.0-0 || 0.5.4 - 0.6.7",
        ];
        public static readonly string[] SimplifiedSample4 =
        [
            ">=1.2.0 <=1.5.0 || >1.7.0-alpha.5 <2.0.0-0 || >=3.0.0-beta.4 || >=1.4.0 ^1.3.0",
            "2.0 - 3.0.5 || 1.x <=3.0.2 || 1.2 - 1.4.5-beta || 5.6 - 5.7 || ~3.2",
            "^2.0.0-beta.5 <2.5.0 || ~1.2.1 >1.2.4 || ^5.0.0 || ~3.4.3",
        ];

    }
}
