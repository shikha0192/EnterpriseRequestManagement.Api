namespace EnterpriseRequestManagement.Api.DTOs
{
    public class ApprovalActionDto
    {

        public int ApproverUserId { get; set; }
        public string Action { get; set; } = ""; // Approved / Rejected
        public string? Comments { get; set; }
    }
}
