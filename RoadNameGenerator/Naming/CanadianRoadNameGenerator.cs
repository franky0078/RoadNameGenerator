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
    public sealed class CanadianRoadNameGenerator : IRoadNameGenerator
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
             * Verteilung:
             * 70 % nummerierte Routen
             * 20 % bekannte kanadische Highway-Namen
             * 10 % generierte benannte Highways
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                int styleChance = m_Random.Next(100);
                string result;

                if (styleChance < 70)
                {
                    int routeType = m_Random.Next(100);

                    if (routeType < 65)
                    {
                        result = $"Highway {m_Random.Next(1, 1000)}";
                    }
                    else if (routeType < 90)
                    {
                        result = $"Route {m_Random.Next(1, 1000)}";
                    }
                    else
                    {
                        // Ein kleiner Quebec-Anteil im gesamtkanadischen Portfolio.
                        result = $"Autoroute {m_Random.Next(1, 1000)}";
                    }
                }
                else if (styleChance < 90)
                {
                    result =
                        CanadianRoadNames.NamedHighwayNames[
                            m_Random.Next(CanadianRoadNames.NamedHighwayNames.Length)
                        ];
                }
                else
                {
                    string baseName =
                        CanadianRoadNames.HighwayBaseNames[
                            m_Random.Next(CanadianRoadNames.HighwayBaseNames.Length)
                        ];

                    string suffix = GetCompatibleSuffix(
                        baseName,
                        CanadianRoadNames.HighwaySuffixes,
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
                return IsNumberedHighway(trimmedName) ||
                       ContainsName(
                           CanadianRoadNames.NamedHighwayNames,
                           trimmedName
                       ) ||
                       MatchesBaseAndSuffix(
                           trimmedName,
                           CanadianRoadNames.HighwayBaseNames,
                           CanadianRoadNames.HighwaySuffixes
                       );
            }

            string[] baseNames = GetBaseNames(category);
            string[] suffixes = GetSuffixes(category);

            return MatchesBaseAndSuffix(
                       trimmedName,
                       baseNames,
                       suffixes
                   ) ||
                   IsNumberedFallbackName(trimmedName, category);
        }

        private static string[] GetBaseNames(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => CanadianRoadNames.DirtBaseNames,
                RoadCategory.Alley => CanadianRoadNames.AlleyBaseNames,
                _ => CanadianRoadNames.GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => CanadianRoadNames.DirtSuffixes,
                RoadCategory.Alley => CanadianRoadNames.AlleySuffixes,
                RoadCategory.Residential => CanadianRoadNames.ResidentialSuffixes,
                RoadCategory.Standard => CanadianRoadNames.StandardSuffixes,
                RoadCategory.Avenue => CanadianRoadNames.AvenueSuffixes,
                RoadCategory.Highway => CanadianRoadNames.HighwaySuffixes,
                _ => CanadianRoadNames.StandardSuffixes
            };
        }

        private static bool HasDuplicateEnding(string baseName, string suffix)
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
                StringComparison.OrdinalIgnoreCase
            );
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

        private static bool IsNumberedHighway(string name)
        {
            return HasNumberAfterPrefix(name, "Highway ") ||
                   HasNumberAfterPrefix(name, "Route ") ||
                   HasNumberAfterPrefix(name, "Autoroute ");
        }

        private static bool HasNumberAfterPrefix(string name, string prefix)
        {
            if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string numberText =
                name.Substring(prefix.Length).Trim();

            return int.TryParse(numberText, out int routeNumber) &&
                   routeNumber > 0;
        }

        private static bool ContainsName(string[] names, string searchedName)
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

                string baseName = name
                    .Substring(0, name.Length - ending.Length)
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
            string suffix = category switch
            {
                RoadCategory.Dirt => " Trail",
                RoadCategory.Alley => " Lane",
                RoadCategory.Residential => " Street",
                RoadCategory.Standard => " Street",
                RoadCategory.Avenue => " Avenue",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(suffix) ||
                !name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string ordinal = name
                .Substring(0, name.Length - suffix.Length)
                .Trim();

            return IsOrdinal(ordinal);
        }

        private static bool IsOrdinal(string value)
        {
            if (value.Length < 3)
            {
                return false;
            }

            string lower = value.ToLowerInvariant();

            string numberText;

            if (lower.EndsWith("st") ||
                lower.EndsWith("nd") ||
                lower.EndsWith("rd") ||
                lower.EndsWith("th"))
            {
                numberText = value.Substring(0, value.Length - 2);
            }
            else
            {
                return false;
            }

            return int.TryParse(numberText, out int number) && number > 0;
        }

        private string GenerateNumberedRoad(RoadCategory category)
        {
            int number = 1;

            while (true)
            {
                string name = category switch
                {
                    RoadCategory.Dirt => $"{GetOrdinal(number)} Trail",
                    RoadCategory.Alley => $"{GetOrdinal(number)} Lane",
                    RoadCategory.Residential => $"{GetOrdinal(number)} Street",
                    RoadCategory.Standard => $"{GetOrdinal(number)} Street",
                    RoadCategory.Avenue => $"{GetOrdinal(number)} Avenue",
                    RoadCategory.Highway => $"Highway {number}",
                    _ => $"{GetOrdinal(number)} Street"
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
