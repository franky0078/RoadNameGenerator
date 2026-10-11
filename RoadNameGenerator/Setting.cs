using Colossal.IO.AssetDatabase;
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
        public NamingStyle SelectedNamingStyle
        {
            get;
            set;
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

            CustomNames = CustomNameMode.Disabled;
            ShowRoadSuggestionsButton = true;
            EnableDetailedLogging = false;
        }
    }
}