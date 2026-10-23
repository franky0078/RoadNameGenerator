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
        private readonly ItalianRoadNameGenerator m_ItalianNameGenerator = new();
        private readonly FrenchRoadNameGenerator m_FrenchNameGenerator = new();
        private readonly SwedishRoadNameGenerator m_SwedishNameGenerator = new();
        private readonly SpanishRoadNameGenerator m_SpanishNameGenerator = new();
        private readonly CanadianRoadNameGenerator m_CanadianNameGenerator = new();
        private readonly AustralianRoadNameGenerator m_AustralianNameGenerator = new();
        private readonly PolishRoadNameGenerator m_PolishNameGenerator = new();
        private readonly PortugueseRoadNameGenerator m_PortugueseNameGenerator = new();
        private readonly BrazilianRoadNameGenerator m_BrazilianNameGenerator = new();
        private readonly DutchRoadNameGenerator m_DutchNameGenerator = new();
        private readonly SwissRegionalRoadNameGenerator m_SwissNameGenerator =
            new(NamingStyle.Swiss);
        private readonly SwissRegionalRoadNameGenerator m_SwissFrenchNameGenerator =
            new(NamingStyle.SwissFrench);
        private readonly SwissRegionalRoadNameGenerator m_SwissItalianNameGenerator =
            new(NamingStyle.SwissItalian);
        private readonly SwissRegionalRoadNameGenerator m_SwissRomanshNameGenerator =
            new(NamingStyle.SwissRomansh);
        private readonly NorwegianRoadNameGenerator m_NorwegianNameGenerator = new();
        private readonly DanishRoadNameGenerator m_DanishNameGenerator = new();
        private readonly MexicanRoadNameGenerator m_MexicanNameGenerator = new();

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

        private bool TryDetectRoadBuilderCategory(
            Entity aggregateEntity,
            string prefabName,
            out RoadCategory detectedCategory)
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

                double speedKmh =
                    speedLimit / 2.0;

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
                             "tiled",
                             "pathway",
                             "pedestrian"))
                {
                    detectedCategory =
                        RoadCategory.Alley;
                }
                else if (ContainsAny(
                             normalizedCategory,
                             "publictransport",
                             "public transport",
                             "transit"))
                {
                    detectedCategory =
                        RoadCategory.Standard;
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
                else if (isOneWay &&
                         carLaneCount <= 2 &&
                         shoulderCount > 0 &&
                         sidewalkCount == 0)
                {
                    detectedCategory =
                        RoadCategory.Alley;
                }
                else if (carLaneCount >= 4)
                {
                    detectedCategory =
                        RoadCategory.Avenue;
                }
                else if (medianCount > 0 &&
                         carLaneCount >= 3)
                {
                    detectedCategory =
                        RoadCategory.Avenue;
                }
                else if (carLaneCount <= 3 &&
                         sidewalkCount > 0 &&
                         medianCount == 0 &&
                         speedKmh <= 50)
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
                    $"Road Builder road detected automatically: " +
                    $"Prefab: \"{prefabName}\", " +
                    $"Name: \"{configurationName}\", " +
                    $"RB category: \"{roadBuilderCategory}\", " +
                    $"Speed limit: {speedKmh:0.#} km/h " +
                    $"(RB internal: {speedLimit}), " +
                    $"Car lanes: {carLaneCount}, " +
                    $"Medians: {medianCount}, " +
                    $"Sidewalks: {sidewalkCount}, " +
                    $"Shoulders: {shoulderCount}, " +
                    $"One-way road: {isOneWay}, " +
                    $"Detected category: {detectedCategory}."
                );

                return true;
            }
            catch (TargetInvocationException exception)
            {
                Exception actualException =
                    exception.InnerException ??
                    exception;

                Mod.Log.Warn(
                    $"Road Builder road could not be " +
                    $"analyzed: {actualException}"
                );

                return false;
            }
            catch (Exception exception)
            {
                Mod.Log.Warn(
                    $"Road Builder road could not be " +
                    $"analyzed: {exception}"
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

            Mod.LogDetailed("RoadNamingSystem was created.");
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
                // Retry ramps that were built before their motorway was
                // connected or were named by an older version of the mod.
                ScheduleTrackedRampRefresh();
                ProcessPendingRampNames();

                if (existingRoadScanRequested)
                {
                    ScanExistingRoads();

                    // Mainline names can change during a manual full scan.
                    // Update mod-owned ramp names afterwards.
                    foreach (Entity ramp in m_TrackedRampNames)
                        m_PendingRampNames.Add(ramp);
                    ProcessPendingRampNames();
                }
            }
            catch (Exception exception)
            {
                Mod.Log.Error(
                    $"Error in RoadNamingSystem: {exception}"
                );
            }
        }

        private IRoadNameGenerator GetSelectedNameGenerator()
        {
            NamingStyle selectedStyle =
                Mod.Settings?.SelectedNamingStyle
                ?? NamingStyle.American;

            IRoadNameGenerator generator = selectedStyle switch
            {
                NamingStyle.British =>
                    m_BritishNameGenerator,

                NamingStyle.German =>
                    m_GermanNameGenerator,

                NamingStyle.Italian =>
                    m_ItalianNameGenerator,

                NamingStyle.French =>
                    m_FrenchNameGenerator,

                NamingStyle.Swedish =>
                    m_SwedishNameGenerator,

                NamingStyle.Spanish =>
                    m_SpanishNameGenerator,

                NamingStyle.Canadian =>
                    m_CanadianNameGenerator,

                NamingStyle.Australian =>
                    m_AustralianNameGenerator,

                NamingStyle.Polish =>
                    m_PolishNameGenerator,

                NamingStyle.PortugueseBrazil =>
                    m_BrazilianNameGenerator,
                NamingStyle.PortuguesePortugal =>
                    m_PortugueseNameGenerator,

                NamingStyle.Dutch =>
                    m_DutchNameGenerator,

                NamingStyle.Swiss =>
                    m_SwissNameGenerator,

                NamingStyle.SwissFrench =>
                    m_SwissFrenchNameGenerator,

                NamingStyle.SwissItalian =>
                    m_SwissItalianNameGenerator,

                NamingStyle.SwissRomansh =>
                    m_SwissRomanshNameGenerator,

                NamingStyle.Norwegian =>
                    m_NorwegianNameGenerator,

                NamingStyle.Danish =>
                    m_DanishNameGenerator,

                NamingStyle.Mexican =>
                    m_MexicanNameGenerator,

                _ =>
                    m_AmericanNameGenerator
            };

            return new CustomRoadNameGenerator(generator,
                Mod.Settings?.CustomNames ?? CustomNameMode.Disabled,
                CustomRoadNameList.Names, m_UsedRoadNames);
        }

        private void RegisterExistingNameInAllGenerators(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                m_UsedRoadNames.Add(name.Trim());
            m_AmericanNameGenerator.RegisterExistingName(name);
            m_BritishNameGenerator.RegisterExistingName(name);
            m_GermanNameGenerator.RegisterExistingName(name);
            m_ItalianNameGenerator.RegisterExistingName(name);
            m_FrenchNameGenerator.RegisterExistingName(name);
            m_SwedishNameGenerator.RegisterExistingName(name);
            m_SpanishNameGenerator.RegisterExistingName(name);
            m_CanadianNameGenerator.RegisterExistingName(name);
            m_AustralianNameGenerator.RegisterExistingName(name);
            m_PolishNameGenerator.RegisterExistingName(name);
            m_PortugueseNameGenerator.RegisterExistingName(name);
            m_BrazilianNameGenerator.RegisterExistingName(name);
            m_DutchNameGenerator.RegisterExistingName(name);
            m_SwissNameGenerator.RegisterExistingName(name);
            m_SwissFrenchNameGenerator.RegisterExistingName(name);
            m_SwissItalianNameGenerator.RegisterExistingName(name);
            m_SwissRomanshNameGenerator.RegisterExistingName(name);
            m_NorwegianNameGenerator.RegisterExistingName(name);
            m_DanishNameGenerator.RegisterExistingName(name);
            m_MexicanNameGenerator.RegisterExistingName(name);
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
                    // Drop automatic ownership after an external/manual name change.
                    if (EntityManager.HasComponent<AutomaticallyAssignedRoadName>(aggregateEntity))
                        IsAutomaticallyAssignedName(aggregateEntity,
                            m_NameSystem.GetRenderedLabelName(aggregateEntity));
                    if (Mod.Settings?.CustomNames != CustomNameMode.Disabled)
                    {
                        string registeredName = m_NameSystem.GetRenderedLabelName(aggregateEntity);
                        if (!string.IsNullOrWhiteSpace(registeredName) &&
                            !m_UsedRoadNames.Contains(registeredName.Trim()))
                            RegisterExistingNameInAllGenerators(registeredName);
                    }

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
                        $"The internal road registry was reloaded. " +
                        $"{currentRoads.Count} roads were registered again, " +
                        $"no roads were renamed."
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

                // Main motorways must receive their names before their ramps.
                unknownRoads.Sort((a, b) =>
                    IsHighwayRampCandidate(a).CompareTo(IsHighwayRampCandidate(b)));

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

                    // A newly discovered aggregate may already carry a player-assigned
                    // or inherited custom name. Automatic naming must preserve it.
                    if (EntityManager.HasComponent<CustomName>(aggregateEntity))
                    {
                        RegisterExistingNameInAllGenerators(currentName);
                        Mod.LogDetailed(
                            $"Newly detected road {aggregateEntity.Index} already has a custom name; " +
                            $"preserved: \"{currentName}\"."
                        );
                        continue;
                    }

                    string prefabName =
                        GetRoadPrefabName(aggregateEntity);

                    RoadCategory category =
                        DetectRoadCategory(aggregateEntity);

                    bool isRamp = IsHighwayRampCandidate(aggregateEntity);
                    string newName;
                    if (isRamp)
                    {
                        if (!TryCreateRampName(aggregateEntity, out newName))
                        {
                            // The motorway may be connected later; retry.
                            m_PendingRampNames.Add(aggregateEntity);
                            RegisterExistingNameInAllGenerators(currentName);
                            continue;
                        }
                    }
                    else
                    {
                        newName = nameGenerator.Generate(category);
                    }

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        RegisterExistingNameInAllGenerators(currentName);
                        Mod.LogDetailed("No allowed road name available; the current name was preserved.");
                        continue;
                    }

                    m_NameSystem.SetCustomName(
                        aggregateEntity,
                        newName
                    );
                    RememberAutomaticallyAssignedName(aggregateEntity, newName);

                    if (isRamp)
                        m_TrackedRampNames.Add(aggregateEntity);

                    RegisterExistingNameInAllGenerators(
                        newName
                    );

                    renamedRoadCount++;

                    Mod.LogDetailed(
                        $"New road renamed: " +
                        $"Entity {aggregateEntity.Index}, " +
                        $"Prefab: \"{prefabName}\", " +
                        $"Category: {category}, " +
                        $"Naming style: {selectedStyle}, " +
                        $"Previous name: \"{currentName}\", " +
                        $"New name: \"{newName}\""
                    );
                }

                if (renamedRoadCount > 0)
                {
                    Mod.LogDetailed(
                        $"Total road segments renamed: " +
                        $"{renamedRoadCount}"
                    );
                }

                if (registeredWithoutRenameCount > 0)
                {
                    Mod.LogDetailed(
                        $"{registeredWithoutRenameCount} new " +
                        $"road segments were not renamed because " +
                        $"automatic road naming is " +
                        $"disabled."
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
                // Name mainlines before ramps to avoid inheriting obsolete
                // highway names during a complete existing-road scan.
                var orderedRoads = new List<Entity>();
                foreach (Entity road in aggregateEntities)
                    if (IsRoadAggregate(road))
                        orderedRoads.Add(road);
                orderedRoads.Sort((a, b) =>
                    IsHighwayRampCandidate(a).CompareTo(IsHighwayRampCandidate(b)));

                foreach (Entity aggregateEntity in orderedRoads)
                {
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

                    bool isRamp = IsHighwayRampCandidate(aggregateEntity);
                    string newName;
                    if (isRamp)
                    {
                        if (!TryCreateRampName(aggregateEntity, out newName))
                        {
                            preservedRoadCount++;
                            continue;
                        }
                    }
                    else
                    {
                        newName = nameGenerator.Generate(category);
                    }

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        RegisterExistingNameInAllGenerators(currentName);
                        Mod.LogDetailed("No allowed road name available; the current name was preserved.");
                        continue;
                    }

                    m_NameSystem.SetCustomName(
                        aggregateEntity,
                        newName
                    );
                    RememberAutomaticallyAssignedName(aggregateEntity, newName);

                    if (isRamp)
                        m_TrackedRampNames.Add(aggregateEntity);

                    RegisterExistingNameInAllGenerators(
                        newName
                    );

                    renamedRoadCount++;

                    Mod.LogDetailed(
                        $"Existing road renamed: " +
                        $"Entity {aggregateEntity.Index}, " +
                        $"Prefab: \"{prefabName}\", " +
                        $"Category: {category}, " +
                        $"Naming style: {selectedStyle}, " +
                        $"Previous name: \"{currentName}\", " +
                        $"New name: \"{newName}\""
                    );
                }
            }
            finally
            {
                aggregateEntities.Dispose();
            }

            Mod.LogDetailed(
                $"Existing road scan completed. " +
                $"Checked: {checkedRoadCount}, " +
                $"renamed: {renamedRoadCount}, " +
                $"preserved: {preservedRoadCount}, " +
                $"Mode: {checkMode}, " +
                $"Overwrite individually named roads: {Mod.Settings?.OverwriteIndividuallyNamedRoads ?? false}, " +
                $"Naming style: {selectedStyle}."
            );
        }

        private bool ShouldRenameExistingRoad(Entity roadEntity, string currentName, RoadCategory category, ExistingRoadMode checkMode, IRoadNameGenerator nameGenerator)
        {
            bool hasCustomName = EntityManager.HasComponent<CustomName>(roadEntity);
            bool automaticallyAssigned = IsAutomaticallyAssignedName(roadEntity, currentName);
            if (hasCustomName && !automaticallyAssigned &&
                !(Mod.Settings?.OverwriteIndividuallyNamedRoads ?? false))
            {
                return false;
            }

            switch (checkMode)
            {
                case ExistingRoadMode.GameGeneratedOnly:
                    {
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
                    $"Unknown road prefab: " +
                    $"\"{cleanedPrefabName}\". " +
                    $"Temporary category: Standard."
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
                return "Unknown";
            }

            if (!EntityManager.HasComponent<PrefabRef>(roadEdge))
            {
                return "No PrefabRef";
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
                    return "Prefab not found";
                }

                return prefab.name;
            }
            catch (Exception exception)
            {
                Mod.Log.Warn(
                    $"Prefab name for road edge " +
                    $"{roadEdge.Index} could not be read: " +
                    $"{exception.Message}"
                );

                return "Prefab access error";
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

                    // Recover one-lane freeways incorrectly given independent
                    // motorway numbers by an earlier ramp-naming build.
                    if (IsHighwayRampCandidate(aggregateEntity) &&
                        IsAutomaticallyAssignedName(aggregateEntity, existingName))
                    {
                        m_TrackedRampNames.Add(aggregateEntity);
                        m_PendingRampNames.Add(aggregateEntity);
                    }
                }
                catch (Exception exception)
                {
                    Mod.Log.Warn(
                        $"Name of entity {aggregateEntity.Index} " +
                        $"could not be read: {exception.Message}"
                    );
                }

                roadCount++;
            }

            m_InitialRoadsLoaded = true;

            Mod.LogDetailed(
                $"Existing road network registered: {roadCount}"
            );

            Mod.LogDetailed(
                "New roads will now be named using the selected naming style."
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
            m_PendingRampNames.RemoveWhere(
                roadEntity => !existingRoads.Contains(roadEntity)
            );
            m_TrackedRampNames.RemoveWhere(
                roadEntity => !existingRoads.Contains(roadEntity)
            );
            // Keep diagnostic state bounded when road aggregates are deleted.
            var obsoleteLogEntities = new List<Entity>();
            foreach (Entity roadEntity in m_LastRampLinkLog.Keys)
                if (!existingRoads.Contains(roadEntity))
                    obsoleteLogEntities.Add(roadEntity);
            foreach (Entity roadEntity in obsoleteLogEntities)
            {
                m_LastRampLinkLog.Remove(roadEntity);
                m_AmbiguousRampsLogged.Remove(roadEntity);
            }
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
            m_PendingRampNames.Clear();
            m_TrackedRampNames.Clear();
            m_LastRampLinkLog.Clear();
            m_AmbiguousRampsLogged.Clear();
            m_UnknownRoadPrefabs.Clear();
            m_RoadBuilderCategoryCache.Clear();

            Mod.LogDetailed("RoadNamingSystem is shutting down.");

            base.OnDestroy();
        }
    }
}
