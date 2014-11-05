﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gauge.VisualStudio.Classification;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Gauge.VisualStudio.TestRunner
{
    [DefaultExecutorUri(TestExecutor.ExecutorUriString)]
    [FileExtension(".spec")]
    [FileExtension(".md")]
    public class TestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            GetSpecs(sources, discoverySink);
        }

        public static List<TestCase> GetSpecs(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var testCases = new List<TestCase>();

            Parallel.ForEach(sources, spec =>
            {
                var source = File.ReadAllText(spec);

                var scenarios = Parser.GetScenarios(source);
                var specificationName = Parser.GetSpecificationName(source);

                foreach (var scenario in scenarios)
                {
                    var testCase = new TestCase(string.Format("{0}.{1}", specificationName, scenario), TestExecutor.ExecutorUri,
                        spec)
                    {
                        CodeFilePath = spec,
                        DisplayName = scenario
                    };

                    if (discoverySink != null)
                    {
                        discoverySink.SendTestCase(testCase);
                    }
                    testCases.Add(testCase);
                }
            });

            return testCases;
        }
    }
}