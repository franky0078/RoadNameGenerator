// Road Name Generator - highway ramp naming
// Copyright (C) 2026 franky0078
using System;
using System.Collections.Generic;
using Game.Common;
using Game.Net;
using RoadNameGenerator.Naming;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{
    public partial class RoadNamingSystem
    {
        private enum RampDirection { Unknown, Exit, Entrance }
        private readonly HashSet<Entity> m_PendingRampNames = new();
        private int m_PendingRampCursor;
        private readonly HashSet<Entity> m_AmbiguousRampsLogged = new();

        private readonly struct HighwayRampConnection
        {
            public readonly Entity Highway;
            public readonly Entity RampEdge;
            public readonly Entity Junction;
            public readonly int Depth;

            public HighwayRampConnection(Entity highway, Entity rampEdge, Entity junction, int depth)
            {
                Highway = highway;
                RampEdge = rampEdge;
                Junction = junction;
                Depth = depth;
            }
        }

        // CS2 vanilla ramps are usually narrow one-way highways; custom assets
        // may explicitly identify themselves as ramps.
        private bool IsHighwayRampCandidate(Entity aggregate)
        {
            if (!IsUsableEntity(aggregate) || !IsRoadAggregate(aggregate))
                return false;

            string prefab = GetRoadPrefabName(aggregate);
            if (string.IsNullOrWhiteSpace(prefab))
                return false;

            string lower = prefab.ToLowerInvariant();
            if (ContainsAny(lower, "ramp", "slip road", "sliproad", "auffahrt", "abfahrt"))
                return true;

            if (DetectRoadCategory(aggregate) != RoadCategory.Highway)
                return false;

            bool narrow = ContainsAny(lower, "1 lane", "1-lane", "one lane", "one-lane",
                "1lane", "1l1w", "1l 1w", "single lane", "single-lane");
            bool oneWay = ContainsAny(lower, "one way", "one-way", "oneway", "1w");

            // Vanilla CS2 uses "Freeway 1 Lane" without "Oneway" in the
            // prefab name. A one-lane highway can serve as a connecting ramp,
            // but must be physically attached to a real motorway before naming.
            bool freeway = ContainsAny(lower, "freeway", "highway", "motorway", "expressway");
            return narrow && (oneWay || freeway);
        }

        private IEnumerable<(Entity Edge, Entity Junction)> GetAdjacentRoadEdges(Entity aggregate)
        {
            if (!EntityManager.HasBuffer<AggregateElement>(aggregate))
                yield break;

            var elements = EntityManager.GetBuffer<AggregateElement>(aggregate, true);
            // Avoid touching graph components on broken or incomplete temporary edges.
            for (int i = 0; i < elements.Length; i++)
            {
                Entity roadEdge = elements[i].m_Edge;
                if (!IsUsableEntity(roadEdge) || !EntityManager.HasComponent<Road>(roadEdge) ||
                    !EntityManager.HasComponent<Edge>(roadEdge))
                    continue;

                Edge edge = EntityManager.GetComponentData<Edge>(roadEdge);
                Entity[] nodes = { edge.m_Start, edge.m_End };
                foreach (Entity node in nodes)
                {
                    if (!IsUsableEntity(node) || !EntityManager.HasBuffer<ConnectedEdge>(node))
                        continue;

                    var connected = EntityManager.GetBuffer<ConnectedEdge>(node, true);
                    for (int c = 0; c < connected.Length; c++)
                    {
                        Entity other = connected[c].m_Edge;
                        if (other == roadEdge || !IsUsableEntity(other) ||
                            !EntityManager.HasComponent<Road>(other) ||
                            !EntityManager.HasComponent<Aggregated>(other))
                            continue;

                        yield return (other, node);
                    }
                }
            }
        }

        private List<HighwayRampConnection> FindHighwayRampConnections(
            Entity ramp, out HashSet<Entity> visited)
        {
            var result = new List<HighwayRampConnection>();
            visited = new HashSet<Entity> { ramp };
            var queue = new List<(Entity Aggregate, int Depth)> { (ramp, 0) };
            int nextIndex = 0;

            // Follow a small connected group of ramp aggregates (some junctions split one
            // physical ramp into several road names). Never traverse an ordinary highway.
            while (nextIndex < queue.Count && visited.Count <= 48)
            {
                var current = queue[nextIndex++];
                if (!EntityManager.HasBuffer<AggregateElement>(current.Aggregate))
                    continue;

                var elements = EntityManager.GetBuffer<AggregateElement>(current.Aggregate, true);
                for (int i = 0; i < elements.Length; i++)
                {
                    Entity rampEdge = elements[i].m_Edge;
                    if (!IsUsableEntity(rampEdge) || !EntityManager.HasComponent<Edge>(rampEdge))
                        continue;

                    Edge edge = EntityManager.GetComponentData<Edge>(rampEdge);
                    Entity[] nodes = { edge.m_Start, edge.m_End };
                    foreach (Entity node in nodes)
                    {
                        if (!IsUsableEntity(node) || !EntityManager.HasBuffer<ConnectedEdge>(node))
                            continue;

                        var connected = EntityManager.GetBuffer<ConnectedEdge>(node, true);
                        for (int c = 0; c < connected.Length; c++)
                        {
                            Entity otherEdge = connected[c].m_Edge;
                            if (otherEdge == rampEdge || !IsUsableEntity(otherEdge) ||
                                !EntityManager.HasComponent<Road>(otherEdge) ||
                                !EntityManager.HasComponent<Aggregated>(otherEdge))
                                continue;

                            Entity adjacent = EntityManager.GetComponentData<Aggregated>(otherEdge).m_Aggregate;
                            if (adjacent == current.Aggregate || !IsUsableEntity(adjacent) ||
                                !IsRoadAggregate(adjacent))
                                continue;

                            if (IsHighwayRampCandidate(adjacent))
                            {
                                if (current.Depth < 8 && visited.Add(adjacent))
                                    queue.Add((adjacent, current.Depth + 1));
                            }
                            else if (DetectRoadCategory(adjacent) == RoadCategory.Highway)
                            {
                                result.Add(new HighwayRampConnection(adjacent, rampEdge, node, current.Depth));
                            }
                        }
                    }
                }
            }
            return result;
        }

        // The PathNode positions of a real car lane determine the actual traffic
        // direction. Edge.m_Start/m_End alone does NOT reliably express traffic flow.
        private RampDirection GetDirectionAtHighway(Entity rampEdge, Entity junction)
        {
            if (!IsUsableEntity(rampEdge) ||
                !EntityManager.HasComponent<Edge>(rampEdge) ||
                !EntityManager.HasBuffer<SubLane>(rampEdge))
                return RampDirection.Unknown;

            Edge edge = EntityManager.GetComponentData<Edge>(rampEdge);
            bool atStart = edge.m_Start == junction;
            bool atEnd = edge.m_End == junction;
            if (atStart == atEnd)
                return RampDirection.Unknown;

            bool forward = false;
            bool backward = false;
            var subLanes = EntityManager.GetBuffer<SubLane>(rampEdge, true);
            for (int i = 0; i < subLanes.Length; i++)
            {
                Entity subLane = subLanes[i].m_SubLane;
                if (!IsUsableEntity(subLane) ||
                    !EntityManager.HasComponent<CarLane>(subLane) ||
                    !EntityManager.HasComponent<Lane>(subLane))
                    continue;

                Lane lane = EntityManager.GetComponentData<Lane>(subLane);
                int startOwner = lane.m_StartNode.GetOwnerIndex();
                int endOwner = lane.m_EndNode.GetOwnerIndex();

                // Vehicle lane PathNodes encode direction, while Edge.m_Start
                // and m_End only describe how the road geometry was created.
                if (startOwner == edge.m_Start.Index && endOwner == edge.m_End.Index)
                    forward = true;
                else if (startOwner == edge.m_End.Index && endOwner == edge.m_Start.Index)
                    backward = true;
                else if (startOwner == rampEdge.Index && endOwner == rampEdge.Index)
                {
                    float delta = lane.m_EndNode.GetCurvePos() - lane.m_StartNode.GetCurvePos();
                    if (delta > 0.5f) forward = true;
                    else if (delta < -0.5f) backward = true;
                }
            }

            if (forward == backward)
                return RampDirection.Unknown;

            // Driving away from the motorway is an exit, towards it an entry.
            bool awayFromMotorway = atStart ? forward : backward;
            return awayFromMotorway ? RampDirection.Exit : RampDirection.Entrance;
        }

        private bool TryGetRampParent(Entity road, out string highwayName,
            out RampDirection direction, out HashSet<Entity> group)
        {
            highwayName = string.Empty;
            direction = RampDirection.Unknown;
            group = new HashSet<Entity>();
            if (!IsHighwayRampCandidate(road))
                return false;

            var connections = FindHighwayRampConnections(road, out group);
            if (connections.Count == 0)
                return false;

            // If the ramp touches more than one motorway, prefer the highway
            // the traffic LEAVES (exit) instead of the one it enters. We only
            // fall back to distance if lane direction cannot be determined.
            connections.Sort((a, b) =>
            {
                RampDirection flowA = GetDirectionAtHighway(a.RampEdge, a.Junction);
                RampDirection flowB = GetDirectionAtHighway(b.RampEdge, b.Junction);
                int rankA = flowA == RampDirection.Exit ? 0 :
                    flowA == RampDirection.Entrance ? 1 : 2;
                int rankB = flowB == RampDirection.Exit ? 0 :
                    flowB == RampDirection.Entrance ? 1 : 2;
                int ranked = rankA.CompareTo(rankB);
                if (ranked != 0) return ranked;
                int depth = a.Depth.CompareTo(b.Depth);
                return depth != 0 ? depth : a.Highway.Index.CompareTo(b.Highway.Index);
            });

            HighwayRampConnection selected = connections[0];
            string selectedName = m_NameSystem.GetRenderedLabelName(selected.Highway);
            if (string.IsNullOrWhiteSpace(selectedName))
                return false;

            selectedName = selectedName.Trim();
            RampDirection selectedDirection =
                GetDirectionAtHighway(selected.RampEdge, selected.Junction);

            // Multiple equidistant highway names without usable directional
            // evidence are ambiguous. Do not attach a ramp to a random road.
            if (selectedDirection == RampDirection.Unknown)
            {
                foreach (HighwayRampConnection connection in connections)
                {
                    if (connection.Depth != selected.Depth)
                        continue;
                    string otherName = m_NameSystem.GetRenderedLabelName(connection.Highway);
                    if (!string.IsNullOrWhiteSpace(otherName) &&
                        !string.Equals(otherName.Trim(), selectedName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        if (m_AmbiguousRampsLogged.Add(road))
                            Mod.LogDetailed($"Ramp {road.Index}: conflicting motorway connections; " +
                                "waiting for an unambiguous parent instead of assigning the wrong name.");
                        return false;
                    }
                }
            }

            m_AmbiguousRampsLogged.Remove(road);
            highwayName = selectedName;
            direction = selectedDirection;
            Mod.LogDetailed($"Ramp {road.Index}: linked to motorway " +
                $"{selected.Highway.Index} (\"{highwayName}\"), " +
                $"direction: {direction}, group segments: {group.Count}.");
            return true;
        }

        private static string GetRampLabel(NamingStyle style, RampDirection direction)
        {
            return style switch
            {
                NamingStyle.German or NamingStyle.Swiss => direction switch
                {
                    RampDirection.Exit => "Abfahrt",
                    RampDirection.Entrance => "Auffahrt",
                    _ => "Rampe"
                },
                NamingStyle.French or NamingStyle.SwissFrench => direction switch
                {
                    RampDirection.Exit => "Sortie",
                    RampDirection.Entrance => "Entrée",
                    _ => "Bretelle"
                },
                NamingStyle.Italian or NamingStyle.SwissItalian => direction switch
                {
                    RampDirection.Exit => "Uscita",
                    RampDirection.Entrance => "Entrata",
                    _ => "Rampa"
                },
                NamingStyle.Spanish or NamingStyle.Mexican => direction switch
                {
                    RampDirection.Exit => "Salida",
                    RampDirection.Entrance => "Entrada",
                    _ => "Ramal"
                },
                NamingStyle.PortuguesePortugal or NamingStyle.PortugueseBrazil => direction switch
                {
                    RampDirection.Exit => "Saída",
                    RampDirection.Entrance => "Entrada",
                    _ => "Rampa"
                },
                NamingStyle.Dutch => direction switch
                {
                    RampDirection.Exit => "Afrit",
                    RampDirection.Entrance => "Oprit",
                    _ => "Verbindingsweg"
                },
                NamingStyle.Polish => direction switch
                {
                    RampDirection.Exit => "Zjazd",
                    RampDirection.Entrance => "Wjazd",
                    _ => "Łącznica"
                },
                NamingStyle.Norwegian => direction switch
                {
                    RampDirection.Exit => "Avkjøring",
                    RampDirection.Entrance => "Påkjøring",
                    _ => "Rampe"
                },
                NamingStyle.Danish => direction switch
                {
                    RampDirection.Exit => "Frakørsel",
                    RampDirection.Entrance => "Tilkørsel",
                    _ => "Rampe"
                },
                NamingStyle.Swedish => direction switch
                {
                    RampDirection.Exit => "Avfart",
                    RampDirection.Entrance => "Påfart",
                    _ => "Ramp"
                },
                _ => direction switch
                {
                    RampDirection.Exit => "Exit",
                    RampDirection.Entrance => "Entrance",
                    _ => "Ramp"
                }
            };
        }

        private static bool HasRampName(string candidate, string motorway, string label)
        {
            string prefix = motorway + " " + label + " ";
            return candidate.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(candidate.Substring(prefix.Length), out int number) &&
                number > 0;
        }

        // Existing pieces of the SAME physical ramp should reuse its number,
        // even if CS2 splits the aggregate while editing a junction.
        private bool TryGetGroupRampName(Entity current, HashSet<Entity> group,
            string motorwayName, string label, out string existingName)
        {
            existingName = string.Empty;
            foreach (Entity member in group)
            {
                if (member == current || !IsUsableEntity(member))
                    continue;
                string candidate = m_NameSystem.GetRenderedLabelName(member);
                if (HasRampName(candidate, motorwayName, label) &&
                    IsAutomaticallyAssignedName(member, candidate))
                {
                    existingName = candidate;
                    return true;
                }
            }
            return false;
        }

        private void ProcessPendingRampNames()
        {
            if (!(Mod.Settings?.AutomaticallyRenameNewRoads ?? true) ||
                m_PendingRampNames.Count == 0)
                return;

            // Work in bounded batches to avoid traversing the full road network
            // every update when many ramps have not yet been connected.
            int start = m_PendingRampCursor % m_PendingRampNames.Count;
            int index = 0;
            int attempted = 0;
            var completed = new List<Entity>();
            foreach (Entity road in m_PendingRampNames)
            {
                if (index++ < start)
                    continue;
                if (attempted >= 32)
                    break;
                attempted++;

                if (!IsUsableEntity(road) || !IsRoadAggregate(road))
                {
                    completed.Add(road);
                    continue;
                }

                string previous = m_NameSystem.GetRenderedLabelName(road);
                bool hasCustom = EntityManager.HasComponent<CustomName>(road);
                if (hasCustom && !IsAutomaticallyAssignedName(road, previous))
                {
                    completed.Add(road);
                    continue;
                }

                if (!IsHighwayRampCandidate(road))
                {
                    completed.Add(road);
                    continue;
                }

                if (!TryCreateRampName(road, out string newName))
                    continue;

                if (!string.Equals(newName, previous, StringComparison.Ordinal))
                {
                    m_NameSystem.SetCustomName(road, newName);
                    RememberAutomaticallyAssignedName(road, newName);
                    RegisterExistingNameInAllGenerators(newName);
                    Mod.LogDetailed($"Deferred ramp named: Entity {road.Index}, " +
                        $"Old name: \"{previous}\", new name: \"{newName}\".");
                }
                completed.Add(road);
            }

            foreach (Entity road in completed)
                m_PendingRampNames.Remove(road);

            // A removed entry can shift HashSet enumeration. We intentionally
            // rotate by a small batch instead of rescanning from the beginning.
            m_PendingRampCursor = m_PendingRampNames.Count == 0 ? 0 :
                (start + attempted) % m_PendingRampNames.Count;
        }

        private bool TryCreateRampName(Entity aggregate, out string name)
        {
            name = string.Empty;
            if (!TryGetRampParent(aggregate, out string highwayName,
                    out RampDirection direction, out HashSet<Entity> group))
                return false;

            NamingStyle style = Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American;
            string label = GetRampLabel(style, direction);
            if (TryGetGroupRampName(aggregate, group, highwayName, label, out string existing))
            {
                name = existing;
                return true;
            }
            for (int number = 1; number <= 4096; number++)
            {
                string suffix = $" {label} {number}";
                int limit = 128 - suffix.Length;
                if (limit <= 0)
                    return false;

                string baseName = highwayName.Length > limit
                    ? highwayName.Substring(0, limit).TrimEnd() : highwayName;
                string candidate = baseName + suffix;
                if (!m_UsedRoadNames.Contains(candidate) && !RoadNameExclusionList.IsExcluded(candidate))
                {
                    name = candidate;
                    return true;
                }
            }
            return false;
        }
    }
}
