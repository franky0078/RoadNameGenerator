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
    public sealed class FrenchRoadNameGenerator
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

            if (IsFrenchRouteNumber(trimmedName) ||
                ContainsName(
                    FrenchRoadNames.NamedHighwayNames,
                    trimmedName))
            {
                return true;
            }

            if (ContainsName(
                    FrenchRoadNames.DirtSpecialNames,
                    trimmedName) ||
                ContainsName(
                    FrenchRoadNames.AlleySpecialNames,
                    trimmedName) ||
                ContainsName(
                    FrenchRoadNames.ResidentialSpecialNames,
                    trimmedName) ||
                ContainsName(
                    FrenchRoadNames.StandardSpecialNames,
                    trimmedName) ||
                ContainsName(
                    FrenchRoadNames.AvenueSpecialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesPrefixAndBase(
                    trimmedName,
                    FrenchRoadNames.DirtPrefixes,
                    FrenchRoadNames.DirtBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    FrenchRoadNames.AlleyPrefixes,
                    FrenchRoadNames.AlleyBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    FrenchRoadNames.ResidentialPrefixes,
                    FrenchRoadNames.ResidentialBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    FrenchRoadNames.StandardPrefixes,
                    FrenchRoadNames.StandardBaseNames
                ) ||
                MatchesPrefixAndBase(
                    trimmedName,
                    FrenchRoadNames.AvenuePrefixes,
                    FrenchRoadNames.AvenueBaseNames
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
             * Distribution:
             * 70 % numbered roads
             * 30 % named motorways, ring roads or expressways
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType =
                        m_Random.Next(100);

                    if (routeType < 38)
                    {
                        // Autoroute: A1, A6, A71, etc.
                        result =
                            $"A{m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 64)
                    {
                        // Route nationale: N7, N10, N165, etc.
                        result =
                            $"N{m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 90)
                    {
                        // Route départementale: D1, D938, etc.
                        result =
                            $"D{m_Random.Next(1, 10000)}";
                    }
                    else
                    {
                        // European route: E5, E15, E60, etc.
                        result =
                            $"E{m_Random.Next(1, 100)}";
                    }
                }
                else
                {
                    result =
                        FrenchRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                FrenchRoadNames
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
                    FrenchRoadNames.DirtPrefixes,

                RoadCategory.Alley =>
                    FrenchRoadNames.AlleyPrefixes,

                RoadCategory.Residential =>
                    FrenchRoadNames.ResidentialPrefixes,

                RoadCategory.Standard =>
                    FrenchRoadNames.StandardPrefixes,

                RoadCategory.Avenue =>
                    FrenchRoadNames.AvenuePrefixes,

                _ =>
                    FrenchRoadNames.StandardPrefixes
            };
        }

        private static string[] GetBaseNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    FrenchRoadNames.DirtBaseNames,

                RoadCategory.Alley =>
                    FrenchRoadNames.AlleyBaseNames,

                RoadCategory.Residential =>
                    FrenchRoadNames.ResidentialBaseNames,

                RoadCategory.Standard =>
                    FrenchRoadNames.StandardBaseNames,

                RoadCategory.Avenue =>
                    FrenchRoadNames.AvenueBaseNames,

                _ =>
                    FrenchRoadNames.StandardBaseNames
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    FrenchRoadNames.DirtSpecialNames,

                RoadCategory.Alley =>
                    FrenchRoadNames.AlleySpecialNames,

                RoadCategory.Residential =>
                    FrenchRoadNames.ResidentialSpecialNames,

                RoadCategory.Standard =>
                    FrenchRoadNames.StandardSpecialNames,

                RoadCategory.Avenue =>
                    FrenchRoadNames.AvenueSpecialNames,

                _ =>
                    FrenchRoadNames.StandardSpecialNames
            };
        }

        private static bool IsFrenchRouteNumber(
            string name)
        {
            return
                HasNumberAfterPrefix(name, "A") ||
                HasNumberAfterPrefix(name, "N") ||
                HasNumberAfterPrefix(name, "D") ||
                HasNumberAfterPrefix(name, "RN") ||
                HasNumberAfterPrefix(name, "RD") ||
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
                "Chemin ",
                "Impasse ",
                "Rue ",
                "Avenue "
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
                            $"Chemin {number}",

                        RoadCategory.Alley =>
                            $"Impasse {number}",

                        RoadCategory.Residential =>
                            $"Rue {number}",

                        RoadCategory.Standard =>
                            $"Rue {number}",

                        RoadCategory.Avenue =>
                            $"Avenue {number}",

                        RoadCategory.Highway =>
                            $"N{number}",

                        _ =>
                            $"Rue {number}"
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
