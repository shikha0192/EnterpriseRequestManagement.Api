using EnterpriseRequestManagement.Api.DTOs;
using EnterpriseRequestManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EnterpriseRequestManagement.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly RequestService _service;

    public RequestsController(RequestService service) => _service = service;

    [Authorize(Roles = "Requester,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRequestDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var approverUserId))
            return Unauthorized(new { Message = "Invalid token: UserId missing." });

        var id = await _service.CreateRequestAsync(dto, approverUserId);
        return Ok(new { RequestId = id });
    }

    [Authorize(Roles = "Approver,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }
    [Authorize(Roles = "Approver,Admin")]
    [HttpPost("{requestId:int}/action")]
    public async Task<IActionResult> TakeAction(int requestId, ApprovalActionDto dto)
    {
        if (dto.Action is null ||
    !(dto.Action.Equals("Approved", StringComparison.OrdinalIgnoreCase) ||
      dto.Action.Equals("Rejected", StringComparison.OrdinalIgnoreCase)))
        {
            return BadRequest(new { Message = "Action must be Approved or Rejected." });
        }
        //  Get ApproverUserId from JWT token
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var approverUserId))
            return Unauthorized(new { Message = "Invalid token: UserId missing." });

        //  Call service with approverUserId
        var ok = await _service.TakeActionAsync(requestId, dto, approverUserId);

        return ok ? Ok(new { Message = "Action saved." }) : NotFound();
    }

    [Authorize(Roles = "Approver,Admin")]
    [HttpGet("{requestId:int}")]
    public async Task<IActionResult> GetById(int requestId)
    {
        var details = await _service.GetDetailsAsync(requestId);
        return details is null ? NotFound() : Ok(details);
    }
}
