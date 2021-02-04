using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System;
using SemVersion.Parser;
using SemVersion;
using SharpPcap;

namespace Test
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class LibpcapVersionAttribute : IncludeExcludeAttribute, IApplyToTest
    {

        /// <summary>
        /// Constructor taking one or more platforms
        /// </summary>
        /// <param name="platforms">Comma-delimited list of platforms</param>
        public LibpcapVersionAttribute(string include) : base(include) { }

        /// <summary>
        /// Causes a test to be skipped if this LibpcapVersion is not satisfied.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(NUnit.Framework.Internal.Test test)
        {
            if (test.RunState != RunState.NotRunnable &&
                test.RunState != RunState.Ignored)
            {
                try
                {
                    if (
                        (Include != null && !IsVersionSupported(Include)) ||
                        (Exclude != null && IsVersionSupported(Exclude))
                        )
                    {
                        var reason = string.Format("Not supported on Libpcap v{0}", Pcap.LibpcapVersion);
                        test.RunState = RunState.Skipped;
                        test.Properties.Add(PropertyNames.SkipReason, reason);
                    }
                }
                catch (Exception ex)
                {
                    test.RunState = RunState.NotRunnable;
                    test.Properties.Add(PropertyNames.SkipReason, ex.Message);
                }
            }
        }

        private static bool IsVersionSupported(string range)
        {
            var parser = new RangeParser();
            var predicate = parser.Evaluate(range);
            var v = Pcap.LibpcapVersion;
            var version = new SemanticVersion(v.Major, v.Minor, Math.Max(v.Build, 0));
            return predicate(version);
        }
    }
}