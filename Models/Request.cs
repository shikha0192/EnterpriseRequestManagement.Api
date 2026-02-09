namespace EnterpriseRequestManagement.Api.Models
{
    public class Request
    {
        public int RequestId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Department { get; set; }
        public string Status { get; set; } = "Pending";
        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
        
    }
}
