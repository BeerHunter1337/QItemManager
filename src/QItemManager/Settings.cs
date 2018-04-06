using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System.Windows.Forms;

namespace QItemManager
{
    class Settings: SettingsBase
    {
        public Settings()
        {
            Enable = false;
            DropKey = Keys.F6;
            ExtraDelay = new RangeNode<int>(50, 0, 2000);
            HighlightQuality = new RangeNode<int>(15, 0, 20);
        }

        [Menu("Drop Key")]
        public HotkeyNode DropKey { get; private set; }

        [Menu("Extra Click Delay")]
        public RangeNode<int> ExtraDelay { get; set; }

        [Menu("Quality for highlighting (0 to disable)")]
        public RangeNode<int> HighlightQuality { get; set; }
    }
}
