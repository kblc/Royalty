using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator
{
    public class DataPreviewRow
    {
        public Parser.Address IncomingAddress { get; set; }
        public Parser.Host IncomingHost { get; set; }
        public Parser.Phone IncomingPhone { get; set; }
        public string IncomingArea { get; set; }
        public string IncomingCity { get; set; }
        public string IncomingMark { get; set; }

        public RoyaltyRepository.Models.City City { get; set; }
        public bool IsNewCity { get; set; }

        public RoyaltyRepository.Models.Mark Mark { get; set; }

        public RoyaltyRepository.Models.Phone Phone { get; set; }
        public bool IsNewPhone { get; set; }

        public RoyaltyRepository.Models.Host Host { get; set; }
        public bool IsNewHost { get; set; }

        public RoyaltyRepository.Models.Street Street { get; set; }
        public bool IsNewStreet { get; set; }
        public string House { get; set; }
    }
}
