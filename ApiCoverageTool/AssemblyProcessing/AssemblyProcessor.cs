using System;
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
    public static class AssemblyProcessor
    {
        private const string DefaultIncludeAssemblyMask = ".+";
        private const string DefaultExcludeAssemblyMask =
            @"^System(\.|$)|^Microsoft(\.|$)|^Mono(\.|$)|^xunit(\.|$)|^RestEase(\.|$)|^FluentAssertions(\.|$)|^Flurl(\.|$)|^Newtonsoft(\.|$)|^ApiCoverageTool$";

        public static IList<MethodInfo> GetAllTests(Assembly assembly)
        {
            assembly.IsNotNullValidation(nameof(assembly));

            // TODO: Move this to a factory
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

                var methodsToProcess = GetMethodCallsFromMethodBody(methodToCheck, assembliesToCheck);
                var newMethods = methodsToProcess.Where(m => !methodsCalled.Contains(m));

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

            assembliesToCheck ??= GetAvailableAssemblies(methodDefinition.Module.Assembly);

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
            var asyncMethodStateMachine = RetrieveAsyncMethodStateMachineType(method);

            if (asyncMethodStateMachine is null)
                throw new InvalidOperationException($"Failed to retrieve state machine class for async method {method.Name}");

            var asyncMethodImplementation = asyncMethodStateMachine.Methods.FirstOrDefault(m => m.Name == "MoveNext");

            if (asyncMethodImplementation is null)
                throw new InvalidOperationException($"Failed to retrieve method definition for async method {method.Name}");

            return GetMethodCallsFromNonAsyncMethodBody(asyncMethodImplementation, assembliesToCheck);
        }

        private static TypeDefinition RetrieveAsyncMethodStateMachineType(MethodDefinition method) =>
            TryRetrieveAsyncMethodStateMachineTypeFromCreationInstruction(method) ??
            TryRetrieveAsyncMethodStateMachineTypeFromStartInstruction(method);

        private static TypeDefinition TryRetrieveAsyncMethodStateMachineTypeFromCreationInstruction(MethodDefinition method)
        {
            var stateMachineInstanceCreationInstruction = method.Body.Instructions
                .FirstOrDefault(instruction => instruction.OpCode == OpCodes.Newobj);

            if (stateMachineInstanceCreationInstruction is null)
                return null;

            var asyncMethodStateMachineInstanceCreation = stateMachineInstanceCreationInstruction.Operand as MethodReference;
            var asyncMethodStateMachine = asyncMethodStateMachineInstanceCreation!.Resolve().DeclaringType;

            return asyncMethodStateMachine;
        }

        private static TypeDefinition TryRetrieveAsyncMethodStateMachineTypeFromStartInstruction(MethodDefinition method)
        {
            var state1 = method.Body.Instructions
                .Where(instruction => instruction.OpCode == OpCodes.Call).Select(instruction => instruction.Operand).OfType<MethodReference>().ToList();

            var stateMachineInstanceCreationInstruction = method.Body.Instructions
                .FirstOrDefault(instruction => instruction.Operand is GenericInstanceMethod);

            if (stateMachineInstanceCreationInstruction is null)
                return null;

            var asyncMethodStateMachineInstanceCreation = stateMachineInstanceCreationInstruction.Operand as GenericInstanceMethod;
            var asyncMethodStateMachine = asyncMethodStateMachineInstanceCreation!.GenericArguments.First().Resolve();

            return asyncMethodStateMachine;
        }

        private static IList<string> GetAvailableAssemblies(
            AssemblyDefinition assembly,
            string includeAssemblyRegex = DefaultIncludeAssemblyMask,
            string excludeAssemblyRegex = DefaultExcludeAssemblyMask)
        {
            assembly.IsNotNullValidation(nameof(assembly));

            var availableAssemblies = assembly.MainModule.AssemblyReferences.Select(a => a.Name).ToList();
            availableAssemblies.Add(assembly.Name.Name);

            var filteredNames = availableAssemblies
                .Where(name => Regex.IsMatch(name, includeAssemblyRegex))
                .Where(name => !Regex.IsMatch(name, excludeAssemblyRegex))
                .ToList();

            return filteredNames;
        }

        #region Extensions

        private static bool IsAsync(this MethodInfo method) => method.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;

        private static bool IsFromAssemblies(this MethodDefinition method, IEnumerable<string> assemblies) => assemblies.Contains(method.Module.Assembly.Name.Name);

        private static bool IsMethodCall(this Instruction instruction) =>
            instruction.OpCode == OpCodes.Call
            || instruction.OpCode == OpCodes.Calli
            || instruction.OpCode == OpCodes.Callvirt;

        private static bool IsLambdaExpression(this Instruction instruction) => instruction.OpCode == OpCodes.Ldftn;

        #endregion Extensions
    }
}
