using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Royalty.ViewModels.Additional
{
    public class LoadType
    {
        public string Name { get; private set; }
        public bool ForAnalize { get; private set; }

        public LoadType(string name, bool forAnalize)
        {
            Name = name;
            ForAnalize = forAnalize;
        }
    }
}
