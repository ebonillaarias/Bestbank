DECLARE @var83 sysname;
SELECT @var83 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sessions]') AND [c].[name] = N'StartDate');
IF @var83 IS NOT NULL EXEC(N'ALTER TABLE [Sessions] DROP CONSTRAINT [' + @var83 + '];');
ALTER TABLE [Sessions] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Sessions] ADD DEFAULT '2019-12-19T15:27:25.3866336+00:00' FOR [StartDate];

GO

DECLARE @var84 sysname;
SELECT @var84 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'StartDate');
IF @var84 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var84 + '];');
ALTER TABLE [Calls] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2019-12-19T15:27:25.3679064+00:00' FOR [StartDate];

GO

DECLARE @var85 sysname;
SELECT @var85 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'EndDate');
IF @var85 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var85 + '];');
ALTER TABLE [Calls] ALTER COLUMN [EndDate] datetimeoffset NULL;

GO

DECLARE @var86 sysname;
SELECT @var86 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'StartDate');
IF @var86 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var86 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2019-12-19T15:27:25.3747280+00:00' FOR [StartDate];

GO

DECLARE @var87 sysname;
SELECT @var87 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'EndDate');
IF @var87 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var87 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [EndDate] datetimeoffset NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191219152725_CallCallPartRemoveEndDateRequiered2', N'3.0.1');

GO