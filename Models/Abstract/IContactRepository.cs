using ContactsCosmosWebApp.Models.Entities;

namespace ContactsCosmosWebApp.Models.Abstract
{
  public interface IContactRepository
  {
    Task<List<Contact>> GetAllContactsAsync();
    Task<List<Contact>> FindContactByPhoneAsync(string phone);
    Task<List<Contact>> FindContactsByContactNameAsync(string contactName);
    Task<Contact> FindContactAsync(string id);
    Task<List<Contact>> FindContactByContactNamePhoneAsync(string contactName, string phone);
    Task<Contact> CreateAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(string id, string contactName, string phone);
  }
}