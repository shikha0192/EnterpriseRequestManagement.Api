namespace EnterpriseRequestManagement.Api.DTOs
{
    public class CreateRequestDto
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Department { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
