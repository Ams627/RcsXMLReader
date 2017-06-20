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
                    .ToLookup(a => a.Key,   // key is flow
                              a => a.GroupBy(b => b.Attribute("r").Value)
                                  .ToLookup(g => g.Key,   // key is route
                                            g => g.GroupBy(c=>c.Element("T").Attribute("t").Value)
                                                    .ToLookup(m=>m.Key,  // key is ticketcode
                                                              m=>m.Elements("T").Select(f=>f.Element("FF"))
                                                                  .Select(ff=>new RCSFF {
                                                                                EndDate = ff.Attribute("u").Value,
                                                                                StartDate = ff.Attribute("f").Value,
                                                                                QuoteDate = ff.Attribute("p")?.Value,
                                                                                SeasonIndicator = ff.Attribute("s")?.Value,
                                                                                Key = ff.Attribute("k")?.Value,
                                                                            }).ToList())));

                var l2 = doc.Descendants("F")
                    .GroupBy(x => x.Attribute("o").Value + x.Attribute("d").Value)
                    .ToLookup(a => a.Key,                                           
                              a => a.Select(b => b.Attribute("r").Value));

                var routes = l1["90139998"];
                foreach (var route in routes.SelectMany(x=>x))
                {
                    Console.WriteLine($"{route.Key}");
                    foreach (var ticket in route.SelectMany(x => x))
                    {
                        Console.WriteLine($"ticket: {ticket.Key}");
                        foreach (var ff in ticket.SelectMany(x => x))
                        {
                            Console.WriteLine($"start: {ff.StartDate} end: {ff.EndDate} key: {ff.Key}");
                        }
                    }
                }

                foreach (var flow in l1)
                {
                    Console.WriteLine($"{flow.Key}");
                    foreach (var route in flow.SelectMany(x=>x))
                    {
                        Console.WriteLine($"route key is {route.Key}");
                        foreach (var ticket in route.SelectMany(x=>x))
                        {
                            Console.WriteLine($"ticket key is {ticket.Key}");
                            foreach (var ff in ticket.SelectMany(x=>x))
                            {
                                Console.WriteLine($"");
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
