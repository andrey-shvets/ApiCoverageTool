// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeMadeStatic.Local
using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.Tests.ObjectsUnderTests
{
    public class TestClass : TestClassBase
    {
        [Get]
        public object PolymorphismMethod()
        {
            return null;
        }

        public override object PolymorphismMethod(object obj)
        {
            return obj;
        }

        public async Task<object> AsyncMethod(object obj)
        {
            return await Task.FromResult(obj);
        }

        public T GenericMethod<T>(T obj)
        {
            return obj;
        }

        public override string AbstractMethod()
        {
            var obj = new AssemblyUnderTests.MockTests();
            obj.NotTestMethodNoClientCall();
            PrivateMethod();
            InternalMethod();
            return null;
        }

        private void PrivateMethod()
        { }

        internal void InternalMethod()
        { }

        private static void PrivateStaticMethod()
        { }
    }
}
