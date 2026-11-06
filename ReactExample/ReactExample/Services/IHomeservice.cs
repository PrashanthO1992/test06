namespace ReactExample.Services
{
    public interface IHomeservice
    {
        Task<IEnumerable<Items>> GetOrder();
    }
}
