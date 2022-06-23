using System.Reflection;
using Xunit;

namespace ApiCoverageTool.AssemblyProcessing
{
    public class XUnitTestsProcessor : ITestsProcessor
    {
        public bool IsTestMethod(MethodInfo method) =>
            method.GetCustomAttribute(typeof(FactAttribute)) is not null ||
            method.GetCustomAttribute(typeof(TheoryAttribute)) is not null;
    }
}
