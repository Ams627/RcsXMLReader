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
                //var l = (from frecord in doc.Descendants("F")
                //         let Route = frecord.Attribute("r")?.Value
                //         select new
                //         {
                //             Key = frecord.Attribute("o")?.Value + frecord.Attribute("d")?.Value,
                //             RouteToTicketList = new
                //             { Route,
                //                 TicketList = (from ticket in frecord.Elements("T")
                //                               select new {
                //                                   TicketCode = ticket.Attribute("T")?.Value,
                //                                   FFList = from ff in ticket.Elements("FF")
                //                                            select new RCSFF
                //                                            {
                //                                                StartDate = ff.Attribute("u")?.Value,
                //                                                EndDate = ff.Attribute("f")?.Value,
                //                                                SeasonIndicator = ff.Attribute("s")?.Value,
                //                                                QuoteDate = ff.Attribute("p")?.Value,
                //                                                Key = ff.Attribute("k")?.Value
                //                                            }
                //                               }
                //                )
                //             }
                //         }
                //);

                //  var l1 = l.ToLookup(x => x.Key, x=>x.TicketList);


                var l1 = doc.Descendants("F")
                    .GroupBy(x => x.Attribute("o").Value + x.Attribute("d").Value)
                    .ToLookup(a => a.Key,                                           // flow
                              a => a.GroupBy(b => b.Attribute("r").Value)
                                  .ToLookup(g => g.Key,                             // route
                                            g => g.GroupBy(c=>c.Element("T").Attribute("t").Value)
                                                    .ToLookup(m=>m.Key,             // ticket code
                                                              m=>m.Elements("T").Select(f=>f.Element("FF")).Select(ff=>new RCSFF { EndDate = ff.Attribute("u").Value}).ToList())));

                var l2 = doc.Descendants("F")
                    .GroupBy(x => x.Attribute("o").Value + x.Attribute("d").Value)
                    .ToLookup(a => a.Key,                                           // flow
                              a => a.Select(b => b.Attribute("r").Value));

                var r = l1["09909998"];
                var r2 = l2["09909998"];

                foreach (var flow in l1)
                {
                    Console.WriteLine($"{flow.Key}");
                    foreach (var routelookup in flow)
                    {
                        foreach (var route in routelookup)
                        {
                            Console.WriteLine($"route key is {route.Key}");
                            foreach (var ticketlookup in route)
                            {
                                foreach (var ticket in ticketlookup)
                                {
                                    Console.WriteLine($"{ticket.Key}");
                                    foreach (var fflookup in ticket)
                                    {
                                        Console.WriteLine($"");
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);

                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
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
