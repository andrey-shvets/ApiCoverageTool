using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ApiCoverageTool.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApiCoverageTool.AssemblyProcessing
{
    public static class AssemblyPocessor
    {
        private const string DefaultIncludeAssemblyMask = ".+";
        private const string DefaultExcludeAssemblyMask =
            @"^System(\.|$)|^Microsoft(\.|$)|^Mono(\.|$)|^xunit(\.|$)|^RestEase(\.|$)|^FluentAssertions(\.|$)|^Flurl(\.|$)|^Newtonsoft(\.|$)|^ApiCoverageTool$";

        public static IList<MethodInfo> GetAllTests(Assembly assembly)
        {
            assembly.IsNotNullValidation(nameof(assembly));

            //TODO: Move this to a factory
            var testFinder = new TestFinder();
            testFinder.TestProcessors.Add(new XUnitTestsProcessor());
            testFinder.TestProcessors.Add(new MSTestTestsProcessor());

            var testMethods = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => testFinder.IsTestMethod(method))
                .ToList();

            return testMethods;
        }

        public static HashSet<MethodInfo> GetAllMethodCalls(MethodInfo method, IEnumerable<string> assembliesToCheck = null)
        {
            method.IsNotNullValidation(nameof(method));

            var methodsCalled = new HashSet<MethodInfo>();
            var methodsToCheck = new Queue<MethodInfo>();

            methodsToCheck.Enqueue(method);

            while (methodsToCheck.Any())
            {
                var methodToCheck = methodsToCheck.Dequeue();

                var methodstoProcess = GetMethodCallsFromMethodBody(methodToCheck, assembliesToCheck);
                var newMethods = methodstoProcess.Where(m => !methodsCalled.Contains(m));

                foreach (var calledMethod in newMethods)
                {
                    methodsToCheck.Enqueue(calledMethod);
                    methodsCalled.Add(calledMethod);
                }
            }

            return methodsCalled;
        }

        public static HashSet<MethodInfo> GetMethodCallsFromMethodBody(MethodInfo method, IEnumerable<string> assembliesToCheck = null)
        {
            method.IsNotNullValidation(nameof(method));

            var methodDefinition = method.ToMethodDefinition();

            if (!methodDefinition.HasBody)
                return new HashSet<MethodInfo>();

            assembliesToCheck ??= GetAvailaibleAssemblies(methodDefinition.Module.Assembly);

            if (method.IsAsync())
                return GetMethodCallsFromAsyncMethodBody(methodDefinition, assembliesToCheck);
            else
                return GetMethodCallsFromNonAsyncMethodBody(methodDefinition, assembliesToCheck);
        }

        private static HashSet<MethodInfo> GetMethodCallsFromNonAsyncMethodBody(MethodDefinition method, IEnumerable<string> assembliesToCheck)
        {
            var calledMethods = method.Body.Instructions
                .Where(instruction => instruction.IsMethodCall() ||
                    instruction.IsLambdaExpression())
                .Select(instruction => instruction.Operand).OfType<MethodReference>()
                .Select(mr => mr.Resolve())
                .Where(mr => mr.IsFromAssemblies(assembliesToCheck))
                .Select(m => m.ToMethodInfo())
                .ToHashSet();

            return calledMethods;
        }

        private static HashSet<MethodInfo> GetMethodCallsFromAsyncMethodBody(MethodDefinition method, IEnumerable<string> assembliesToCheck)
        {
            var asyncMethodStateMachineInstanceCreation = method.Body.Instructions
                .First(instruction => instruction.OpCode == OpCodes.Newobj)
                .Operand as MethodReference;

            var asyncMethodStateMachine = asyncMethodStateMachineInstanceCreation.Resolve().DeclaringType;
            var asyncMethodImplementation = asyncMethodStateMachine.Methods.First(m => m.Name == "MoveNext");
            
            return GetMethodCallsFromNonAsyncMethodBody(asyncMethodImplementation, assembliesToCheck);
        }

        private static IList<string> GetAvailaibleAssemblies(AssemblyDefinition assembly,
            string includeAssemblyRegex = DefaultIncludeAssemblyMask,
            string excludeAssemblyRegex = DefaultExcludeAssemblyMask)
        {
            assembly.IsNotNullValidation(nameof(assembly));

            var availaibleAssemblies = assembly.MainModule.AssemblyReferences.Select(a => a.Name).ToList();
            availaibleAssemblies.Add(assembly.Name.Name);

            var filteredNames = availaibleAssemblies
                .Where(name => Regex.IsMatch(name, includeAssemblyRegex))
                .Where(name => !Regex.IsMatch(name, excludeAssemblyRegex))
                .ToList();

            return filteredNames;
        }

        #region Extensions
        private static bool IsAsync(this MethodInfo method)
        {
            return method.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;
        }
        
        private static bool IsFromAssemblies(this MethodDefinition method, IEnumerable<string> assemblies)
        {
            return assemblies.Contains(method.Module.Assembly.Name.Name);
        }

        private static bool IsMethodCall(this Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Call 
                || instruction.OpCode == OpCodes.Calli 
                || instruction.OpCode == OpCodes.Callvirt;
        }

        private static bool IsLambdaExpression(this Instruction instruction)
        {
            return instruction.OpCode == OpCodes.Ldftn;
        }
        #endregion Extensions
    }
}
