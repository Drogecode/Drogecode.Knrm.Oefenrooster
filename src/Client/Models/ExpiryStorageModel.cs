namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class ExpiryStorageModel<T>
{
    public long Ttl { get; set; }
    public T Data { get; set; }
    public bool PostRequest { get; set; }
}
