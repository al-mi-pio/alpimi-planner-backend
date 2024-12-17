namespace AlpimiAPI.Database
{
    public interface IDbService
    {
        Task<T?> Get<T>(string command, object parms);
        Task<IEnumerable<T>?> GetAll<T>(string command, object parms);
        Task<T?> Post<T>(string command, object parms);
        Task<T?> Update<T>(string command, object parms);
        Task Delete(string command, object parms);
        Task Raw(string command);
    }
}
