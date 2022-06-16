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
      QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
      FeedIterator<Contact> queryResultIterator = _container.GetItemQueryIterator<Contact>(queryDefinition);
      List<Contact> contactsList = new List<Contact>();
      while (queryResultIterator.HasMoreResults)
      {
        FeedResponse<Contact> currentResultSet = await queryResultIterator.ReadNextAsync();
        foreach (var item in currentResultSet)
        {
          contactsList.Add(item);
        }
        return contactsList;
      }
      return null;
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
      contact.Id = Guid.NewGuid().ToString();
      ItemResponse<Contact> contactResponse = await _container.CreateItemAsync<Contact>(contact);
      if (contactResponse != null)
      {
        _logger.LogInformation($"--- CosmosContactRepository.CreateAsync, New Contact with the name '{contact.ContactName}' created successfully ---");
        return contact;
      }
      return null;
    }

    public async Task DeleteAsync(string id, string contactName, string phone)
    {
      ItemResponse<Contact> contactResponse = await _container.DeleteItemAsync<Contact>(id, new PartitionKey(contactName));
      _logger.LogInformation($"--- CosmosContactRepository.DeleteAsync, Deleted '{contactName}'s' record successfully ---");
    }
    public async Task<Contact> FindContactAsync(string id)
    {
      var sqlQuery = $"Select * from c where c.id='{id}'";
      var contactsList = await GetContacts(sqlQuery);
      return contactsList[0];
    }

    public async Task<List<Contact>> FindContactByPhoneAsync(string phone)
    {
      var sqlQuery = $"Select * from c where c.phone='{phone}'";
      var contactsList = await GetContacts(sqlQuery);
      return contactsList;
    }

    public async Task<List<Contact>> FindContactByContactNamePhoneAsync(string contactName, string phone)
    {
      var sqlQuery = $"Select * from c where c.contactName='{contactName}' and c.phone='{phone}'";
      var contactsList = await GetContacts(sqlQuery);
      return contactsList;
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
      ItemResponse<Contact> contactResponse = await _container.ReadItemAsync<Contact>(contact.Id, new PartitionKey(contact.ContactName));
      var contactResult = contactResponse.Resource;

      contactResult.Id = contact.Id;
      contactResult.ContactName = contact.ContactName;
      contactResult.Phone = contact.Phone;
      contactResult.ContactType = contact.ContactType;
      contactResult.Email = contact.Email;

      contactResponse = await _container.ReplaceItemAsync<Contact>(contactResult, contactResult.Id);

      if (contactResponse.Resource != null)
      {
        _logger.LogInformation($"--- CosmosContactRepository.UpdateAsync, Updated '{contact.ContactName}'s' record successfully ---");
        return contactResponse;
      }
      return null;
    }
  }
}
