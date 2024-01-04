using System.Data;
using Dapper;
using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using Microsoft.Data.SqlClient;

namespace EmploymentBureau.DataAccess.Repositories;

public sealed class ContractRepository
{
    private readonly string _connectionString;

    public ContractRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
    
    // Create
    public async Task<int> AddContract(Contract contract)
    {
        var sql = "INSERT INTO Contracts (ApplicantId, EmployerId, Position, Commission) VALUES (@ApplicantId, @EmployerId, @Position, @Commission); SELECT SCOPE_IDENTITY();";

        using (var connection = CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(sql, contract);
        }
    }

    // Read (Get a single contract)
    public async Task<Contract?> GetContract(int id)
    {
        var sql = "SELECT * FROM Contracts WHERE ContractId = @Id";

        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<Contract>(sql, new { Id = id });
        }
    }

    // Read (Get all contracts)
    public async Task<IEnumerable<Contract>> GetAllContracts()
    {
        var sql = "SELECT * FROM Contracts";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Contract>(sql);
        }
    }

    // Update
    public async Task UpdateContract(Contract contract)
    {
        var sql = "UPDATE Contracts SET ApplicantId = @ApplicantId, EmployerId = @EmployerId, Position = @Position, Commission = @Commission WHERE ContractId = @ContractId";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, contract);
        }
    }

    // Delete
    public async Task DeleteContract(int id)
    {
        var sql = "DELETE FROM Contracts WHERE ContractId = @Id";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
    
    public async Task<IEnumerable<Contract>> GetContractsWithHighCommission(decimal threshold)
    {
        var sql = @"
        SELECT *
        FROM Contracts
        WHERE Commission > @CommissionThreshold";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Contract>(sql, new { CommissionThreshold = threshold });
        }
    }
    
    public async Task<IEnumerable<AverageCommissionByPosition>> GetAverageCommissionByPosition()
    {
        var sql = @"
        SELECT Position, AVG(Commission) as AverageCommission
        FROM Contracts
        GROUP BY Position";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<AverageCommissionByPosition>(sql);
        }
    }
}