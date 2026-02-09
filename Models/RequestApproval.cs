namespace EnterpriseRequestManagement.Api.Models
{
    public class RequestApproval
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public int ApproverUserId { get; set; }
        public string? Action { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionAt { get; set; }
    }
}
