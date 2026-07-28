// Road Name Generator
// Copyright (C) 2026 franky0078
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3
// or, at your option, any later version.

using System;
using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    public sealed class ItalianRoadNameGenerator
        : IRoadNameGenerator
    {
        private readonly Random m_Random = new();

        private readonly HashSet<string> m_UsedNames =
            new(StringComparer.OrdinalIgnoreCase);

        public string Generate(RoadCategory category)
        {
            if (category == RoadCategory.Highway)
            {
                return GenerateHighwayName();
            }

            return GenerateRegularRoadName(category);
        }

        public bool IsNameFromPortfolio(
            string name,
            RoadCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string trimmedName = name.Trim();

            if (IsItalianRouteNumber(trimmedName) ||
                ContainsName(
                    ItalianRoadNames.NamedHighwayNames,
                    trimmedName))
            {
                return true;
            }

            if (ContainsName(
                    ItalianRoadNames.DirtSpecialNames,
                    trimmedName) ||
                ContainsName(
                    ItalianRoadNames.AlleySpecialNames,
                    trimmedName) ||
                ContainsName(
                    ItalianRoadNames.ResidentialSpecialNames,
                    trimmedName) ||
                ContainsName(
                    ItalianRoadNames.StandardSpecialNames,
                    trimmedName) ||
                ContainsName(
                    ItalianRoadNames.AvenueSpecialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesPrefixAndBase(
                    trimmedName,
                    ItalianRoadNames.DirtPrefixes,
                    ItalianRoadNames.DirtBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    ItalianRoadNames.AlleyPrefixes,
                    ItalianRoadNames.AlleyBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    ItalianRoadNames.ResidentialPrefixes,
                    ItalianRoadNames.ResidentialBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    ItalianRoadNames.StandardPrefixes,
                    ItalianRoadNames.StandardBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    ItalianRoadNames.AvenuePrefixes,
                    ItalianRoadNames.AvenueBaseNames
                ) ||
                IsNumberedFallbackName(trimmedName);
        }

        public void RegisterExistingName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            m_UsedNames.Add(name.Trim());
        }

        private string GenerateRegularRoadName(
            RoadCategory category)
        {
            string[] prefixes =
                GetPrefixes(category);

            string[] baseNames =
                GetBaseNames(category);

            string[] specialNames =
                GetSpecialNames(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                // Vollständige Namen sorgen für typische italienische
                // Formen wie "Piazza del Popolo" oder
                // "Lungomare Cristoforo Colombo".
                if (specialNames.Length > 0 &&
                    m_Random.Next(100) < 35)
                {
                    result =
                        specialNames[
                            m_Random.Next(
                                specialNames.Length
                            )
                        ];
                }
                else
                {
                    string prefix =
                        prefixes[
                            m_Random.Next(
                                prefixes.Length
                            )
                        ];

                    string baseName =
                        baseNames[
                            m_Random.Next(
                                baseNames.Length
                            )
                        ];

                    result = $"{prefix} {baseName}";
                }

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateFallbackName(category);
        }

        private string GenerateHighwayName()
        {
            /*
             * Verteilung:
             * 70 % nummerierte Straßen
             * 30 % benannte Autobahnen oder Schnellstraßen
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType =
                        m_Random.Next(100);

                    if (routeType < 40)
                    {
                        // Autostrada: A1, A4, A22 usw.
                        result =
                            $"A{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 68)
                    {
                        // Strada Statale.
                        result =
                            $"SS {m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 82)
                    {
                        // Strada Regionale.
                        result =
                            $"SR {m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 94)
                    {
                        // Strada Provinciale.
                        result =
                            $"SP {m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 98)
                    {
                        // Raccordo Autostradale.
                        result =
                            $"RA {m_Random.Next(1, 30)}";
                    }
                    else
                    {
                        // Europastraße.
                        result =
                            $"E{m_Random.Next(1, 100)}";
                    }
                }
                else
                {
                    result =
                        ItalianRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                ItalianRoadNames
                                    .NamedHighwayNames
                                    .Length
                            )
                        ];
                }

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateFallbackName(
                RoadCategory.Highway
            );
        }

        private static string[] GetPrefixes(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    ItalianRoadNames.DirtPrefixes,

                RoadCategory.Alley =>
                    ItalianRoadNames.AlleyPrefixes,

                RoadCategory.Residential =>
                    ItalianRoadNames.ResidentialPrefixes,

                RoadCategory.Standard =>
                    ItalianRoadNames.StandardPrefixes,

                RoadCategory.Avenue =>
                    ItalianRoadNames.AvenuePrefixes,

                _ =>
                    ItalianRoadNames.StandardPrefixes
            };
        }

        private static string[] GetBaseNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    ItalianRoadNames.DirtBaseNames,

                RoadCategory.Alley =>
                    ItalianRoadNames.AlleyBaseNames,

                RoadCategory.Residential =>
                    ItalianRoadNames.ResidentialBaseNames,

                RoadCategory.Standard =>
                    ItalianRoadNames.StandardBaseNames,

                RoadCategory.Avenue =>
                    ItalianRoadNames.AvenueBaseNames,

                _ =>
                    ItalianRoadNames.StandardBaseNames
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    ItalianRoadNames.DirtSpecialNames,

                RoadCategory.Alley =>
                    ItalianRoadNames.AlleySpecialNames,

                RoadCategory.Residential =>
                    ItalianRoadNames.ResidentialSpecialNames,

                RoadCategory.Standard =>
                    ItalianRoadNames.StandardSpecialNames,

                RoadCategory.Avenue =>
                    ItalianRoadNames.AvenueSpecialNames,

                _ =>
                    ItalianRoadNames.StandardSpecialNames
            };
        }

        private static bool IsItalianRouteNumber(
            string name)
        {
            return
                HasNumberAfterPrefix(name, "A") ||
                HasNumberAfterPrefix(name, "SS") ||
                HasNumberAfterPrefix(name, "SR") ||
                HasNumberAfterPrefix(name, "SP") ||
                HasNumberAfterPrefix(name, "RA") ||
                HasNumberAfterPrefix(name, "E");
        }

        private static bool HasNumberAfterPrefix(
            string name,
            string prefix)
        {
            if (!name.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string numberText =
                name.Substring(prefix.Length).Trim();

            return int.TryParse(
                       numberText,
                       out int routeNumber
                   ) &&
                   routeNumber > 0;
        }

        private static bool ContainsName(
            string[] names,
            string searchedName)
        {
            foreach (string name in names)
            {
                if (string.Equals(
                        name,
                        searchedName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MatchesPrefixAndBase(
            string name,
            string[] prefixes,
            string[] baseNames)
        {
            foreach (string prefix in prefixes)
            {
                foreach (string baseName in baseNames)
                {
                    string allowedName =
                        $"{prefix} {baseName}";

                    if (string.Equals(
                            name,
                            allowedName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsNumberedFallbackName(
            string name)
        {
            string[] prefixes =
            {
                "Strada Vicinale ",
                "Vicolo ",
                "Via ",
                "Viale "
            };

            foreach (string prefix in prefixes)
            {
                if (!name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string numberText =
                    name.Substring(
                        prefix.Length
                    ).Trim();

                if (int.TryParse(
                        numberText,
                        out int number) &&
                    number > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private string GenerateFallbackName(
            RoadCategory category)
        {
            int number = 1;

            while (true)
            {
                string name =
                    category switch
                    {
                        RoadCategory.Dirt =>
                            $"Strada Vicinale {number}",

                        RoadCategory.Alley =>
                            $"Vicolo {number}",

                        RoadCategory.Residential =>
                            $"Via {number}",

                        RoadCategory.Standard =>
                            $"Via {number}",

                        RoadCategory.Avenue =>
                            $"Viale {number}",

                        RoadCategory.Highway =>
                            $"SS {number}",

                        _ =>
                            $"Via {number}"
                    };

                if (m_UsedNames.Add(name))
                {
                    return name;
                }

                number++;
            }
        }
    }
}