﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ApiCoverageTool.Configurations;
using ApiCoverageTool.Extensions;
using Microsoft.Extensions.Configuration;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApiCoverageTool.AssemblyProcessing;

public static class AssemblyProcessor
{
    private static ApiCoverageOptions ApiCoverageConfiguration { get; }

    static AssemblyProcessor()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.tests.json", optional: true, reloadOnChange: false)
            .Build();

        ApiCoverageConfiguration = configuration.GetSection(ApiCoverageOptions.ApiCoverageSettings).Get<ApiCoverageOptions>();
    }

    public static IList<MethodBase> GetAllTests(Assembly assembly)
    {
        assembly.IsNotNullValidation(nameof(assembly));

        // TODO: Move this to a factory
        var testFinder = new TestFinder();
        testFinder.TestProcessors.Add(new XUnitTestsProcessor());
        testFinder.TestProcessors.Add(new MSTestTestsProcessor());

        var testMethods = assembly.GetTypes()
            .SelectMany(type => type.GetMethods())
            .Where(method => testFinder.IsTestMethod(method))
            .Select(m => m as MethodBase)
            .ToList();

        return testMethods;
    }

    public static HashSet<MethodBase> GetAllMethodCalls(MethodBase method, IEnumerable<string> assembliesToCheck = null)
    {
        method.IsNotNullValidation(nameof(method));

        var methodsCalled = new HashSet<MethodBase>();
        var methodsToCheck = new Queue<MethodBase>();

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

    public static HashSet<MethodBase> GetMethodCallsFromMethodBody(MethodBase method, IEnumerable<string> assembliesToCheck = null)
    {
        method.IsNotNullValidation(nameof(method));

        var methodDefinition = method.ToMethodDefinition();

        if (!methodDefinition.HasBody)
            return new HashSet<MethodBase>();

        assembliesToCheck ??= GetAvailableAssemblies(methodDefinition.Module.Assembly);

        if (method.IsAsync())
            return GetMethodCallsFromAsyncMethodBody(methodDefinition, assembliesToCheck);
        else
            return GetMethodCallsFromNonAsyncMethodBody(methodDefinition, assembliesToCheck);
    }

    private static HashSet<MethodBase> GetMethodCallsFromNonAsyncMethodBody(MethodDefinition method, IEnumerable<string> assembliesToCheck)
    {
        var calledMethods = method.Body.Instructions
            .Where(instruction => instruction.IsMethodCall() ||
                                  instruction.IsLambdaExpression())
            .Select(instruction => instruction.Operand).OfType<MethodReference>()
            .Where(mr => mr.IsFromAssemblies(assembliesToCheck))
            .Select(mr => mr.Resolve())
            .Select(m => m.ToMethodBase())
            .ToHashSet();

        return calledMethods;
    }

    private static HashSet<MethodBase> GetMethodCallsFromAsyncMethodBody(MethodDefinition method, IEnumerable<string> assembliesToCheck)
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
        var stateMachineInstanceCreationInstruction = method.Body.Instructions
            .FirstOrDefault(instruction => instruction.Operand is GenericInstanceMethod);

        if (stateMachineInstanceCreationInstruction is null)
            return null;

        var asyncMethodStateMachineInstanceCreation = stateMachineInstanceCreationInstruction.Operand as GenericInstanceMethod;
        var asyncMethodStateMachine = asyncMethodStateMachineInstanceCreation!.GenericArguments.First().Resolve();

        return asyncMethodStateMachine;
    }

    private static IList<string> GetAvailableAssemblies(
        AssemblyDefinition assembly)
    {
        assembly.IsNotNullValidation(nameof(assembly));

        var availableAssemblies = assembly.MainModule.AssemblyReferences.Select(a => a.Name).ToList();
        availableAssemblies.Add(assembly.Name.Name);

        var includeAssemblyRegex = ApiCoverageConfiguration.IncludeAssemblyMask;
        var excludeAssemblyRegex = ApiCoverageConfiguration.ExcludeAssemblyMask;

        var filteredNames = availableAssemblies
            .Where(name => Regex.IsMatch(name, includeAssemblyRegex))
            .Where(name => !Regex.IsMatch(name, excludeAssemblyRegex))
            .ToList();

        return filteredNames;
    }

    #region Extensions

    private static bool IsAsync(this MethodBase method) => method.GetCustomAttribute<AsyncStateMachineAttribute>() is not null;

    private static bool IsFromAssemblies(this MemberReference member, IEnumerable<string> assemblies) =>
        assemblies.Any(a => member.DeclaringType.FullName.Contains(a));

    private static bool IsMethodCall(this Instruction instruction) =>
        instruction.OpCode == OpCodes.Call
        || instruction.OpCode == OpCodes.Calli
        || instruction.OpCode == OpCodes.Callvirt;

    private static bool IsLambdaExpression(this Instruction instruction) => instruction.OpCode == OpCodes.Ldftn;

    #endregion Extensions
}
