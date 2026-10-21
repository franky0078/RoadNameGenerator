using System;
using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Entities;

namespace RoadNameGenerator.Systems
{

    public struct AutomaticallyAssignedRoadName : IComponentData, ISerializable
    {
        public FixedString512Bytes Name;

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(1);
            writer.Write(Name.ToString());
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out int version);
            if (version != 1)
                throw new InvalidOperationException("Unsupported automatic road name data version.");
            reader.Read(out string name);
            Name = new FixedString512Bytes(name ?? string.Empty);
        }
    }
}
