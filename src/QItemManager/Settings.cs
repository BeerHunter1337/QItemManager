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
        }

        [Menu("Drop Key")]
        public HotkeyNode DropKey { get; private set; }

        [Menu("Extra Click Delay")]
        public RangeNode<int> ExtraDelay { get; set; }
    }
}
