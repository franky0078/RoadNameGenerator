// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

namespace RoadNameGenerator.Naming
{
    public sealed class DanishRoadNameGenerator : RegionalRoadNameGenerator
    {
        private static readonly string[] s_Nature =
        {
            "Bøge", "Ege", "Birke", "Fyrre", "Gran", "Linde", "Pile",
            "Aske", "Elme", "Rønne", "Hassel", "Kastanje", "Skov",
            "Eng", "Hede", "Mose", "Sø", "Strand", "Kyst", "Havn",
            "Fjord", "Å", "Bakke", "Dal", "Sol", "Måne", "Kløver", "Rosen",
            "Lilje", "Valmue", "Hjorte", "Ræve", "Egern", "Lærke",
            "Svale", "Fiske", "Mølle", "Kilde", "Korn", "Hvede",
            "Byg", "Rug", "Havre", "Siv", "Rør", "Klint", "Vinge",
            "Drossel", "Høge", "Sten"
        };

        private static readonly string[] s_City =
        {
            "Konge", "Dronning", "Kirke", "Skole", "Station",
            "Rådhus", "Torve", "Park", "Bro", "Slots", "Markeds",
            "Havne", "Universitets", "Københavns", "Aarhus", "Odense",
            "Aalborg", "Esbjerg", "Roskilde", "Helsingør", "Vejle",
            "Kolding", "Frederiksberg", "Amager", "Nyborg", "Ribe"
        };

        public DanishRoadNameGenerator() : base(
            new[]
            {
                NatureNames("sti", "vej", "spor"),
                Compound(s_City, "stræde", "gyde"),
                NatureNames("vej", "gade"),
                Compound(s_City, "gade", "vej"),
                Compound(s_City, "allé", "boulevard")
            },
            new[]
            {
                "Hærvejen", "Kongevejen", "Øresundsmotorvejen",
                "Vestmotorvejen", "Fynske Motorvej", "Nordjyske Motorvej",
                "Sydmotorvejen", "Holbækmotorvejen", "Helsingørmotorvejen"
            },
            new[] { "E", "Primærrute ", "Sekundærrute " },
            new[]
            {
                "Skovvej ", "Stræde ", "Gade ", "Vej ",
                "Allé ", "Primærrute "
            })
        {
        }

        private static string[] NatureNames(params string[] endings)
        {
            return DirectionalVariants(
                Compound(s_Nature, endings),
                "Nordre", "Søndre", "Østre", "Vestre");
        }
    }
}
