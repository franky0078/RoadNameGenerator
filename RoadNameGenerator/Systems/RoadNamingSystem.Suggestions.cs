using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using Game.Tools;
using RoadNameGenerator.Naming;
using Unity.Collections;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{
    public partial class RoadNamingSystem
    {
        private readonly HashSet<string> m_UsedRoadNames = new(StringComparer.OrdinalIgnoreCase);

        public Entity ResolveRoad(Entity selected)
        {
            if (!IsUsableEntity(selected))
                return Entity.Null;
            if (EntityManager.HasComponent<Aggregated>(selected))
                selected = EntityManager.GetComponentData<Aggregated>(selected).m_Aggregate;
            return IsUsableEntity(selected) && EntityManager.HasBuffer<AggregateElement>(selected)
                && IsRoadAggregate(selected) ? selected : Entity.Null;
        }

        private bool IsUsableEntity(Entity entity) => entity != Entity.Null &&
            EntityManager.Exists(entity) && !EntityManager.HasComponent<Deleted>(entity) &&
            !EntityManager.HasComponent<Temp>(entity);

        public string GetRoadName(Entity road) => road != Entity.Null && ResolveRoad(road) == road
            ? m_NameSystem.GetRenderedLabelName(road) : string.Empty;

        private HashSet<string> GetCurrentRoadNames()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using NativeArray<Entity> roads = m_AggregateQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity road in roads)
            {
                if (!IsRoadAggregate(road))
                    continue;
                string name = m_NameSystem.GetRenderedLabelName(road);
                if (!string.IsNullOrWhiteSpace(name))
                    names.Add(name.Trim());
            }
            return names;
        }

        public List<string> GetSuggestions(Entity road)
        {
            var suggestions = new List<string>();
            if (road == Entity.Null || ResolveRoad(road) != road)
                return suggestions;

            HashSet<string> usedNames = GetCurrentRoadNames();
            IRoadNameGenerator generator = CreatePreviewGenerator(
                Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American);
            foreach (string name in usedNames)
                generator.RegisterExistingName(name);
            generator = new CustomRoadNameGenerator(generator,
                Mod.Settings?.CustomNames ?? CustomNameMode.Disabled,
                CustomRoadNameList.Names, usedNames);

            RoadCategory category = DetectRoadCategory(road);
            for (int i = 0; i < 5; i++)
            {
                string name = generator.Generate(category);
                if (string.IsNullOrWhiteSpace(name))
                    break;
                suggestions.Add(name);
            }
            return suggestions;
        }

        public bool TryApplySuggestion(Entity road, string name)
        {
            if (road == Entity.Null || ResolveRoad(road) != road || string.IsNullOrWhiteSpace(name) ||
                name.Length > 128 || RoadNameExclusionList.IsExcluded(name) || GetCurrentRoadNames().Contains(name.Trim()))
                return false;

            m_NameSystem.SetCustomName(road, name);
            RegisterExistingNameInAllGenerators(name);
            m_KnownRoads.Add(road);
            Mod.LogDetailed($"Selected road {road.Index} renamed to \"{name}\".");
            return true;
        }

        private static IRoadNameGenerator CreatePreviewGenerator(NamingStyle style) => style switch
        {
            NamingStyle.British => new BritishRoadNameGenerator(),
            NamingStyle.German => new GermanRoadNameGenerator(),
            NamingStyle.Italian => new ItalianRoadNameGenerator(),
            NamingStyle.French => new FrenchRoadNameGenerator(),
            NamingStyle.Swedish => new SwedishRoadNameGenerator(),
            NamingStyle.Spanish => new SpanishRoadNameGenerator(),
            NamingStyle.Canadian => new CanadianRoadNameGenerator(),
            NamingStyle.Australian => new AustralianRoadNameGenerator(),
            NamingStyle.Polish => new PolishRoadNameGenerator(),
            NamingStyle.PortuguesePortugal => new PortugueseRoadNameGenerator(),
            NamingStyle.Dutch => new DutchRoadNameGenerator(),
            NamingStyle.Swiss => new SwissRegionalRoadNameGenerator(NamingStyle.Swiss),
            NamingStyle.SwissFrench => new SwissRegionalRoadNameGenerator(NamingStyle.SwissFrench),
            NamingStyle.SwissItalian => new SwissRegionalRoadNameGenerator(NamingStyle.SwissItalian),
            NamingStyle.SwissRomansh => new SwissRegionalRoadNameGenerator(NamingStyle.SwissRomansh),
            NamingStyle.Norwegian => new NorwegianRoadNameGenerator(),
            NamingStyle.Danish => new DanishRoadNameGenerator(),
            NamingStyle.Mexican => new MexicanRoadNameGenerator(),
            _ => new AmericanRoadNameGenerator()
        };
    }
}
