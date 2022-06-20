using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApiCoverageTool.AssemblyProcessing;

public class MSTestTestsProcessor : ITestsProcessor
{
    public bool IsTestMethod(MethodInfo method) => method.GetCustomAttribute(typeof(TestMethodAttribute)) is not null;
}
