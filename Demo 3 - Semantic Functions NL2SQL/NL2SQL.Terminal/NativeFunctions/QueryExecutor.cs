using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;
using NL2SQL.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL2SQL.Terminal.NativeFunctions;
internal class QueryExecutor
{
    private readonly DapperConnectionProvider _connectionProvider;

    public QueryExecutor(DapperConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    [KernelFunction, Description("Executes a T-SQL query")]
    public IEnumerable<dynamic> ExecuteQuery([Description("The T-SQL query to execute")]string query)
    {
        using (IDbConnection db = _connectionProvider.Connect())
        {
            try
            {
                var queryResult = db.Query(query);
                return queryResult;
            }
            catch (Exception ex)
            {
                string message = "An Exception Occured while executing query";
                throw new Exception(message, ex);
            }
        }
    }
}
