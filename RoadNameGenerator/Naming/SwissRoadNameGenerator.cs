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
    public sealed class SwissRoadNameGenerator : IRoadNameGenerator
    {
        private readonly Random m_Random = new();

        private readonly HashSet<string> m_UsedNames =
            new(StringComparer.OrdinalIgnoreCase);

        public string Generate(RoadCategory category)
        {
            string[] names = GetNames(category);

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result =
                    names[m_Random.Next(names.Length)];

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            return GenerateFallbackName(category);
        }

        public void RegisterExistingName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            m_UsedNames.Add(name.Trim());
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

            return
                ContainsName(
                    GetNames(category),
                    trimmedName
                ) ||
                IsNumberedFallbackName(
                    trimmedName,
                    category
                );
        }

        private static string[] GetNames(
            RoadCategory category)
        {
            return category switch
            {
                RoadCategory.Dirt =>
                    SwissRoadNames.DirtNames,

                RoadCategory.Alley =>
                    SwissRoadNames.AlleyNames,

                RoadCategory.Residential =>
                    SwissRoadNames.ResidentialNames,

                RoadCategory.Standard =>
                    SwissRoadNames.StandardNames,

                RoadCategory.Avenue =>
                    SwissRoadNames.AvenueNames,

                RoadCategory.Highway =>
                    SwissRoadNames.HighwayRouteNames,

                _ =>
                    SwissRoadNames.StandardNames
            };
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

        private static bool IsNumberedFallbackName(
            string name,
            RoadCategory category)
        {
            string prefix = category switch
            {
                RoadCategory.Dirt => "Chemin ",
                RoadCategory.Alley => "Passage ",
                RoadCategory.Residential => "Via ",
                RoadCategory.Standard => "Route ",
                RoadCategory.Avenue => "Avenue ",
                RoadCategory.Highway => "A",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(prefix) ||
                !name.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string numberText =
                name.Substring(prefix.Length).Trim();

            return int.TryParse(
                       numberText,
                       out int number
                   ) &&
                   number > 0;
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
                        $"Chemin {number}",

                    RoadCategory.Alley =>
                        $"Passage {number}",

                    RoadCategory.Residential =>
                        $"Via {number}",

                    RoadCategory.Standard =>
                        $"Route {number}",

                    RoadCategory.Avenue =>
                        $"Avenue {number}",

                    RoadCategory.Highway =>
                        $"A{number}",

                    _ =>
                        $"Route {number}"
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
