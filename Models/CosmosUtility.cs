namespace ContactsCosmosWebApp.Models
{
  public class CosmosUtility
  {
    public string CosmosEndpoint { get; set; }
    public string CosmosKey { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
    public string PartitionKeyName { get; set; }
  }
}