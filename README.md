# Introduction 
Provides data about test coverage for the API by analysing service client methods called from tests.

# Features
### Test coverage

```
var apiCoverage = RestEaseTestCoverageBuilder
                .ForTestsInAssembly(AssemblyWithTests)
                .ForController<IFirstController>()
                .ForController<ISecondController>()
                .UseSwaggerJson(swaggerJson)
                .ApiTestCoverage;
```

`ForTestsInAssembly` sets assembly with tests that need to be analized. E.g. `AssemblyWithTests = Assembly.GetCallingAssembly()`, if api coverage is called from the project with tests for Api.
Right now only xUnit and MSTests are supported.

`ForController<TController>()`/`ForController(Type controllerType)` sets controller/client classes that represent Api methods.
Right now, only RestEase is supported. E.g.
```
[BasePath("/api/Operation")]
public interface ITestController
{
    [Get]
    Task<object> GetNoPathMethod();

    [Get("get")]
    Task<object> GetMethod();

    [Get("/All/")]
    Task<object> GetAllMethod([Query] string parameter);
}
```

There are multiple ways to provide desciption of a mapped Api:
`UseSwaggerJson(string swaggerJson)` sets swagger json that describes Api.
`UseSwaggerJsonPath(string swaggerJsonPath)` read swagger json from the file.
`UseSwaggerUrl(string swaggerUrl)` get swagger json from service's swagger.
`UseSwagger(HttpClient client)` get swagger json from service's http client.

`ApiTestCoverage` calculates Api coverage by tests. In the result, each endpoint will have mapped list of tests where this endpoint was called. 
It does not guarantee that the test sis testing a specific endpoint, it just means that somewhere in the tests there is a statement where the Api method is called. 

# Reporting

```
var apiCoverage = RestEaseTestCoverageBuilder
                .ForTestsInAssembly(AssemblyWithTests)
                .ForController<IFirstController>()
                .ForController<ISecondController>()
                .UseSwaggerJson(swaggerJson)
                .ApiTestCoverage;

apiCoverage.ToXlsx("CoverageReport.xlsx", worksheetName: "SomethingApi");

```

`void ToXlsx(this MappedApiResult mappedApiResult, string path, string worksheetName, bool deleteFileIfExists = false)`
`ToXlsx` is an extension method. As parameters, it takes the path to the Excel file and the name of the worksheet that would be created in the Excel table.
By default, if the file with the table already exists, a new worksheet will be created in the existing table. If a worksheet with this name already exists, it will be overridden.
If `deleteFileIfExists` set to `true` existing file will be deleted.

# Testing
`ApiCoverageTool.Tests` - the project with unit tests.
`ApiCoverageTool.AssemblyUnderTests` - the assembly that is used in the unit test project. Test from this project are not expected to pass successfully.

# Notes and known issues

Only test methods are analyzed. Calls to Api in Init/CleanUp methods, fixtures, etc do not count.
For now, the endpoints with the parametrized paths can have issues, like `/addresses/turkey/{cityName}/{districtName}/neighborhoods`. Controller interface need to have the exact same path parameters names to make it work properly.