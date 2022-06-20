namespace ApiCoverageTool.Tests.ObjectsUnderTests;

public class GenericMockClass<T> where T : new()
{
    public T GenericMethod<TP>(TP param)
    {
        if (param?.ToString() != string.Empty)
            return default;

        return new T();
    }
}
