namespace RoadNameGenerator
{
    internal static class ModVersion
    {
        // The assembly version is set from ModVersion.props by MSBuild.
        public static readonly string Current =
            typeof(ModVersion).Assembly.GetName().Version.ToString(3);
    }
}
