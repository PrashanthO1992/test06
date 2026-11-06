using Dapper;
using Microsoft.AspNetCore.Http;

namespace ReactExample.Repositary
{
    public class HomeRepositary : IHomeRepositary
    {
        private readonly Dappercontext _dapper;

        public HomeRepositary(Dappercontext dappercontext)
        {
            _dapper = dappercontext;
        }
        public async Task<IEnumerable<Items>> GetOrderrepo()
        {
            using var connection = _dapper.createconnection();
            var query = "select * from dbo.Items";
            var result = await connection.QueryAsync<Items>(query);
            return result;
        }

    }
}
