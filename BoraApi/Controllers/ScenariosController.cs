using Bora.Scenarios;
using Microsoft.AspNetCore.Mvc;
using Bora.Entities;
using Repository.AzureTables;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class ScenariosController : ODataController<Scenario>
    {
        private readonly IScenarioService _scenarioService;

        public ScenariosController(IAzureTablesRepository boraRepository, IScenarioService scenarioService) : base(boraRepository)
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
