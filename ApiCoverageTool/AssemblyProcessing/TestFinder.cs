using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCoverageTool.AssemblyProcessing
{
    public class TestFinder
    {
        public List<ITestsProcessor> TestProcessors { get; } = new List<ITestsProcessor>();

        public bool IsTestMethod(MethodInfo method)
        {
            return TestProcessors.Any(processor => processor.IsTestMethod(method));
        }
    }
}
