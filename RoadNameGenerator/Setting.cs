using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Game.UI.Localization;
using Game.UI.Widgets;
using System;
using Game.Modding;
using Game.Settings;
using RoadNameGenerator.Naming;

namespace RoadNameGenerator
{
    [FileLocation("RoadNameGenerator")]
    [SettingsUIGroupOrder(
    kAutomaticNamingGroup,
    kCustomNamesGroup,
    kSingleRoadGroup,
    kExistingRoadsGroup,
    kDiagnosticsGroup
)]
    [SettingsUIShowGroupName(
    kAutomaticNamingGroup,
    kCustomNamesGroup,
    kSingleRoadGroup,
    kExistingRoadsGroup,
    kDiagnosticsGroup
)]
    public sealed class Setting : ModSetting
    {
        public const string kMainSection = "Main";
        public const string kAutomaticNamingGroup = "AutomaticNaming";
        public const string kExistingRoadsGroup = "ExistingRoads";
        public const string kCustomNamesGroup = "CustomNames";
        public const string kSingleRoadGroup = "SingleRoad";
        public const string kDiagnosticsGroup = "Diagnostics";


        public Setting(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(
            kMainSection,
            kAutomaticNamingGroup
        )]
        [SettingsUIHidden]
        public NamingStyle SelectedNamingStyle
        {
            get;
            set;
        }

        // UI order is independent from the persisted enum values.
        [Exclude]
        [SettingsUIDropdown(typeof(Setting), nameof(GetNamingStyleItems))]
        [SettingsUISection(kMainSection, kAutomaticNamingGroup)]
        public int NamingStyleSelection
        {
            get => (int)SelectedNamingStyle;
            set
            {
                if (Enum.IsDefined(typeof(NamingStyle), value))
                    SelectedNamingStyle = (NamingStyle)value;
            }
        }

        public DropdownItem<int>[] GetNamingStyleItems()
        {
            NamingStyle[] styles =
            {
                NamingStyle.American, NamingStyle.Australian,
                NamingStyle.British, NamingStyle.Canadian,
                NamingStyle.Danish, NamingStyle.Dutch,
                NamingStyle.French, NamingStyle.German,
                NamingStyle.Italian, NamingStyle.Mexican,
                NamingStyle.Norwegian, NamingStyle.Polish,
                NamingStyle.PortugueseBrazil, NamingStyle.PortuguesePortugal,
                NamingStyle.Spanish,
                NamingStyle.Swedish, NamingStyle.Swiss,
                NamingStyle.SwissFrench, NamingStyle.SwissItalian,
                NamingStyle.SwissRomansh
            };
            DropdownItem<int>[] items = new DropdownItem<int>[styles.Length];
            for (int i = 0; i < styles.Length; i++)
            {
                items[i] = new DropdownItem<int>
                {
                    value = (int)styles[i],
                    displayName = LocalizedString.Id(GetEnumValueLocaleID(styles[i]))
                };
            }
            return items;
        }

        [SettingsUISection(
            kMainSection,
            kAutomaticNamingGroup
        )]
        public bool AutomaticallyRenameNewRoads
        {
            get;
            set;
        }

        [SettingsUISection(
            kMainSection,
            kExistingRoadsGroup
        )]
        public ExistingRoadMode ExistingRoadCheckMode
        {
            get;
            set;
        }

        [SettingsUISection(kMainSection, kExistingRoadsGroup)]
        public bool OverwriteIndividuallyNamedRoads { get; set; }

        [SettingsUIButton]
        [SettingsUISection(
            kMainSection,
            kExistingRoadsGroup
        )]
        public bool CheckExistingRoads
        {
            set
            {
                Mod.RequestExistingRoadScan();
            }
        }

        [SettingsUISection(kMainSection, kCustomNamesGroup)]
        public CustomNameMode CustomNames { get; set; }

        [SettingsUIButton]
        [SettingsUISection(kMainSection, kCustomNamesGroup)]
        public bool OpenCustomNamesFolder { set => CustomRoadNameList.OpenFolder(); }

        [SettingsUIButton]
        [SettingsUISection(kMainSection, kCustomNamesGroup)]
        public bool ReloadCustomNames { set => CustomRoadNameList.Reload(); }

        [SettingsUISection(kMainSection, kCustomNamesGroup)]
        public bool UseNameExclusions { get; set; }

        [SettingsUIButton]
        [SettingsUISection(kMainSection, kCustomNamesGroup)]
        public bool ReloadExcludedNames
        {
            set
            {
                RoadNameExclusionList.Reload();
                UseNameExclusions = true;
                ApplyAndSave();
            }
        }

        [SettingsUISection(kMainSection, kSingleRoadGroup)]
        public bool ShowRoadSuggestionsButton { get; set; }

        [SettingsUISection(
            kMainSection,
            kDiagnosticsGroup
        )]
        public bool EnableDetailedLogging
        {
            get;
            set;
        }

        public override void SetDefaults()
        {
            SelectedNamingStyle =
                NamingStyle.American;

            AutomaticallyRenameNewRoads =
                true;

            ExistingRoadCheckMode =
                ExistingRoadMode.GameGeneratedOnly;

            OverwriteIndividuallyNamedRoads = false;

            CustomNames = CustomNameMode.Disabled;
            UseNameExclusions = true;
            ShowRoadSuggestionsButton = true;
            EnableDetailedLogging = false;
        }
    }
}