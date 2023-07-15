using Bora.Scenarios;
using Bora.Database;
using Bora.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScenariosController : ODataController<Scenario>
    {
        private readonly IScenarioService _scenarioService;

        public ScenariosController(IScenarioService scenarioService, IBoraDatabase boraDatabase) : base(boraDatabase)
        {
            _scenarioService = scenarioService;
        }

        [HttpPatch("{scenarioId}")]
        public async Task<IActionResult> UpdateAsync(int scenarioId, ScenarioInput scenarioInput)
        {
            await _scenarioService.UpdateAsync(scenarioId, scenarioInput);
            return Ok();
        }
    }
}
