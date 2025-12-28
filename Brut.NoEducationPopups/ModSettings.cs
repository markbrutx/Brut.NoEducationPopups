// Brut.NoEducationPopups - Auto-complete child education popups
// by Brut | Open Source | MIT License
// https://github.com/markbrutx/Brut.NoEducationPopups

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace Brut.NoEducationPopups
{
    internal sealed class ModSettings : AttributeGlobalSettings<ModSettings>
    {
        public override string Id => "Brut.NoEducationPopups_v1";
        public override string DisplayName => "No Education Popups (by Brut)";
        public override string FolderName => "Brut.NoEducationPopups";
        public override string FormatType => "json2";

        // General Settings
        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Enable Mod", Order = 0, RequireRestart = false,
            HintText = "Enable or disable automatic child education.")]
        public bool IsEnabled { get; set; } = true;

        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyDropdown("Selection Mode", Order = 1, RequireRestart = false,
            HintText = "How to choose education options. Random picks randomly, Attribute Priority picks options that give preferred attributes.")]
        public Dropdown<string> SelectionMode { get; set; } = new Dropdown<string>(
            new[] { "Random", "Attribute Priority" }, 0);

        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Show Notifications", Order = 2, RequireRestart = false,
            HintText = "Show a message when a child's education is automatically completed.")]
        public bool ShowNotifications { get; set; } = true;

        [SettingPropertyGroup("General", GroupOrder = 0)]
        [SettingPropertyBool("Detailed Notifications", Order = 3, RequireRestart = false,
            HintText = "Show attribute and skill bonuses in the notification message.")]
        public bool DetailedNotifications { get; set; } = false;

        // Preferred Attributes (only used when Selection Mode is "Attribute Priority")
        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Vigor", Order = 0, RequireRestart = false,
            HintText = "Prefer options that give Vigor (One Handed, Two Handed, Polearm).")]
        public bool PreferVigor { get; set; } = false;

        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Control", Order = 1, RequireRestart = false,
            HintText = "Prefer options that give Control (Bow, Crossbow, Throwing).")]
        public bool PreferControl { get; set; } = false;

        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Endurance", Order = 2, RequireRestart = false,
            HintText = "Prefer options that give Endurance (Riding, Athletics, Smithing).")]
        public bool PreferEndurance { get; set; } = false;

        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Cunning", Order = 3, RequireRestart = false,
            HintText = "Prefer options that give Cunning (Scouting, Tactics, Roguery).")]
        public bool PreferCunning { get; set; } = false;

        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Social", Order = 4, RequireRestart = false,
            HintText = "Prefer options that give Social (Charm, Leadership, Trade).")]
        public bool PreferSocial { get; set; } = false;

        [SettingPropertyGroup("Preferred Attributes", GroupOrder = 1)]
        [SettingPropertyBool("Intelligence", Order = 5, RequireRestart = false,
            HintText = "Prefer options that give Intelligence (Steward, Medicine, Engineering).")]
        public bool PreferIntelligence { get; set; } = false;

        public bool IsAttributePriorityMode => SelectionMode?.SelectedIndex == 1;

        public bool HasPreferredAttributes =>
            PreferVigor || PreferControl || PreferEndurance ||
            PreferCunning || PreferSocial || PreferIntelligence;
    }
}
