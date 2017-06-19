using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RcsXMLReader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Count() < 1)
                {
                    throw new Exception("You must supply a Parkeon RCS XML filename.");
                }
                var filename = args[0];
                var doc = XDocument.Load(filename);
                var l = (from frecord in doc.Descendants("F")
                         let Route = frecord.Attribute("r")?.Value
                         select new
                         {
                             Key = new
                             {
                                 Origin = frecord.Attribute("o")?.Value,
                                 Destination = frecord.Attribute("d")?.Value
                             },
                             TicketList = (from ticket in frecord.Elements("T")
                                           select new {
                                               Route,
                                               TicketCode = ticket.Attribute("T")?.Value,
                                               FFList = from ff in ticket.Elements("FF")
                                                        select new RCSFF
                                                        {
                                                            StartDate = ""
                                                            //StartDate = ff.Attribute("u")?.Value,
                                                            //EndDate = ff.Attribute("f")?.Value,
                                                            //SeasonIndicator = ff.Attribute("s")?.Value,
                                                            //QuoteDate = ff.Attribute("p")?.Value,
                                                            //Key = ff.Attribute("k")?.Value
                                                        }
                                              }
                         ).ToDictionary(x => x.Route, x => x.TicketCode)
                         }
            );

                var l1 = l.ToLookup(x => x.Key, x=>x.TicketList);
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}


//tickets = from ticket in frecord.Elements("T")
//                                           select new RCSTicket
//                                           {
//                                               TicketCode = ticket.Attribute("t").Value,
//                                               FFList = (from ff in ticket.Elements("FF")
//                                                     select new RCSFF
//                                                     {

//                                                     }).ToList()
//                                           }
//                             }
