using System.Collections.Generic;
using Colossal;
using RoadNameGenerator.Naming;

namespace RoadNameGenerator.Localization
{
    public sealed class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleDE(Setting setting)
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
                 * Name der Mod im Optionsmenü.
                 */
                {
                    m_Setting.GetSettingsLocaleID(),
                    "Road Name Generator"
                },

                /*
                 * Hauptseite.
                 */
                {
                    m_Setting.GetOptionTabLocaleID(
                        Setting.kMainSection
                    ),
                    "Allgemein"
                },

                /*
                 * Gruppen.
                 */
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kAutomaticNamingGroup
                    ),
                    "Automatische Straßenbenennung"
                },
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kExistingRoadsGroup
                    ),
                    "Bestehende Straßen"
                },
                {
                    m_Setting.GetOptionGroupLocaleID(
                        Setting.kDiagnosticsGroup
                    ),
                    "Version 1.2.0"
                },

                /*
                 * Namensstil.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.SelectedNamingStyle)
                    ),
                    "Namensstil"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.SelectedNamingStyle)
                    ),
                    "Legt fest, aus welchem regionalen Namensportfolio neue Straßennamen erzeugt werden."
                },

                /*
                 * Neue Straßen automatisch benennen.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(
                            Setting.AutomaticallyRenameNewRoads
                        )
                    ),
                    "Neue Straßen automatisch benennen"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(
                            Setting.AutomaticallyRenameNewRoads
                        )
                    ),
                    "Neu gebaute Straßen erhalten automatisch einen Namen aus dem ausgewählten Namensstil."
                },

                /*
                 * Prüfmodus.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(
                            Setting.ExistingRoadCheckMode
                        )
                    ),
                    "Prüfmodus für bestehende Straßen"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(
                            Setting.ExistingRoadCheckMode
                        )
                    ),
                    "Legt fest, welche bestehenden Straßennamen bei der manuellen Prüfung ersetzt werden."
                },

                /*
                 * Bestandsprüfung.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.CheckExistingRoads)
                    ),
                    "Bestehende Straßen jetzt prüfen"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.CheckExistingRoads)
                    ),
                    "Führt einmalig eine Prüfung der bestehenden Straßennamen mit dem ausgewählten Prüfmodus durch."
                },

                /*
                 * Werte des Namensstil-Dropdowns.
                 */
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]",
                    "Amerikanisch"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]",
                    "Britisch"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]",
                    "Deutsch"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]",
                    "Italienisch"
                },

                /*
                 * Werte des Prüfmodus-Dropdowns.
                 */
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Nur vom Spiel erzeugte Namen"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Nicht zum Namensstil passende Namen"
                },
                {
                    "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Alle Straßen neu benennen"
                },

                /*
                 * Detaillierte Logausgabe.
                 */
                {
                    m_Setting.GetOptionLabelLocaleID(
                        nameof(Setting.EnableDetailedLogging)
                    ),
                    "Detaillierte Logausgabe"
                },
                {
                    m_Setting.GetOptionDescLocaleID(
                        nameof(Setting.EnableDetailedLogging)
                    ),
                    "Schreibt ausführliche Informationen zu erkannten, geprüften und umbenannten Straßen in die Logdatei. Für die normale Verwendung kann diese Option deaktiviert bleiben."
                }
            };
        }

        public void Unload()
        {
        }
    }
}