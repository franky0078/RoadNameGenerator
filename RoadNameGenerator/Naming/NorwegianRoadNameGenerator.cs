// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

namespace RoadNameGenerator.Naming
{
    public sealed class NorwegianRoadNameGenerator : RegionalRoadNameGenerator
    {
        private static readonly string[] s_Nature =
        {
            "Bjørke", "Eike", "Furu", "Gran", "Linde", "Osp", "Rogne",
            "Selje", "Bøk", "Hassel", "Eine", "Hegge", "Myr", "Fjell",
            "Dal", "Elve", "Foss", "Sjø", "Strand", "Vik", "Berg",
            "Ås", "Eng", "Blåbær", "Tyttebær", "Rein", "Elg", "Hjort",
            "Ravn", "Ørn", "Hav",
            "Sol", "Måne", "Løkke", "Seter", "Haug", "Kvern", "Bekke",
            "Tjern", "Granli", "Røsslyng", "Heste", "Multe", "Bjørn",
            "Fiske", "Stein", "Sand", "Lyng", "Holme", "Fjære"
        };

        private static readonly string[] s_City =
        {
            "Konge", "Dronning", "Kirke", "Skole", "Stasjon", "Park",
            "Havne", "Brygge", "Torg", "Rådhus", "Universitet",
            "Mølle", "Festning", "Bakke", "Gamle", "Ny", "Sentrum",
            "Oslo", "Bergen", "Trondheim", "Stavanger", "Tromsø",
            "Kristiansand", "Drammen", "Ålesund", "Bodø", "Lillehammer"
        };

        public NorwegianRoadNameGenerator() : base(
            new[]
            {
                NatureNames("stien", "veien", "løypa"),
                Compound(s_City, "smuget", "stien"),
                NatureNames("veien", "gata"),
                Compound(s_City, "gata", "veien"),
                Compound(s_City, "alléen", "veien")
            },
            new[]
            {
                "Atlanterhavsveien", "Trollstigen", "Lærdalstunnelen",
                "Gudbrandsdalsveien", "Sørlandsveien", "Nordlandsveien",
                "Hardangervidda", "Dovreveien", "Kystveien"
            },
            new[] { "E", "Rv ", "Fv " },
            new[]
            {
                "Skogsvei ", "Smug ", "Gate ", "Vei ",
                "Allé ", "Rv "
            })
        {
        }

        private static string[] NatureNames(params string[] endings)
        {
            return DirectionalVariants(
                Compound(s_Nature, endings),
                "Nordre", "Søndre", "Øvre", "Nedre");
        }
    }
}
