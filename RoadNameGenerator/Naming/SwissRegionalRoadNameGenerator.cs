// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System;
using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    // Reuses the existing Swiss portfolio, selecting names by language.
    public sealed class SwissRegionalRoadNameGenerator : IRoadNameGenerator
    {
        private static readonly string[][] s_Source =
        {
            SwissRoadNames.DirtNames, SwissRoadNames.AlleyNames,
            SwissRoadNames.ResidentialNames, SwissRoadNames.StandardNames,
            SwissRoadNames.AvenueNames, SwissRoadNames.HighwayRouteNames
        };

        private readonly NamingStyle m_Style;
        private readonly Random m_Random = new();
        private readonly HashSet<string> m_Used = new(StringComparer.OrdinalIgnoreCase);
        private readonly string[][] m_Names = new string[6][];
        private readonly HashSet<string>[] m_Portfolio = new HashSet<string>[6];

        public SwissRegionalRoadNameGenerator(NamingStyle style)
        {
            m_Style = style;
            for (int i = 0; i < 6; i++)
            {
                var names = new List<string>();
                foreach (string name in s_Source[i])
                {
                    if (Matches(name, style, i)) names.Add(name);
                }
                names.AddRange(SwissRegionalExtraNames.Get(style, i));
                names = new List<string>(new HashSet<string>(names, StringComparer.OrdinalIgnoreCase));
                if (names.Count == 0)
                    throw new InvalidOperationException($"Missing Swiss names: {style}, {i}");
                m_Names[i] = names.ToArray();
                m_Portfolio[i] = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
            }
        }

        public string Generate(RoadCategory category)
        {
            int i = Index(category);
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string name = m_Names[i][m_Random.Next(m_Names[i].Length)];
                if (m_Used.Add(name)) return name;
            }
            for (int number = 1; ; number++)
            {
                string name = Fallback(i) + number;
                if (m_Used.Add(name)) return name;
            }
        }

        public void RegisterExistingName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name)) m_Used.Add(name.Trim());
        }

        public bool IsNameFromPortfolio(string name, RoadCategory category)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            int i = Index(category);
            string trimmed = name.Trim();
            if (m_Portfolio[i].Contains(trimmed)) return true;
            string prefix = Fallback(i);
            return trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(trimmed.Substring(prefix.Length), out int number) && number > 0;
        }

        private static int Index(RoadCategory category)
        {
            int i = (int)category;
            return i >= 0 && i < 6 ? i : (int)RoadCategory.Standard;
        }

        private string Fallback(int i)
        {
            return m_Style switch
            {
                NamingStyle.SwissFrench => new[]
                    { "Sentier ", "Passage ", "Chemin ", "Rue ", "Avenue ", "Route " }[i],
                NamingStyle.SwissItalian => new[]
                    { "Sentiero ", "Vicolo ", "Via ", "Via ", "Viale ", "Strada principale " }[i],
                NamingStyle.SwissRomansh => new[]
                    { "Via da ", "Giassa ", "Via da ", "Via da ", "Promenada ", "Via principala " }[i],
                _ => new[]
                    { "Pfad ", "Gässli ", "Weg ", "Strasse ", "Allee ", "Hauptstrasse " }[i]
            };
        }

        private static bool Matches(string name, NamingStyle style, int category)
        {
            switch (style)
            {
                case NamingStyle.Swiss:
                    return category switch
                    {
                        0 => Ends(name, "weg", "pfad"),
                        1 => Ends(name, "gasse", "gässli", "strasse") || name == "Schifflände",
                        2 => Ends(name, "strasse", "weg", "hof", "rain"),
                        3 => Ends(name, "strasse", "gasse", "quai"),
                        4 => Ends(name, "allee", "promenade", "quai"),
                        _ => name.StartsWith("Hauptstrasse ", StringComparison.Ordinal) ||
                             name.Length > 1 && name[0] == 'A' && char.IsDigit(name[1]) ||
                             Ends(name, "route", "korridor", "-Achse") &&
                             !name.StartsWith("Rhône", StringComparison.Ordinal)
                    };
                case NamingStyle.SwissFrench:
                    return Starts(name, "Chemin ", "Sentier ", "Rue ",
                        "Ruelle ", "Impasse ", "Passage ", "Route ",
                        "Avenue ", "Boulevard ", "Quai ", "Promenade ", "Bisse ",
                        "Rhônekorridor");
                case NamingStyle.SwissRomansh:
                    return Starts(name, "Via da ", "Via d'", "Via principala ",
                        "Giassa ", "Passagi ", "Promenada ", "Corredur ") ||
                        name.StartsWith("Via ", StringComparison.Ordinal) &&
                        RomanshPlace(name.Substring(4));
                case NamingStyle.SwissItalian:
                    return Starts(name, "Sentiero ", "Strada ", "Corte ",
                        "Salita ", "Vicolo ", "Contrada ", "Viale ",
                        "Riva ", "Lungolago", "Passeggiata ",
                        "Autostrada ", "Corridoio ") ||
                        name.StartsWith("Via ", StringComparison.Ordinal) &&
                        !Starts(name, "Via da ", "Via d'", "Via principala ") &&
                        !RomanshPlace(name.Substring(4));
                default:
                    return false;
            }
        }

        private static bool RomanshPlace(string place)
        {
            // Stems of the Romansh 'Via da ...' names in the existing list.
            foreach (string name in SwissRoadNames.DirtNames)
                if (name.StartsWith("Via da ", StringComparison.Ordinal) &&
                    string.Equals(name.Substring(7), place, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static bool Starts(string name, params string[] prefixes)
        {
            foreach (string prefix in prefixes)
                if (name.StartsWith(prefix, StringComparison.Ordinal)) return true;
            return false;
        }

        private static bool Ends(string name, params string[] suffixes)
        {
            foreach (string suffix in suffixes)
                if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
