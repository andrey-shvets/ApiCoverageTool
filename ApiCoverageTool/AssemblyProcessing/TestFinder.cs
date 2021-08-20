using System.Collections.Generic;
using System.Reflection;

namespace ApiCoverageTool.AssemblyProcessing
{
    public class TestFinder
    {
        public List<ITestsProcessor> TestProcessors { get; } = new List<ITestsProcessor>();

        public bool IsTestMethod(MethodInfo method)
        {
            foreach (var processor in TestProcessors)
                if (processor.IsTestMethod(method))
                    return true;

            return false;
        }
    }
}
