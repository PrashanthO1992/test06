using Microsoft.Data.SqlClient;
using System.Data;


namespace ReactExample
{
    public class Dappercontext
    {
        private readonly IConfiguration _config;
        private readonly string Connectionstring;

        public Dappercontext(IConfiguration config)
        {
            _config = config;
            Connectionstring = _config.GetConnectionString("DefaultConnection")!;
        }
        public IDbConnection createconnection() => new SqlConnection(Connectionstring);

    }
}
