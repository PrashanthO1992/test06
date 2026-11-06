namespace ReactExample.Repositary
{
    public interface IHomeRepositary
    {
        Task<IEnumerable<Items>> GetOrderrepo();
    }
}
