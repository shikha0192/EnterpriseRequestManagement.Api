using EnterpriseRequestManagement.Api.DTOs;
using EnterpriseRequestManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseRequestManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly RequestService _service;

    public RequestsController(RequestService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateRequestDto dto)
    {
        var id = await _service.CreateRequestAsync(dto);
        return Ok(new { RequestId = id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpPost("{requestId:int}/action")]
    public async Task<IActionResult> TakeAction(int requestId, ApprovalActionDto dto)
    {
        var ok = await _service.TakeActionAsync(requestId, dto);
        return ok ? Ok(new { Message = "Action saved." }) : NotFound();
    }
}
