using MergeAndInsert.Data;
using Microsoft.EntityFrameworkCore;

namespace MergeAndInsert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using(var db = new MergeAndInsertContext())
            {
                var data= db.ParentTables.Include(t=> t.ChildTables).ToList();

                foreach(var pt in data)
                {
                    Console.WriteLine($"{pt.Id},\t{pt.TraceId},\t{pt.Description},\t{pt.SomeDateTimeUtc}");
                    foreach(var ct in pt.ChildTables)
                    {
                        Console.WriteLine($"{ct.Id},\t{ct.ParentTableId},\t{ct.SomeMessage},\t{ct.StatusDateTimeUtc}");
                    }
                }
            }
            Console.WriteLine("ALL DONE!");
            Console.ReadKey();
        }
    }
}
