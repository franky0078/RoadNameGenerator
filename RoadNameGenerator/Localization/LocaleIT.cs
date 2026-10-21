// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System.Collections.Generic;
using Colossal;

namespace RoadNameGenerator.Localization
{
    public sealed class LocaleIT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleIT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "Generale" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAutomaticNamingGroup),
                    "Assegnazione automatica dei nomi" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kExistingRoadsGroup),
                    "Strade esistenti" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDiagnosticsGroup),
                    "Versione " + ModVersion.Current },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Stile dei nomi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Seleziona il repertorio regionale usato per generare i nomi delle strade." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Assegna automaticamente un nome alle nuove strade" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Le nuove strade ricevono automaticamente un nome dello stile selezionato." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Modalità di controllo delle strade esistenti" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Determina quali nomi esistenti vengono sostituiti durante il controllo manuale." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Controlla ora le strade esistenti" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Esegue un controllo una tantum dei nomi esistenti secondo la modalità selezionata." },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]", "Statunitense" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]", "Britannico" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]", "Tedesco" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]", "Italiano" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[French]", "Francese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swedish]", "Svedese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Spanish]", "Spagnolo" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Canadian]", "Canadese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Australian]", "Australiano" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Polish]", "Polacco" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Dutch]", "Olandese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swiss]", "Svizzera (tedesco)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[PortugueseBrazil]", "Portoghese (Brasile)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[PortuguesePortugal]", "Portoghese (Portogallo)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissFrench]", "Svizzera (francese)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissItalian]", "Svizzera (italiano)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissRomansh]", "Svizzera (romancio)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Norwegian]", "Norvegese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Danish]", "Danese" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Mexican]", "Messicano" },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Solo i nomi generati dal gioco" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Nomi non corrispondenti allo stile selezionato" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Rinomina tutte le strade" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Registrazione dettagliata" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Scrive nel registro informazioni dettagliate sulle strade rilevate, controllate e rinominate. Questa opzione può restare disattivata durante l'uso normale." }
            }, m_Setting, "it");
        }

        public void Unload()
        {
        }
    }
}
