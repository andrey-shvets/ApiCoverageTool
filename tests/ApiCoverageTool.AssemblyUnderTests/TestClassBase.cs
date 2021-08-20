namespace ApiCoverageTool.AssemblyUnderTests
{
    /// <summary>
    /// Class is exactly the same as TestClassBase from ApiCoverageTool.Tests project (except namespace)
    /// </summary>
    public abstract class TestClassBase
    {
        public abstract string AbstractMethod();

        public virtual object PolymorphismMethod(object obj)
        {
            return obj;
        }
    }
}