namespace Chasm.SemanticVersioning.Benchmarks
{
    public static class VersionSamples
    {
        public static readonly string[] Sample1 =
        [
            // Just simple numeric versions, the most common scenario
            "0.0.0",
            "1.0.0",
            "2.3.4",
            "12.34.56",
            "99999.333444.767676767",
        ];
        public static readonly string[] Sample2 =
        [
            // Versions with a bunch of pre-release identifiers
            "1.0.0-0",
            "1.2.3-alpha.5.test",
            "4.7.2-nightly-beta.542",
            "7.21.8-pre.2.beta.7",
            "1212122.43434.465643565-pre-beta-test.70.pre-alpha",
        ];
        public static readonly string[] Sample3 =
        [
            // Versions with a lot of both pre-release and build metadata identifiers
            "0.0.1-pre.67+DEV.BUILD",
            "1.2.3-alpha.beta.2.theta.70+BUILD.METADATA.0090.DEV",
            "8.12.5-nightly-beta.7.pre+TEST-BUILD-NOT-FOR-USE",
            "67.2.50-beta-test.07t.5+DEV-TEST.05.03.2023",
            "12222223.5545454.7-alpha.34.beta.23+TEST.BUILD-METADATA.0123456789000.230",
        ];

        // The samples above converted into the libraries' semver representations
        public static readonly ChasmVersion[] ChasmSample1 = Array.ConvertAll(Sample1, ChasmVersion.Parse);
        public static readonly ChasmVersion[] ChasmSample2 = Array.ConvertAll(Sample2, ChasmVersion.Parse);
        public static readonly ChasmVersion[] ChasmSample3 = Array.ConvertAll(Sample3, ChasmVersion.Parse);

        public static readonly McSherryVersion[] McSherrySample1 = Array.ConvertAll(Sample1, McSherryVersion.Parse);
        public static readonly McSherryVersion[] McSherrySample2 = Array.ConvertAll(Sample2, McSherryVersion.Parse);
        public static readonly McSherryVersion[] McSherrySample3 = Array.ConvertAll(Sample3, McSherryVersion.Parse);

        public static readonly ReeveVersion[] ReeveSample1 = Array.ConvertAll(Sample1, v => ReeveVersion.Parse(v));
        public static readonly ReeveVersion[] ReeveSample2 = Array.ConvertAll(Sample2, v => ReeveVersion.Parse(v));
        public static readonly ReeveVersion[] ReeveSample3 = Array.ConvertAll(Sample3, v => ReeveVersion.Parse(v));

        public static readonly HauserVersion[] HauserSample1 = Array.ConvertAll(Sample1, v => HauserVersion.Parse(v));
        public static readonly HauserVersion[] HauserSample2 = Array.ConvertAll(Sample2, v => HauserVersion.Parse(v));
        public static readonly HauserVersion[] HauserSample3 = Array.ConvertAll(Sample3, v => HauserVersion.Parse(v));

        public static readonly NuGetVersion[] NuGetSample1 = Array.ConvertAll(Sample1, NuGetVersion.Parse);
        public static readonly NuGetVersion[] NuGetSample2 = Array.ConvertAll(Sample2, NuGetVersion.Parse);
        public static readonly NuGetVersion[] NuGetSample3 = Array.ConvertAll(Sample3, NuGetVersion.Parse);

    }
}
