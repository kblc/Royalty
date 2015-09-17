using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator
{
    public class DataPreviewRow
    {
        public AddressParser.Address IncomingAddress { get; set; }
        
        public RoyaltyRepository.Models.Street Street { get; set; }
        public string House { get; set; }
    }
}
