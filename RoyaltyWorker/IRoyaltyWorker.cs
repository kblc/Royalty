using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository;

namespace RoyaltyWorker
{
    public interface IRoyaltyWorker
    {
        void AddFile(string file);
    }
}
