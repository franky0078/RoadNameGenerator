// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System.Collections.Generic;
using Colossal;

namespace RoadNameGenerator.Localization
{
    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleFR(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return RoadNameGeneratorLocale.AddEntries(new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Road Name Generator" },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "Général" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAutomaticNamingGroup),
                    "Nommage automatique des voies" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kExistingRoadsGroup),
                    "Voies existantes" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDiagnosticsGroup),
                    "Version " + ModVersion.Current },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Style de noms" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Sélectionne le répertoire régional utilisé pour générer les noms des voies." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Nommer automatiquement les nouvelles voies" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Les nouvelles voies reçoivent automatiquement un nom du style sélectionné." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Mode de vérification des voies existantes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Détermine quels noms existants sont remplacés lors de la vérification manuelle." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Vérifier les voies existantes maintenant" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Vérifie une fois les noms existants selon le mode sélectionné." },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]", "Américain" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]", "Britannique" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]", "Allemand" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]", "Italien" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[French]", "Français" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swedish]", "Suédois" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Spanish]", "Espagnol" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Canadian]", "Canadien" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Australian]", "Australien" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Polish]", "Polonais" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Dutch]", "Néerlandais" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swiss]", "Suisse (allemand)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[PortugueseBrazil]", "Brésil" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[PortuguesePortugal]", "Portugal" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissFrench]", "Suisse (français)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissItalian]", "Suisse (italien)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissRomansh]", "Suisse (romanche)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Norwegian]", "Norvégien" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Danish]", "Danois" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Mexican]", "Mexicain" },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Uniquement les noms générés par le jeu" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Noms ne correspondant pas au style sélectionné" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Renommer toutes les voies" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Journalisation détaillée" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Consigne les informations détaillées sur les voies détectées, vérifiées et renommées dans le journal. Cette option peut rester désactivée en utilisation normale." }
            }, m_Setting, "fr");
        }

        public void Unload()
        {
        }
    }
}
