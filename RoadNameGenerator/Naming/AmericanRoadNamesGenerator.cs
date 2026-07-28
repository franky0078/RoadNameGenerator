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

    public sealed class AmericanRoadNameGenerator : IRoadNameGenerator
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
            System.Random random)
        {
            if (suffixes == null || suffixes.Length == 0)
            {
                return string.Empty;
            }

            const int maxAttempts = 10;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
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

        private string GenerateRegularRoadName(RoadCategory category)
        {
            string[] baseNames = GetBaseNames(category);
            string[] suffixes = GetSuffixes(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string baseName =
                    baseNames[m_Random.Next(baseNames.Length)];

                string suffix = GetCompatibleSuffix(
                    baseName,
                    suffixes,
                    m_Random
                );

                string result = $"{baseName} {suffix}";

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateNumberedRoad(category);
        }

        private string GenerateHighwayName()
        {
            /*
             * Fünf Varianten:
             *
             * 0 = Interstate
             * 1 = US Route
             * 2 = State Route
             * 3–4 = benannte Autobahn
             *
            * Verteilung:
             *
             * 70 % nummerierte Autobahnen:
            * - Interstate
             * - US Route
             * - State Route
             *
             * 30 % benannte Autobahnen:
             * - Lincoln Freeway
             * - Pacific Expressway
             * - Veterans Memorial Highway
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                int styleChance = m_Random.Next(100);

                string result;

                if (styleChance < 70)
                {
                    int routeType = m_Random.Next(3);

                    switch (routeType)
                    {
                        case 0:
                            {
                                int number = m_Random.Next(2, 100);
                                result = $"Interstate {number}";
                                break;
                            }

                        case 1:
                            {
                                int number = m_Random.Next(1, 200);
                                result = $"US Route {number}";
                                break;
                            }

                        default:
                            {
                                int number = m_Random.Next(1, 300);
                                result = $"State Route {number}";
                                break;
                            }
                    }
                }
                else
                {

                    // Benannte Autobahn.

                    string baseName =
                        AmericanRoadNames.HighwayBaseNames[
                            m_Random.Next(AmericanRoadNames.HighwayBaseNames.Length)
                        ];

                    string suffix = GetCompatibleSuffix(
                        baseName,
                        AmericanRoadNames.HighwaySuffixes,
                        m_Random
                    );

                    result = $"{baseName} {suffix}";
                }

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateNumberedRoad(RoadCategory.Highway);
        }

        private static string[] GetBaseNames(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => AmericanRoadNames.DirtBaseNames,
                RoadCategory.Alley => AmericanRoadNames.AlleyBaseNames,
                _ => AmericanRoadNames.GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => AmericanRoadNames.DirtSuffixes,
                RoadCategory.Alley => AmericanRoadNames.AlleySuffixes,
                RoadCategory.Residential => AmericanRoadNames.ResidentialSuffixes,
                RoadCategory.Standard => AmericanRoadNames.StandardSuffixes,
                RoadCategory.Avenue => AmericanRoadNames.AvenueSuffixes,
                RoadCategory.Highway => AmericanRoadNames.HighwaySuffixes,
                _ => AmericanRoadNames.StandardSuffixes
            };
        }

        public void RegisterExistingName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            m_UsedNames.Add(name.Trim());
        }

        public bool IsNameFromPortfolio(string name, RoadCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string trimmedName = name.Trim();

            if (category == RoadCategory.Highway)
            {

                if (IsNumberedHighway(trimmedName))
                {
                    return true;
                }

                if (MatchesBaseAndSuffix(
                        trimmedName,
                        AmericanRoadNames.HighwayBaseNames,
                        AmericanRoadNames.HighwaySuffixes))
                {
                    return true;
                }

                return MatchesBaseAndSuffix(
                    trimmedName,
                    AmericanRoadNames.GeneralBaseNames,
                    AmericanRoadNames.HighwaySuffixes
                );
            }

            string[] baseNames = GetBaseNames(category);
            string[] suffixes = GetSuffixes(category);

            return MatchesBaseAndSuffix(
                trimmedName,
                baseNames,
                suffixes
            );
        }

        private static bool IsNumberedHighway(string name)
        {
            return HasNumberAfterPrefix(
                       name,
                       "Interstate "
                   ) ||
                   HasNumberAfterPrefix(
                       name,
                       "US Route "
                   ) ||
                   HasNumberAfterPrefix(
                       name,
                       "State Route "
                   );
        }

        private static bool HasNumberAfterPrefix(string name, string prefix)
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

        private static bool MatchesBaseAndSuffix(string name, string[] baseNames, string[] suffixes)
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

                string baseName = name
                    .Substring(
                        0,
                        name.Length - ending.Length
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

        private string GenerateNumberedRoad(RoadCategory category)
        {
            int number = 1;

            while (true)
            {
                string name = category switch
                {
                    RoadCategory.Dirt =>
                        $"{GetOrdinal(number)} Trail",

                    RoadCategory.Alley =>
                        $"{GetOrdinal(number)} " +
                        AmericanRoadNames.AlleySuffixes[
                            (number - 1) % AmericanRoadNames.AlleySuffixes.Length
                        ],

                    RoadCategory.Residential =>
                        $"{GetOrdinal(number)} Street",

                    RoadCategory.Standard =>
                        $"{GetOrdinal(number)} Street",

                    RoadCategory.Avenue =>
                        $"{GetOrdinal(number)} Avenue",

                    RoadCategory.Highway =>
                        $"State Route {number}",

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
            int lastTwoDigits = number % 100;

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