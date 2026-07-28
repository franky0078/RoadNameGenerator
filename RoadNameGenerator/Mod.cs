// Road Name Generator
// Copyright (C) 2026 franky0078
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3
// or, at your option, any later version.

using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using RoadNameGenerator.Localization;
using RoadNameGenerator.Systems;

namespace RoadNameGenerator
{
    public class Mod : IMod
    {
        public static readonly ILog Log =
            LogManager
                .GetLogger("RoadNameGenerator.Mod")
                .SetShowsErrorsInUI(false);

        public static Setting Settings
        {
            get;
            private set;
        }

        private static bool s_ExistingRoadScanRequested;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info(
                "Road Name Generator wird geladen."
            );

            if (GameManager.instance.modManager
                .TryGetExecutableAsset(
                    this,
                    out var asset))
            {
                Log.Info(
                    $"Mod-Verzeichnis: {asset.path}"
                );
            }

            Settings = new Setting(this);
            Settings.RegisterInOptionsUI();

            GameManager.instance.localizationManager.AddSource(
                "de-DE",
                new LocaleDE(Settings)
            );

            GameManager.instance.localizationManager.AddSource(
                "en-US",
                new LocaleEN(Settings)
            );

            AssetDatabase.global.LoadSettings(
                "RoadNameGenerator",
                Settings,
                new Setting(this)
            );

            updateSystem.UpdateAt<RoadNamingSystem>(
                SystemUpdatePhase.GameSimulation
            );

            Log.Info(
                "Road Name Generator wurde erfolgreich geladen."
            );
        }

        public static void RequestExistingRoadScan()
        {
            s_ExistingRoadScanRequested = true;

            LogDetailed(
                "Prüfung bestehender Straßen wurde " +
                "im Optionsmenü angefordert."
            );
        }

        public static bool ConsumeExistingRoadScanRequest()
        {
            if (!s_ExistingRoadScanRequested)
            {
                return false;
            }

            s_ExistingRoadScanRequested = false;
            return true;
        }

        public void OnDispose()
        {
            Log.Info(
                "Road Name Generator wird beendet."
            );

            Settings?.UnregisterInOptionsUI();
            Settings = null;

            s_ExistingRoadScanRequested = false;
        }

        public static void LogDetailed(string message)
        {
            if (Settings?.EnableDetailedLogging == true)
            {
                Log.Info(message);
            }
        }

        public static void LogDiagnosticWarning(
            string message)
        {
            if (Settings?.EnableDetailedLogging == true)
            {
                Log.Warn(message);
            }
        }
    }
}