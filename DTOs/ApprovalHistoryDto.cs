namespace EnterpriseRequestManagement.Api.DTOs
{
    public class ApprovalHistoryDto
    {
        public int ApprovalId { get; set; }
        public int ApproverUserId { get; set; }
        public string ApproverName { get; set; } = "";
        public string Action { get; set; } = "";
        public string? Comments { get; set; }
        public DateTime? ActionAt { get; set; }
    }
}
