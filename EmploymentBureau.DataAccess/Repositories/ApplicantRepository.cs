using System.Data;
using Dapper;
using EmploymentBureau.DataAccess.DTOs;
using EmploymentBureau.DataAccess.Entities;
using Microsoft.Data.SqlClient;

namespace EmploymentBureau.DataAccess.Repositories;

public sealed class ApplicantRepository
{
    private readonly string _connectionString;

    public ApplicantRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
    
    // Create
    public async Task<int> AddApplicant(Applicant applicant)
    {
        var sql = "INSERT INTO Applicants (Surname, Name, Qualification, ActivityType, OtherData, DesiredSalary) VALUES (@Surname, @Name, @Qualification, @ActivityType, @OtherData, @DesiredSalary); SELECT SCOPE_IDENTITY();";

        using (var connection = CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(sql, applicant);
        }
    }

    // Read (Get a single applicant)
    public async Task<Applicant?> GetApplicant(int id)
    {
        var sql = "SELECT * FROM Applicants WHERE ApplicantId = @Id";

        using (var connection = CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<Applicant>(sql, new { Id = id });
        }
    }

    // Read (Get all applicants)
    public async Task<IEnumerable<Applicant>> GetAllApplicants()
    {
        var sql = "SELECT * FROM Applicants";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Applicant>(sql);
        }
    }

    // Update
    public async Task UpdateApplicant(Applicant applicant)
    {
        var sql = "UPDATE Applicants SET Surname = @Surname, Name = @Name, Qualification = @Qualification, ActivityType = @ActivityType, OtherData = @OtherData, DesiredSalary = @DesiredSalary WHERE ApplicantId = @ApplicantId";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, applicant);
        }
    }

    // Delete
    public async Task DeleteApplicant(int id)
    {
        var sql = "DELETE FROM Applicants WHERE ApplicantId = @Id";

        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
    
    public async Task<IEnumerable<ApplicantWithContracts>> GetApplicantsWithContractsAndEmployers()
    {
        var sql = @"
        SELECT a.*, c.*, e.*
        FROM Applicants a
        LEFT JOIN Contracts c ON a.ApplicantId = c.ApplicantId
        LEFT JOIN Employers e ON c.EmployerId = e.EmployerId";

        using (var connection = CreateConnection())
        {
            var applicantDict = new Dictionary<int, ApplicantWithContracts>();

            var result = await connection.QueryAsync<Applicant, Contract, Employer, ApplicantWithContracts>(
                sql,
                (applicant, contract, employer) =>
                {
                    if (!applicantDict.TryGetValue(applicant.ApplicantId, out var applicantEntry))
                    {
                        applicantEntry = new ApplicantWithContracts { Applicant = applicant, Contracts = new List<ContractDetail>() };
                        applicantDict.Add(applicant.ApplicantId, applicantEntry);
                    }

                    if (contract != null && employer != null)
                    {
                        applicantEntry.Contracts.Add(new ContractDetail { Contract = contract, Employer = employer });
                    }

                    return applicantEntry;
                },
                splitOn: "ContractId,EmployerId");

            return applicantDict.Values;
        }
    }
    
    public async Task<IEnumerable<SuitableApplicant>> FindApplicantsSuitableForEmployers()
    {
        var sql = @"
        SELECT a.*, e.*
        FROM Applicants a
        INNER JOIN Employers e ON a.ActivityType = e.ActivityType";

        using (var connection = CreateConnection())
        {
            var result = await connection.QueryAsync<Applicant, Employer, SuitableApplicant>(
                sql,
                (applicant, employer) =>
                {
                    return new SuitableApplicant { Applicant = applicant, SuitableEmployer = employer };
                },
                splitOn: "EmployerId");

            return result;
        }
    }
    
    public async Task<IEnumerable<Applicant>> GetApplicantsWithNoContracts()
    {
        var sql = @"
        SELECT *
        FROM Applicants a
        WHERE NOT EXISTS (
            SELECT 1 FROM Contracts c WHERE c.ApplicantId = a.ApplicantId
        )";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Applicant>(sql);
        }
    }
    
    public async Task<IEnumerable<ApplicantWithEmployersCount>> GetApplicantsWithMultipleEmployers()
    {
        var sql = @"
        SELECT a.*, COUNT(DISTINCT c.EmployerId) as EmployersCount
        FROM Applicants a
        INNER JOIN Contracts c ON a.ApplicantId = c.ApplicantId
        GROUP BY a.ApplicantId, a.Surname, a.Name, a.Qualification, a.ActivityType, a.OtherData, a.DesiredSalary
        HAVING COUNT(DISTINCT c.EmployerId) > 1";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Applicant, int, ApplicantWithEmployersCount>(
                sql,
                (applicant, employersCount) => new ApplicantWithEmployersCount { Applicant = applicant, EmployersCount = employersCount },
                splitOn: "EmployersCount");
        }
    }
    
    public async Task<IEnumerable<Applicant>> SearchApplicantsByQualificationAndSalary(string qualification, decimal minSalary, decimal maxSalary)
    {
        var sql = @"
        SELECT *
        FROM Applicants
        WHERE Qualification = @Qualification AND DesiredSalary BETWEEN @MinSalary AND @MaxSalary";

        using (var connection = CreateConnection())
        {
            return await connection.QueryAsync<Applicant>(sql, new { Qualification = qualification, MinSalary = minSalary, MaxSalary = maxSalary });
        }
    }
}