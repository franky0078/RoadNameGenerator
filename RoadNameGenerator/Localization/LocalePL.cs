// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System.Collections.Generic;
using Colossal;

namespace RoadNameGenerator.Localization
{
    public sealed class LocalePL : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePL(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Road Name Generator" },
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "Ogólne" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAutomaticNamingGroup),
                    "Automatyczne nadawanie nazw ulic" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kExistingRoadsGroup),
                    "Istniejące ulice" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDiagnosticsGroup),
                    "Wersja 1.9.5" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Styl nazewnictwa" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Wybiera regionalny zestaw nazw używany do nazywania ulic." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Automatycznie nazywaj nowe ulice" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Nowo zbudowane ulice automatycznie otrzymują nazwy zgodne z wybranym stylem." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Tryb sprawdzania istniejących ulic" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Określa, które istniejące nazwy zostaną zastąpione podczas ręcznego sprawdzania." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Sprawdź teraz istniejące ulice" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Jednorazowo sprawdza istniejące nazwy według wybranego trybu." },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]", "Amerykański" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]", "Brytyjski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]", "Niemiecki" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]", "Włoski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[French]", "Francuski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swedish]", "Szwedzki" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Spanish]", "Hiszpański" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Canadian]", "Kanadyjski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Australian]", "Australijski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Polish]", "Polski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Dutch]", "Holenderski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swiss]", "Szwajcaria (niemiecki)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissFrench]", "Szwajcaria (francuski)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissItalian]", "Szwajcaria (włoski)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissRomansh]", "Szwajcaria (retoromański)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Norwegian]", "Norweski" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Danish]", "Duński" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Mexican]", "Meksykański" },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Tylko nazwy wygenerowane przez grę" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Nazwy niepasujące do wybranego stylu" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Zmień nazwy wszystkich ulic" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Szczegółowe logowanie" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Zapisuje szczegółowe informacje o wykrytych, sprawdzonych i przemianowanych ulicach. Przy zwykłym użytkowaniu można wyłączyć tę opcję." }
            };
        }

        public void Unload()
        {
        }
    }
}
