using System;
using System.Collections.Generic;

namespace RoadNameGenerator.Naming
{
    public sealed class CustomRoadNameGenerator : IRoadNameGenerator
    {
        private readonly IRoadNameGenerator m_Fallback;
        private readonly CustomNameMode m_Mode;
        private readonly IReadOnlyList<string> m_Names;
        private readonly HashSet<string> m_UsedNames;
        private readonly Random m_Random = new();

        public CustomRoadNameGenerator(IRoadNameGenerator fallback, CustomNameMode mode,
            IReadOnlyList<string> names, HashSet<string> usedNames)
        {
            m_Fallback = fallback;
            m_Mode = mode;
            m_Names = names;
            m_UsedNames = usedNames;
        }

        public string Generate(RoadCategory category)
        {
            if (m_Mode == CustomNameMode.Disabled)
                return GenerateFallback(category);
            bool triedFallback = m_Mode == CustomNameMode.Mixed && m_Random.Next(2) == 0;
            if (triedFallback)
            {
                string generated = GenerateFallback(category);
                if (generated != null)
                    return generated;
            }

            // Reservoir sampling gives each available name the same chance, even in large lists.
            string selected = null;
            int available = 0;
            foreach (string name in m_Names)
            {
                if (!m_UsedNames.Contains(name) && !RoadNameExclusionList.IsExcluded(name) && m_Random.Next(++available) == 0)
                    selected = name;
            }
            if (selected != null)
            {
                RegisterExistingName(selected);
                return selected;
            }

            // Custom-only never invents suffixes or silently switches to another naming style.
            return m_Mode == CustomNameMode.CustomOnly || triedFallback ? null : GenerateFallback(category);
        }

        private string GenerateFallback(RoadCategory category)
        {
            // Bound rejection sampling: broad rules must never stall the simulation.
            for (int attempt = 0; attempt < 256; attempt++)
            {
                if (RoadNameExclusionList.ExcludesEverything)
                    break;
                string name = m_Fallback.Generate(category);
                if (string.IsNullOrWhiteSpace(name))
                    return null;
                if (RoadNameExclusionList.IsExcluded(name))
                    continue;
                RegisterExistingName(name);
                return name;
            }
            RoadNameExclusionList.ReportUnavailable();
            return null;
        }

        public bool IsNameFromPortfolio(string name, RoadCategory category)
        {
            if (RoadNameExclusionList.IsExcluded(name))
                return false;
            if (m_Mode != CustomNameMode.Disabled)
                foreach (string customName in m_Names)
                    if (string.Equals(customName, name?.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
            return m_Mode != CustomNameMode.CustomOnly && m_Fallback.IsNameFromPortfolio(name, category);
        }

        public void RegisterExistingName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            m_UsedNames.Add(name.Trim());
            m_Fallback.RegisterExistingName(name);
        }
    }
}
