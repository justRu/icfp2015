using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Um.Engine;

namespace Um
{
    class Program
    {
        static void Main(string[] args)
        {
            //var file = @"E:\Dev\Um\sandmark.umz";
            var files = new[]
            {
                @"E:\Dev\Um\um1.um"
                //@"E:\Dev\Um\codex.umz"
            };
            var log = new List<byte>(1024*1024);
            var redirect = false;
            if (args != null && args.Any())
                files = args;
            try
            {
                var proc = new Processor();
                proc.LoadStreams(files.Select(f=>File.OpenRead(f)));

                proc.Output = (x) =>
                {
                    if(!redirect)
                        Console.Write((char) x);
                    else
                        log.Add((byte)x);
                };
                proc.Input = () =>
                {
                    var c = (uint) Console.Read();
                    if (c == 96 ) // `
                    {
                        Console.WriteLine("Redirected!");
                        redirect = true;
                        return proc.Input();
                    }
                    return c;
                };

                while (true)
                {
                    proc.Pulse();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Exception:");
                Console.WriteLine(ex);
                if (log.Any())
                {
                    var name = string.Format(@"E:\Dev\Um\Log{0:yy-MM-dd_hh-mm}.log", DateTime.Now);
                    Console.WriteLine("dumping log to {0}", name);
                    File.WriteAllBytes(name, log.ToArray());
                }
                Console.WriteLine("anykey...");
                Console.ReadKey();
            }
        }
    }
}
