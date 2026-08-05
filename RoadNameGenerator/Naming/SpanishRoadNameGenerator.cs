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
    public sealed class SpanishRoadNameGenerator
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

            if (category == RoadCategory.Highway)
            {
                return IsSpanishRouteNumber(trimmedName) ||
                       ContainsName(
                           SpanishRoadNames.NamedHighwayNames,
                           trimmedName
                       ) ||
                       IsNumberedFallbackName(
                           trimmedName,
                           "N-"
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
             * 70 % numbered motorways and national or regional roads
             * 30 % named motorways and ring roads
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType = m_Random.Next(100);

                    if (routeType < 28)
                    {
                        result =
                            $"A-{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 43)
                    {
                        result =
                            $"AP-{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 68)
                    {
                        result =
                            $"N-{m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 80)
                    {
                        result =
                            $"E-{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 90)
                    {
                        result =
                            $"M-{m_Random.Next(1, 1000)}";
                    }
                    else
                    {
                        result =
                            $"CV-{m_Random.Next(1, 1000)}";
                    }
                }
                else
                {
                    result =
                        SpanishRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                SpanishRoadNames
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
                    SpanishRoadNames.DirtNames,

                RoadCategory.Alley =>
                    SpanishRoadNames.AlleyNames,

                RoadCategory.Residential =>
                    SpanishRoadNames.ResidentialNames,

                RoadCategory.Standard =>
                    SpanishRoadNames.StandardNames,

                RoadCategory.Avenue =>
                    SpanishRoadNames.AvenueNames,

                _ =>
                    SpanishRoadNames.StandardNames
            };
        }

        private static bool IsSpanishRouteNumber(
            string name)
        {
            string[] prefixes =
            {
                "A-",
                "AP-",
                "N-",
                "E-",
                "M-",
                "CV-",
                "CA-",
                "SE-",
                "B-",
                "C-"
            };

            foreach (string prefix in prefixes)
            {
                if (IsNumberedFallbackName(
                        name,
                        prefix))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRegularFallbackName(
            string name,
            RoadCategory category)
        {
            string prefix = category switch
            {
                RoadCategory.Dirt =>
                    "Camino Rural ",

                RoadCategory.Alley =>
                    "Callejón ",

                RoadCategory.Residential =>
                    "Calle ",

                RoadCategory.Standard =>
                    "Carretera ",

                RoadCategory.Avenue =>
                    "Avenida ",

                _ =>
                    "Calle "
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
                        $"Camino Rural {number}",

                    RoadCategory.Alley =>
                        $"Callejón {number}",

                    RoadCategory.Residential =>
                        $"Calle {number}",

                    RoadCategory.Standard =>
                        $"Carretera {number}",

                    RoadCategory.Avenue =>
                        $"Avenida {number}",

                    RoadCategory.Highway =>
                        $"N-{number}",

                    _ =>
                        $"Calle {number}"
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
