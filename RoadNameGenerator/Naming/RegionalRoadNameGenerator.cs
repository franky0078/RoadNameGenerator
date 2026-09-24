// Road Name Generator
// Copyright (C) 2026 franky0078
// Licensed under the GNU General Public License version 3 or later.

using System;
using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    // Shared uniqueness and portfolio checks for the three regional name sets.
    public abstract class RegionalRoadNameGenerator : IRoadNameGenerator
    {
        private readonly Random m_Random = new();
        private readonly HashSet<string> m_UsedNames =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly string[][] m_Names;
        private readonly HashSet<string>[] m_NameSets;
        private readonly string[] m_NamedHighways;
        private readonly HashSet<string> m_HighwaySet;
        private readonly string[] m_RoutePrefixes;
        private readonly string[] m_FallbackPrefixes;

        protected RegionalRoadNameGenerator(
            string[][] names,
            string[] namedHighways,
            string[] routePrefixes,
            string[] fallbackPrefixes)
        {
            m_Names = names;
            m_NamedHighways = namedHighways;
            m_RoutePrefixes = routePrefixes;
            m_FallbackPrefixes = fallbackPrefixes;
            m_NameSets = new HashSet<string>[5];

            for (int category = 0; category < m_NameSets.Length; category++)
            {
                m_NameSets[category] = new HashSet<string>(
                    names[category], StringComparer.OrdinalIgnoreCase);
            }

            m_HighwaySet = new HashSet<string>(
                namedHighways, StringComparer.OrdinalIgnoreCase);
        }

        public string Generate(RoadCategory category)
        {
            int index = (int)category;
            if (index < 0 || index > (int)RoadCategory.Highway)
            {
                index = (int)RoadCategory.Standard;
            }

            for (int attempt = 0; attempt < 200; attempt++)
            {
                string result;
                if (index == (int)RoadCategory.Highway)
                {
                    if (m_Random.Next(100) < 75)
                    {
                        string prefix = m_RoutePrefixes[
                            m_Random.Next(m_RoutePrefixes.Length)];
                        result = prefix + m_Random.Next(1, 1000);
                    }
                    else
                    {
                        result = m_NamedHighways[
                            m_Random.Next(m_NamedHighways.Length)];
                    }
                }
                else
                {
                    string[] names = m_Names[index];
                    result = names[m_Random.Next(names.Length)];
                }

                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }

            // All random choices may collide in a large city. This terminates
            // only when a unique name has been found.
            for (int number = 1; ; number++)
            {
                string result = m_FallbackPrefixes[index] + number;
                if (m_UsedNames.Add(result))
                {
                    return result;
                }
            }
        }

        public bool IsNameFromPortfolio(string name, RoadCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            int index = (int)category;
            if (index < 0 || index > (int)RoadCategory.Highway)
            {
                return false;
            }

            string trimmed = name.Trim();
            if (index == (int)RoadCategory.Highway)
            {
                if (m_HighwaySet.Contains(trimmed))
                {
                    return true;
                }

                foreach (string prefix in m_RoutePrefixes)
                {
                    if (HasPositiveNumber(trimmed, prefix))
                    {
                        return true;
                    }
                }
            }
            else if (m_NameSets[index].Contains(trimmed))
            {
                return true;
            }

            return HasPositiveNumber(trimmed, m_FallbackPrefixes[index]);
        }

        public void RegisterExistingName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                m_UsedNames.Add(name.Trim());
            }
        }

        private static bool HasPositiveNumber(string name, string prefix)
        {
            return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(name.Substring(prefix.Length), out int number) &&
                number > 0;
        }

        protected static string[] Compound(string[] stems, params string[] endings)
        {
            string[] names = new string[stems.Length * endings.Length];
            int index = 0;
            foreach (string stem in stems)
            {
                foreach (string ending in endings)
                {
                    names[index++] = stem + ending;
                }
            }

            return names;
        }

        protected static string[] Prefix(string[] bases, params string[] prefixes)
        {
            string[] names = new string[bases.Length * prefixes.Length];
            int index = 0;
            foreach (string baseName in bases)
            {
                foreach (string prefix in prefixes)
                {
                    names[index++] = prefix + baseName;
                }
            }

            return names;
        }

        protected static string[] DirectionalVariants(
            string[] names, params string[] directions)
        {
            string[] result = new string[names.Length * (directions.Length + 1)];
            int index = 0;
            foreach (string name in names)
            {
                result[index++] = name;
                foreach (string direction in directions)
                {
                    result[index++] = direction + " " + name;
                }
            }

            return result;
        }
    }
}
