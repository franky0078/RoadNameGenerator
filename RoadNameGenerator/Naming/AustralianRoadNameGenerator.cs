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
    public sealed class AustralianRoadNameGenerator : IRoadNameGenerator
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
             * 70 % alphanumerische Routennummern
             * 30 % bekannte australische Highway-/Freeway-Namen
             *
             * M = wichtige Motorway-Routen
             * A = wichtige Hauptverbindungen
             * B = regionale Verbindungen
             * C = weitere regionale Routen (in einigen Bundesstaaten)
             */
            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;

                if (m_Random.Next(100) < 70)
                {
                    int routeType = m_Random.Next(100);

                    if (routeType < 35)
                    {
                        result = $"M{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 65)
                    {
                        result = $"A{m_Random.Next(1, 100)}";
                    }
                    else if (routeType < 90)
                    {
                        result = $"B{m_Random.Next(1, 1000)}";
                    }
                    else
                    {
                        result = $"C{m_Random.Next(1, 1000)}";
                    }
                }
                else
                {
                    result =
                        AustralianRoadNames.NamedHighwayNames[
                            m_Random.Next(AustralianRoadNames.NamedHighwayNames.Length)
                        ];
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
                return IsAustralianRouteNumber(trimmedName) ||
                       ContainsName(
                           AustralianRoadNames.NamedHighwayNames,
                           trimmedName
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
                RoadCategory.Dirt => AustralianRoadNames.DirtBaseNames,
                RoadCategory.Alley => AustralianRoadNames.AlleyBaseNames,
                _ => AustralianRoadNames.GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => AustralianRoadNames.DirtSuffixes,
                RoadCategory.Alley => AustralianRoadNames.AlleySuffixes,
                RoadCategory.Residential => AustralianRoadNames.ResidentialSuffixes,
                RoadCategory.Standard => AustralianRoadNames.StandardSuffixes,
                RoadCategory.Avenue => AustralianRoadNames.AvenueSuffixes,
                _ => AustralianRoadNames.StandardSuffixes
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

        private static bool IsAustralianRouteNumber(string name)
        {
            if (name.Length < 2)
            {
                return false;
            }

            char routeClass = char.ToUpperInvariant(name[0]);

            if (routeClass != 'M' &&
                routeClass != 'A' &&
                routeClass != 'B' &&
                routeClass != 'C')
            {
                return false;
            }

            string numberText = name.Substring(1).Trim();

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
                RoadCategory.Dirt => " Track",
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
                    RoadCategory.Dirt => $"{GetOrdinal(number)} Track",
                    RoadCategory.Alley => $"{GetOrdinal(number)} Lane",
                    RoadCategory.Residential => $"{GetOrdinal(number)} Street",
                    RoadCategory.Standard => $"{GetOrdinal(number)} Street",
                    RoadCategory.Avenue => $"{GetOrdinal(number)} Avenue",
                    RoadCategory.Highway => $"M{number}",
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
