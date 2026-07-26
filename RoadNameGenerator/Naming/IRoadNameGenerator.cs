using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadNameGenerator.Naming
{
    public interface IRoadNameGenerator
    {
        string Generate(RoadCategory category);

        bool IsNameFromPortfolio(
            string name,
            RoadCategory category
        );

        void RegisterExistingName(string name);
    }
}
