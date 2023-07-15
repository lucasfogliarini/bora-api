using Bora.Database;
using Bora.Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Scenarios
{
    public class ScenarioService : IScenarioService
    {
        private readonly IBoraDatabase _boraDatabase;

        public ScenarioService(IBoraDatabase boraDatabase)
        {
            _boraDatabase = boraDatabase;
        }
        public async Task UpdateAsync(int scenarioId, ScenarioInput scenarioInput)
        {
            var scenario = _boraDatabase.Query<Scenario>().FirstOrDefault(e=>e.Id == scenarioId);
            if (scenario == null)
            {
                throw new ValidationException("Não existe um cenário com esse id.");
            }
            else
            {
                scenario.UpdatedAt = DateTime.Now;
                if (scenarioInput.Title != null)
                    scenario.Title = scenarioInput.Title!;
                if (scenarioInput.Enabled.HasValue)
                    scenario.Enabled = scenarioInput.Enabled.Value;

                _boraDatabase.Update(scenario);
                await _boraDatabase.CommitAsync();
            }
        }
    }
}
