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


        private static readonly string[] s_DirtBaseNames =
        {
            "Auen",
            "Bach",
            "Berg",
            "Birken",
            "Buchen",
            "Dorf",
            "Eichen",
            "Erlen",
            "Feld",
            "Fichten",
            "Föhren",
            "Forst",
            "Grenz",
            "Heide",
            "Hirten",
            "Höhen",
            "Hohl",
            "Hof",
            "Jagd",
            "Kastanien",
            "Kiefern",
            "Kirch",
            "Koppel",
            "Krähen",
            "Linden",
            "Moor",
            "Mühlen",
            "Obst",
            "Quellen",
            "Rosen",
            "Sand",
            "Schäfer",
            "Sonnen",
            "Stein",
            "Tal",
            "Tannen",
            "Wacholder",
            "Wald",
            "Weinberg",
            "Wiesen"
        };

        private static readonly string[] s_DirtSpecialNames =
        {
            "Feldweg",
            "Waldweg",
            "Wiesenweg",
            "Forstweg",
            "Mühlenweg",
            "Hohlweg",
            "Kirchsteig",
            "Bergpfad",
            "Talweg",
            "Auweg",
            "Heideweg",
            "Koppelweg",
            "Weinbergweg",
            "Eichenweg",
            "Buchenweg",
            "Birkenweg",
            "Fichtenweg",
            "Kiefernweg",
            "Tannenweg",
            "Am Waldrand",
            "Am Mühlbach",
            "Am Feldrain",
            "Am Weinberg",
            "Durch die Aue",
            "Zur alten Mühle",
            "Zum Forsthaus",
            "Zum Steinbruch",
            "Zum Wiesengrund",
            "Alter Postweg",

            // Weitere ländliche Namen
            "Am alten Forst",
            "Am Auwald",
            "Am Bachlauf",
            "Am Birkengrund",
            "Am Buchenwald",
            "Am Eichenrain",
            "Am Heuweg",
            "Am Hochwald",
            "Am Hohlweg",
            "Am Krähenberg",
            "Am Moosgrund",
            "Am Moor",
            "Am Quellenhang",
            "Am Seegrund",
            "Am Waldbach",
            "An der Trift",
            "Im Auwald",
            "Im Bruch",
            "Im Feldgrund",
            "Im Wiesental",
            "Unter den Eichen",
            "Zur Jagdhütte",
            "Zur Schäferei",
            "Zur Waldhütte",
            "Zum Aussichtspunkt",
            "Zum Gutshof",
            "Zum Hochsitz",
            "Zum Jagdhaus",
            "Zum Moorgrund",
            "Zum Waldsee"
        };

        private static readonly string[] s_AlleyBaseNames =
        {
            "Adler",
            "Anker",
            "Bäcker",
            "Bad",
            "Brunnen",
            "Burg",
            "Engel",
            "Färber",
            "Fischer",
            "Garten",
            "Gerber",
            "Hinter",
            "Kapellen",
            "Kaufmann",
            "Keller",
            "Kirch",
            "Kloster",
            "Korn",
            "Krämer",
            "Küfer",
            "Leder",
            "Löwen",
            "Markt",
            "Mauer",
            "Mühl",
            "Münz",
            "Pfarr",
            "Rathaus",
            "Rosen",
            "Salz",
            "Sattler",
            "Schloss",
            "Schmied",
            "Schuster",
            "Schwan",
            "Seiler",
            "Sonne",
            "Stern",
            "Töpfer",
            "Tor",
            "Trauben",
            "Turm",
            "Weber",
            "Zehnt",
            "Zoll"
        };

        private static readonly string[] s_AlleySpecialNames =
        {
            "Kirchgasse",
            "Mühlgasse",
            "Rosengasse",
            "Schustergasse",
            "Fischergasse",
            "Bäckergasse",
            "Pfarrgasse",
            "Marktgasse",
            "Turmgasse",
            "Brunnengasse",
            "Hintergasse",
            "Kleine Gasse",
            "Klostergasse",
            "Burggasse",
            "Rathausgasse",
            "Gerbergasse",
            "Webergasse",
            "Schmiedgasse",
            "Zehntgasse",
            "Badgasse",
            "Kellergasse",
            "Schlossgasse",
            "Mauergasse",
            "Kapellengasse",
            "Färbergasse",
            "Torgasse",
            "Sackgasse",
            "Gartenpassage",
            "Marktpassage",
            "Rathauspassage",

            // Weitere historische und innerstädtische Namen
            "Adlergasse",
            "Alte Gasse",
            "Am Torbogen",
            "Ankergasse",
            "Enge Gasse",
            "Engelgasse",
            "Hinter dem Markt",
            "Hinter dem Rathaus",
            "Kornmarktgasse",
            "Krämergasse",
            "Krumme Gasse",
            "Kupfergasse",
            "Löwengasse",
            "Münzgasse",
            "Neue Gasse",
            "Obere Gasse",
            "Salzgasse",
            "Schwanengasse",
            "Seilergasse",
            "Sternengasse",
            "Töpfergasse",
            "Traubengasse",
            "Untere Gasse",
            "Zollgasse",
            "Zum Alten Tor"
        };

        private static readonly string[] s_ResidentialBaseNames =
        {
            // Bäume und Pflanzen
            "Ahorn",
            "Akazien",
            "Birken",
            "Buchen",
            "Eichen",
            "Erlen",
            "Fichten",
            "Flieder",
            "Kastanien",
            "Kiefern",
            "Linden",
            "Nelken",
            "Pappel",
            "Rosen",
            "Tannen",
            "Tulpen",
            "Ulmen",
            "Weiden",

            // Vögel und Tiere
            "Amsel",
            "Biber",
            "Bussard",
            "Drossel",
            "Eulen",
            "Falken",
            "Finken",
            "Forellen",
            "Fuchs",
            "Hasen",
            "Hirsch",
            "Kranich",
            "Lerchen",
            "Meisen",
            "Möwen",
            "Reh",
            "Reiher",
            "Rotkehlchen",
            "Schwalben",
            "Staren",

            // Landschaft und Wohngebiet
            "Bach",
            "Berg",
            "Garten",
            "Heide",
            "Höhen",
            "Mühlen",
            "Park",
            "Quellen",
            "See",
            "Sonnen",
            "Tal",
            "Wald",
            "Wiesen"
        };

        private static readonly string[] s_ResidentialSpecialNames =
        {
            "Ahornweg",
            "Akazienweg",
            "Birkenweg",
            "Buchenweg",
            "Eichenweg",
            "Erlenweg",
            "Fichtenweg",
            "Kastanienweg",
            "Kiefernweg",
            "Lindenweg",
            "Pappelweg",
            "Tannenweg",
            "Ulmenweg",
            "Weidenweg",
            "Rosenweg",
            "Tulpenweg",
            "Nelkenweg",
            "Fliederweg",
            "Sonnenweg",
            "Gartenweg",
            "Amselweg",
            "Drosselweg",
            "Finkenweg",
            "Lerchenweg",
            "Meisenweg",
            "Falkenweg",
            "Am Birkenhain",
            "An der Linde",
            "Im Wiesengrund",
            "Am Stadtpark",
            "Am Sonnenhang",
            "Zum Feldrain",
            "Am Bach",
            "Im Grünen",
            "Am Anger",
            "Am Sportplatz",
            "Am Mühlengraben",
            "Am Eichenhain",
            "Im Rosengarten",
            "Im Talgrund",
            "Auf der Höhe",
            "An den Gärten",
            "Am Waldeck",
            "Am Seeufer",
            "Am Quellenweg",
            "Zum Lindenhof",

            // Weitere Wohngebiets- und Siedlungsnamen
            "Am Buchenhain",
            "Am Dorfanger",
            "Am Fichtenhang",
            "Am Gartenfeld",
            "Am Hasenrain",
            "Am Kastanienhof",
            "Am Mühlenpark",
            "Am Obstgarten",
            "Am Parkrand",
            "Am Rosenhang",
            "Am Tannenhof",
            "Am Waldsaum",
            "An den Linden",
            "Auf dem Berg",
            "Im Amselgrund",
            "Im Eichenhof",
            "Im Erlenhain",
            "Im Finkenpark",
            "Im Gartenhof",
            "Im Lindenpark",
            "Im Mühlenfeld",
            "Im Quellenhof",
            "Im Sonnenfeld",
            "Im Tannenhain",
            "Im Vogelgrund",
            "Im Waldgarten",
            "Im Weidengrund",
            "Unter den Birken",
            "Unter den Linden",
            "Zum Birkenhof",
            "Zum Eichenpark",
            "Zum Rosenhain",
            "Zum Sonnenhof",
            "Zum Waldgarten"
        };

        private static readonly string[] s_StandardBaseNames =
        {
            // Orte und Einrichtungen
            "Bahnhof",
            "Berg",
            "Brücken",
            "Dorf",
            "Friedhof",
            "Garten",
            "Hafen",
            "Haupt",
            "Hof",
            "Hospital",
            "Industrie",
            "Kanal",
            "Kirch",
            "Kloster",
            "Markt",
            "Messe",
            "Mittel",
            "Mühlen",
            "Park",
            "Post",
            "Rathaus",
            "Schloss",
            "Schul",
            "See",
            "Tal",
            "Ufer",
            "Wald",
            "Wiesen",

            // Persönlichkeiten und historische Bezeichnungen
            "Alexander",
            "Augusta",
            "Bach",
            "Beethoven",
            "Bismarck",
            "Carl",
            "Dürer",
            "Friedrich",
            "Goethe",
            "Heinrich",
            "Hermann",
            "Johann",
            "Kant",
            "Kaiser",
            "König",
            "Lessing",
            "Luisen",
            "Luther",
            "Mozart",
            "Richard",
            "Robert",
            "Schiller",
            "Viktoria",
            "Wilhelm",

            // Allgemeine Bezeichnungen
            "Einheit",
            "Europa",
            "Freiheit",
            "Frieden",
            "Fürsten",
            "Grafen",
            "Handels",
            "Herzog",
            "Republik",
            "Verfassungs",
            "Werks"
        };

        private static readonly string[] s_StandardSpecialNames =
        {
            "Hauptstraße",
            "Bahnhofstraße",
            "Marktstraße",
            "Kirchstraße",
            "Schulstraße",
            "Gartenstraße",
            "Bergstraße",
            "Talstraße",
            "Waldstraße",
            "Wiesenstraße",
            "Dorfstraße",
            "Rathausstraße",
            "Poststraße",
            "Mühlenstraße",
            "Brückenstraße",
            "Uferstraße",
            "Seestraße",
            "Goethestraße",
            "Schillerstraße",
            "Lessingstraße",
            "Mozartstraße",
            "Beethovenstraße",
            "Bachstraße",
            "Bismarckstraße",
            "Kantstraße",
            "Lutherstraße",
            "Dürerstraße",
            "Friedrichstraße",
            "Wilhelmstraße",
            "Luisenstraße",
            "Augustastraße",
            "Schlossstraße",
            "Klosterstraße",
            "Hospitalstraße",
            "Friedhofstraße",
            "Hofstraße",
            "Mittelstraße",
            "Neue Straße",
            "Alte Straße",
            "Neustädter Straße",
            "Berliner Straße",
            "Hamburger Straße",
            "Münchner Straße",
            "Nürnberger Straße",
            "Leipziger Straße",
            "Dresdner Straße",
            "Kölner Straße",
            "Frankfurter Straße",
            "Bremer Straße",
            "Hannoversche Straße",
            "Stuttgarter Straße",
            "Regensburger Straße",
            "Bamberger Straße",
            "Coburger Straße",

            // Weitere Städte und Regionen
            "Aachener Straße",
            "Augsburger Straße",
            "Bayreuther Straße",
            "Bonner Straße",
            "Braunschweiger Straße",
            "Darmstädter Straße",
            "Dortmunder Straße",
            "Düsseldorfer Straße",
            "Erfurter Straße",
            "Essener Straße",
            "Flensburger Straße",
            "Freiburger Straße",
            "Gießener Straße",
            "Göttinger Straße",
            "Heidelberger Straße",
            "Jenaer Straße",
            "Karlsruher Straße",
            "Kasseler Straße",
            "Kieler Straße",
            "Koblenzer Straße",
            "Konstanzer Straße",
            "Lübecker Straße",
            "Magdeburger Straße",
            "Mainzer Straße",
            "Mannheimer Straße",
            "Oldenburger Straße",
            "Potsdamer Straße",
            "Rostocker Straße",
            "Saarbrücker Straße",
            "Schweriner Straße",
            "Trierer Straße",
            "Weimarer Straße",
            "Wiesbadener Straße",
            "Würzburger Straße",

            // Weitere klassische Straßennamen
            "Amtsstraße",
            "Burgstraße",
            "Fabrikstraße",
            "Feuerwehrstraße",
            "Grenzstraße",
            "Handelsstraße",
            "Kanalstraße",
            "Kreuzstraße",
            "Landwehrstraße",
            "Marienstraße",
            "Museumstraße",
            "Neue Heimat",
            "Oberstraße",
            "Ringstraße",
            "Unterstraße",
            "Werkstraße",
            "Zechenstraße"
        };

        private static readonly string[] s_AvenueBaseNames =
        {
            "Campus",
            "Donau",
            "Elbe",
            "Europa",
            "Frieden",
            "Hafen",
            "Handels",
            "Industrie",
            "Innovations",
            "Isar",
            "Kaiser",
            "Kastanien",
            "Königs",
            "Kurfürsten",
            "Linden",
            "Main",
            "Messe",
            "Mosel",
            "Neckar",
            "Nord",
            "Ost",
            "Park",
            "Platanen",
            "Rhein",
            "Spree",
            "Stadtpark",
            "Süd",
            "Technologie",
            "Universitäts",
            "West",
            "Weser",
            "Zentral"
        };

        private static readonly string[] s_AvenueSpecialNames =
        {
            "Berliner Allee",
            "Münchner Allee",
            "Kaiserallee",
            "Parkallee",
            "Stadtparkallee",
            "Europallee",
            "Friedensallee",
            "Messeallee",
            "Zentralallee",
            "Königsallee",
            "Maximilianstraße",
            "Kaiserstraße",
            "Kurfürstenstraße",
            "Konrad-Adenauer-Straße",
            "Friedrich-Ebert-Straße",
            "Theodor-Heuss-Straße",
            "Willy-Brandt-Straße",
            "Ernst-Reuter-Allee",
            "Geschwister-Scholl-Straße",
            "Sophie-Scholl-Allee",
            "Nordring",
            "Südring",
            "Ostring",
            "Westring",
            "Stadtring",
            "Innenstadtring",
            "Industriestraße",
            "Hafenstraße",
            "Messestraße",
            "Flughafenstraße",
            "Werner-von-Siemens-Straße",
            "Robert-Bosch-Straße",
            "Carl-Zeiss-Straße",
            "Europa-Ring",
            "Friedensring",
            "Stadtpark-Ring",
            "Nordallee",
            "Südallee",
            "Ostallee",
            "Westallee",

            // Weitere repräsentative Namen
            "Albert-Einstein-Allee",
            "Alexander-von-Humboldt-Allee",
            "August-Bebel-Straße",
            "Campusallee",
            "Donaupromenade",
            "Elbpromenade",
            "Erfinderstraße",
            "Europaboulevard",
            "Franz-Josef-Strauß-Allee",
            "Hafencity-Allee",
            "Innovationsallee",
            "Isarpromenade",
            "Karl-Marx-Allee",
            "Ludwig-Erhard-Straße",
            "Mainpromenade",
            "Marie-Curie-Allee",
            "Moselpromenade",
            "Neckarpromenade",
            "Platanenallee",
            "Rheinpromenade",
            "Spreepromenade",
            "Technologieallee",
            "Universitätsallee",
            "Weserpromenade",
            "Wissenschaftsallee",
            "Zentralring"
        };

        private static readonly string[] s_NamedHighwayNames =
        {
            "Nordtangente",
            "Südtangente",
            "Osttangente",
            "Westtangente",
            "Nordosttangente",
            "Nordwesttangente",
            "Südosttangente",
            "Südwesttangente",
            "Nordumfahrung",
            "Südumfahrung",
            "Ostumfahrung",
            "Westumfahrung",
            "Stadtumfahrung",
            "Ringautobahn",
            "Stadtautobahn",
            "Hafenautobahn",
            "Flughafenzubringer",
            "Messezubringer",
            "Industriezubringer",
            "Autobahnzubringer Nord",
            "Autobahnzubringer Süd",
            "Autobahnzubringer Ost",
            "Autobahnzubringer West",
            "Nord-Süd-Verbindung",
            "Ost-West-Verbindung",
            "Rhein-Main-Verbindung",
            "Stadtschnellweg",
            "Messeschnellweg",
            "Hafenschnellweg",
            "Industrieschnellweg",
            "Flughafenschnellstraße",
            "Metropolring",
            "Nordring-Autobahn",
            "Südring-Autobahn",
            "Ostring-Autobahn",
            "Westring-Autobahn",
            "Hafentangente",
            "Messetangente",
            "Umgehungsstraße Nord",
            "Umgehungsstraße Süd",
            "Umgehungsstraße Ost",
            "Umgehungsstraße West"
        };

        private static readonly string[] s_DirtSuffixes =
        {
            "weg",
            "weg",
            "weg",
            "pfad",
            "pfad",
            "steig",
            "grund"
        };

        private static readonly string[] s_AlleySuffixes =
        {
            "gasse",
            "gasse",
            "gasse",
            "passage",
            "hof",
            "winkel",
            "gang"
        };

        private static readonly string[] s_ResidentialSuffixes =
        {
            "weg",
            "weg",
            "weg",
            "straße",
            "straße",
            "ring",
            "bogen",
            "höhe",
            "grund",
            "garten",
            "hain",
            "hof",
            "blick"
        };

        private static readonly string[] s_StandardSuffixes =
        {
            "straße",
            "straße",
            "straße",
            "weg",
            "weg",
            "platz",
            "ring",
            "allee"
        };

        private static readonly string[] s_AvenueSuffixes =
        {
            "allee",
            "allee",
            "allee",
            "ring",
            "ring",
            "straße",
            "promenade"
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
                return IsGermanRouteNumber(trimmedName) ||
                       ContainsName(
                           s_NamedHighwayNames,
                           trimmedName
                       );
            }

            if (ContainsName(
                    s_DirtSpecialNames,
                    trimmedName) ||
                ContainsName(
                    s_AlleySpecialNames,
                    trimmedName) ||
                ContainsName(
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
                    s_ResidentialBaseNames,
                    s_ResidentialSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_StandardBaseNames,
                    s_StandardSuffixes
                ) ||
                MatchesBaseAndSuffix(
                    trimmedName,
                    s_AvenueBaseNames,
                    s_AvenueSuffixes
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

                RoadCategory.Residential =>
                    s_ResidentialBaseNames,

                RoadCategory.Standard =>
                    s_StandardBaseNames,

                RoadCategory.Avenue =>
                    s_AvenueBaseNames,

                _ =>
                    s_StandardBaseNames
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
                RoadCategory.Dirt =>
                    s_DirtSpecialNames,

                RoadCategory.Alley =>
                    s_AlleySpecialNames,

                RoadCategory.Residential =>
                    s_ResidentialSpecialNames,

                RoadCategory.Standard =>
                    s_StandardSpecialNames,

                RoadCategory.Avenue =>
                    s_AvenueSpecialNames,

                _ =>
                    s_StandardSpecialNames
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
