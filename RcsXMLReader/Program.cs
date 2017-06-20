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

                var l1 = doc.Descendants("F")
                    .GroupBy(x => x.Attribute("o").Value + x.Attribute("d").Value)
                    .ToLookup(a => a.Key,   // key is flow
                              a => a.GroupBy(b => b.Attribute("r").Value)
                                  .ToLookup(g => g.Key,   // key is route
                                            g => g.Elements("T").GroupBy(c => c.Attribute("t").Value)
                                                    .ToDictionary(m => m.Key,  // key is ticketcode
                                                              m => m.Elements("FF").Select(o => new
                                                              {
                                                                  StartDate = o.Attribute("f").Value,
                                                                  EndDate = o.Attribute("u").Value,
                                                                  SeasonIndicator = o.Attribute("s")?.Value,
                                                                  QuoteDate = o.Attribute("p")?.Value,
                                                                  Key = o.Attribute("k").Value
                                                              }
                                                              ).ToList())));
                var origin = "8681";
                var destination = "9998";
                var l2 = l1[origin + destination];

                foreach (var route in l2.SelectMany(x=>x))
                {
                    Console.WriteLine($"ROUTE: {route.Key}");
                    foreach (var ticket in route.SelectMany(x=>x))
                    {
                        Console.WriteLine($"ticket is {ticket.Key}");
                        var fflist = ticket.Value;
                        foreach (var ff in fflist)
                        {
                            var season = ff.SeasonIndicator == null ? "" : $" season {ff.SeasonIndicator} ";
                            var quote = ff.QuoteDate == null ? "" : $" quote {ff.QuoteDate} ";
                            Console.WriteLine($"start {ff.StartDate} end {ff.EndDate}" + season + quote +  $"key {ff.Key}");
                        }
                    }
                    Console.WriteLine("----------------");
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
