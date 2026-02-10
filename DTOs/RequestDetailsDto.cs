namespace EnterpriseRequestManagement.Api.DTOs
{
    public class RequestDetailsDto
    {
        public int RequestId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Department { get; set; }
        public string Status { get; set; } = "";
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public List<ApprovalHistoryDto> Approvals { get; set; } = new();
    }
}
