﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MergeAndInsert.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeAndInsert
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class MergeAndInsertBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void WithEFCore()
        {
            WithEFCore(Guid.Empty);
            WithEFCore(Guid.NewGuid());
        }
        private void WithEFCore(Guid traceId)
        {
            using var dbContext = new MergeAndInsertContext();
            {
                var existingParent = dbContext.ParentTables.AsNoTracking()
                .Select(p => new { p.Id, p.TraceId })
                .FirstOrDefault(p => p.TraceId == traceId);

                var child = new ChildTable
                {
                    SomeMessage = "WithEFCore",
                    StatusDateTimeUtc = DateTime.UtcNow
                };

                if (existingParent is null)
                {
                    dbContext.ParentTables.Add(new ParentTable
                    {
                        TraceId = traceId,
                        Description = "WithEFCore",
                        SomeDateTimeUtc = DateTime.UtcNow,
                        ChildTables = new List<ChildTable> { child }
                    });
                }
                else
                {
                    child.ParentTableId = existingParent.Id;
                    dbContext.ChildTables.Add(child);
                }
                dbContext.SaveChanges();
            }
        }

        [Benchmark]
        public void WithInlineSql()
        {
            WithInlineSql(Guid.Empty);
            WithInlineSql(Guid.NewGuid());
        }
        private void WithInlineSql(Guid traceId)
        {
            using var dbContext = new MergeAndInsertContext();
            {
                var sql = @"DECLARE @InsertedIDs TABLE (ID INT);
                        MERGE TOP (1) INTO dbo.ParentTable as Target
                        USING 
                        (VALUES (@traceId, @parentDescription)
                        ) AS Source (TraceId, Description) 
                        ON Target.TraceId = Source.TraceId
                        WHEN NOT MATCHED BY Target THEN
                            INSERT (TraceId, Description, SomeDateTimeUtc) VALUES (Source.TraceId, Source.Description, GETUTCDATE())
                        WHEN MATCHED THEN
	                        UPDATE SET Target.TraceId = Source.TraceId
                        OUTPUT inserted.ID INTO @InsertedIDs; 
                        INSERT INTO [dbo].[ChildTable] (ParentTableId, SomeMessage, StatusDateTimeUtc)
                        VALUES ((select top 1 ID from @InsertedIDs), @childMessage, GETUTCDATE())";

                object[] parameters = new object[]
                {
                new SqlParameter("@traceId", traceId),
                new SqlParameter("@parentDescription", "WithInlineSql"),
                new SqlParameter("@childMessage", "WithInlineSql"),
                };

                dbContext.Database.ExecuteSqlRaw(sql, parameters);
            }
        }

    }
}
