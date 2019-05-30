using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FishBot
{
    public class ModEntry : Mod
    {
        private bool catchingTreasure;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.UpdateTicking += GameEvents_UpdateTick;
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            var player = Game1.player;

            if (player == null)
                return;

            if (Game1.player.CurrentTool is FishingRod rod)
            {
                // auto pullin && autohit
                if (rod.isNibbling && !rod.isReeling && !rod.hit && !rod.pullingOutOfWater && !rod.fishCaught)
                    rod.DoFunction(player.currentLocation, 1, 1, 1, player);

                // max castingpower
                if (rod.isTimingCast)
                    rod.castingPower = 1.01f;

                //if (rod.isFishing && rod.timeUntilFishingBite > 1)
                //{
                //    rod.timeUntilFishingBite = 1;
                //}

                if (Game1.activeClickableMenu is BobberBar bobberBar)
                {
                    //float bobberBarPos = Helper.Reflection.GetField<float>(bobberBar, "bobberBarPos", true).GetValue();
                    float bobberBarHeight = Helper.Reflection.GetField<int>(bobberBar, "bobberBarHeight", true).GetValue();
                    bool treasure = Helper.Reflection.GetField<bool>(bobberBar, "treasure", true).GetValue();
                    bool treasureCaught = Helper.Reflection.GetField<bool>(bobberBar, "treasureCaught", true).GetValue();
                    float distanceFromCatching = Helper.Reflection.GetField<float>(bobberBar, "distanceFromCatching", true).GetValue();

                    float fishPosition = Helper.Reflection.GetField<float>(bobberBar, "bobberPosition", true).GetValue();
                    float treasurePosition = Helper.Reflection.GetField<float>(bobberBar, "treasurePosition", true).GetValue();
                    //var bobberBarStart = 0f;
                    var bobberBarEnd = 568f - bobberBarHeight;
                    var offset = (bobberBarHeight / 4f);
                    //var bobberStart = bobberBarStart + bobberOffset;
                    //var bobberEnd = bobberBarEnd + bobberOffset;

                    if (!rod.fishCaught)
                    {
                        var fishPositionPercent = ((fishPosition - offset) / bobberBarEnd);
                        var scaledTargetPosition = bobberBarEnd * fishPositionPercent;
                        Helper.Reflection.GetField<float>(bobberBar, "bobberBarPos", true).SetValue(scaledTargetPosition);
                        //Helper.Reflection.GetField<float>(bobberBar, "bobberPosition", true).SetValue(bobberStart);
                        Helper.Reflection.GetField<float>(bobberBar, "bobberBarSpeed", true).SetValue(0f);
                    }

                    if (treasure && !treasureCaught && distanceFromCatching > 0.75)
                        catchingTreasure = true;

                    if (catchingTreasure)
                    {
                        var treasurePositionPercent = ((treasurePosition - offset) / bobberBarEnd);
                        var scaledTargetPosition = bobberBarEnd * treasurePositionPercent;
                        Helper.Reflection.GetField<float>(bobberBar, "bobberBarPos", true).SetValue(scaledTargetPosition);
                    }

                    if ((catchingTreasure && treasureCaught))
                        catchingTreasure = false;
                }
                else
                {
                    catchingTreasure = false;
                }
            }
        }
    }
}
