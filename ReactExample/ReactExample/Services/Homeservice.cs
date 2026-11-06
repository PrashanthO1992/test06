using ReactExample.Repositary;

namespace ReactExample.Services
{
    public class Homeservice : IHomeservice
    {
        private readonly IHomeRepositary homeRepositary;
        public Homeservice(IHomeRepositary _homeRepositary)
        {
            homeRepositary = _homeRepositary;
        }
        public async Task<IEnumerable<Items>> GetOrder()
        {
            var result = await homeRepositary.GetOrderrepo();
            return result;
        }
    }

}
