// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

namespace RoadNameGenerator.Naming
{
    public sealed class MexicanRoadNameGenerator : RegionalRoadNameGenerator
    {
        private static readonly string[] s_Nature =
        {
            "Los Pinos", "Los Cedros", "Los Olivos", "Los Álamos",
            "Los Encinos", "Los Laureles", "Los Sauces", "Los Robles",
            "Las Flores", "Las Rosas", "Las Palmas", "Las Jacarandas",
            "Las Bugambilias", "Las Acacias", "El Mirador", "El Bosque",
            "El Lago", "El Río", "La Cañada", "La Sierra", "La Loma",
            "La Laguna", "La Cascada", "La Primavera", "El Sol",
            "La Luna", "El Volcán", "La Montaña", "El Naranjo",
            "El Mezquite", "Los Girasoles", "Las Orquídeas",
            "El Jardín", "La Barranca", "La Esperanza", "El Paraíso",
            "Los Cipreses", "Los Fresnos", "Los Nogales", "Los Abedules",
            "Los Duraznos", "Los Naranjos", "Los Limones", "Los Guayabos",
            "Las Magnolias", "Las Gardenias", "Las Violetas", "Las Dalias",
            "Las Amapolas", "Las Azucenas", "Las Camelias", "Las Gaviotas",
            "El Manantial", "El Arroyo", "El Valle", "El Pedregal",
            "El Cerro", "El Pinar", "El Palmar", "El Cenote",
            "La Colina", "La Arboleda", "La Huerta", "La Pradera",
            "La Ribera", "La Hacienda", "La Palma", "La Ceiba"
        };

        private static readonly string[] s_History =
        {
            "Benito Juárez", "Miguel Hidalgo", "José María Morelos",
            "Vicente Guerrero", "Francisco I. Madero", "Emiliano Zapata",
            "Francisco Villa", "Lázaro Cárdenas", "Leona Vicario",
            "Josefa Ortiz de Domínguez", "Sor Juana Inés de la Cruz",
            "Ignacio Allende", "Ignacio Zaragoza", "Venustiano Carranza",
            "Cuauhtémoc", "Nezahualcóyotl", "Reforma", "Independencia",
            "Revolución", "Constitución", "Insurgentes", "Universidad",
            "México", "Guadalajara", "Monterrey", "Puebla", "Oaxaca",
            "Veracruz", "Yucatán", "Chiapas", "Michoacán", "Sonora",
            "Jalisco", "Guanajuato", "Querétaro", "Zacatecas",
            "Belisario Domínguez", "Ricardo Flores Magón",
            "Manuel Ávila Camacho", "Adolfo López Mateos",
            "Plutarco Elías Calles", "Álvaro Obregón",
            "Felipe Ángeles", "Nicolás Bravo", "Mariano Matamoros",
            "Guadalupe Victoria", "José Vasconcelos", "Amado Nervo",
            "Rosario Castellanos", "Octavio Paz", "Diego Rivera",
            "Frida Kahlo", "Juan Rulfo", "Agustín de Iturbide",
            "Tamaulipas", "Coahuila", "Durango", "Tabasco",
            "Campeche", "Sinaloa", "Nayarit", "Colima",
            "Hermosillo", "Mérida", "Morelia", "Acapulco",
            "Toluca", "Tijuana", "Chihuahua", "Xalapa"
        };

        public MexicanRoadNameGenerator() : base(
            new[]
            {
                Places("Camino", "Sendero", "Vereda", "Brecha"),
                Places("Callejón", "Privada", "Cerrada", "Andador"),
                Places("Calle", "Paseo", "Circuito", "Retorno"),
                Prefix(s_History, "Calle ", "Calzada ", "Privada ", "Circuito "),
                Prefix(s_History, "Avenida ", "Bulevar ", "Calzada ", "Paseo ")
            },
            new[]
            {
                "Autopista del Sol", "Autopista México–Querétaro",
                "Autopista México–Puebla", "Autopista México–Toluca",
                "Carretera Panamericana", "Carretera Transpeninsular",
                "Autopista Puebla–Orizaba", "Autopista Monterrey–Saltillo"
            },
            new[] { "MEX ", "Carretera Federal ", "Autopista " },
            new[]
            {
                "Camino ", "Callejón ", "Calle ", "Calzada ",
                "Avenida ", "Carretera Federal "
            })
        {
        }

        private static string[] Places(params string[] roadTypes)
        {
            string[] names = new string[s_Nature.Length * roadTypes.Length];
            int index = 0;
            foreach (string place in s_Nature)
            {
                string ending = place.StartsWith("El ")
                    ? "del " + place.Substring(3)
                    : "de " + char.ToLowerInvariant(place[0]) + place.Substring(1);

                foreach (string roadType in roadTypes)
                {
                    names[index++] = roadType + " " + ending;
                }
            }

            return names;
        }
    }
}
