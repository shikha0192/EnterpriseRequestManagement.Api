using Dapper;
using EnterpriseRequestManagement.Api.Data;
using EnterpriseRequestManagement.Api.DTOs;
using EnterpriseRequestManagement.Api.Models;

namespace EnterpriseRequestManagement.Api.Services;

public class RequestService
{
    private readonly DbConnectionFactory _db;

    public RequestService(DbConnectionFactory db) => _db = db;

    public async Task<int> CreateRequestAsync(CreateRequestDto dto)
    {
        const string sql = @"
            INSERT INTO Requests (Title, Description, Department, Status, CreatedByUserId)
            VALUES (@Title, @Description, @Department, 'Pending', @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        using var conn = _db.Create();
        return await conn.QuerySingleAsync<int>(sql, dto);
    }

    public async Task<IEnumerable<Request>> GetAllAsync()
    {
        using var conn = _db.Create();
        return await conn.QueryAsync<Request>("SELECT * FROM Requests ORDER BY RequestId DESC");
    }

    public async Task<bool> TakeActionAsync(int requestId, ApprovalActionDto dto)
    {
        using var conn = _db.Create();

        // Insert approval action
        const string approvalSql = @"
            INSERT INTO RequestApprovals (RequestId, ApproverUserId, Action, Comments, ActionAt)
            VALUES (@RequestId, @ApproverUserId, @Action, @Comments, SYSDATETIME());
        ";

        await conn.ExecuteAsync(approvalSql, new
        {
            RequestId = requestId,
            dto.ApproverUserId,
            dto.Action,
            dto.Comments
        });

        // Update request status
        const string updateSql = @"
            UPDATE Requests
            SET Status = @Status
            WHERE RequestId = @RequestId;
        ";

        var status = dto.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase)
            ? "Approved"
            : "Rejected";

        var rows = await conn.ExecuteAsync(updateSql, new { Status = status, RequestId = requestId });
        return rows > 0;
    }
}
