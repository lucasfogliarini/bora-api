using Bora.Contents;
using Bora.Database;
using Bora.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentsController : ODataController<Content>
    {
        private readonly IContentService _contentService;

        public ContentsController(IBoraDatabase boraDatabase, IContentService contentService) : base(boraDatabase)
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
