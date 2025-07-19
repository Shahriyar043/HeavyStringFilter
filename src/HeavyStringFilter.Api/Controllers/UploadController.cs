using AutoMapper;
using HeavyStringFilter.Api.Models;
using HeavyStringFilter.Application.Enums;
using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace HeavyStringFilter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController(IUploadService uploadService, IMapper mapper) : ControllerBase
{
    [DisableRequestSizeLimit]
    [HttpPost]
    public async Task<IActionResult> UploadChunk([FromBody] UploadChunkRequest request)
    {
        var chunkDto = mapper.Map<UploadChunkDto>(request);

        await uploadService.StoreChunkAsync(chunkDto);

        if (request.IsLastChunk)
        {
            await uploadService.EnqueueForProcessingAsync(request.UploadId);
        }

        return Accepted(new { status = UploadStatus.Accepted.ToString() });
    }
}
