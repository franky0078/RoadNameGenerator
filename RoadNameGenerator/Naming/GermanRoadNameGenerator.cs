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
    public sealed class GermanRoadNameGenerator
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
                return IsGermanRouteNumber(trimmedName) ||
                       ContainsName(
                           GermanRoadNames.NamedHighwayNames,
                           trimmedName
                       );
            }

            if (ContainsName(
                    GermanRoadNames.DirtSpecialNames,
                    trimmedName) ||
                ContainsName(
                    GermanRoadNames.AlleySpecialNames,
                    trimmedName) ||
                ContainsName(
                    GermanRoadNames.ResidentialSpecialNames,
                    trimmedName) ||
                ContainsName(
                    GermanRoadNames.StandardSpecialNames,
                    trimmedName) ||
                ContainsName(
                    GermanRoadNames.AvenueSpecialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesBaseAndSuffix(
                    trimmedName,
                    GermanRoadNames.DirtBaseNames,
                    GermanRoadNames.DirtSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    GermanRoadNames.AlleyBaseNames,
                    GermanRoadNames.AlleySuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    GermanRoadNames.ResidentialBaseNames,
                    GermanRoadNames.ResidentialSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    GermanRoadNames.StandardBaseNames,
                    GermanRoadNames.StandardSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    GermanRoadNames.AvenueBaseNames,
                    GermanRoadNames.AvenueSuffixes
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
            string[] baseNames =
                GetBaseNames(category);

            string[] suffixes =
                GetSuffixes(category);

            string[] specialNames =
                GetSpecialNames(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (specialNames.Length > 0 &&
                    m_Random.Next(100) < 45)
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
                        ComposeGermanRoadName(
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
            /* Verteilung:
            * 70 % Routennummern
            * 30 % benannte Fernstraßen
            */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType =
                        m_Random.Next(100);

                    if (routeType < 50)
                    {

                        // Autobahnen.

                        result =
                            $"A{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 82)
                    {

                        // Bundesstraßen.

                        result =
                            $"B{m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 95)
                    {

                        // Landstraßen.

                        result =
                            $"L{m_Random.Next(1, 1000)}";
                    }
                    else
                    {

                        // Kreisstraßen.

                        result =
                            $"K{m_Random.Next(1, 1000)}";
                    }
                }
                else
                {
                    result =
                        GermanRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                GermanRoadNames.NamedHighwayNames.Length
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

        private static string[] GetBaseNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    GermanRoadNames.DirtBaseNames,

                RoadCategory.Alley =>
                    GermanRoadNames.AlleyBaseNames,

                RoadCategory.Residential =>
                    GermanRoadNames.ResidentialBaseNames,

                RoadCategory.Standard =>
                    GermanRoadNames.StandardBaseNames,

                RoadCategory.Avenue =>
                    GermanRoadNames.AvenueBaseNames,

                _ =>
                    GermanRoadNames.StandardBaseNames
            };
        }

        private static string[] GetSuffixes(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    GermanRoadNames.DirtSuffixes,

                RoadCategory.Alley =>
                    GermanRoadNames.AlleySuffixes,

                RoadCategory.Residential =>
                    GermanRoadNames.ResidentialSuffixes,

                RoadCategory.Standard =>
                    GermanRoadNames.StandardSuffixes,

                RoadCategory.Avenue =>
                    GermanRoadNames.AvenueSuffixes,

                _ =>
                    GermanRoadNames.StandardSuffixes
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    GermanRoadNames.DirtSpecialNames,

                RoadCategory.Alley =>
                    GermanRoadNames.AlleySpecialNames,

                RoadCategory.Residential =>
                    GermanRoadNames.ResidentialSpecialNames,

                RoadCategory.Standard =>
                    GermanRoadNames.StandardSpecialNames,

                RoadCategory.Avenue =>
                    GermanRoadNames.AvenueSpecialNames,

                _ =>
                    GermanRoadNames.StandardSpecialNames
            };
        }

        private static string ComposeGermanRoadName(
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

            return baseName.Trim() + suffix.Trim();
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

            if (string.Equals(
                    trimmedBaseName,
                    trimmedSuffix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return
                trimmedBaseName.EndsWith(
                    trimmedSuffix,
                    StringComparison.OrdinalIgnoreCase
                ) ||
                trimmedBaseName.EndsWith(
                    " " + trimmedSuffix,
                    StringComparison.OrdinalIgnoreCase
                ) ||
                trimmedBaseName.EndsWith(
                    "-" + trimmedSuffix,
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

        private static bool IsGermanRouteNumber(
            string name)
        {
            if (name.Length < 2)
            {
                return false;
            }

            char prefix =
                char.ToUpperInvariant(
                    name[0]
                );

            if (prefix != 'A' &&
                prefix != 'B' &&
                prefix != 'L' &&
                prefix != 'K')
            {
                return false;
            }

            string numberText =
                name.Substring(1).Trim();

            return int.TryParse(
                       numberText,
                       out int routeNumber
                   ) &&
                   routeNumber > 0;
        }

        private static bool IsNumberedFallbackName(
            string name)
        {
            string[] prefixes =
            {
                "Weg ",
                "Gasse ",
                "Straße ",
                "Allee "
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

                foreach (
                    string allowedBaseName in baseNames)
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
                            $"Weg {number}",

                        RoadCategory.Alley =>
                            $"Gasse {number}",

                        RoadCategory.Residential =>
                            $"Weg {number}",

                        RoadCategory.Standard =>
                            $"Straße {number}",

                        RoadCategory.Avenue =>
                            $"Allee {number}",

                        RoadCategory.Highway =>
                            $"A{number}",

                        _ =>
                            $"Straße {number}"
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
