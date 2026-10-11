using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RoadNameGenerator.Naming
{
    public static class CustomRoadNameList
    {
        public static string Folder => Path.Combine(Application.persistentDataPath,
            "ModsData", "RoadNameGenerator");
        public static string FilePath => Path.Combine(Folder, "CustomRoadNames.txt");
        public static IReadOnlyList<string> Names { get; private set; } = Array.Empty<string>();
        public static int Revision { get; private set; }
        public static bool HasReadError { get; private set; }

        public static void Reload()
        {
            try
            {
                Directory.CreateDirectory(Folder);
                if (!File.Exists(FilePath))
                {
                    // Create a documented empty list without automatically assigning examples.
                    using var file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                    using var writer = new StreamWriter(file, new UTF8Encoding(false));
                    writer.WriteLine("# Road Name Generator - custom road names (UTF-8)");
                    writer.WriteLine("# One complete road name per line. Empty lines and # comments are ignored.");
                    writer.WriteLine("# Example: Lindenstrasse / Maple Street");
                }

                if (new FileInfo(FilePath).Length > 4 * 1024 * 1024)
                    throw new IOException("The custom road name file exceeds 4 MiB.");

                var names = new List<string>();
                var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                string contents = new UTF8Encoding(false, true).GetString(File.ReadAllBytes(FilePath));
                foreach (string line in contents.Split(new[] { '\r', '\n' }))
                {
                    string name = line.Trim().TrimStart('\uFEFF').Trim();
                    if (name.Length == 0 || name.StartsWith("#"))
                        continue;
                    if (name.Length > 128 || ContainsControlCharacter(name))
                        continue;
                    if (unique.Add(name))
                        names.Add(name);
                }

                Names = names.ToArray();
                HasReadError = false;
                Revision++;
                Mod.LogDetailed($"Loaded {Names.Count} custom road names.");
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException || exception is DecoderFallbackException)
            {
                // Keep the last successfully loaded list if editing/importing fails.
                HasReadError = true;
                Revision++;
                Mod.Log.Warn($"Could not load custom road names: {exception.Message}");
            }
        }

        private static bool ContainsControlCharacter(string name)
        {
            foreach (char character in name)
                if (char.IsControl(character))
                    return true;
            return false;
        }

        public static void OpenFolder()
        {
            try
            {
                Directory.CreateDirectory(Folder);
                Application.OpenURL(new Uri(Folder + Path.DirectorySeparatorChar).AbsoluteUri);
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException)
            {
                Mod.Log.Warn($"Could not open the custom road names folder: {exception.Message}");
            }
        }
    }
}
