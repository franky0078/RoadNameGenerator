using System;
using System.Text;
using Game.UI;
using Unity.Collections;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{
    public partial class RoadNamingSystem
    {
        private void RememberAutomaticallyAssignedName(Entity road, string name)
        {

            if (string.IsNullOrEmpty(name) || Encoding.UTF8.GetByteCount(name) > 509)
            {
                ForgetAutomaticallyAssignedName(road);
                return;
            }

            var data = new AutomaticallyAssignedRoadName
            {
                Name = new FixedString512Bytes(name)
            };
            if (EntityManager.HasComponent<AutomaticallyAssignedRoadName>(road))
                EntityManager.SetComponentData(road, data);
            else
                EntityManager.AddComponentData(road, data);
        }

        private void ForgetAutomaticallyAssignedName(Entity road)
        {
            if (EntityManager.HasComponent<AutomaticallyAssignedRoadName>(road))
                EntityManager.RemoveComponent<AutomaticallyAssignedRoadName>(road);
        }

        private bool IsAutomaticallyAssignedName(Entity road, string currentName)
        {
            if (!EntityManager.HasComponent<AutomaticallyAssignedRoadName>(road))
                return false;

            var data = EntityManager.GetComponentData<AutomaticallyAssignedRoadName>(road);
            if (EntityManager.HasComponent<CustomName>(road) &&
                string.Equals(data.Name.ToString(), currentName, StringComparison.Ordinal))
                return true;

            ForgetAutomaticallyAssignedName(road);
            return false;
        }
    }
}
