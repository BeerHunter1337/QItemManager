using PoeHUD.Framework;
using PoeHUD.Plugins;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using QItemManager.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace QItemManager
{
    internal class QItemManager : BaseSettingsPlugin<Settings>
    {
        public QItemManager()
        {
            PluginName = "Quality Item Manager";
        }

        public override void Render()
        {
            if (WinApi.IsKeyDown(Settings.DropKey))
            {
                var set = ScanStash();
                if(set != null)
                {
                    DropToInventory(set);
                }
            }
        }

        public NormalInventoryItem[] ScanStash()
        {
            var stashPanel = GameController.Game.IngameState.ServerData.StashPanel;

            if (stashPanel == null)
            {
                LogMessage("ServerData.StashPanel is null", 3);
                return null;
            }

            var gems = new List<NormalInventoryItem>();

            foreach (var item in stashPanel.VisibleStash.VisibleInventoryItems)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.Item.HasComponent<SkillGem>() && item.Item.HasComponent<Quality>())
                {
                    gems.Add(item);
                }
            }

            var sum = 0;
            var set = new List<NormalInventoryItem>();
            foreach(var gem in gems)
            {
                if (sum + gem.Item.GetComponent<Quality>().ItemQuality == 40 || sum + gem.Item.GetComponent<Quality>().ItemQuality <=35)
                {
                    set.Add(gem);
                    sum += gem.Item.GetComponent<Quality>().ItemQuality;
                }
                if(sum == 40)
                {
                    break;
                }
            }
            if (sum == 40)
                return set.ToArray();
            return null;
        }

        private void DropToInventory(IEnumerable<NormalInventoryItem> items)
        {
            if (items == null)
                return;

            if (!items.Any())
                return;

            var gameWindowPos = GameController.Window.GetWindowRectangle();
            var latency = (int)GameController.Game.IngameState.CurLatency;

            Keyboard.KeyDown(Keys.LControlKey);
            Thread.Sleep(latency + Settings.ExtraDelay);

            foreach (var item in items)
            {
                Mouse.SetCursorPosAndLeftClick(item.GetClientRect().Center + gameWindowPos.TopLeft, Settings.ExtraDelay);
                Thread.Sleep(latency + Settings.ExtraDelay);
            }

            Keyboard.KeyUp(Keys.LControlKey);
        }

    }
}
