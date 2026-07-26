// Road Name Generator
// Copyright (C) 2026 franky0078
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3
// or, at your option, any later version.

using Game;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Game.UI;
using RoadNameGenerator.Naming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{
    public partial class RoadNamingSystem : GameSystemBase
    {
        private EntityQuery m_AggregateQuery;
        private NameSystem m_NameSystem;
        private readonly AmericanRoadNameGenerator m_AmericanNameGenerator = new();
        private readonly BritishRoadNameGenerator m_BritishNameGenerator = new();
        private readonly GermanRoadNameGenerator m_GermanNameGenerator = new();
        
        private PrefabSystem m_PrefabSystem;
        private readonly HashSet<Entity> m_KnownRoads = new();
        private int m_UpdateCounter;
        private bool m_InitialRoadsLoaded;
        private const int CHECK_INTERVAL = 30;

        private readonly Dictionary<string, RoadCategory> m_RoadBuilderCategoryCache =
            new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, RoadCategory> s_CustomRoadCategories =
            new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Suburban Road",
                RoadCategory.Residential
            }
        };

        private readonly HashSet<string> m_UnknownRoadPrefabs =
            new(StringComparer.OrdinalIgnoreCase);

        private object GetRoadBuilderConfiguration(Entity aggregateEntity)
        {
            Type roadBuilderSystemType =
                FindLoadedType(
                    "RoadBuilder.Systems.RoadBuilderSystem"
                );

            if (roadBuilderSystemType == null)
            {
                return null;
            }

            object roadBuilderSystem =
                GetExistingManagedSystem(
                    roadBuilderSystemType
                );

            if (roadBuilderSystem == null)
            {
                return null;
            }

            Entity roadEdge =
                GetFirstRoadEdge(aggregateEntity);

            if (roadEdge == Entity.Null)
            {
                return null;
            }

            MethodInfo getConfigurationMethod =
                roadBuilderSystemType.GetMethod(
                    "GetOrGenerateConfiguration",
                    BindingFlags.Public |
                    BindingFlags.Instance,
                    null,
                    new[]
                    {
                typeof(Entity)
                    },
                    null
                );

            if (getConfigurationMethod == null)
            {
                return null;
            }

            return getConfigurationMethod.Invoke(
                roadBuilderSystem,
                new object[]
                {
            roadEdge
                }
            );
        }

        private bool TryDetectRoadBuilderCategory(Entity aggregateEntity, string prefabName, out RoadCategory detectedCategory)
        {
            detectedCategory =
                RoadCategory.Standard;


            if (m_RoadBuilderCategoryCache.TryGetValue(
                    prefabName,
                    out RoadCategory cachedCategory))
            {
                detectedCategory =
                    cachedCategory;

                return true;
            }

            try
            {
                object configuration =
                    GetRoadBuilderConfiguration(
                        aggregateEntity
                    );

                if (configuration == null)
                {
                    return false;
                }

                string configurationName =
                    GetPropertyValueAsText(
                        configuration,
                        "Name"
                    );

                string roadBuilderCategory =
                    GetPropertyValueAsText(
                        configuration,
                        "Category"
                    );

                string speedLimitText =
                    GetPropertyValueAsText(
                        configuration,
                        "SpeedLimit"
                    );

                double.TryParse(
                    speedLimitText,
                    out double speedLimit
                );

                object lanesObject =
                    configuration
                        .GetType()
                        .GetProperty(
                            "Lanes",
                            BindingFlags.Public |
                            BindingFlags.Instance
                        )
                        ?.GetValue(configuration);

                int carLaneCount = 0;
                int medianCount = 0;
                int sidewalkCount = 0;
                int shoulderCount = 0;

                bool hasInvertedCarLane = false;
                bool hasNonInvertedCarLane = false;

                if (lanesObject is IEnumerable lanes)
                {
                    foreach (object lane in lanes)
                    {
                        if (lane == null)
                        {
                            continue;
                        }

                        string groupPrefabName =
                            GetPropertyValueAsText(
                                lane,
                                "GroupPrefabName"
                            );

                        string invertText =
                            GetPropertyValueAsText(
                                lane,
                                "Invert"
                            );

                        bool isInverted =
                            bool.TryParse(
                                invertText,
                                out bool invertValue
                            ) &&
                            invertValue;

                        if (groupPrefabName.Contains(
                                "CarGroupPrefab",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            carLaneCount++;

                            if (isInverted)
                            {
                                hasInvertedCarLane = true;
                            }
                            else
                            {
                                hasNonInvertedCarLane = true;
                            }
                        }

                        if (groupPrefabName.Contains(
                                "MedianGroupPrefab",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            medianCount++;
                        }

                        if (groupPrefabName.Contains(
                                "SidewalkGroupPrefab",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            sidewalkCount++;
                        }

                        if (groupPrefabName.Contains(
                                "ShoulderGroupPrefab",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            shoulderCount++;
                        }
                    }
                }

                bool isOneWay =
                    carLaneCount > 0 &&
                    hasInvertedCarLane !=
                    hasNonInvertedCarLane;

                string normalizedName =
                    configurationName
                        .Trim()
                        .ToLowerInvariant();

                string normalizedCategory =
                    roadBuilderCategory
                        .Trim()
                        .ToLowerInvariant();

                if (ContainsAny(
                        normalizedCategory,
                        "gravel",
                        "dirt",
                        "unpaved") ||
                    ContainsAny(
                        normalizedName,
                        "gravel",
                        "dirt",
                        "unpaved",
                        "schotter",
                        "feldweg"))
                {
                    detectedCategory =
                        RoadCategory.Dirt;
                }
                else if (ContainsAny(
                             normalizedCategory,
                             "highway",
                             "motorway") ||
                         ContainsAny(
                             normalizedName,
                             "highway",
                             "motorway",
                             "freeway",
                             "expressway",
                             "autobahn"))
                {
                    detectedCategory =
                        RoadCategory.Highway;
                }
                else if (ContainsAny(
                             normalizedName,
                             "gasse",
                             "alley",
                             "mews",
                             "passage"))
                {
                    detectedCategory =
                        RoadCategory.Alley;
                }
                else if (ContainsAny(
                             normalizedName,
                             "residential",
                             "suburban",
                             "wohnstraße",
                             "wohnstrasse",
                             "neighbourhood",
                             "neighborhood"))
                {
                    detectedCategory =
                        RoadCategory.Residential;
                }

                else if (medianCount > 0)
                {
                    detectedCategory =
                        RoadCategory.Avenue;
                }
                else if (carLaneCount >= 4)
                {
                    detectedCategory =
                        RoadCategory.Avenue;
                }
                else if (isOneWay &&
                         carLaneCount <= 2 &&
                         shoulderCount > 0 &&
                         sidewalkCount == 0)
                {

                    detectedCategory =
                        RoadCategory.Alley;
                }
                else if (carLaneCount <= 2 &&
                         speedLimit <= 50)
                {
                    detectedCategory =
                        RoadCategory.Residential;
                }
                else if (carLaneCount <= 2 &&
                         sidewalkCount > 0 &&
                         speedLimit <= 60)
                {
                    detectedCategory =
                        RoadCategory.Residential;
                }
                else
                {
                    detectedCategory =
                        RoadCategory.Standard;
                }

                m_RoadBuilderCategoryCache[prefabName] =
                    detectedCategory;

                Mod.LogDetailed(
                    $"Road-Builder-Straße automatisch erkannt: " +
                    $"Prefab: \"{prefabName}\", " +
                    $"Name: \"{configurationName}\", " +
                    $"RB-Kategorie: \"{roadBuilderCategory}\", " +
                    $"Geschwindigkeit: {speedLimit}, " +
                    $"Fahrspuren: {carLaneCount}, " +
                    $"Mittelstreifen: {medianCount}, " +
                    $"Gehwege: {sidewalkCount}, " +
                    $"Seitenstreifen: {shoulderCount}, " +
                    $"Einbahnstraße: {isOneWay}, " +
                    $"erkannte Kategorie: {detectedCategory}."
                );

                return true;
            }
            catch (TargetInvocationException exception)
            {
                Exception actualException =
                    exception.InnerException ??
                    exception;

                Mod.Log.Warn(
                    $"Road-Builder-Straße konnte nicht " +
                    $"analysiert werden: {actualException}"
                );

                return false;
            }
            catch (Exception exception)
            {
                Mod.Log.Warn(
                    $"Road-Builder-Straße konnte nicht " +
                    $"analysiert werden: {exception}"
                );

                return false;
            }
        }

        private static bool ContainsAny(string source, params string[] searchTerms)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            foreach (string searchTerm in searchTerms)
            {
                if (source.Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


        private static Type FindLoadedType(string fullTypeName)
        {
            Assembly[] loadedAssemblies =
                AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in loadedAssemblies)
            {
                Type foundType =
                    assembly.GetType(
                        fullTypeName,
                        false
                    );

                if (foundType != null)
                {
                    return foundType;
                }
            }

            return null;
        }

        private object GetExistingManagedSystem(Type systemType)
        {
            object existingSystem =
                InvokeWorldSystemMethod(
                    "GetExistingSystemManaged",
                    systemType
                );

            if (existingSystem != null)
            {
                return existingSystem;
            }

            return InvokeWorldSystemMethod(
                "GetOrCreateSystemManaged",
                systemType
            );
        }

        private object InvokeWorldSystemMethod(string methodName, Type systemType)
        {
            MethodInfo[] methods =
                typeof(World).GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance
                );

            foreach (MethodInfo method in methods)
            {
                if (method.Name != methodName ||
                    !method.IsGenericMethodDefinition ||
                    method.GetParameters().Length != 0)
                {
                    continue;
                }

                try
                {
                    MethodInfo concreteMethod =
                        method.MakeGenericMethod(
                            systemType
                        );

                    return concreteMethod.Invoke(
                        World,
                        null
                    );
                }
                catch
                {

                }
            }

            return null;
        }

        private Entity GetFirstRoadEdge(Entity aggregateEntity)
        {
            DynamicBuffer<AggregateElement> elements =
                EntityManager.GetBuffer<AggregateElement>(
                    aggregateEntity
                );

            foreach (AggregateElement element in elements)
            {
                Entity roadEdge =
                    element.m_Edge;

                if (roadEdge != Entity.Null &&
                    EntityManager.HasComponent<Road>(
                        roadEdge
                    ))
                {
                    return roadEdge;
                }
            }

            return Entity.Null;
        }

        private static string GetPropertyValueAsText(object source, string propertyName)
        {
            if (source == null)
            {
                return string.Empty;
            }

            PropertyInfo property =
                source
                    .GetType()
                    .GetProperty(
                        propertyName,
                        BindingFlags.Public |
                        BindingFlags.Instance
                    );

            object value =
                property?.GetValue(source);

            return value?.ToString() ??
                   string.Empty;
        }




        protected override void OnCreate()
        {
            base.OnCreate();

            m_AggregateQuery = GetEntityQuery(
                ComponentType.ReadOnly<Aggregate>(),
                ComponentType.ReadOnly<AggregateElement>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>()
            );

            m_NameSystem =
                World.GetOrCreateSystemManaged<NameSystem>();
            m_PrefabSystem =
                World.GetOrCreateSystemManaged<PrefabSystem>();

            Mod.LogDetailed("RoadNamingSystem wurde erstellt.");
        }

        protected override void OnUpdate()
        {
            bool existingRoadScanRequested =
                Mod.ConsumeExistingRoadScanRequest();

            m_UpdateCounter++;

            if (m_UpdateCounter < CHECK_INTERVAL &&
                !existingRoadScanRequested)
            {
                return;
            }

            m_UpdateCounter = 0;

            try
            {
  
                CheckForNewRoads();

                if (existingRoadScanRequested)
                {
                    ScanExistingRoads();
                }
            }
            catch (Exception exception)
            {
                Mod.Log.Error(
                    $"Fehler im RoadNamingSystem: {exception}"
                );
            }
        }

        private IRoadNameGenerator GetSelectedNameGenerator()
        {
            NamingStyle selectedStyle =
                Mod.Settings?.SelectedNamingStyle
                ?? NamingStyle.American;

            return selectedStyle switch
            {
                NamingStyle.British =>
                    m_BritishNameGenerator,

                NamingStyle.German =>
                    m_GermanNameGenerator,

                _ =>
                    m_AmericanNameGenerator
            };
        }

        private void RegisterExistingNameInAllGenerators(string name)
        {
            m_AmericanNameGenerator.RegisterExistingName(name);
            m_BritishNameGenerator.RegisterExistingName(name);
            m_GermanNameGenerator.RegisterExistingName(name);
        }

        private void CheckForNewRoads()
        {
            NativeArray<Entity> aggregateEntities =
                m_AggregateQuery.ToEntityArray(Allocator.Temp);

            try
            {
                if (!m_InitialRoadsLoaded)
                {
                    LoadExistingRoads(aggregateEntities);
                    return;
                }

                List<Entity> currentRoads = new();
                List<Entity> unknownRoads = new();

                foreach (Entity aggregateEntity in aggregateEntities)
                {
                    if (!IsRoadAggregate(aggregateEntity))
                    {
                        continue;
                    }

                    currentRoads.Add(aggregateEntity);

                    if (!m_KnownRoads.Contains(aggregateEntity))
                    {
                        unknownRoads.Add(aggregateEntity);
                    }
                }

                bool possibleReload =
                    currentRoads.Count > 0 &&
                    unknownRoads.Count >=
                        currentRoads.Count * 0.5f;

                if (possibleReload)
                {
                    m_KnownRoads.Clear();

                    foreach (Entity roadEntity in currentRoads)
                    {
                        m_KnownRoads.Add(roadEntity);

                        string existingName =
                            m_NameSystem.GetRenderedLabelName(
                                roadEntity
                            );

                        RegisterExistingNameInAllGenerators(
                            existingName
                        );
                    }

                    Mod.LogDetailed(
                        $"Straßenbestand wurde intern neu geladen. " +
                        $"{currentRoads.Count} Straßen neu registriert, " +
                        $"keine Umbenennung durchgeführt."
                    );

                    return;
                }

                bool automaticallyRename =
                    Mod.Settings?.AutomaticallyRenameNewRoads
                    ?? true;

                IRoadNameGenerator nameGenerator =
                    GetSelectedNameGenerator();

                NamingStyle selectedStyle =
                    Mod.Settings?.SelectedNamingStyle
                    ?? NamingStyle.American;

                int renamedRoadCount = 0;
                int registeredWithoutRenameCount = 0;

                foreach (Entity aggregateEntity in unknownRoads)
                {
                    m_KnownRoads.Add(aggregateEntity);

                    string currentName =
                        m_NameSystem.GetRenderedLabelName(
                            aggregateEntity
                        );

                    if (!automaticallyRename)
                    {
                        RegisterExistingNameInAllGenerators(
                            currentName
                        );

                        registeredWithoutRenameCount++;
                        continue;
                    }

                    string prefabName =
                        GetRoadPrefabName(aggregateEntity);

                    RoadCategory category =
                        DetectRoadCategory(aggregateEntity);

                    string newName =
                        nameGenerator.Generate(category);

                    m_NameSystem.SetCustomName(
                        aggregateEntity,
                        newName
                    );

                    RegisterExistingNameInAllGenerators(
                        newName
                    );

                    renamedRoadCount++;

                    Mod.LogDetailed(
                        $"Neue Straße umbenannt: " +
                        $"Entity {aggregateEntity.Index}, " +
                        $"Prefab: \"{prefabName}\", " +
                        $"Kategorie: {category}, " +
                        $"Namensstil: {selectedStyle}, " +
                        $"alter Name: \"{currentName}\", " +
                        $"neuer Name: \"{newName}\""
                    );
                }

                if (renamedRoadCount > 0)
                {
                    Mod.LogDetailed(
                        $"Insgesamt umbenannte Straßenzüge: " +
                        $"{renamedRoadCount}"
                    );
                }

                if (registeredWithoutRenameCount > 0)
                {
                    Mod.LogDetailed(
                        $"{registeredWithoutRenameCount} neue " +
                        $"Straßenzüge wurden nicht umbenannt, " +
                        $"da die automatische Benennung " +
                        $"deaktiviert ist."
                    );
                }

                RemoveDeletedRoads(aggregateEntities);
            }
            finally
            {
                aggregateEntities.Dispose();
            }
        }

        private void ScanExistingRoads()
        {
            ExistingRoadMode checkMode =
                Mod.Settings?.ExistingRoadCheckMode
                ?? ExistingRoadMode.GameGeneratedOnly;

            NamingStyle selectedStyle =
                Mod.Settings?.SelectedNamingStyle
                ?? NamingStyle.American;

            IRoadNameGenerator nameGenerator =
                GetSelectedNameGenerator();

            NativeArray<Entity> aggregateEntities =
                m_AggregateQuery.ToEntityArray(Allocator.Temp);

            int checkedRoadCount = 0;
            int renamedRoadCount = 0;
            int preservedRoadCount = 0;

            try
            {
                foreach (Entity aggregateEntity in aggregateEntities)
                {
                    if (!IsRoadAggregate(aggregateEntity))
                    {
                        continue;
                    }

                    checkedRoadCount++;

                    string currentName =
                        m_NameSystem.GetRenderedLabelName(
                            aggregateEntity
                        );

                    string prefabName =
                        GetRoadPrefabName(aggregateEntity);

                    RoadCategory category =
                        DetectRoadCategory(aggregateEntity);

                    if (!ShouldRenameExistingRoad(
                            aggregateEntity,
                            currentName,
                            category,
                            checkMode,
                            nameGenerator))
                    {
                        preservedRoadCount++;
                        continue;
                    }

                    string newName =
                        nameGenerator.Generate(category);

                    m_NameSystem.SetCustomName(
                        aggregateEntity,
                        newName
                    );

                    RegisterExistingNameInAllGenerators(
                        newName
                    );

                    renamedRoadCount++;

                    Mod.LogDetailed(
                        $"Bestehende Straße umbenannt: " +
                        $"Entity {aggregateEntity.Index}, " +
                        $"Prefab: \"{prefabName}\", " +
                        $"Kategorie: {category}, " +
                        $"Namensstil: {selectedStyle}, " +
                        $"alter Name: \"{currentName}\", " +
                        $"neuer Name: \"{newName}\""
                    );
                }
            }
            finally
            {
                aggregateEntities.Dispose();
            }

            Mod.LogDetailed(
                $"Bestandsprüfung abgeschlossen. " +
                $"Geprüft: {checkedRoadCount}, " +
                $"umbenannt: {renamedRoadCount}, " +
                $"beibehalten: {preservedRoadCount}, " +
                $"Modus: {checkMode}, " +
                $"Namensstil: {selectedStyle}."
            );
        }

        private bool ShouldRenameExistingRoad(Entity roadEntity, string currentName, RoadCategory category, ExistingRoadMode checkMode, IRoadNameGenerator nameGenerator)
        {
            switch (checkMode)
            {
                case ExistingRoadMode.GameGeneratedOnly:
                    {
                        bool hasCustomName =
                            EntityManager.HasComponent<CustomName>(
                                roadEntity
                            );

                        return !hasCustomName;
                    }

                case ExistingRoadMode.NonMatchingPortfolio:
                    {
                        return !nameGenerator.IsNameFromPortfolio(
                            currentName,
                            category
                        );
                    }

                case ExistingRoadMode.AllRoads:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        private bool TryGetRoadEdge(Entity aggregateEntity, out Entity roadEdge)
        {
            roadEdge = Entity.Null;

            if (!EntityManager.HasBuffer<AggregateElement>(aggregateEntity))
            {
                return false;
            }

            DynamicBuffer<AggregateElement> elements =
                EntityManager.GetBuffer<AggregateElement>(
                    aggregateEntity,
                    true
                );

            foreach (AggregateElement element in elements)
            {
                Entity edgeEntity = element.m_Edge;

                if (edgeEntity == Entity.Null)
                {
                    continue;
                }

                if (!EntityManager.HasComponent<Road>(edgeEntity))
                {
                    continue;
                }

                roadEdge = edgeEntity;
                return true;
            }

            return false;
        }

        private RoadCategory DetectRoadCategory(Entity aggregateEntity)
        {
            string prefabName =
                GetRoadPrefabName(aggregateEntity);

            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return RoadCategory.Standard;
            }

            string cleanedPrefabName =
                prefabName.Trim();

            if (s_CustomRoadCategories.TryGetValue(
                    cleanedPrefabName,
                    out RoadCategory mappedCategory))
            {
                return mappedCategory;
            }

            string normalizedName =
                cleanedPrefabName.ToLowerInvariant();


            if (normalizedName.Contains("gravel") ||
                normalizedName.Contains("dirt") ||
                normalizedName.Contains("unpaved"))
            {
                return RoadCategory.Dirt;
            }


            if (normalizedName.Contains("alley"))
            {
                return RoadCategory.Alley;
            }


            if (normalizedName.Contains("highway") ||
                normalizedName.Contains("freeway") ||
                normalizedName.Contains("expressway") ||
                normalizedName.Contains("motorway"))
            {
                return RoadCategory.Highway;
            }


            if (normalizedName.Contains("large") ||
                normalizedName.Contains("xl road") ||
                normalizedName.Contains("arterial") ||
                normalizedName.Contains("boulevard") ||
                normalizedName.Contains("medium road divided"))
            {
                return RoadCategory.Avenue;
            }


            if (normalizedName.Contains("medium road oneway") &&
                normalizedName.Contains("5 lanes"))
            {
                return RoadCategory.Avenue;
            }

            if (normalizedName.Contains("suburban") ||
                normalizedName.Contains("residential") ||
                normalizedName.Contains("neighborhood") ||
                normalizedName.Contains("neighbourhood") ||
                normalizedName.Contains("local road") ||
                normalizedName.Contains("small"))
            {
                return RoadCategory.Residential;
            }


            if (normalizedName.Contains("medium") ||
                normalizedName.Contains("collector"))
            {
                return RoadCategory.Standard;
            }


            if (TryDetectRoadBuilderCategory(
                    aggregateEntity,
                    cleanedPrefabName,
                    out RoadCategory roadBuilderCategory))
            {
                return roadBuilderCategory;
            }


            if (m_UnknownRoadPrefabs.Add(cleanedPrefabName))
            {
                Mod.LogDiagnosticWarning(
                    $"Unbekannter Straßen-Prefab: " +
                    $"\"{cleanedPrefabName}\". " +
                    $"Vorläufige Kategorie: Standard."
                );
            }

            return RoadCategory.Standard;
        }

        private string GetRoadPrefabName(Entity aggregateEntity)
        {
            if (!TryGetRoadEdge(
                    aggregateEntity,
                    out Entity roadEdge))
            {
                return "Unbekannt";
            }

            if (!EntityManager.HasComponent<PrefabRef>(roadEdge))
            {
                return "Kein PrefabRef";
            }

            PrefabRef prefabRef =
                EntityManager.GetComponentData<PrefabRef>(roadEdge);

            if (prefabRef.m_Prefab == Entity.Null)
            {
                return "Prefab Entity.Null";
            }

            try
            {
                PrefabBase prefab =
                    m_PrefabSystem.GetPrefab<PrefabBase>(
                        prefabRef.m_Prefab
                    );

                if (prefab == null)
                {
                    return "Prefab nicht gefunden";
                }

                return prefab.name;
            }
            catch (Exception exception)
            {
                Mod.Log.Warn(
                    $"Prefabname für Straßenkante " +
                    $"{roadEdge.Index} konnte nicht gelesen werden: " +
                    $"{exception.Message}"
                );

                return "Fehler beim Prefabzugriff";
            }
        }

        private void LoadExistingRoads(NativeArray<Entity> aggregateEntities)
        {
            int roadCount = 0;

            foreach (Entity aggregateEntity in aggregateEntities)
            {
                if (!IsRoadAggregate(aggregateEntity))
                {
                    continue;
                }

                m_KnownRoads.Add(aggregateEntity);

                try
                {
                    string existingName =
                        m_NameSystem.GetRenderedLabelName(aggregateEntity);

                    RegisterExistingNameInAllGenerators(existingName);
                }
                catch (Exception exception)
                {
                    Mod.Log.Warn(
                        $"Name von Entity {aggregateEntity.Index} " +
                        $"konnte nicht gelesen werden: {exception.Message}"
                    );
                }

                roadCount++;
            }

            m_InitialRoadsLoaded = true;

            Mod.LogDetailed(
                $"Vorhandener Straßenbestand registriert: {roadCount}"
            );

            Mod.LogDetailed(
                "Ab jetzt werden neue Straßen nach dem ausgewählten Namensstil benannt."
            );
        }

        private void RemoveDeletedRoads(NativeArray<Entity> aggregateEntities)
        {
            HashSet<Entity> existingRoads = new();

            foreach (Entity aggregateEntity in aggregateEntities)
            {
                if (IsRoadAggregate(aggregateEntity))
                {
                    existingRoads.Add(aggregateEntity);
                }
            }

            m_KnownRoads.RemoveWhere(
                roadEntity => !existingRoads.Contains(roadEntity)
            );
        }

        private bool IsRoadAggregate(Entity aggregateEntity)
        {
            return TryGetRoadEdge(
                aggregateEntity,
                out _
            );
        }

        protected override void OnDestroy()
        {
            m_KnownRoads.Clear();
            m_UnknownRoadPrefabs.Clear();
            m_RoadBuilderCategoryCache.Clear();

            Mod.LogDetailed("RoadNamingSystem wird beendet.");

            base.OnDestroy();
        }
    }
}