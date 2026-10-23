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
            return narrow && oneWay;
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

        private List<HighwayRampConnection> FindHighwayRampConnections(Entity ramp)
        {
            var result = new List<HighwayRampConnection>();
            var visited = new HashSet<Entity> { ramp };
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
            // Do not infer traffic direction from Edge.m_Start / m_End:
            // those fields describe geometry, not which way cars can travel.
            // Until a verified directional lane API is available we use the
            // safe generic label rather than calling an on-ramp an off-ramp.
            return RampDirection.Unknown;
        }

        private bool TryGetRampParent(Entity road, out string highwayName, out RampDirection direction)
        {
            highwayName = string.Empty;
            direction = RampDirection.Unknown;
            if (!IsHighwayRampCandidate(road))
                return false;

            var connections = FindHighwayRampConnections(road);
            if (connections.Count == 0)
                return false;

            // Choose the nearest main highway; do not use an arbitrary road at the
            // opposite end of a multi-highway interchange.
            connections.Sort((a, b) =>
            {
                int byDepth = a.Depth.CompareTo(b.Depth);
                return byDepth != 0 ? byDepth : a.Highway.Index.CompareTo(b.Highway.Index);
            });

            HighwayRampConnection selected = connections[0];
            string selectedName = m_NameSystem.GetRenderedLabelName(selected.Highway);
            if (string.IsNullOrWhiteSpace(selectedName))
                return false;

            highwayName = selectedName.Trim();
            RampDirection detected = RampDirection.Unknown;
            bool ambiguous = false;

            foreach (HighwayRampConnection connection in connections)
            {
                string connectedName = m_NameSystem.GetRenderedLabelName(connection.Highway);
                if (!string.Equals(connectedName?.Trim(), highwayName, StringComparison.OrdinalIgnoreCase))
                {
                    ambiguous = true;
                    continue;
                }

                // Direction is only reliable at the actual connection, not on a
                // different segment further down the ramp chain.
                RampDirection current = GetDirectionAtHighway(connection.RampEdge, connection.Junction);
                if (current == RampDirection.Unknown)
                    continue;
                if (detected != RampDirection.Unknown && current != detected)
                    ambiguous = true;
                detected = current;
            }

            direction = ambiguous ? RampDirection.Unknown : detected;
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

        private bool TryCreateRampName(Entity aggregate, out string name)
        {
            name = string.Empty;
            if (!TryGetRampParent(aggregate, out string highwayName, out RampDirection direction))
                return false;

            NamingStyle style = Mod.Settings?.SelectedNamingStyle ?? NamingStyle.American;
            string label = GetRampLabel(style, direction);
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
