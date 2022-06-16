# ContactsCosmosWebApp - .Net 6 MVC Web App with CosmosDb

#### Packages

- Nuget Packages for the project
  - For Razor Page Runtime Compilation
    - dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --Version 6.0.5
      -For Json
    - dotnet add package Newtonsoft.json --Version 13.0.1
  - For Cosmos Db
    - dotnet add package Microsoft.Azure.Cosmos --Version 3.27.2
  - For Swagger
    - dotnet add package Swashbuckle.AspNetCore -v 6.3.1
  - For Azure Application Insights
    - dotnet add package Microsoft.ApplicationInsights.AspNetCore --Version 2.20.0
    - dotnet add package Microsoft.Extensions.Logging.ApplicationInsights --Version 2.20.0 (for logging ILogger user defined logs in ApplicationInsights)
  - For Azure KeyVault
    - dotnet add package Microsoft.Azure.KeyVault --Version 3.0.5
    - dotnet add package Microsoft.Azure.Services.AppAuthentication --Version 1.6.2
    - dotnet add package Microsoft.Extensions.Configuration.AzureKeyVault --Version 3.1.24

#### libman - lightweight client-side library tool

- Commands

  - dotnet tool install --global Microsoft.Web.LibraryManager.Cli --version 2.1.161 (for installing it globally)
  - libman --version
  - libman --help
  - libman init --default-destination wwwroot/lib --default-provider jsdelivr(will create a libman.json file)
  - libman restore (will restore all the packages)

- Libman Packages for the Project
  - jquery@3.6.0
  - jquery-validation@1.19.3
  - jquery-validation-unobtrusive@3.2.12
  - bootstrap@5.1.3
  - bootstrap-icons@v1.8.3
