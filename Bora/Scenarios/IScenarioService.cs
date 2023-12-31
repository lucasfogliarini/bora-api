namespace Bora.Scenarios
{
	public interface IScenarioService
    {
        Task UpdateAsync(int scenarioId, ScenarioInput scenarioInput);
    }
}
