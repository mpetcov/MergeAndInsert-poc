use [MergeAndInsert]
go

BEGIN TRAN

DECLARE @InsertedIDs TABLE (ID INT);

MERGE TOP (1) INTO dbo.ParentTable as Target
USING 
(VALUES ('00000000-9F5D-4D3C-9EDC-65BCBEA2D88F', 'Some Description')
) AS Source (TraceId, Description) 
ON Target.TraceId = Source.TraceId
WHEN NOT MATCHED BY Target THEN
    INSERT (TraceId, Description, SomeDateTimeUtc) VALUES (Source.TraceId, Source.Description, GETUTCDATE())
WHEN MATCHED THEN
	UPDATE SET Target.TraceId = Source.TraceId
		-- this is the match condition, nothing will be changed by the UPDATE
		-- I only do this to bring the record into the 'inserted' special table so I can output the ID
OUTPUT inserted.ID INTO @InsertedIDs; 

INSERT INTO [dbo].[ChildTable] (ParentTableId, SomeMessage, StatusDateTimeUtc)
-- Assumption: only one row will be MERGED into ParentTable at a time
-- we will always have only one record in the @InsertedIDs
VALUES ((select top 1 ID from @InsertedIDs), 'Some Message', GETUTCDATE())

select * from ParentTable
select * from ChildTable

ROLLBACK
--COMMIT