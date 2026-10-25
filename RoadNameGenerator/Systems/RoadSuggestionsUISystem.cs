using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game;
using Game.UI;
using Game.UI.InGame;
using RoadNameGenerator.Naming;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{
    public partial class RoadSuggestionsUISystem : UISystemBase
    {
        private const string Group = "roadNameGenerator";
        private RoadNamingSystem m_NamingSystem;
        private SelectedInfoUISystem m_SelectedInfo;
        private Entity m_Road;
        private List<string> m_Suggestions = new();
        private string m_RoadName = string.Empty;
        private string m_Message = string.Empty;
        private string m_Token = string.Empty;
        private long m_Revision;
        private bool m_Open;
        private NamingStyle m_Style;
        private CustomNameMode m_CustomMode;
        private int m_ListRevision;
        private int m_ExclusionRevision;
        private bool m_ExclusionsEnabled;
        private bool m_NumberRamps;

        public override GameMode gameMode => GameMode.Game;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_NamingSystem = World.GetOrCreateSystemManaged<RoadNamingSystem>();
            m_SelectedInfo = World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "showButton",
                () => Mod.Settings?.ShowRoadSuggestionsButton ?? true));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "open", () => m_Open));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "hasRoad", () => m_Road != Entity.Null));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "roadName", () => m_RoadName));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "token", () => m_Token));
            AddUpdateBinding(new GetterValueBinding<string>(Group, "message", () => m_Message));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "customCount", () => CustomRoadNameList.Names.Count));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "listError", () => CustomRoadNameList.HasReadError));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "namingStyle",
                () => (int)(Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American)));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "customMode",
                () => (int)(Mod.Settings?.CustomNames ?? CustomNameMode.Disabled)));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "autoNaming",
                () => Mod.Settings?.AutomaticallyRenameNewRoads ?? true));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "numberRamps",
                () => Mod.Settings?.NumberRamps ?? true));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "existingMode",
                () => (int)(Mod.Settings?.ExistingRoadCheckMode ?? ExistingRoadMode.GameGeneratedOnly)));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "overwriteNames",
                () => Mod.Settings?.OverwriteIndividuallyNamedRoads ?? false));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "detailedLogging",
                () => Mod.Settings?.EnableDetailedLogging ?? false));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "exclusionsEnabled",
                () => Mod.Settings?.UseNameExclusions ?? true));
            AddUpdateBinding(new GetterValueBinding<int>(Group, "excludedCount",
                () => RoadNameExclusionList.ActiveCount));
            AddUpdateBinding(new GetterValueBinding<bool>(Group, "exclusionError",
                () => RoadNameExclusionList.HasReadError));
            AddBinding(new TriggerBinding(Group, "reloadExclusions", () =>
            {
                if (Mod.Settings != null)
                    Mod.Settings.ReloadExcludedNames = true;
            }));
            AddBinding(new TriggerBinding(Group, "removeExclusions", () =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.UseNameExclusions = false;
                Mod.Settings.ApplyAndSave();
            }));
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                AddUpdateBinding(new GetterValueBinding<string>(Group, $"suggestion{i}",
                    () => index < m_Suggestions.Count ? m_Suggestions[index] : string.Empty));
            }
            AddBinding(new TriggerBinding(Group, "toggle", Toggle));
            AddBinding(new TriggerBinding(Group, "close", () => Clear()));
            AddBinding(new TriggerBinding(Group, "refresh", () => Refresh()));
            AddBinding(new TriggerBinding<string, string>(Group, "apply", Apply));
            AddBinding(new TriggerBinding(Group, "reloadList", CustomRoadNameList.Reload));
            AddBinding(new TriggerBinding(Group, "openFolder", CustomRoadNameList.OpenFolder));
            AddBinding(new TriggerBinding<int>(Group, "setNamingStyle", SetNamingStyle));
            AddBinding(new TriggerBinding<int>(Group, "setCustomMode", SetCustomMode));
            AddBinding(new TriggerBinding<int>(Group, "setAutoNaming", value =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.AutomaticallyRenameNewRoads = value != 0;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding<int>(Group, "setNumberRamps", value =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.NumberRamps = value != 0;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding<int>(Group, "setExistingMode", value =>
            {
                if (Mod.Settings == null ||
                    !Enum.IsDefined(typeof(ExistingRoadMode), value)) return;
                Mod.Settings.ExistingRoadCheckMode = (ExistingRoadMode)value;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding<int>(Group, "setOverwriteNames", value =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.OverwriteIndividuallyNamedRoads = value != 0;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding<int>(Group, "setExclusions", value =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.UseNameExclusions = value != 0;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding<int>(Group, "setDetailedLogging", value =>
            {
                if (Mod.Settings == null) return;
                Mod.Settings.EnableDetailedLogging = value != 0;
                Mod.Settings.ApplyAndSave();
            }));
            AddBinding(new TriggerBinding(Group, "scanExisting", () =>
            {
                if (Mod.Settings == null) return;
                Mod.RequestExistingRoadScan();
                m_Message = "RoadNameGenerator.UI.ScanQueued";
            }));
        }

        private void SetNamingStyle(int value)
        {
            if (Mod.Settings == null || !Enum.IsDefined(typeof(NamingStyle), value))
                return;
            Mod.Settings.SelectedNamingStyle = (NamingStyle)value;
            Mod.Settings.CustomNames = CustomNameMode.Disabled;
            Mod.Settings.ApplyAndSave();
            Refresh();
        }

        private void SetCustomMode(int value)
        {
            if (Mod.Settings == null ||
                (value != (int)CustomNameMode.CustomOnly && value != (int)CustomNameMode.Mixed))
                return;
            // Keep the predefined style for switching back and for mixed mode.
            Mod.Settings.CustomNames = (CustomNameMode)value;
            Mod.Settings.ApplyAndSave();
            Refresh();
        }

        private void Toggle()
        {
            if (!(Mod.Settings?.ShowRoadSuggestionsButton ?? true))
                return;
            m_Open = !m_Open;
            if (m_Open)
                Refresh();
            else
                Clear();
        }

        private void Clear()
        {
            m_Open = false;
            m_Road = Entity.Null;
            m_RoadName = string.Empty;
            m_Token = string.Empty;
            m_Message = string.Empty;
            m_Suggestions.Clear();
        }

        private void Refresh(string message = "")
        {
            if (!m_Open)
                return;
            m_Road = m_NamingSystem.ResolveRoad(m_SelectedInfo.selectedEntity);
            m_RoadName = m_NamingSystem.GetRoadName(m_Road);
            m_Style = Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American;
            m_CustomMode = Mod.Settings?.CustomNames ?? CustomNameMode.Disabled;
            m_ListRevision = CustomRoadNameList.Revision;
            m_ExclusionRevision = RoadNameExclusionList.Revision;
            m_NumberRamps = Mod.Settings?.NumberRamps ?? true;
            m_ExclusionsEnabled = Mod.Settings?.UseNameExclusions ?? true;
            m_Suggestions = m_NamingSystem.GetSuggestions(m_Road);
            m_Token = $"{m_Road.Index}:{m_Road.Version}:{++m_Revision}";
            m_Message = message;
        }

        private void Apply(string token, string name)
        {
            if (!m_Open || token != m_Token || !m_Suggestions.Contains(name))
                return;
            // Selection, settings and the city may change between preview and click.
            if (m_NamingSystem.ResolveRoad(m_SelectedInfo.selectedEntity) != m_Road ||
                m_Style != (Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American) ||
                m_CustomMode != (Mod.Settings?.CustomNames ?? CustomNameMode.Disabled) ||
                m_NumberRamps != (Mod.Settings?.NumberRamps ?? true) ||
                m_ListRevision != CustomRoadNameList.Revision ||
                m_ExclusionRevision != RoadNameExclusionList.Revision ||
                m_ExclusionsEnabled != (Mod.Settings?.UseNameExclusions ?? true))
            {
                Refresh("RoadNameGenerator.UI.Stale");
                return;
            }
            bool renamed = m_NamingSystem.TryApplySuggestion(m_Road, name);
            Refresh(renamed ? "RoadNameGenerator.UI.Applied" : "RoadNameGenerator.UI.Unavailable");
        }

        protected override void OnUpdate()
        {
            if (!(Mod.Settings?.ShowRoadSuggestionsButton ?? true))
                Clear();
            if (m_Open)
            {
                Entity selectedRoad = m_NamingSystem.ResolveRoad(m_SelectedInfo.selectedEntity);
                if (selectedRoad != m_Road || m_NamingSystem.GetRoadName(m_Road) != m_RoadName ||
                    m_Style != (Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American) ||
                    m_CustomMode != (Mod.Settings?.CustomNames ?? CustomNameMode.Disabled) ||
                    m_NumberRamps != (Mod.Settings?.NumberRamps ?? true) ||
                    m_ListRevision != CustomRoadNameList.Revision ||
                    m_ExclusionRevision != RoadNameExclusionList.Revision ||
                    m_ExclusionsEnabled != (Mod.Settings?.UseNameExclusions ?? true))
                    Refresh();
            }
            base.OnUpdate();
        }
    }
}
