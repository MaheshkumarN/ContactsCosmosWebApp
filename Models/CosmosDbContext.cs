using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ContactsCosmosWebApp.Models
{
  public class CosmosDbContext
  {
    private readonly ILogger<CosmosDbContext> _logger;
    private readonly string _cosmosEndpoint;
    private readonly string _cosmosKey;
    private readonly string _databaseName;
    private readonly string _containerName;
    private readonly string _partitionKeyName;
    private readonly CosmosClient _cosmosClient;

    public CosmosDbContext(IOptions<CosmosUtility> cosmosUtility, ILogger<CosmosDbContext> logger)
    {
      _logger = logger;
      _cosmosEndpoint = cosmosUtility.Value.CosmosEndpoint;
      _cosmosKey = cosmosUtility.Value.CosmosKey;
      _databaseName = cosmosUtility.Value.DatabaseName;
      _containerName = cosmosUtility.Value.ContainerName;
      _partitionKeyName = cosmosUtility.Value.PartitionKeyName;
      _cosmosClient = new CosmosClient(_cosmosEndpoint, _cosmosKey);
    }

    public async Task<bool> CreateDatabaseAsync()
    {
      // DatabaseResponse dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName, 400);
      DatabaseResponse dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
      if (dbResponse.StatusCode == System.Net.HttpStatusCode.Created)
      {
        _logger.LogInformation($"--- Database: '{_databaseName}' created successfully ---");
        return true;
      }
      return false;
    }

    public async Task<bool> CreateContainerAsync()
    {
      ContainerProperties containerProperties = new ContainerProperties(_containerName, $"/{_partitionKeyName}");
      ContainerResponse containerResponse = await _cosmosClient.GetDatabase(_databaseName).CreateContainerIfNotExistsAsync(containerProperties);
      if (containerResponse.StatusCode == System.Net.HttpStatusCode.Created)
      {
        _logger.LogInformation($"--- Container: '{_containerName}' with the PartitionKey: '/{_partitionKeyName}' has been created successfully in the Database: '{_databaseName}' ---");
        return true;
      }
      return false;
    }
  }
}