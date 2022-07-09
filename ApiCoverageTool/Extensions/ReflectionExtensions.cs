using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace ApiCoverageTool.Extensions;

public static class ReflectionExtensions
{
    private static Dictionary<string, MethodBase> MethodDefinitionsMap { get; } = new Dictionary<string, MethodBase>();
    private static Dictionary<MethodBase, MethodDefinition> MethodBasesMap { get; } = new Dictionary<MethodBase, MethodDefinition>();
    private static Dictionary<string, Type> TypeDefinitionsMap { get; } = new Dictionary<string, Type>();

    public static bool SameAs(this TypeDefinition currentTypeDefinition, Type type)
    {
        if (currentTypeDefinition is null && type is null)
            return true;

        if (currentTypeDefinition is null || type is null)
            return false;

        var typeDefinition = type.ToTypeDefinition();

        return currentTypeDefinition.FullName == typeDefinition.FullName &&
               currentTypeDefinition.Module.Assembly.Name.FullName == typeDefinition.Module.Assembly.Name.FullName;
    }

    public static bool SameAs(this Type type, TypeDefinition typeDefinition) => typeDefinition.SameAs(type);

    public static bool SameAs(this MethodDefinition methodDefinition, MethodBase methodBase)
    {
        if (methodDefinition is null && methodBase is null)
            return true;

        if (methodDefinition is null || methodBase is null)
            return false;

        if (methodDefinition.Name != methodBase.Name)
            return false;

        if (methodDefinition.DeclaringType.FullName != methodBase.DeclaringType.FullName)
            return false;

        return methodDefinition.ToMethodBase() == methodBase;
    }

    public static bool SameAs(this MethodBase methodBase, MethodDefinition methodDefinition) => methodDefinition.SameAs(methodBase);

    public static void IsNotNullValidation(this object obj, string paramName)
    {
        if (obj is null)
            throw new ArgumentNullException(paramName, $"{paramName} can not be null.");
    }

    public static MethodBase ToMethodBase(this MethodDefinition methodDefinition)
    {
        methodDefinition.IsNotNullValidation(nameof(methodDefinition));

        if (MethodDefinitionsMap.ContainsKey(methodDefinition.FullName))
            return MethodDefinitionsMap[methodDefinition.FullName];

        return MethodToMethodBase(methodDefinition);
    }

    private static MethodBase MethodToMethodBase(MethodDefinition methodDefinition)
    {
        try
        {
            var type = methodDefinition.DeclaringType.ToType();

            var allMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var metadataToken = methodDefinition.MetadataToken.GetHashCode();
            var method = allMethods.Single(m => m.Name == methodDefinition.Name && m.MetadataToken == metadataToken);
            MethodDefinitionsMap[methodDefinition.FullName] = method;

            return method;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to convert MethodDefinition to MethodBase for method '{methodDefinition.FullName}'. Error: {ex.Message}");

            throw;
        }
    }

    public static MethodDefinition ToMethodDefinition(this MethodBase method)
    {
        method.IsNotNullValidation(nameof(method));

        if (MethodBasesMap.ContainsKey(method))
            return MethodBasesMap[method];

        var assemblyDefinition = AssemblyDefinition.ReadAssembly(method.DeclaringType.Assembly.Location);
        var typeDefinition = assemblyDefinition.MainModule.GetType(method.DeclaringType.FullName.Replace("+", "/"));
        var methodDefinition = typeDefinition.Methods.Single(m =>
            m.Name == method.Name && m.MetadataToken.GetHashCode() == method.MetadataToken);

        MethodBasesMap[method] = methodDefinition;

        return methodDefinition;
    }

    public static TypeDefinition ToTypeDefinition(this Type type)
    {
        type.IsNotNullValidation(nameof(type));
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(type.Module.Assembly.Location);

        return assemblyDefinition.MainModule.ImportReference(type).Resolve();
    }

    private static Type ToType(this TypeDefinition typeDefinition)
    {
        if (TypeDefinitionsMap.ContainsKey(typeDefinition.FullName))
            return TypeDefinitionsMap[typeDefinition.FullName];

        var assemblyFullName = typeDefinition.Module.Assembly.FullName;
        var assembly = Assembly.Load(assemblyFullName);

        if (typeDefinition.IsNested)
        {
            var declaringType = typeDefinition.DeclaringType.ToType();

            if (TypeDefinitionsMap.ContainsKey(typeDefinition.FullName))
                return TypeDefinitionsMap[typeDefinition.FullName];

            var declaringTypeInfo = declaringType.GetTypeInfo();

            var typeInfo = declaringTypeInfo.DeclaredNestedTypes
                .FirstOrDefault(t => t.FullName == typeDefinition.FullName.Replace("/", "+"));

            TypeDefinitionsMap[typeDefinition.FullName] = typeInfo.AsType();

            return typeInfo.AsType();
        }

        var type = assembly.GetType(typeDefinition.GetReflectionName());
        TypeDefinitionsMap[typeDefinition.FullName] = type;

        return type;
    }

    private static string GetReflectionName(this TypeReference type)
    {
        if (!type.IsGenericInstance)
            return type.FullName;

        var genericInstance = (GenericInstanceType)type;
        var genericArguments = string.Join(",", genericInstance.GenericArguments.Select(p => p.GetReflectionName()).ToList());
        return string.Format($"{genericInstance.Namespace}.{type.Name}[{genericArguments}]");
    }
}
