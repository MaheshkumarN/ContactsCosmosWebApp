using ContactsCosmosWebApp.Models.Abstract;
using ContactsCosmosWebApp.Models.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ContactsCosmosWebApp.Models.Concrete
{
  public class CosmosContactRepository : IContactRepository
  {
    private readonly ILogger<CosmosContactRepository> _logger;
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly Container _container;

    public CosmosContactRepository(IOptions<CosmosUtility> cosmosUtility, ILogger<CosmosContactRepository> logger)
    {
      _logger = logger;
      _cosmosClient = new CosmosClient(cosmosUtility.Value.CosmosEndpoint, cosmosUtility.Value.CosmosKey);
      _database = _cosmosClient.GetDatabase(cosmosUtility.Value.DatabaseName);
      _container = _database.GetContainer(cosmosUtility.Value.ContainerName);
    }

    private async Task<List<Contact>> GetContacts(string sqlQuery)
    {
      return null;
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
      return null;
    }

    public async Task DeleteAsync(string id, string contactName, string phone)
    {
      _logger.LogInformation($"--- CosmosContactRepository.DeleteAsync, Deleted '{contactName}'s' record successfully ---");
    }
    public async Task<Contact> FindContactAsync(string id)
    {
      return null;
    }

    public async Task<List<Contact>> FindContactByPhoneAsync(string phone)
    {
      return null;
    }

    public async Task<List<Contact>> FindContactByContactNamePhoneAsync(string contactName, string phone)
    {
      return null;
    }

    public async Task<List<Contact>> FindContactsByContactNameAsync(string contactName)
    {
      var sqlQuery = $"Select * from c where c.contactName='{contactName}'";
      var contactsList = await GetContacts(sqlQuery);
      return contactsList;
    }

    public async Task<List<Contact>> GetAllContactsAsync()
    {
      var sqlQuery = $"Select * from c";
      var contactsList = await GetContacts(sqlQuery);
      // _logger.LogInformation($"--- CosmosContactRepository.GetAllContactsAsync Got all Records and set it into ContactsList ---");
      return contactsList;
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {

      return null;
    }
  }
}
