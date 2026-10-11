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
            if (m_Mode == CustomNameMode.Disabled ||
                (m_Mode == CustomNameMode.Mixed && m_Random.Next(2) == 0))
                return GenerateFallback(category);

            // Reservoir sampling gives each available name the same chance, even in large lists.
            string selected = null;
            int available = 0;
            foreach (string name in m_Names)
            {
                if (!m_UsedNames.Contains(name) && m_Random.Next(++available) == 0)
                    selected = name;
            }
            if (selected != null)
            {
                RegisterExistingName(selected);
                return selected;
            }

            // Custom-only never invents suffixes or silently switches to another naming style.
            return m_Mode == CustomNameMode.CustomOnly ? null : GenerateFallback(category);
        }

        private string GenerateFallback(RoadCategory category)
        {
            string name = m_Fallback.Generate(category);
            RegisterExistingName(name);
            return name;
        }

        public bool IsNameFromPortfolio(string name, RoadCategory category)
        {
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
