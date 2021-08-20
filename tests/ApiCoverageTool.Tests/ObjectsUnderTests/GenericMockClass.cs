namespace ApiCoverageTool.Tests.ObjectsUnderTests
{
    public class GenericMockClass<T> where T: new()
    {
        public T GenericMethod<TP>(TP param)
        {
            return new T();
        }
    }
}
