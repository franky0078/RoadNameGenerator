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
            "Ravn", "Ørn", "Hav", "Nord", "Sør", "Vest", "Øst",
            "Sol", "Måne", "Løkke", "Seter", "Haug", "Kvern", "Bekke"
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
                Compound(s_Nature, "stien", "veien", "løypa"),
                Compound(s_City, "smuget", "stien"),
                Compound(s_Nature, "veien", "gata"),
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
    }
}
