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
    public sealed class SwedishRoadNameGenerator
        : IRoadNameGenerator
    {
        private readonly Random m_Random = new();
        private readonly HashSet<string> m_UsedNames =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly int[] s_EuropeanRouteNumbers =
        {
            4, 6, 10, 12, 14, 16, 18, 20, 22, 45, 65
        };

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

            if (category == RoadCategory.Highway)
            {
                return IsSwedishRouteNumber(trimmedName) ||
                       ContainsName(
                           SwedishRoadNames.NamedHighwayNames,
                           trimmedName
                       ) ||
                       IsNumberedFallbackName(
                           trimmedName,
                           "Riksväg "
                       );
            }

            return ContainsName(
                       GetNames(category),
                       trimmedName
                   ) ||
                   IsRegularFallbackName(
                       trimmedName,
                       category
                   );
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
            string[] names = GetNames(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result =
                    names[m_Random.Next(names.Length)];

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
             * 70 % numbered European, national or county roads
             * 30 % named major roads
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType = m_Random.Next(100);

                    if (routeType < 25)
                    {
                        int routeNumber =
                            s_EuropeanRouteNumbers[
                                m_Random.Next(
                                    s_EuropeanRouteNumbers.Length
                                )
                            ];

                        result = $"E{routeNumber}";
                    }
                    else if (routeType < 60)
                    {
                        result =
                            $"Riksväg {m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 90)
                    {
                        result =
                            $"Länsväg {m_Random.Next(100, 1000)}";
                    }
                    else
                    {
                        result =
                            $"Väg {m_Random.Next(1, 1000)}";
                    }
                }
                else
                {
                    result =
                        SwedishRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                SwedishRoadNames
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

        private static string[] GetNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    SwedishRoadNames.DirtNames,

                RoadCategory.Alley =>
                    SwedishRoadNames.AlleyNames,

                RoadCategory.Residential =>
                    SwedishRoadNames.ResidentialNames,

                RoadCategory.Standard =>
                    SwedishRoadNames.StandardNames,

                RoadCategory.Avenue =>
                    SwedishRoadNames.AvenueNames,

                _ =>
                    SwedishRoadNames.StandardNames
            };
        }

        private static bool IsSwedishRouteNumber(
            string name)
        {
            if (name.StartsWith(
                    "E",
                    StringComparison.OrdinalIgnoreCase))
            {
                string europeanNumber =
                    name.Substring(1).Trim();

                if (int.TryParse(
                        europeanNumber,
                        out int routeNumber) &&
                    routeNumber > 0)
                {
                    return true;
                }
            }

            return IsNumberedFallbackName(
                       name,
                       "Riksväg "
                   ) ||
                   IsNumberedFallbackName(
                       name,
                       "Länsväg "
                   ) ||
                   IsNumberedFallbackName(
                       name,
                       "Väg "
                   );
        }

        private static bool IsRegularFallbackName(
            string name,
            RoadCategory category)
        {
            string prefix = category switch
            {
                RoadCategory.Dirt =>
                    "Skogsväg ",

                RoadCategory.Alley =>
                    "Gränd ",

                RoadCategory.Residential =>
                    "Gata ",

                RoadCategory.Standard =>
                    "Väg ",

                RoadCategory.Avenue =>
                    "Allé ",

                _ =>
                    "Väg "
            };

            return IsNumberedFallbackName(
                name,
                prefix
            );
        }

        private static bool IsNumberedFallbackName(
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
                       out int number
                   ) &&
                   number > 0;
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

        private string GenerateFallbackName(
            RoadCategory category)
        {
            int number = 1;

            while (true)
            {
                string name = category switch
                {
                    RoadCategory.Dirt =>
                        $"Skogsväg {number}",

                    RoadCategory.Alley =>
                        $"Gränd {number}",

                    RoadCategory.Residential =>
                        $"Gata {number}",

                    RoadCategory.Standard =>
                        $"Väg {number}",

                    RoadCategory.Avenue =>
                        $"Allé {number}",

                    RoadCategory.Highway =>
                        $"Riksväg {number}",

                    _ =>
                        $"Väg {number}"
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
