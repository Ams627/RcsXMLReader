using System.Collections.Generic;
using System.Linq;

namespace RcsXMLReader
{
    class RCSTicket : System.IEquatable<RCSTicket>
    {
        public string TicketCode { get; set; }
        public List<RCSFF> FFList { get; set; }
        public RCSTicket()
        {
            FFList = new List<RCSFF>();
        }

        public bool Equals(RCSTicket other)
        {
            bool result1 = true;
            if (TicketCode != other.TicketCode)
            {
                result1 = false;
            }
            else if (FFList.Count != other.FFList.Count)
            {
                result1 = false;
            }
            else
            {
                if (!FFList.SequenceEqual(other.FFList))
                {
                    result1 = false;
                }
            }
            return result1;
        }
    }
}
