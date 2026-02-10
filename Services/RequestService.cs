using Dapper;
using EnterpriseRequestManagement.Api.Data;
using EnterpriseRequestManagement.Api.DTOs;
using EnterpriseRequestManagement.Api.Models;

namespace EnterpriseRequestManagement.Api.Services;

public class RequestService
{
    private readonly DbConnectionFactory _db;

    public RequestService(DbConnectionFactory db) => _db = db;

    public async Task<int> CreateRequestAsync(CreateRequestDto dto, int createdByUserId)
    {
        const string sql = @"
            INSERT INTO Requests (Title, Description, Department, Status, CreatedByUserId)
            VALUES (@Title, @Description, @Department, 'Pending', @CreatedByUserId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        using var conn = _db.Create();

        //  insert request and capture id
        var id = await conn.QuerySingleAsync<int>(sql, new
        {
            dto.Title,
            dto.Description,
            dto.Department,
            CreatedByUserId = createdByUserId
        });

        //  write audit log
        await AddAuditAsync(id, "RequestCreated", createdByUserId,
            $"Title={dto.Title}, Department={dto.Department}");

        //  return id
        return id;
    }
    private async Task AddAuditAsync(int? requestId, string action, int? doneByUserId, string? details)
    {
        const string sql = @"
        INSERT INTO AuditLogs (RequestId, Action, DoneByUserId, Details)
        VALUES (@RequestId, @Action, @DoneByUserId, @Details);
    ";

        using var conn = _db.Create();
        await conn.ExecuteAsync(sql, new
        {
            RequestId = requestId,
            Action = action,
            DoneByUserId = doneByUserId,
            Details = details
        });
    }
    public async Task<IEnumerable<Request>> GetAllAsync()
    {
        using var conn = _db.Create();
        return await conn.QueryAsync<Request>("SELECT * FROM Requests ORDER BY RequestId DESC");
    }

    public async Task<bool> TakeActionAsync(int requestId, ApprovalActionDto dto, int approverUserId)

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
            ApproverUserId = approverUserId,
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
        if (rows > 0)
        {
            await AddAuditAsync(
                requestId,
                $"Request{status}",
                approverUserId,
                $"Comments={dto.Comments}"
            );
        }
        return rows > 0;
    }

    public async Task<RequestDetailsDto?> GetDetailsAsync(int requestId)
    {
        using var conn = _db.Create();

        // 1) Request details + creator name
        const string requestSql = @"
SELECT 
    r.RequestId,
    r.Title,
    r.Description,
    r.Department,
    r.Status,
    r.CreatedByUserId,
    u.FullName AS CreatedByName,
    r.CreatedAt
FROM Requests r
INNER JOIN Users u ON u.UserId = r.CreatedByUserId
WHERE r.RequestId = @RequestId;
";

        var request = await conn.QuerySingleOrDefaultAsync<RequestDetailsDto>(
            requestSql, new { RequestId = requestId });

        if (request is null)
            return null;

        // 2) Approval history + approver name
        const string approvalsSql = @"
SELECT 
    a.ApprovalId,
    a.ApproverUserId,
    u.FullName AS ApproverName,
    a.Action,
    a.Comments,
    a.ActionAt
FROM RequestApprovals a
INNER JOIN Users u ON u.UserId = a.ApproverUserId
WHERE a.RequestId = @RequestId
ORDER BY a.ApprovalId DESC;
";

        var approvals = await conn.QueryAsync<ApprovalHistoryDto>(
            approvalsSql, new { RequestId = requestId });

        request.Approvals = approvals.ToList();
        return request;
    }


}
