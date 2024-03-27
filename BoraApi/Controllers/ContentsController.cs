using Bora.Contents;
using Bora.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class ContentsController : ODataController<Content>
    {
        private readonly IContentService _contentService;

        public ContentsController(IRepository boraRepository, IContentService contentService) : base(boraRepository)
        {
            _contentService = contentService;
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAsync(ContentInput contentInput)
        {
            await _contentService.UpdateAsync(AuthenticatedUserEmail, contentInput);

            return Ok();
        }
    }
}
