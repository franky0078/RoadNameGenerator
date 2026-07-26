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

        private static readonly string[] s_GeneralBaseNames =
        {
            // Bäume und Pflanzen
            "Aspen",
            "Oak",
            "Maple",
            "Pine",
            "Cedar",
            "Willow",
            "Birch",
            "Elm",
            "Hickory",
            "Sycamore",
            "Chestnut",
            "Walnut",
            "Magnolia",
            "Cypress",
            "Juniper",
            "Redwood",
            "Dogwood",
            "Laurel",
            "Poplar",
            "Cottonwood",
            "Alder",
            "Ash",
            "Beech",
            "Buckeye",
            "Hawthorn",
            "Hemlock",
            "Holly",
            "Locust",
            "Mulberry",
            "Spruce",
            "Sequoia",
            "Tamarack",
            "Palm",
            "Palmetto",

            // Natur und Landschaft
            "Mountain View",
            "Eagle Ridge",
            "Silver Creek",
            "Clearwater",
            "Canyon Ridge",
            "Bear Creek",
            "Pine Valley",
            "Hidden Valley",
            "Crystal Lake",
            "Sunset",
            "Highland",
            "Meadow",
            "Riverside",
            "Lakeside",
            "Southridge",
            "Fairview",
            "Greenwood",
            "Riverbend",
            "Spring Creek",
            "Blue Ridge",
            "Rocky Point",
            "Pleasant Valley",
            "Rolling Hills",
            "Cedar Grove",
            "Oak Ridge",
            "Pine Ridge",
            "Lakeview",
            "Brookside",
            "Hillcrest",
            "Woodland",
            "Forest Hill",
            "Deer Creek",
            "Fox Hollow",
            "Golden Valley",
            "Red Rock",
            "Stone Creek",
            "Timber Ridge",
            "Wildflower",
            "Clear Lake",
            "Silver Lake",
            "Autumn Ridge",
            "Beaver Creek",
            "Blackwater",
            "Boulder Creek",
            "Briarwood",
            "Cedar Hill",
            "Copper Ridge",
            "Eagle Point",
            "Elk Creek",
            "Evergreen",
            "Grandview",
            "Lone Pine",
            "Misty Hollow",
            "Morning Star",
            "North Ridge",
            "Prairie View",
            "Ravenwood",
            "Rock Creek",
            "Sagebrush",
            "Sandstone",
            "Shadow Creek",
            "Sierra Vista",
            "Spring Valley",
            "Summit",
            "Sunset Ridge",
            "Twin Lakes",
            "Valley View",
            "Whispering Pines",
            "White Oak",
            "Windy Hill",
            "Wolf Creek",

            // Klassische amerikanische Namen
            "Washington",
            "Jefferson",
            "Lincoln",
            "Franklin",
            "Madison",
            "Monroe",
            "Jackson",
            "Hamilton",
            "Roosevelt",
            "Adams",
            "Grant",
            "Sherman",
            "Harrison",
            "Wilson",
            "Kennedy",
            "Eisenhower",
            "Truman",
            "Cleveland",
            "Clinton",
            "Taylor",
            "Pierce",
            "Douglas",
            "Clay",
            "Marshall",
            "Carson",
            "Benton",
            "Fremont",
            "Morgan",
            "Parker",
            "Miller",
            "Anderson",
            "Baker",
            "Carter",
            "Clark",
            "Cooper",
            "Davis",
            "Edwards",
            "Foster",
            "Harris",
            "Hayes",
            "Johnson",
            "Lewis",
            "Mitchell",
            "Nelson",
            "Reed",
            "Roberts",
            "Robinson",
            "Scott",
            "Smith",
            "Thompson",
            "Turner",
            "Walker",
            "Williams",
            "Wright",

            // Orte und Regionen
            "Pacific",
            "Atlantic",
            "Western",
            "Eastern",
            "Northern",
            "Southern",
            "Colorado",
            "Nevada",
            "Arizona",
            "Montana",
            "Dakota",
            "Virginia",
            "Carolina",
            "California",
            "Texas",
            "Utah",
            "Oregon",
            "Alaska",
            "Denver",
            "Austin",
            "Boston",
            "Savannah",
            "Charleston",
            "Springfield",
            "Westwood",
            "Northwood",
            "Eastwood",
            "Westfield",
            "Northfield",
            "Brookfield",

            // Typische Siedlungsnamen
            "Liberty",
            "Union",
            "Independence",
            "Constitution",
            "Freedom",
            "Heritage",
            "Prospect",
            "Central",
            "Market",
            "Church",
            "School",
            "College",
            "Park",
            "Main",
            "Mill",
            "Station",
            "Depot",
            "Harbor",
            "Frontier",
            "Pioneer",
            
            // Stadtzentrum, Gewerbe und öffentliche Einrichtungen
            "Broadway",
            "Center",
            "Commerce",
            "Courthouse",
            "Grand",
            "State",
            "Water",
            "Railroad",
            "Canal",
            "Bridge",
            "Foundry",
            "Orchard",
            "Farm",
            "Ranch",
            "Mission",
            "Capitol",
            "Veterans",
            "Memorial",

            // Amerikanische Regionen, Landschaften und Orte
            "Appalachian",
            "Cascade",
            "Sierra",
            "Ozark",
            "Prairie",
            "Shenandoah",
            "Hudson",
            "Potomac",
            "Columbia",
            "Rio Grande",
            "Yellowstone",
            "Yosemite",
            "Tahoe",
            "Santa Fe",
            "Monterey",
            "Richmond",
            "Lexington",
            "Arlington",
            "Phoenix",
            "Dallas",
            "Houston",
            "Nashville",
            "Memphis",
            "Portland",
            "Seattle",
            "Atlanta",
            "Raleigh"
        };

        private static readonly string[] s_DirtBaseNames =
        {
            "Aspen",
            "Pine",
            "Cedar",
            "Willow",
            "Birch",
            "Juniper",
            "Redwood",
            "Cottonwood",
            "Bear Creek",
            "Deer Creek",
            "Silver Creek",
            "Spring Creek",
            "Clearwater",
            "Pine Valley",
            "Hidden Valley",
            "Canyon Ridge",
            "Eagle Ridge",
            "Timber Ridge",
            "Rocky Point",
            "Red Rock",
            "Fox Hollow",
            "Riverbend",
            "Wildflower",
            "Mountain View",
            "Pleasant Valley",
            "Rolling Hills",
            "Forest Hill",
            "Oak Ridge",
            "Pine Ridge",
            "Crystal Lake",
            "Silver Lake",
            "Clear Lake",
            "Blue Ridge",
            "Meadow",
            "Highland",
            "Woodland",
            "Brookside",
            "Stone Creek",
            "Golden Valley",
            "Southridge",
            "Beaver Creek",
            "Boulder Creek",
            "Copper Ridge",
            "Dry Creek",
            "Elk Creek",
            "Lost Creek",
            "Rock Creek",
            "Shadow Creek",
            "White River",
            "Wolf Creek",

            // Ländliche und westliche Namen
            "Coyote",
            "Mustang",
            "Buffalo",
            "Mesquite",
            "Sagebrush",
            "Prairie View",
            "Lone Pine",
            "Twin Lakes",
            "Whispering Pines",
            "White Oak",
            "Cedar Hollow",
            "Long Hollow",
            "Red Canyon",
            "Eagle Pass",
            "Cottonwood Creek",
            "Black Bear",

            // Farm-, Ranch- und Pioniernamen
            "Old Mill",
            "Old Ranch",
            "Homestead",
            "Wagon Wheel",
            "Lone Star",
            "Dusty",
            "Rustic",
            "Settlers",
            "Frontier"
        };

        private static readonly string[] s_AlleyBaseNames =
        {
            "Market",
            "Mill",
            "Church",
            "Station",
            "Depot",
            "Harbor",
            "Union",
            "Liberty",
            "Franklin",
            "Lincoln",
            "Madison",
            "Monroe",
            "Jackson",
            "Hamilton",
            "Adams",
            "Grant",
            "Marshall",
            "Parker",
            "Miller",
            "Morgan",
            "Oak",
            "Maple",
            "Pine",
            "Cedar",
            "Willow",
            "Birch",
            "Elm",
            "Hickory",
            "Sycamore",
            "Chestnut",

            // Innenstadt und Gewerbe
            "Broadway",
            "Commerce",
            "Center",
            "Water",
            "Railroad",
            "Canal",
            "Bridge",
            "Foundry",
            "Warehouse",
            "Orchard",

            // Weitere klassische Familiennamen
            "Mason",
            "Baker",
            "Carter",
            "Clark",
            "Cooper",
            "Davis",
            "Foster",
            "Harris",
            "Lewis",
            "Reed",
            "Scott",
            "Smith",
            "Turner",
            "Walker",
            "Wright",

            // Pflanzen und Bäume
            "Laurel",
            "Holly",
            "Poplar",
            "Walnut",
            "Spruce"
        };

        private static readonly string[] s_HighwayBaseNames =
        {
            "Lincoln",
            "Washington",
            "Jefferson",
            "Franklin",
            "Roosevelt",
            "Eisenhower",
            "Kennedy",
            "Truman",
            "Grant",
            "Sherman",
            "Hamilton",
            "Madison",
            "Monroe",
            "Jackson",
            "Pacific",
            "Atlantic",
            "Western",
            "Eastern",
            "Northern",
            "Southern",
            "Mountain",
            "Frontier",
            "Pioneer",
            "Liberty",
            "Freedom",
            "Independence",
            "Constitution",
            "Heritage",
            "Veterans Memorial",
            "Blue Ridge",
            "Red Rock",
            "Silver Creek",
            "Golden Valley",
            "Canyon Ridge",
            "Rocky Mountain",
            "Great Plains",
            "Lakeside",
            "Riverside",
            "Clearwater",
            "Eagle Ridge",

            // Große amerikanische Landschaften
            "Appalachian",
            "Cascade",
            "Sierra",
            "Ozark",
            "Prairie",
            "Columbia",
            "Hudson",
            "Potomac",
            "Rio Grande",
            "Yellowstone",
            "Yosemite",
            "Santa Fe",
            "Lone Star",

            // Regionen und Küsten
            "Gulf Coast",
            "Atlantic Coast",
            "Pacific Coast",
            "Great Lakes",
            "Desert",
            "Coastal",
            "Valley",
            "Summit",

            // Nationale und staatliche Namen
            "National",
            "Capital",
            "Veterans",
            "Purple Heart",
            "Gold Star",
            "Blue Star",
            "Memorial",
            "Patriot",
            "Victory",

            // Weitere Landschaftsnamen
            "Evergreen",
            "Sunset",
            "Pioneer Memorial"
        };

        private static readonly string[] s_DirtSuffixes =
        {
            "Trail",
            "Trail",
            "Track",
            "Path",
            "Road",
            "Road",
            "Route",
            "Trace",
            "Pike",
            "Cutoff"
        };

        private static readonly string[] s_AlleySuffixes =
        {
            "Alley",
            "Alley",
            "Lane",
            "Way",
            "Court",
            "Place",
            "Row",
            "Walk",
            "Passage",
            "Plaza",
            "Arcade"
        };

        private static readonly string[] s_ResidentialSuffixes =
        {
            "Street",
            "Lane",
            "Way",
            "Court",
            "Place",
            "Circle",
            "Terrace",
            "Drive",
            "Road",
            "Loop",
            "Bend",
            "Glen",
            "Green",
            "Ridge",
            "View",
            "Crossing",
            "Hollow",
            "Run"
        };

        private static readonly string[] s_StandardSuffixes =
        {
            "Street",
            "Street",
            "Road",
            "Road",
            "Drive",
            "Way",
            "Lane",
            "Route"
        };

        private static readonly string[] s_AvenueSuffixes =
        {
            "Avenue",
            "Avenue",
            "Boulevard",
            "Boulevard",
            "Parkway",
            "Promenade",
            "Esplanade",
            "Causeway"
        };

        private static readonly string[] s_HighwaySuffixes =
        {
            "Highway",
            "Highway",
            "Expressway",
            "Tollway",
            "Turnpike",
            "Freeway"
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
                        s_HighwayBaseNames[
                            m_Random.Next(s_HighwayBaseNames.Length)
                        ];

                    string suffix = GetCompatibleSuffix(
                        baseName,
                        s_HighwaySuffixes,
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
                RoadCategory.Dirt => s_DirtBaseNames,
                RoadCategory.Alley => s_AlleyBaseNames,
                _ => s_GeneralBaseNames
            };
        }

        private static string[] GetSuffixes(RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt => s_DirtSuffixes,
                RoadCategory.Alley => s_AlleySuffixes,
                RoadCategory.Residential => s_ResidentialSuffixes,
                RoadCategory.Standard => s_StandardSuffixes,
                RoadCategory.Avenue => s_AvenueSuffixes,
                RoadCategory.Highway => s_HighwaySuffixes,
                _ => s_StandardSuffixes
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
                        s_HighwayBaseNames,
                        s_HighwaySuffixes))
                {
                    return true;
                }

                return MatchesBaseAndSuffix(
                    trimmedName,
                    s_GeneralBaseNames,
                    s_HighwaySuffixes
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
                        s_AlleySuffixes[
                            (number - 1) % s_AlleySuffixes.Length
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