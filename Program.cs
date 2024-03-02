using BenchmarkDotNet.Running;
using MergeAndInsert.Data;
using Microsoft.EntityFrameworkCore;

namespace MergeAndInsert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ResetDatabase();

            BenchmarkRunner.Run<MergeAndInsertBenchmarks>();
        }

        static void ResetDatabase()
        {
            using (var dbContext = new MergeAndInsertContext())
            {
                var sql = $@"DELETE FROM [dbo].[ChildTable];
                            DELETE FROM [dbo].[ParentTable];
                            INSERT INTO [dbo].[ParentTable] (TraceId, Description, SomeDateTimeUtc) 
                            VALUES ('{Guid.Empty}', 'Initial Parent Seed', GETUTCDATE());";
                dbContext.Database.ExecuteSqlRaw(sql);
            }
        }
    }
}
