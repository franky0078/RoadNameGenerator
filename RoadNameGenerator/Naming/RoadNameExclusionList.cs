using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoadNameGenerator.Naming
{
    public static class RoadNameExclusionList
    {
        public static string FilePath => Path.Combine(CustomRoadNameList.Folder, "ExcludedRoadNames.txt");
        private static string[] s_Rules = Array.Empty<string>();
        private static int s_WarnedRevision = -1;
        public static int Revision { get; private set; }
        public static bool HasReadError { get; private set; }
        public static int ActiveCount => (Mod.Settings?.UseNameExclusions ?? true) ? s_Rules.Length : 0;
        public static bool ExcludesEverything => ActiveCount > 0 && s_ExcludesEverything;

        public static void Reload()
        {
            try
            {
                Directory.CreateDirectory(CustomRoadNameList.Folder);
                if (!File.Exists(FilePath))
                {
                    using var file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    using var writer = new StreamWriter(file, new UTF8Encoding(false));
                    writer.WriteLine("# Road Name Generator - excluded names (UTF-8)");
                    writer.WriteLine("# One complete name or * wildcard pattern per line; case-insensitive.");
                    writer.WriteLine("# Empty lines and # comments are ignored. No rules are active by default.");
                    writer.WriteLine("# Autobahn 1    excludes only Autobahn 1");
                    writer.WriteLine("# Autobahn *    excludes Autobahn 1, Autobahn 2, Autobahn Nord, etc.");
                    writer.WriteLine("# *Autoroute*   excludes every name containing Autoroute");
                    writer.WriteLine("# Reload the exclusion list after editing. Existing roads are not renamed.");
                }
                if (new FileInfo(FilePath).Length > 4 * 1024 * 1024)
                    throw new IOException("The exclusion file exceeds 4 MiB.");
                string contents = new UTF8Encoding(false, true).GetString(File.ReadAllBytes(FilePath));
                var rules = new List<string>();
                var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                bool excludesEverything = false;
                foreach (string line in contents.Split(new[] { '\r', '\n' }))
                {
                    string rule = line.Trim().TrimStart('\uFEFF').Trim();
                    if (rule.Length == 0 || rule.StartsWith("#")) continue;
                    if (rule.Length > 128 || ContainsControlCharacter(rule)) continue;
                    // Collapse adjacent wildcards once at load time.
                    while (rule.Contains("**")) rule = rule.Replace("**", "*");
                    if (unique.Add(rule)) rules.Add(rule);
                    if (rule == "*") excludesEverything = true;
                }
                s_Rules = rules.ToArray();
                s_ExcludesEverything = excludesEverything;
                HasReadError = false;
                Revision++;
                Mod.LogDetailed($"Loaded {s_Rules.Length} road name exclusion rules.");
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException || exception is DecoderFallbackException)
            {
                HasReadError = true;
                Revision++;
                Mod.Log.Warn($"Could not load excluded road names: {exception.Message}");
            }
        }

        private static bool s_ExcludesEverything;

        public static bool IsExcluded(string name)
        {
            if (ActiveCount == 0 || string.IsNullOrWhiteSpace(name)) return false;
            name = name.Trim();
            foreach (string rule in s_Rules)
                if (Matches(name, rule)) return true;
            return false;
        }

        // Only * is special. Punctuation, brackets and ? are matched literally.
        internal static bool Matches(string name, string rule)
        {
            int n = 0, r = 0, star = -1, retry = 0;
            while (n < name.Length)
            {
                if (r < rule.Length && rule[r] == '*')
                {
                    star = r++;
                    retry = n;
                }
                else if (r < rule.Length && char.ToUpperInvariant(name[n]) == char.ToUpperInvariant(rule[r]))
                {
                    n++;
                    r++;
                }
                else if (star >= 0)
                {
                    r = star + 1;
                    n = ++retry;
                }
                else return false;
            }
            while (r < rule.Length && rule[r] == '*') r++;
            return r == rule.Length;
        }

        public static void ReportUnavailable()
        {
            if (s_WarnedRevision == Revision) return;
            s_WarnedRevision = Revision;
            Mod.Log.Warn("No allowed road name found within 256 attempts. The current name is preserved. Review ExcludedRoadNames.txt or choose another name source.");
        }

        private static bool ContainsControlCharacter(string value)
        {
            foreach (char character in value)
                if (char.IsControl(character)) return true;
            return false;
        }
    }
}
