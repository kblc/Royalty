using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    public class Host
    {
        public string Hostname { get; private set; }

        public static Host FromString(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return new Host() { Hostname = uri.Host.ToLower() };
            }
            return new Host() { Hostname = string.Empty };
        }
    }
}
