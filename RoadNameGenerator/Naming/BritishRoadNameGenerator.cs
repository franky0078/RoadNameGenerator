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
    public sealed class BritishRoadNameGenerator
        : IRoadNameGenerator
    {
        private readonly Random m_Random = new();

        private readonly HashSet<string> m_UsedNames =
            new(StringComparer.OrdinalIgnoreCase);

        private static bool HasDuplicateEnding(
            string baseName,
            string suffix)
        {
            if (string.IsNullOrWhiteSpace(baseName) ||
                string.IsNullOrWhiteSpace(suffix))
            {
                return false;
            }

            string trimmedBaseName = baseName.Trim();
            string trimmedSuffix = suffix.Trim();

            if (string.Equals(
                    trimmedBaseName,
                    trimmedSuffix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return trimmedBaseName.EndsWith(
                " " + trimmedSuffix,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GetCompatibleSuffix(
            string baseName,
            string[] suffixes,
            Random random)
        {
            if (suffixes == null || suffixes.Length == 0)
            {
                return string.Empty;
            }

            const int maxAttempts = 10;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                string suffix =
                    suffixes[random.Next(suffixes.Length)];

                if (!HasDuplicateEnding(baseName, suffix))
                {
                    return suffix;
                }
            }

            foreach (string suffix in suffixes)
            {
                if (!HasDuplicateEnding(baseName, suffix))
                {
                    return suffix;
                }
            }

            return suffixes[0];
        }

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

            if (IsBritishRouteNumber(trimmedName))
            {
                return true;
            }

            if (ContainsName(
                    BritishRoadNames.NamedHighwayNames,
                    trimmedName))
            {
                return true;
            }

            if (ContainsName(
                    BritishRoadNames.ResidentialSpecialNames,
                    trimmedName) ||
                ContainsName(
                    BritishRoadNames.StandardSpecialNames,
                    trimmedName) ||
                ContainsName(
                    BritishRoadNames.AvenueSpecialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesBaseAndSuffix(
                    trimmedName,
                    BritishRoadNames.DirtBaseNames,
                    BritishRoadNames.DirtSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    BritishRoadNames.AlleyBaseNames,
                    BritishRoadNames.AlleySuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    BritishRoadNames.GeneralBaseNames,
                    BritishRoadNames.ResidentialSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    BritishRoadNames.GeneralBaseNames,
                    BritishRoadNames.StandardSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    BritishRoadNames.GeneralBaseNames,
                    BritishRoadNames.AvenueSuffixes
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
                    m_Random.Next(100) < 15)
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

                    result = string.IsNullOrWhiteSpace(suffix)
                        ? baseName
                        : $"{baseName} {suffix}";
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

                    if (routeType < 60)
                    {

                        // A-Straßen sind am häufigsten.
                        int number =
                            m_Random.Next(1, 1000);

                        result = $"A{number}";
                    }
                    else if (routeType < 95)
                    {

                        // Motorways.
                        int number =
                            m_Random.Next(1, 100);

                        result = $"M{number}";
                    }
                    else
                    {
                        // Sonderform wie A1(M).
                        int number =
                            m_Random.Next(1, 100);

                        result = $"A{number}(M)";
                    }
                }
                else
                {
                    result =
                        BritishRoadNames.NamedHighwayNames[
                            m_Random.Next(
                                BritishRoadNames.NamedHighwayNames.Length
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
                    BritishRoadNames.DirtBaseNames,

                RoadCategory.Alley =>
                    BritishRoadNames.AlleyBaseNames,

                _ =>
                    BritishRoadNames.GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    BritishRoadNames.DirtSuffixes,

                RoadCategory.Alley =>
                    BritishRoadNames.AlleySuffixes,

                RoadCategory.Residential =>
                    BritishRoadNames.ResidentialSuffixes,

                RoadCategory.Standard =>
                    BritishRoadNames.StandardSuffixes,

                RoadCategory.Avenue =>
                    BritishRoadNames.AvenueSuffixes,

                _ =>
                    BritishRoadNames.StandardSuffixes
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Residential =>
                    BritishRoadNames.ResidentialSpecialNames,

                RoadCategory.Standard =>
                    BritishRoadNames.StandardSpecialNames,

                RoadCategory.Avenue =>
                    BritishRoadNames.AvenueSpecialNames,

                _ =>
                    Array.Empty<string>()
            };
        }

        private static bool IsBritishRouteNumber(
            string name)
        {

            // M1, M25, A1 oder A303.
            if (name.Length >= 2 &&
                (name[0] == 'A' ||
                 name[0] == 'a' ||
                 name[0] == 'M' ||
                 name[0] == 'm'))
            {
                string numberText =
                    name.Substring(1);

                if (int.TryParse(
                        numberText,
                        out int routeNumber) &&
                    routeNumber > 0)
                {
                    return true;
                }
            }

            // A1(M), A38(M) usw.
            if (name.StartsWith(
                    "A",
                    StringComparison.OrdinalIgnoreCase) &&
                name.EndsWith(
                    "(M)",
                    StringComparison.OrdinalIgnoreCase))
            {
                string numberText =
                    name.Substring(
                        1,
                        name.Length - 4
                    );

                return int.TryParse(
                           numberText,
                           out int routeNumber
                       ) &&
                       routeNumber > 0;
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
                string ending = $" {suffix}";

                if (!name.EndsWith(
                        ending,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string baseName =
                    name.Substring(
                            0,
                            name.Length - ending.Length
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
                string name = category switch
                {
                    RoadCategory.Dirt =>
                        $"{GetOrdinal(number)} Track",

                    RoadCategory.Alley =>
                        $"{GetOrdinal(number)} Mews",

                    RoadCategory.Residential =>
                        $"{GetOrdinal(number)} Close",

                    RoadCategory.Standard =>
                        $"{GetOrdinal(number)} Street",

                    RoadCategory.Avenue =>
                        $"{GetOrdinal(number)} Avenue",

                    RoadCategory.Highway =>
                        $"A{number}",

                    _ =>
                        $"{GetOrdinal(number)} Street"
                };

                if (m_UsedNames.Add(name))
                {
                    return name;
                }

                number++;
            }
        }

        private static string GetOrdinal(int number)
        {
            int lastTwoDigits =
                number % 100;

            if (lastTwoDigits is >= 11 and <= 13)
            {
                return $"{number}th";
            }

            return (number % 10) switch
            {
                1 => $"{number}st",
                2 => $"{number}nd",
                3 => $"{number}rd",
                _ => $"{number}th"
            };
        }
    }
}