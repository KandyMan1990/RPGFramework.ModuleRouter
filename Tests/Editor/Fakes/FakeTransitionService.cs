using System.Threading.Tasks;
using RPGFramework.Core.Transitions;

namespace RPGFramework.ModuleRouter.Tests.Fakes
{
    /// <summary>Transitions that play at once, every cover taking three quarters of a second.</summary>
    internal sealed class FakeTransitionService : ITransitionService
    {
        internal const float COVER_SECONDS = 0.75f;

        Task ITransitionService.CoverAsync(ulong transition) => Task.CompletedTask;

        Task<bool> ITransitionService.RevealAsync(ulong transition) => Task.FromResult(true);

        float ITransitionService.CoverSeconds(ulong transition) => COVER_SECONDS;
    }
}
