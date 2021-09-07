# Introduction 
Provides test coverage for the API (right now only RestEase is supported).

# Features
### Test coverage

To get test coverage use `GetTestCoverage` method from `ApiTestCoverage` class.
As parameters `GetTestCoverage` takes an Assembly with tests then need to be processed and list of interfaces that describe rest controllers (right now only RestEase is supported).
`GetTestCoverage` returns a Dictionary where the endpoint's full path is a key, and the list of test methods is a value.
If the endpoint method is not called in any test it won't be listed.
```
var constrollers = new Type[]
{
    typeof(ICustomerController),
    typeof(IQuotationController)
};

var assembly = Assembly.GetExecutingAssembly();
var testCoverage = ApiTestCoverage<RestEaseMethodsProcessor>.GetTestCoverage(assembly, constrollers);
```

Insted of `ApiTestCoverage<RestEaseMethodsProcessor>.GetTestCoverage(assembly, constrollers)` call, use `using static` directive:
```
using static ApiCoverageTool.Coverage.ApiTestCoverage<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;
...
var testCoverage = GetTestCoverage(assembly, constrollers);
```

Result example:
```
POST /api/account/login
    AccountApiTests.AccountLoginReturnsTokenForAuthorizedDealer
    AccountApiTests.AccountLoginThrowsExceptionIfPasswordIsWrong
GET /api/common/getcurrtime
    CommonApiTests.CommonGetCurrTimeReturnsCurrentTime
GET /api/vehiclecommon/getmileagelist
    VehicleApiTests.VehicleCommonGetMileageListReturnsAList
...
```

### Endpoints mapping by rest clients

`ApiClientCoverage` class contains methods that can return endpoints coverage by the REST clients.
Returns a list of endpoints that have implemented methods in specified controllers. And the list of endpoints that don't have mapped methods.
There are three methods that differ by the source of swagger OpenAPI specification.
`GetCoverageByClientFromUri` - from the uri to service swagger.json.
`GetCoverageByClientFromFile` - from json file.
`GetCoverageByClient` - from string.

```
var constrollers = new Type[]
{
    typeof(ICustomerController),
    typeof(IQuotationController)
};

var swaggerJsonUri = new Uri("https://customer-api-***.azurewebsites.net/swagger/v1/swagger.json");
var resEndpointsCoverage = await GetCoverageByClientFromUri(swaggerJsonUri, constrollers);
```

Result example:
Mapped endpoints:
```
Mapped:
POST /api/customer/add
    ICustomerController.AddCustomer
GET /api/customer/get-safe
    ICustomerController.GetCustomerSafe
GET /api/customer/get
    ICustomerController.GetCustomer
POST /api/customer/signin
    ICustomerController.Signin
GET /api/customer/getcustomerbyregistration
    ICustomerController.GetCustomerByRegistration
```
Not mapped:
```
GET /api/apibase/heartbeat
PUT /api/customer/update
PUT /api/customer/activate
GET /api/customer/getall
POST /api/customer/contactus
POST /api/customer/apptbookingcallback
```

# Reporting
No reporting tools are implemented yet.
TODO:
1. Export result to string
2. Export result in JSON

# Testing
`ApiCoverageTool.Tests` - the project with unit tests.
`ApiCoverageTool.AssemblyUnderTests` - the assembly that is used in the unit test project. Test from this project are not expected to pass successfully.

# Add as submodule
Details can be found at https://www.atlassian.com/git/tutorials/git-submodule

To add repository as submodule, run next script from a target folder:
`git submodule add https://vavacars@dev.azure.com/vavacars/VavaCars/_git/ApiCoverageTool`

To pull changes for all submodules, run from root folder: 
`git submodule update --init --recursive`