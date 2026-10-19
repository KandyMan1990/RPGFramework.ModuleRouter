using NUnit.Framework;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Field.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Field.SharedTypes.Stores;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>A defeat, or a countdown running out, ends the game unless the battle mode allows it.</summary>
    internal sealed class WhenABattleEnds
    {
        [TestCase(BattleCompleteState.DEFEAT, BattleFlags.DEFAULT, true)]
        [TestCase(BattleCompleteState.DEFEAT, BattleFlags.DISABLE_GAME_OVER_ON_DEFEAT, false)]
        [TestCase(BattleCompleteState.OUT_OF_TIME, BattleFlags.DISABLE_GAME_OVER_ON_DEFEAT, true)]
        [TestCase(BattleCompleteState.OUT_OF_TIME, BattleFlags.DISABLE_GAME_OVER_ON_TIME_OUT, false)]
        [TestCase(BattleCompleteState.ESCAPE, BattleFlags.DEFAULT, false)]
        [TestCase(BattleCompleteState.VICTORY, BattleFlags.DEFAULT, false)]
        public void ThenTheGameIsOverOnlyWhenTheModeDoesNotAllowIt(BattleCompleteState state, BattleFlags mode, bool gameOver)
        {
            TestRouting   routing = new TestRouting();
            IModuleRouter router  = routing.Router();

            routing.Fight(router, state, mode);

            ModuleChange change   = router.Route(BattleConstants.MODULE_ID, BattleOutcomes.FINISHED);
            FieldArgs    next     = ((IFieldArgsStore)routing.FieldArgs).Args;
            string       music    = routing.Music.Log;
            byte         entry    = gameOver ? ModuleEntries.NEW : ModuleEntries.RETURN;
            int          fieldId  = gameOver ? 55 : 1;
            string       fadedOut = gameOver ? "stop 0.75" : string.Empty;

            routing.Destroy();

            Assert.AreEqual(ModuleChangeKind.Replace, change.Kind);
            Assert.AreEqual(FieldConstants.MODULE_ID, change.ModuleId);
            Assert.AreEqual(entry, change.Entry);
            Assert.AreEqual(fieldId, next.FieldId);
            Assert.AreEqual(fadedOut, music);
        }
    }
}
