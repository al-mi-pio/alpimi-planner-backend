namespace AlpimiAPI
{
    public interface IDbService
    {
        Task<T?> Get<T>(string command, object parms);
        Task<List<T>> GetAll<T>(string command, object parms);
        Task<T?> Post<T>(string command, object parms);
        Task<T?> Update<T>(string command, object parms);
        Task Delete(string command, object parms);
    }
}
