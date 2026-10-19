// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System.Collections.Generic;
using Colossal;

namespace RoadNameGenerator.Localization
{
    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleES(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.kMainSection), "General" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAutomaticNamingGroup),
                    "Asignación automática de nombres" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kExistingRoadsGroup),
                    "Vías existentes" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kDiagnosticsGroup),
                    "Versión 2.2.0" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Estilo de nombres" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedNamingStyle)),
                    "Selecciona el conjunto regional utilizado para generar los nombres de las vías." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Poner nombre automáticamente a las vías nuevas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutomaticallyRenameNewRoads)),
                    "Las vías nuevas reciben automáticamente un nombre del estilo seleccionado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Modo de revisión de vías existentes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ExistingRoadCheckMode)),
                    "Determina qué nombres existentes se sustituyen durante la revisión manual." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Revisar ahora las vías existentes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CheckExistingRoads)),
                    "Realiza una revisión única de los nombres existentes según el modo seleccionado." },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[American]", "Estadounidense" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[British]", "Británico" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[German]", "Alemán" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Italian]", "Italiano" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[French]", "Francés" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swedish]", "Sueco" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Spanish]", "Español" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Canadian]", "Canadiense" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Australian]", "Australiano" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Polish]", "Polaco" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Dutch]", "Neerlandés" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Swiss]", "Suiza (alemán)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[PortuguesePortugal]", "Portugués (Portugal)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissFrench]", "Suiza (francés)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissItalian]", "Suiza (italiano)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[SwissRomansh]", "Suiza (romanche)" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Norwegian]", "Noruego" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Danish]", "Danés" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.NAMINGSTYLE[Mexican]", "Mexicano" },

                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[GameGeneratedOnly]",
                    "Solo nombres generados por el juego" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[NonMatchingPortfolio]",
                    "Nombres que no coinciden con el estilo seleccionado" },
                { "Options.RoadNameGenerator.RoadNameGenerator.Mod.EXISTINGROADMODE[AllRoads]",
                    "Cambiar el nombre de todas las vías" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Registro detallado" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDetailedLogging)),
                    "Escribe información detallada sobre las vías detectadas, revisadas y renombradas en el registro. Puede desactivarse durante el uso normal." }
            }, m_Setting, "es");
        }

        public void Unload()
        {
        }
    }
}
