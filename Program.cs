// var builder = WebApplication.CreateBuilder(args);
// To configure any other Type before the build happens
using ContactsCosmosWebApp.Models;
using ContactsCosmosWebApp.Models.Abstract;
using ContactsCosmosWebApp.Models.Concrete;
using ContactsCosmosWebApp.Models.Entities;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
  Args = args,
  ApplicationName = typeof(Program).Assembly.FullName,
  ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
  WebRootPath = "wwwroot"
});

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
  SetupKeyVault(hostingContext, config);
});

// Add Services to the Container

builder.Services.Configure<CosmosUtility>(cfg =>
{
  cfg.CosmosEndpoint = builder.Configuration["CosmosConnectionString:CosmosEndpoint"];
  cfg.CosmosKey = builder.Configuration["CosmosConnectionString:CosmosKey"];
  cfg.DatabaseName = builder.Configuration["CosmosConnectionString:DatabaseName"];
  cfg.ContainerName = builder.Configuration["CosmosConnectionString:ContainerName"];
  cfg.PartitionKeyName = builder.Configuration["CosmosConnectionString:PartitionKeyName"];
});

#region Swagger
builder.Services.AddSwaggerGen(cfg =>
{
  cfg.SwaggerDoc(name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Contacts API", Version = "v1" });
});
#endregion

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddScoped<CosmosDbContext>();
builder.Services.AddScoped<IContactRepository, CosmosContactRepository>();

builder.Services.AddApplicationInsightsTelemetry(cfg =>
{
  cfg.InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
});

builder.Services.AddLogging(cfg =>
{
  cfg.AddApplicationInsights(builder.Configuration["ApplicationInsights:InstrumentationKey"]);
  // Optional: Apply filters to configure LogLevel Information or above is sent to
  // ApplicationInsights for all categories.
  cfg.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);

  // Additional filtering For category starting in "Microsoft",
  // only Warning or above will be sent to Application Insights.
  //cfg.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
});

// Create app (WebApplication)
var app = builder.Build();

var appInsightsFlag = app.Services.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
if (builder.Configuration["EnableAppInsightsDisableTelemetry"] == "false")
{
  appInsightsFlag.DisableTelemetry = false;
}
else
{
  appInsightsFlag.DisableTelemetry = true;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseDeveloperExceptionPage();
// app.UseHttpsRedirection();
// app.UseStaticFiles();
app.UseFileServer();

using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
  var databaseFlag = context.CreateDatabaseAsync().GetAwaiter().GetResult();
  var containerFlag = context.CreateContainerAsync().GetAwaiter().GetResult();
  //  var contactSource = app.ApplicationServices.CreateScope().ServiceProvider.GetService<IContactRepository>();
  var contactSource = app.Services.CreateScope().ServiceProvider.GetService<IContactRepository>();
  SeedData(contactSource);
}

app.UseSwagger();
app.UseSwaggerUI(cfg =>
{
  cfg.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Contacts API");
});

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(ConfigureRoutes);


//app.MapGet("/", () => "Hello World!");

app.Run();


void SetupKeyVault(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
{
  var buildConfig = config.Build();
  var keyVaultEndpoint = buildConfig["MNKeyVault"];
  if (!string.IsNullOrEmpty(keyVaultEndpoint))
  {
    var azureServiceTokeProvider = new AzureServiceTokenProvider();
    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokeProvider.KeyVaultTokenCallback));
    config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
  }
}

void ConfigureRoutes(IEndpointRouteBuilder endpoints)
{
  endpoints.MapControllerRoute(name: "Default", pattern: "{controller=Contact}/{action=Index}/{id?}");
  //endpoints.MapControllerRoute(name: "Default", pattern: "{controller=Home}/{action=Index}/{id?}");
  // endpoints.Map("/", () => "Hello World!");
}

void SeedData(IContactRepository contactRepository)
{
  var contactList = contactRepository.GetAllContactsAsync().GetAwaiter().GetResult();
  if (contactList.Count == 0)
  {
    contactList = new List<Contact>
    {
      new Contact {ContactName="Mowgli", ContactType = "Family", Email = "mowgli@m.com", Phone = "1234567890" },
      new Contact {ContactName="Bagheera", ContactType = "Friend", Email = "bagheera@b.com", Phone = "2345678901" },
      new Contact {ContactName="Sherekhan", ContactType = "Professional", Email = "sherekhan.com", Phone = "3456789012" },
      new Contact {ContactName="Kaa", ContactType = "Professional", Email = "kaa@k.com", Phone = "4567890123" },
      new Contact {ContactName="Baloo", ContactType = "Friend", Email = "Baloo@b.com", Phone = "5678901234" }
    };
    foreach (Contact item in contactList)
    { contactRepository.CreateAsync(item); }
  }
}
