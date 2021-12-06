using System;
using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Fields;

namespace Kysect.Centum.Sample
{
    internal static class FieldsSample
    {
        public static void Run()
        {
            var e1 = Fields<Sheet>.From(s => s.Properties);
            var e2 = Fields<Sheet>.FromMany(
                s => s.Properties,
                p => p.Hidden, p => p.Index);
            var e3 = Fields<Spreadsheet>.FromSequence<Sheet>(
                sp => sp.Sheets,
                s => s.Charts, s => s.Data);
            
            Console.WriteLine(e1);
            Console.WriteLine(e2);
            Console.WriteLine(e3);
        }
    }
}