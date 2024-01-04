using System.Data;
using Dapper;
using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using Microsoft.Data.SqlClient;

namespace EmploymentBureau.DataAccess.Repositories;

public sealed class EmployerRepository
{
    private readonly string _connectionString;

    public EmployerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    // Create
    public async Task<int> AddEmployer(Employer employer)
    {
        var sql = "INSERT INTO Employers (Name, ActivityType, Address, Number) VALUES (@Name, @ActivityType, @Address, @Number); SELECT SCOPE_IDENTITY();";

        using (var connection = CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(sql, employer);
        }
    }

    // Read (Get a single employer)
    public async Task<Employer?> GetEmployer(int id)
    {
        var sql = "SELECT * FROM Employers WHERE EmployerId = @Id";

        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<Employer>(sql, new { Id = id });
        }
    }

    // Read (Get all employers)
    public async Task<IEnumerable<Employer>> GetAllEmployers()
    {
        var sql = "SELECT * FROM Employers";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Employer>(sql);
        }
    }

    // Update
    public async Task UpdateEmployer(Employer employer)
    {
        var sql = "UPDATE Employers SET Name = @Name, ActivityType = @ActivityType, Address = @Address, Number = @Number WHERE EmployerId = @EmployerId";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, employer);
        }
    }

    // Delete
    public async Task DeleteEmployer(int id)
    {
        var sql = "DELETE FROM Employers WHERE EmployerId = @Id";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
    
    public async Task<IEnumerable<EmployerWithContracts>> GetEmployersWithContracts()
    {
        var sql = @"
        SELECT e.*, c.*
        FROM Employers e
        LEFT JOIN Contracts c ON e.EmployerId = c.EmployerId";

        using (var connection = CreateConnection())
        {
            var employerDict = new Dictionary<int, EmployerWithContracts>();

            var result = await connection.QueryAsync<Employer, Contract, EmployerWithContracts>(
                sql,
                (employer, contract) =>
                {
                    if (!employerDict.TryGetValue(employer.EmployerId, out var employerEntry))
                    {
                        employerEntry = new EmployerWithContracts { Employer = employer, Contracts = new List<Contract>() };
                        employerDict.Add(employer.EmployerId, employerEntry);
                    }

                    if (contract != null)
                    {
                        employerEntry.Contracts.Add(contract);
                    }

                    return employerEntry;
                },
                splitOn: "ContractId");

            return employerDict.Values;
        }
    }
    
    public async Task<IEnumerable<Employer>> GetEmployersWithActiveContracts()
    {
        var sql = @"
        SELECT DISTINCT e.*
        FROM Employers e
        INNER JOIN Contracts c ON e.EmployerId = c.EmployerId";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Employer>(sql);
        }
    }
    
    public async Task<IEnumerable<Employer>> GetEmployersByActivityType(string activityType)
    {
        var sql = @"
        SELECT *
        FROM Employers
        WHERE ActivityType = @ActivityType";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Employer>(sql, new { ActivityType = activityType });
        }
    }
    
    public async Task<IEnumerable<ContractCountPerEmployer>> GetContractCountPerEmployer()
    {
        var sql = @"
        SELECT e.Name, COUNT(c.ContractId) as ContractCount
        FROM Employers e
        LEFT JOIN Contracts c ON e.EmployerId = c.EmployerId
        GROUP BY e.Name";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<ContractCountPerEmployer>(sql);
        }
    }
}