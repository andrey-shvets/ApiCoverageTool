namespace ApiCoverageTool.Configurations
{
    internal class ApiCoverageOptions
    {
        public const string ApiCoverageSettings = "ApiCoverageSettings";

        public string IncludeAssemblyMask { get; set; } = ".+";
        public string ExcludeAssemblyMask { get; set; } = @"^System(\.|$)|^Microsoft(\.|$)|^Mono(\.|$)|^xunit(\.|$)|^RestEase(\.|$)|^FluentAssertions(\.|$)|^Newtonsoft(\.|$)|^ApiCoverageTool$";
    }
}
