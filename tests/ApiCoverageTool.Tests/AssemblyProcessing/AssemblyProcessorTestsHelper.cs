using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;

namespace ApiCoverageTool.Tests.AssemblyProcessing
{
    public static class AssemblyProcessorTestsHelper
    {
        public static Assembly GetAssemblyByName(string name)
        {
            var steve = AppDomain.CurrentDomain.GetAssemblies();
            return AppDomain.CurrentDomain.GetAssemblies().
                SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static void VerifyMethodsNames(IEnumerable<MethodInfo> methods, IList<string> expected)
        {
            var names = methods.Select(m => m.Name).ToList();
            names.Should().BeEquivalentTo(expected);
        }
    }
}
