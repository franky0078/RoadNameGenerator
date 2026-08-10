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
    public sealed class DutchRoadNameGenerator : IRoadNameGenerator
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

        public void RegisterExistingName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            m_UsedNames.Add(name.Trim());
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
                return ContainsName(
                           DutchRoadNames.HighwayRouteNames,
                           trimmedName
                       ) ||
                       IsNumberedFallbackName(
                           trimmedName,
                           category
                       );
            }

            string[] specialNames =
                GetSpecialNames(category);

            if (ContainsName(
                    specialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesBaseAndSuffix(
                    trimmedName,
                    GetBaseNames(category),
                    GetSuffixes(category)
                ) ||
                IsNumberedFallbackName(
                    trimmedName,
                    category
                );
        }

        private string GenerateRegularRoadName(
            RoadCategory category)
        {
            string[] baseNames =
                GetBaseNames(category);

            string[] suffixes =
                GetSuffixes(category);

            string[] specialNames =
                GetSpecialNames(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                // Bekannte und bereits sprachlich fertige Namen
                // werden bevorzugt, damit das Portfolio natürlich wirkt.
                if (specialNames.Length > 0 &&
                    m_Random.Next(100) < 55)
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
                    string baseName =
                        baseNames[
                            m_Random.Next(
                                baseNames.Length
                            )
                        ];

                    string suffix =
                        GetCompatibleSuffix(
                            baseName,
                            suffixes,
                            m_Random
                        );

                    result =
                        ComposeDutchRoadName(
                            baseName,
                            suffix
                        );
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
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result =
                    DutchRoadNames.HighwayRouteNames[
                        m_Random.Next(
                            DutchRoadNames.HighwayRouteNames.Length
                        )
                    ];

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateFallbackName(
                RoadCategory.Highway
            );
        }

        private static string[] GetBaseNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    DutchRoadNames.DirtBaseNames,

                RoadCategory.Alley =>
                    DutchRoadNames.AlleyBaseNames,

                RoadCategory.Residential =>
                    DutchRoadNames.ResidentialBaseNames,

                RoadCategory.Standard =>
                    DutchRoadNames.StandardBaseNames,

                RoadCategory.Avenue =>
                    DutchRoadNames.AvenueBaseNames,

                _ =>
                    DutchRoadNames.StandardBaseNames
            };
        }

        private static string[] GetSuffixes(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    DutchRoadNames.DirtSuffixes,

                RoadCategory.Alley =>
                    DutchRoadNames.AlleySuffixes,

                RoadCategory.Residential =>
                    DutchRoadNames.ResidentialSuffixes,

                RoadCategory.Standard =>
                    DutchRoadNames.StandardSuffixes,

                RoadCategory.Avenue =>
                    DutchRoadNames.AvenueSuffixes,

                _ =>
                    DutchRoadNames.StandardSuffixes
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    DutchRoadNames.DirtSpecialNames,

                RoadCategory.Alley =>
                    DutchRoadNames.AlleySpecialNames,

                RoadCategory.Residential =>
                    DutchRoadNames.ResidentialSpecialNames,

                RoadCategory.Standard =>
                    DutchRoadNames.StandardSpecialNames,

                RoadCategory.Avenue =>
                    DutchRoadNames.AvenueSpecialNames,

                _ =>
                    DutchRoadNames.StandardSpecialNames
            };
        }

        private static string ComposeDutchRoadName(
            string baseName,
            string suffix)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                return baseName.Trim();
            }

            // Niederländische Straßentypen werden in der Regel
            // direkt an den Namensstamm angefügt.
            return
                baseName.Trim() +
                suffix.Trim();
        }

        private static bool HasDuplicateEnding(
            string baseName,
            string suffix)
        {
            if (string.IsNullOrWhiteSpace(baseName) ||
                string.IsNullOrWhiteSpace(suffix))
            {
                return false;
            }

            string trimmedBaseName =
                baseName.Trim();

            string trimmedSuffix =
                suffix.Trim();

            return
                trimmedBaseName.EndsWith(
                    trimmedSuffix,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        private static string GetCompatibleSuffix(
            string baseName,
            string[] suffixes,
            Random random)
        {
            if (suffixes == null ||
                suffixes.Length == 0)
            {
                return string.Empty;
            }

            const int maxAttempts = 10;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                string suffix =
                    suffixes[
                        random.Next(
                            suffixes.Length
                        )
                    ];

                if (!HasDuplicateEnding(
                        baseName,
                        suffix))
                {
                    return suffix;
                }
            }

            foreach (string suffix in suffixes)
            {
                if (!HasDuplicateEnding(
                        baseName,
                        suffix))
                {
                    return suffix;
                }
            }

            return suffixes[0];
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

        private static bool MatchesBaseAndSuffix(
            string name,
            string[] baseNames,
            string[] suffixes)
        {
            foreach (string suffix in suffixes)
            {
                if (!name.EndsWith(
                        suffix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string baseName =
                    name.Substring(
                            0,
                            name.Length - suffix.Length
                        )
                        .Trim();

                foreach (string allowedBaseName in baseNames)
                {
                    if (string.Equals(
                            baseName,
                            allowedBaseName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsNumberedFallbackName(
            string name,
            RoadCategory category)
        {
            string prefix = category switch
            {
                RoadCategory.Dirt => "Landweg ",
                RoadCategory.Alley => "Steeg ",
                RoadCategory.Residential => "Straat ",
                RoadCategory.Standard => "Straat ",
                RoadCategory.Avenue => "Laan ",
                RoadCategory.Highway => "A",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(prefix) ||
                !name.StartsWith(
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

        private string GenerateFallbackName(
            RoadCategory category)
        {
            int number = 1;

            while (true)
            {
                string name = category switch
                {
                    RoadCategory.Dirt =>
                        $"Landweg {number}",

                    RoadCategory.Alley =>
                        $"Steeg {number}",

                    RoadCategory.Residential =>
                        $"Straat {number}",

                    RoadCategory.Standard =>
                        $"Straat {number}",

                    RoadCategory.Avenue =>
                        $"Laan {number}",

                    RoadCategory.Highway =>
                        $"A{number}",

                    _ =>
                        $"Straat {number}"
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
