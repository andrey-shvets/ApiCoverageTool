using System;
using System.Linq;
using System.Reflection;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Tests.ObjectsUnderTests;
using FluentAssertions;
using Mono.Cecil;
using Xunit;

namespace ApiCoverageTool.Tests.Extensions
{
    public class ReflectionExtensionsTests
    {
        #region IsNotNullValidation
        [Fact]
        public void IsNotNullValidation_ForNullParameter_ThrowsArgumentNullException()
        {
            object obj = null;
            var ex = Assert.Throws<ArgumentNullException>(() => obj.IsNotNullValidation(nameof(obj)));

            ex.Message.Should().StartWith($"{nameof(obj)} can not be null.");
        }

        [Fact]
        public void IsNotNullValidation_ForNotNullParameter_DowsNotThrowsException()
        {
            var obj = string.Empty;
            obj.IsNotNullValidation(nameof(obj));
        }
        #endregion IsNotNullValidation

        #region SameAs
        [Fact]
        public void SameAs_GivenNullType_ReturnsFalse()
        {
            Type type = null;

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(typeof(TestClass).FullName);

            typeDefinition.SameAs(type).Should().BeFalse();
            type.SameAs(typeDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_GivenNullTypeDefinition_ReturnsFalse()
        {
            var type = typeof(TestClass);

            TypeDefinition typeDefinition = null;

            typeDefinition.SameAs(type).Should().BeFalse();
            type.SameAs(typeDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_WithTypeDefinitionAndTypeBothNull_ReturnsTrue()
        {
            Type type = null;
            TypeDefinition typeDefinition = null;

            typeDefinition.SameAs(type).Should().BeTrue();
            type.SameAs(typeDefinition).Should().BeTrue();
        }

        [Fact]
        public void SameAs_GivenTypeAndTypeDefinitionOfInheritedType_ReturnsFalse()
        {
            var type = typeof(TestClassBase);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(typeof(TestClass).FullName);

            typeDefinition.SameAs(type).Should().BeFalse();
            type.SameAs(typeDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_GivenTypeDefinitionAndTypeOfInheritedType_ReturnsFalse()
        {
            var type = typeof(TestClass);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(typeof(TestClassBase).FullName);

            typeDefinition.SameAs(type).Should().BeFalse();
            type.SameAs(typeDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_GivenTypeDefinitionAndTypeOfTheSameType_ReturnsTrue()
        {
            var type = typeof(TestClass);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(typeof(TestClass).FullName);

            typeDefinition.SameAs(type).Should().BeTrue();
            type.SameAs(typeDefinition).Should().BeTrue();
        }

        [Fact]
        public void SameAs_GivenTypeDefinitionAndTypeOfTheSameGenericType_ReturnsTrue()
        {
            var type = typeof(GenericMockClass<object>);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType("ApiCoverageTool.Tests.ObjectsUnderTests.GenericMockClass`1");

            typeDefinition.SameAs(type).Should().BeTrue();
            type.SameAs(typeDefinition).Should().BeTrue();
        }

        [Fact]
        public void SameAs_GivenTypeDefinitionAndTypeOfTheSameTypeFromDifferentAssemblies_ReturnsFalse()
        {
            var type = typeof(AssemblyUnderTests.TestClassBase);

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var typeDefinition = assemblyDefinition.MainModule.GetType(typeof(TestClassBase).FullName);

            typeDefinition.SameAs(type).Should().BeFalse();
            type.SameAs(typeDefinition).Should().BeFalse();
        }
        #endregion SameAs

        #region SameAs MethodDefinition
        [Fact]
        public void SameAs_WithMethodInfoAndMethodDefinitionPointingToSameMethod_ReturnsTrue()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("PolymorphismMethod", new Type[] { typeof(object) });

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod(System.Object)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            methodDefinition.SameAs(methodInfo).Should().BeTrue();
            methodInfo.SameAs(methodDefinition).Should().BeTrue();
        }

        [Fact]
        public void SameAs_WithMethodInfoAndMethodDefinitionBothNull_ReturnsTrue()
        {
            MethodInfo methodInfo = null;
            MethodDefinition methodDefinition = null;

            methodDefinition.SameAs(methodInfo).Should().BeTrue();
            methodInfo.SameAs(methodDefinition).Should().BeTrue();
        }

        [Fact]
        public void SameAs_MethodInfoNullAndMethodDefinitionNotNull_ReturnsFalse()
        {
            MethodInfo methodInfo = null;

            var type = typeof(TestClass);
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod(System.Object)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            methodDefinition.SameAs(methodInfo).Should().BeFalse();
            methodInfo.SameAs(methodDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_MethodInfoNotNullAndMethodDefinitionNull_ReturnsFalse()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("PolymorphismMethod", new Type[] { typeof(object) });

            MethodDefinition methodDefinition = null;

            methodDefinition.SameAs(methodInfo).Should().BeFalse();
            methodInfo.SameAs(methodDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_ComparingDifferentMethodsMethodsWithTheSameName_ReturnsFalse()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("PolymorphismMethod", Array.Empty<Type>());

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod(System.Object)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            methodDefinition.SameAs(methodInfo).Should().BeFalse();
            methodInfo.SameAs(methodDefinition).Should().BeFalse();
        }

        [Fact]
        public void SameAs_ComparingEquivalentMethodsFromDifferentAssemblies_ReturnsFalse()
        {
            var methodInfo = typeof(TestClassBase).GetMethod("AbstractMethod");

            var type = typeof(AssemblyUnderTests.TestClassBase);
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(type.Assembly.Location);
            var methodFullName = "System.String ApiCoverageTool.AssemblyUnderTests.TestClassBase::AbstractMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            methodDefinition.SameAs(methodInfo).Should().BeFalse();
            methodInfo.SameAs(methodDefinition).Should().BeFalse();
        }
        #endregion SameAs MethodDefinition

        #region ToTypeDefinition
        [Fact]
        public void ToTypeDefinition_GivenNullType_ThrowsArgumentNullException()
        {
            Type type = null;
            Assert.Throws<ArgumentNullException>(() => type.ToTypeDefinition());
        }

        [Fact]
        public void ToTypeDefinition_GivenType_ReturnsTypeDefinitionForTheSameType()
        {
            var type = typeof(TestClass);

            var typeDefinition = type.ToTypeDefinition();
            typeDefinition.FullName.Should().Be("ApiCoverageTool.Tests.ObjectsUnderTests.TestClass");
        }

        [Fact]
        public void ToTypeDefinition_GivenTypeOfTheGenericClass_ReturnsTypeDefinitionForTheSameType()
        {
            var type = typeof(GenericMockClass<object>);

            var typeDefinition = type.ToTypeDefinition();
            typeDefinition.FullName.Should().Be("ApiCoverageTool.Tests.ObjectsUnderTests.GenericMockClass`1");
        }
        #endregion ToTypeDefinition

        #region ToMethodInfo
        [Fact]
        public void ToMethodInfo_ForNullParameter_ThrowsArgumentNullException()
        {
            MethodDefinition methodDefinition = null;
            Assert.Throws<ArgumentNullException>(() => methodDefinition.ToMethodInfo());
        }

        [Fact]
        public void ToMethodInfo_ForMethodDefinition_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("PolymorphismMethod", Array.Empty<Type>());

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForMethodWithParameters_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("PolymorphismMethod", new Type[] { typeof(object) });

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod(System.Object)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForAsyncMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("AsyncMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Threading.Tasks.Task`1<System.Object> ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::AsyncMethod(System.Object)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForGenericMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("GenericMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "T ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::GenericMethod(T)";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForOverridenMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("AbstractMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.String ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::AbstractMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForPrivateMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PrivateMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForInternalMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("InternalMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::InternalMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }

        [Fact]
        public void ToMethodInfo_ForPrivateStaticMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var expectedMethod = type.GetMethod("PrivateStaticMethod", BindingFlags.NonPublic | BindingFlags.Static);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PrivateStaticMethod()";
            var methodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodInfo = methodDefinition.ToMethodInfo();

            actualMethodInfo.Should().BeSameAs(expectedMethod);
        }
        #endregion ToMethodInfo

        #region ToMethodDefinition
        [Fact]
        public void ToMethodDefinition_ForNullParameter_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => method.ToMethodDefinition());
        }

        [Fact]
        public void ToMethodDefinition_ForMethodDefinition_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("PolymorphismMethod", Array.Empty<Type>());

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod()";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForMethodWithParameters_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("PolymorphismMethod", new Type[] { typeof(object) });

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Object ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PolymorphismMethod(System.Object)";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForAsyncMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("AsyncMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Threading.Tasks.Task`1<System.Object> ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::AsyncMethod(System.Object)";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForGenericMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("GenericMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "T ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::GenericMethod(T)";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForOverridenMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("AbstractMethod");

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.String ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::AbstractMethod()";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForPrivateMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PrivateMethod()";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForInternalMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("InternalMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::InternalMethod()";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }

        [Fact]
        public void ToMethodDefinition_ForPrivateStaticMethod_ReturnsMethodInfoForSpecifiedMethod()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var type = typeof(TestClass);
            var method = type.GetMethod("PrivateStaticMethod", BindingFlags.NonPublic | BindingFlags.Static);

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);

            var methodFullName = "System.Void ApiCoverageTool.Tests.ObjectsUnderTests.TestClass::PrivateStaticMethod()";
            var expectedMethodDefinition = assemblyDefinition.MainModule
                .GetType(type.FullName)
                .Methods.Single(m => m.FullName == methodFullName);

            var actualMethodDefinition = method.ToMethodDefinition();

            actualMethodDefinition?.FullName.Should().Be(expectedMethodDefinition.FullName);
            actualMethodDefinition?.Module.Assembly.FullName.Should().Be(expectedMethodDefinition.Module.Assembly.FullName);
        }
        #endregion ToMethodDefinition
    }
}
