using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace NL2SQL.Infra;

public class DapperConnectionProvider
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperConnectionProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("NorthwindDb")!;
    }

    public IDbConnection Connect()
        => new SqlConnection(_connectionString);
}
