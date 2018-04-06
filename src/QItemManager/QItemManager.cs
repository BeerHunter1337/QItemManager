using PoeHUD.Framework;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe.RemoteMemoryObjects;
using QItemManager.Utilities;
using SharpDX;
using System;
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
            var stashPanel = GameController.Game.IngameState.ServerData.StashPanel;
            var playerInventory = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];

            if (Settings.HighlightQuality != 0)
            {
                if (stashPanel != null && stashPanel.IsVisible)
                {
                    HighlightQItems<SkillGem>(stashPanel.VisibleStash);
                    HighlightQItems<Flask>(stashPanel.VisibleStash);

                }

                if (playerInventory != null && playerInventory.InventoryUiElement.IsVisible)
                {
                    HighlightQItems<SkillGem>(playerInventory);
                    HighlightQItems<Flask>(playerInventory);
                }
            }

            if (WinApi.IsKeyDown(Settings.DropKey))
            {
                if (stashPanel.IsVisible)
                {
                    var set = ScanInventory<SkillGem>(stashPanel.VisibleStash) ?? ScanInventory<Flask>(stashPanel.VisibleStash);
                    if (set != null)
                    {
                        DropToInventory(set);
                    }
                } else if (GameController.Game.IngameState.UIRoot
                    .Children[1]
                    .Children[47]
                    .Children[3].IsVisible)
                {
                    SellItems<SkillGem>();
                    SellItems<Flask>();
                }
            }
        }

        public void HighlightQItems<T>(Inventory inventory)
            where T: Component, new()
        {
            var items = from invItem in inventory.VisibleInventoryItems
                        let item = invItem.Item
                        where item.HasComponent<T>() && item.HasComponent<Quality>()
                            && item.GetComponent<Quality>().ItemQuality >= Settings.HighlightQuality
                        select invItem;

            foreach (var item in items)
            {
                var rect = item.GetClientRect();

                var borderColor = Color.SmoothStep(Color.DarkBlue, Color.LightBlue,
                    1f - (float)(item.Item.GetComponent<Quality>().ItemQuality - Settings.HighlightQuality) / Settings.HighlightQuality);

                rect.X += 2;
                rect.Y += 2;

                rect.Width -= 4;
                rect.Height -= 4;

                Graphics.DrawFrame(rect, 2, borderColor);
            }
        }

        public NormalInventoryItem[] ScanInventory<T>(Inventory inventory)
            where T : Component, new()
        {
            var items = new List<NormalInventoryItem>();

            foreach (var invItem in inventory.VisibleInventoryItems)
            {
                var item = invItem.Item;

                if (item == null)
                {
                    continue;
                }

                if (item.HasComponent<T>() && item.HasComponent<Quality>())
                {
                    items.Add(invItem);
                }
            }

            var sum = 0;
            var set = new List<NormalInventoryItem>();
            foreach(var invItem in items)
            {
                var item = invItem.Item;

                if (sum + item.GetComponent<Quality>().ItemQuality == 40 || sum + item.GetComponent<Quality>().ItemQuality <=35)
                {
                    set.Add(invItem);
                    sum += item.GetComponent<Quality>().ItemQuality;
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

        private void SellItems<T>()
            where T : Component, new()
        {
            var inv = GameController.Game.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory];
            var itemsToDrop = inv.VisibleInventoryItems.Where(i => i.Item.HasComponent<T>());
            if (itemsToDrop.Any())
            {
                var gameWindowPos = GameController.Window.GetWindowRectangle();
                var latency = (int)GameController.Game.IngameState.CurLatency;

                Keyboard.KeyDown(Keys.LControlKey);
                Thread.Sleep(latency + Settings.ExtraDelay);

                foreach (var item in itemsToDrop)
                {
                    Mouse.SetCursorPosAndLeftClick(item.GetClientRect().Center + gameWindowPos.TopLeft, Settings.ExtraDelay);
                    Thread.Sleep(latency + Settings.ExtraDelay);
                }
                Keyboard.KeyUp(Keys.LControlKey);
            }
        }

        private void DropToInventory(IEnumerable<NormalInventoryItem> items)
        {
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
