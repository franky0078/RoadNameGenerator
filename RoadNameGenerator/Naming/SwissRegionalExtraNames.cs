// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    // Curated Swiss people and regional place names. The street combinations are
    // generated names for the game; they do not claim to be official addresses.
    internal static class SwissRegionalExtraNames
    {
        private static readonly string[] s_GermanPeople =
        {
            "Emil Welti", "Simon Gfeller", "Elisabeth Kopp", "Lise Girardin",
            "Katharina von Zimmern", "Anna Heer", "Ida Hoff", "Clara Ragaz",
            "Lydia Welti-Escher", "Cuno Amiet", "Max Bill", "Carl Lutz",
            "Emil Frey", "Emmy Hennings", "Annemarie Schwarzenbach",
            "Hans Erni", "Friedrich Glauser", "Hermann Burger", "Ruth Dreifuss",
            "Elisabeth Joris", "Marie Heim-Vögtlin", "Lina Bögli", "Ulrich Wille",
            "August Forel", "Eduard Spelterini"
        };

        private static readonly string[] s_FrenchPeople =
        {
            "Germaine de Staël", "Isabelle Eberhardt", "Alice Bailly",
            "Marguerite Burnat-Provins", "Charles Pictet de Rochemont",
            "François Diday", "Alexandre Yersin", "Théodore de Bèze",
            "Françoise-Louise de Warens", "Nicolas Bouvier", "Catherine Colomb",
            "Charles Gleyre", "Jacques Chessex", "Jacques Dubochet",
            "Michel Mayor", "Didier Queloz", "Daniel Bovet", "Jean Starobinski",
            "André Chavanne", "Alexandre Calame",
            "Suzanne Curchod", "Marie Dentière", "Jeanne Hersch"
        };

        private static readonly string[] s_ItalianPeople =
        {
            "Francesco Borromini", "Domenico Fontana", "Carlo Maderno",
            "Giacomo Luvini", "Francesco Chiesa", "Piero Bianconi",
            "Giuseppe Zoppi", "Flavio Cotti", "Nello Celio",
            "Dimitri", "Hermann Hesse", "Felice Filippini", "Giorgio Orelli",
            "Giovanni Orelli", "Alberto Nessi", "Plinio Martini", "Anna Felder",
            "Alice Ceresa", "Enrico Dassetto",
            "Raffaello Ceschi", "Luigi Snozzi", "Livio Vacchini"
        };

        private static readonly string[] s_RomanshPeople =
        {
            "Giacun Hasper Muoth", "Peider Lansel", "Cla Biert", "Selina Chönz",
            "Alois Carigiet", "Jon Semadeni", "Flurin Spescha", "Gion Deplazes",
            "Tista Murk", "Andri Peer", "Leta Semadeni", "Oscar Peer",
            "Dumenic Andry", "Leo Tuor", "Arno Camenisch", "Gion Antoni Bühler",
            "Chasper Po", "Gian Fontana", "Peder Cadotsch", "Gion Clau Vincenz"
        };

        private static readonly string[] s_GermanPlaces =
        {
            "Säntis", "Walenstadt", "Engelberg", "Sarnen", "Bürgenstock",
            "Toggenburg", "Bündnerland", "Thunersee", "Entlebuch",
            "Emmental", "Kandersteg", "Brienz", "Appenzell", "Schaffhausen",
            "Schwyz", "Altdorf", "Glarus", "Winterthur", "Baden"
        };

        private static readonly string[] s_FrenchPlaces =
        {
            "Yverdon", "Payerne", "Estavayer", "La Sarraz", "Moudon",
            "Aigle", "Bex", "Nendaz", "Sierre", "Château-d'Oex",
            "Villars", "Coppet", "Nyon", "Rolle", "Pully",
            "Morges", "Moutier", "Orbe"
        };

        private static readonly string[] s_ItalianPlaces =
        {
            "Mendrisiotto", "Malcantone", "Leventina", "Blenio", "Onsernone",
            "Maggia", "Verzasca", "Lugano", "Locarno", "Bellinzona",
            "Chiasso", "Mendrisio", "Brissago", "Ascona", "Biasca",
            "Airolo", "Faido", "Rovio", "Morcote", "Capolago"
        };

        private static readonly string[] s_RomanshPlaces =
        {
            "Scuol", "Zuoz", "Samedan", "Guarda", "Ardez", "Sent",
            "Tschierv", "Müstair", "Zernez", "Savognin", "Trun", "Vrin",
            "Falera", "Laax", "Ftan", "Lavin", "Susch", "Tarasp",
            "Pontresina", "Silvaplana", "Sils", "Sedrun", "Disentis", "Ilanz"
        };

        internal static IEnumerable<string> Get(NamingStyle style, int category)
        {
            string[] people;
            string[] places;
            string personPrefix;
            string personSuffix = "";
            string placePrefix;
            string placeSuffix = "";

            switch (style)
            {
                case NamingStyle.Swiss:
                    people = s_GermanPeople;
                    places = s_GermanPlaces;
                    personPrefix = "";
                    personSuffix = new[] { "-Pfad", "-Gässli", "-Weg", "-Strasse", "-Allee", "-Route" }[category];
                    placePrefix = "";
                    placeSuffix = new[] { "pfad", "gässli", "weg", "strasse", "allee", "route" }[category];
                    break;
                case NamingStyle.SwissFrench:
                    people = s_FrenchPeople;
                    places = s_FrenchPlaces;
                    personPrefix = new[] { "Sentier ", "Passage ", "Chemin ", "Rue ", "Avenue ", "Route " }[category];
                    placePrefix = new[] { "Sentier ", "Passage ", "Chemin ", "Rue ", "Avenue ", "Route " }[category];
                    break;
                case NamingStyle.SwissItalian:
                    people = s_ItalianPeople;
                    places = s_ItalianPlaces;
                    personPrefix = new[] { "Sentiero ", "Vicolo ", "Via ", "Via ", "Viale ", "Strada " }[category];
                    placePrefix = new[] { "Sentiero ", "Vicolo ", "Via ", "Strada ", "Viale ", "Strada " }[category];
                    break;
                case NamingStyle.SwissRomansh:
                    people = s_RomanshPeople;
                    places = s_RomanshPlaces;
                    personPrefix = new[] { "Via ", "Giassa ", "Via ", "Via ", "Promenada ", "Via principala " }[category];
                    placePrefix = new[] { "Via da ", "Giassa da ", "Via da ", "Via da ", "Promenada da ", "Via principala da " }[category];
                    break;
                default:
                    yield break;
            }

            // Person names are used on residential streets, ordinary streets and avenues.
            // Place names are used for paths and routes as well as urban streets.
            if (category >= 1 && category <= 4)
                foreach (string person in people)
                    yield return personPrefix + (style == NamingStyle.Swiss ? person.Replace(' ', '-') : person) + personSuffix;

            foreach (string place in places)
            {
                string location = place;
                if (style == NamingStyle.SwissFrench)
                    location = place.StartsWith("La ") ? "de la " + place.Substring(3) :
                        "AEIOUYÉÈÊÎÔ".IndexOf(place[0]) >= 0 ? "d'" + place : "de " + place;
                else if (style == NamingStyle.SwissItalian)
                    location = "AEIOU".IndexOf(place[0]) >= 0 ? "d'" + place : "di " + place;
                yield return placePrefix + location + placeSuffix;
            }
        }
    }
}
