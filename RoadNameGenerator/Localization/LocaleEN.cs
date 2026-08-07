using System.Collections.Generic;
using Colossal;

namespace RoadNameGenerator.Localization
{
    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>>
            ReadEntries(
                IList<IDictionaryEntryError> errors,
                Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                /*
                 * Mod name.
                 */
                {
                    m_Setting.GetSettingsLocaleID(),
                    "Road Name Generator"
                },

                /*
                 * Main tab.
                 */
                {
                    m_Setting.GetOptionTabLocaleID(
                        Setting.kMainSection
                    ),
                    "General"
                },

                /*
                 * Option groups.
                 */
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kAutomaticNamingGroup
                    ),
                    "Automatic road naming"
                },
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kExistingRoadsGroup
                    ),
                    "Existing roads"
                },
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kDiagnosticsGroup
                    ),
                    "Version 1.5.1"
                },

                /*
                 * Naming style setting.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.SelectedNamingStyle)
                    ),
                    "Naming style"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.SelectedNamingStyle)
                    ),
                    "Selects the regional name portfolio used for generated road names."
                },

                /*
                 * Automatic naming setting.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(
                            Setting.AutomaticallyRenameNewRoads
                        )
                    ),
                    "Automatically name new roads"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(
                            Setting.AutomaticallyRenameNewRoads
                        )
                    ),
                    "Newly built roads automatically receive a name from the selected naming style."
                },

                /*
                 * Existing-road check mode.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(
                            Setting.ExistingRoadCheckMode
                        )
                    ),
                    "Existing road check mode"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(
                            Setting.ExistingRoadCheckMode
                        )
                    ),
                    "Determines which existing road names are replaced during a manual scan."
                },

                /*
                 * Manual existing-road scan.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.CheckExistingRoads)
                    ),
                    "Check existing roads now"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.CheckExistingRoads)
                    ),
                    "Runs a one-time check of existing road names using the selected check mode."
                },

                /*
                 * Naming style values.
                 */
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]",
                    "American"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]",
                    "British"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]",
                    "German"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]",
                    "Italian"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[French]",
                    "French"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swedish]",
                    "Swedish"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Spanish]",
                    "Spanish"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Canadian]",
                    "Canadian"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Australian]",
                    "Australian"
                },

                /*
                 * Existing-road mode values.
                 */
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Game-generated names only"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Names not matching the selected style"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Rename all roads"
                },

                /*
                 * Detailed logging.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.EnableDetailedLogging)
                    ),
                    "Detailed logging"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.EnableDetailedLogging)
                    ),
                    "Writes detailed information about detected, checked and renamed roads to the log file. This option can remain disabled during normal use."
                }
            };
        }

        public void Unload()
        {
        }
    }
}