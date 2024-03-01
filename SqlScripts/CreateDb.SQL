USE [MASTER]
GO
CREATE DATABASE [MergeAndInsert];
GO

USE [MergeAndInsert]
GO

CREATE TABLE [dbo].[ParentTable] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
	[TraceId]			 UNIQUEIDENTIFIER NOT NULL,
    [Description]          VARCHAR (200)  NOT NULL,
    [SomeDateTimeUtc]    DATETIME        NOT NULL,
    CONSTRAINT [PK_ParentTable] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_ParentTable_TraceId] ON [dbo].[ParentTable]([TraceId] ASC);
GO

CREATE TABLE [dbo].[ChildTable] (
    [Id]                   INT             IDENTITY (1, 1) NOT NULL,
    [ParentTableId] INT             NOT NULL,
    [SomeMessage]        NVARCHAR (4000) NULL,
    [StatusDateTimeUtc]    DATETIME        NOT NULL,
    CONSTRAINT [PK_ChildTable] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChildTable_ParentTable_ParentTableId] FOREIGN KEY ([ParentTableId]) REFERENCES [dbo].[ParentTable] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_ChildTable_ParentTableId] ON [dbo].[ChildTable]([ParentTableId] ASC);
GO