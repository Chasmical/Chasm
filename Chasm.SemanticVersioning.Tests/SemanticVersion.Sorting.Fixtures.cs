﻿using Chasm.Collections;
using JetBrains.Annotations;

namespace Chasm.SemanticVersioning.Tests
{
    public partial class SemanticVersionTests
    {
        [Pure] public static SemanticVersion[] CreateSortingFixtures()
        {
            string[] sources =
            [
                SemanticVersion.MinValue.ToString(),
                "0.0.0-0.0",
                "0.0.0--",
                "0.0.0-A",
                "0.0.0-Z",
                "0.0.0-a",
                "0.0.0-z",
                "0.0.1",
                "0.0.4",
                "0.1.0-pre",
                "0.1.0",
                "0.1.9",
                "0.5.0",
                "1.0.0",
                "1.2.3-a",
                "1.2.3-al",
                "1.2.3-al.beta",
                "1.2.3-al-beta",
                "1.2.3-alpha",
                "1.2.3-beta",
                "1.2.3-beta.0",
                "1.2.3-beta.91",
                "1.2.3-beta.456",
                "1.2.3-beta.gamma",
                "1.2.3",
                "4.5.6-0",
                "4.5.6-pre.0",
                "4.5.6",
                "99.99.99-0",
                "99.99.99",
                SemanticVersion.MaxValue.ToString(),
            ];
            return sources.ConvertAll(SemanticVersion.Parse);
        }
    }
}
