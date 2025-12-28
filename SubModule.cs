// Brut.NoEducationPopups - Auto-complete child education popups
// by Brut | Open Source | MIT License
// https://github.com/markbrutx/Brut.NoEducationPopups

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Brut.NoEducationPopups
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Debug.Print("[Brut.NoEducationPopups] SubModule loaded!");
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (game.GameType is Campaign)
            {
                Debug.Print("[Brut.NoEducationPopups] Adding behavior to campaign...");
                var campaignStarter = gameStarter as CampaignGameStarter;
                campaignStarter?.AddBehavior(new AutoEducationBehavior());
            }
        }
    }
}
