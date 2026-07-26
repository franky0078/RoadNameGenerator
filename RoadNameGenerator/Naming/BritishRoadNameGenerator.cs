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

        private static readonly string[] s_GeneralBaseNames =
        {
            // Bäume, Hecken und Pflanzen
            "Alder",
            "Apple",
            "Ash",
            "Beech",
            "Birch",
            "Blackthorn",
            "Bluebell",
            "Bramble",
            "Cedar",
            "Chestnut",
            "Elder",
            "Elm",
            "Foxglove",
            "Hawthorn",
            "Hazel",
            "Heather",
            "Holly",
            "Hornbeam",
            "Ivy",
            "Laburnum",
            "Laurel",
            "Lavender",
            "Lilac",
            "Lime",
            "Maple",
            "Oak",
            "Pine",
            "Poplar",
            "Primrose",
            "Rose",
            "Rowan",
            "Sycamore",
            "Walnut",
            "Willow",
            "Wisteria",
            "Yew",

            // Königshaus, Adel und historische Namen
            "Albert",
            "Alexandra",
            "Anne",
            "Arthur",
            "Beatrice",
            "Charles",
            "Charlotte",
            "Churchill",
            "Clarence",
            "Crown",
            "Duke",
            "Duchess",
            "Edward",
            "Elizabeth",
            "George",
            "Hanover",
            "Henry",
            "James",
            "Jubilee",
            "King",
            "Mary",
            "Nelson",
            "Prince",
            "Princess",
            "Queen",
            "Regent",
            "Royal",
            "Victoria",
            "Wellington",
            "Windsor",

            // Orte, Grafschaften und Regionen
            "Abbey",
            "Balmoral",
            "Bath",
            "Bedford",
            "Berkshire",
            "Bristol",
            "Buckingham",
            "Cambridge",
            "Canterbury",
            "Chatsworth",
            "Chelsea",
            "Chester",
            "Cornwall",
            "Cumberland",
            "Devon",
            "Dover",
            "Durham",
            "Edinburgh",
            "Essex",
            "Exeter",
            "Gloucester",
            "Hampton",
            "Hampstead",
            "Kensington",
            "Kent",
            "Lancaster",
            "Leicester",
            "London",
            "Manchester",
            "Norfolk",
            "Nottingham",
            "Oxford",
            "Pembroke",
            "Plymouth",
            "Richmond",
            "Salisbury",
            "Somerset",
            "Surrey",
            "Sussex",
            "Warwick",
            "Westminster",
            "Winchester",
            "Worcester",
            "York",

            // Typische britische Bezeichnungen
            "Bridge",
            "Castle",
            "Chapel",
            "Church",
            "College",
            "Common",
            "Cross",
            "Dean",
            "Glebe",
            "Grange",
            "Green",
            "Hall",
            "Hill",
            "Lodge",
            "Manor",
            "Market",
            "Meadow",
            "Mill",
            "Orchard",
            "Park",
            "Priory",
            "Railway",
            "Rectory",
            "Riverside",
            "School",
            "Stables",
            "Station",
            "Theatre",
            "Town",
            "Trinity",
            "Village",
            "Water",
            "Wharf",
            "Woodland",

            // Landschaft und ländliche Begriffe
            "Beacon",
            "Brook",
            "Brookfield",
            "Burnside",
            "Dale",
            "Downs",
            "Glen",
            "Heath",
            "Highfield",
            "Hillcrest",
            "Leys",
            "Marsh",
            "Moor",
            "Moorland",
            "Northfield",
            "Riverbank",
            "Southfield",
            "Springfield",
            "Stonebridge",
            "The Dales",
            "Vale",
            "Waterside",
            "Westfield",
            "Eastfield",
            "Woodside"
        };

        private static readonly string[] s_DirtBaseNames =
        {
            // Pflanzen, Hecken und Gehölze
            "Ash",
            "Beech",
            "Birch",
            "Blackthorn",
            "Bluebell",
            "Bramble",
            "Bracken",
            "Elder",
            "Foxglove",
            "Hawthorn",
            "Hazel",
            "Heather",
            "Holly",
            "Oak",
            "Primrose",
            "Rowan",
            "Willow",
            "Yew",

            // Tiere und ländliche Namen
            "Badger",
            "Deer",
            "Fox",
            "Hare",
            "Hedgehog",
            "Lark",
            "Otter",
            "Owl",
            "Pheasant",
            "Robin",
            "Shepherd's",
            "Squirrel",
            "Stag",
            "Swan",

            // Landschaft, Landwirtschaft und Wege
            "Brook",
            "Coppice",
            "Downland",
            "Farm",
            "Forest",
            "Green Lane",
            "Heath",
            "High Moor",
            "Hollow",
            "Low Moor",
            "Marsh",
            "Meadow",
            "Mill",
            "Moorland",
            "Old Farm",
            "Old Mill",
            "Old Quarry",
            "Orchard",
            "Quarry",
            "River",
            "Ridge",
            "Stone",
            "The Dales",
            "Windmill",
            "Wood",
            "Woodland"
        };

        private static readonly string[] s_AlleyBaseNames =
        {
            // Historische, königliche und innerstädtische Namen
            "Abbey",
            "Albert",
            "Angel",
            "Bell",
            "Blackfriars",
            "Brewer",
            "Castle",
            "Chapel",
            "Church",
            "Clarence",
            "Coach",
            "Cooper",
            "Crown",
            "Duke",
            "Falcon",
            "Fisher",
            "Friar",
            "Garden",
            "George",
            "Guildhall",
            "King",
            "Lion",
            "Market",
            "Merchant",
            "Mill",
            "Queen",
            "Regent",
            "Royal",
            "Rose",
            "Station",
            "Temple",
            "Victoria",
            "Wellington",
            "Wharf",
            "White Hart",
            "Windsor",
            "York",

            // Bäume und Pflanzen
            "Ash",
            "Elm",
            "Holly",
            "Ivy",
            "Laurel",
            "Oak",
            "Willow",
            "Yew"
        };

        private static readonly string[] s_ResidentialSpecialNames =
        {
            "The Beeches",
            "The Brambles",
            "The Cedars",
            "The Close",
            "The Coppice",
            "The Crescent",
            "The Dene",
            "The Gardens",
            "The Glade",
            "The Green",
            "The Grove",
            "The Laurels",
            "The Limes",
            "The Meadows",
            "The Mews",
            "The Oaks",
            "The Paddock",
            "The Spinney",
            "The Willows",
            "The Woodlands"
        };

        private static readonly string[] s_StandardSpecialNames =
        {
            "Bridge Street",
            "Church Walk",
            "High Street",
            "London Road",
            "Main Street",
            "Market Place",
            "Mill Lane",
            "Old Road",
            "Station Approach",
            "The Broadway",
            "The Green",
            "The Parade",
            "The Square",
            "The Street",
            "Town Street",
            "Water Lane"
        };

        private static readonly string[] s_AvenueSpecialNames =
        {
            "Eastway",
            "Kingsway",
            "Queensway",
            "Westway",
            "The Avenue",
            "The Causeway",
            "The Embankment",
            "The Esplanade",
            "The Parkway",
            "The Promenade"
        };

        private static readonly string[] s_NamedHighwayNames =
        {
            "Atlantic Highway",
            "Bath Road",
            "Bristol Road",
            "Coast Road",
            "Devon Expressway",
            "East Cross Route",
            "East Lancashire Road",
            "Ermine Street",
            "Fosse Way",
            "Great Cambridge Road",
            "Great Eastern Road",
            "Great North Road",
            "Great South West Road",
            "Great West Road",
            "Great Western Road",
            "Heads of the Valleys Road",
            "Highland Route",
            "London Road",
            "Mersey Gateway",
            "Moorland Route",
            "North Circular Road",
            "Northern Route",
            "Old Kent Road",
            "Oxford Road",
            "Severn Way",
            "South Circular Road",
            "Southern Route",
            "Thames Gateway",
            "Wales Way",
            "Watling Street",
            "Westway"
        };

        private static readonly string[] s_DirtSuffixes =
        {
            "Track",
            "Track",
            "Lane",
            "Lane",
            "Byway",
            "Path",
            "Bridleway",
            "Drove",
            "Drift",
            "Greenway",
            "Way"
        };

        private static readonly string[] s_AlleySuffixes =
        {
            "Mews",
            "Mews",
            "Passage",
            "Yard",
            "Alley",
            "Court",
            "Row",
            "Walk",
            "Wynd",
            "Arcade"
        };

        private static readonly string[] s_ResidentialSuffixes =
        {
            "Close",
            "Close",
            "Crescent",
            "Court",
            "Gardens",
            "Grove",
            "Lane",
            "Mews",
            "Place",
            "Rise",
            "Terrace",
            "View",
            "Walk",
            "Drive",
            "End",
            "Green",
            "Heights",
            "Mead",
            "Park",
            "Reach",
            "Row",
            "Square",
            "Vale",
            "Villas",
            "Wood"
        };

        private static readonly string[] s_StandardSuffixes =
        {
            "Road",
            "Road",
            "Street",
            "Street",
            "Lane",
            "Drive",
            "Way",
            "Hill",
            "Green",
            "Row",
            "Walk",
            "Place",
            "Approach"
        };

        private static readonly string[] s_AvenueSuffixes =
        {
            "Avenue",
            "Avenue",
            "Way",
            "Road",
            "Parade",
            "Causeway",
            "Parkway",
            "Embankment",
            "Esplanade",
            "Promenade"
        };

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
                    s_NamedHighwayNames,
                    trimmedName))
            {
                return true;
            }

            if (ContainsName(
                    s_ResidentialSpecialNames,
                    trimmedName) ||
                ContainsName(
                    s_StandardSpecialNames,
                    trimmedName) ||
                ContainsName(
                    s_AvenueSpecialNames,
                    trimmedName))
            {
                return true;
            }

            return
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_DirtBaseNames,
                    s_DirtSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_AlleyBaseNames,
                    s_AlleySuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_GeneralBaseNames,
                    s_ResidentialSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_GeneralBaseNames,
                    s_StandardSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_GeneralBaseNames,
                    s_AvenueSuffixes
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
                        s_NamedHighwayNames[
                            m_Random.Next(
                                s_NamedHighwayNames.Length
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
                    s_DirtBaseNames,

                RoadCategory.Alley =>
                    s_AlleyBaseNames,

                _ =>
                    s_GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    s_DirtSuffixes,

                RoadCategory.Alley =>
                    s_AlleySuffixes,

                RoadCategory.Residential =>
                    s_ResidentialSuffixes,

                RoadCategory.Standard =>
                    s_StandardSuffixes,

                RoadCategory.Avenue =>
                    s_AvenueSuffixes,

                _ =>
                    s_StandardSuffixes
            };
        }

        private static string[] GetSpecialNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Residential =>
                    s_ResidentialSpecialNames,

                RoadCategory.Standard =>
                    s_StandardSpecialNames,

                RoadCategory.Avenue =>
                    s_AvenueSpecialNames,

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