namespace ApiCoverageTool.Tests.ObjectsUnderTests
{
    public abstract class TestClassBase
    {
        public abstract string AbstractMethod();

        public virtual object PolymorphismMethod(object obj)
        {
            return obj;
        }
    }
}