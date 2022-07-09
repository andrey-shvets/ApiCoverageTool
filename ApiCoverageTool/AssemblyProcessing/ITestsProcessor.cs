using System.Reflection;

namespace ApiCoverageTool.AssemblyProcessing;

public interface ITestsProcessor
{
    public bool IsTestMethod(MethodInfo method);
}
