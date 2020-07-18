DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sessions]') AND [c].[name] = N'StartDate');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [Sessions] DROP CONSTRAINT [' + @var78 + '];');
ALTER TABLE [Sessions] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Sessions] ADD DEFAULT '2019-12-19T13:30:49.6763131+00:00' FOR [StartDate];

GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'StartDate');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var79 + '];');
ALTER TABLE [Calls] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2019-12-19T13:30:49.6644825+00:00' FOR [StartDate];

GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'EndDate');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var80 + '];');
ALTER TABLE [Calls] ALTER COLUMN [EndDate] datetimeoffset NOT NULL;

GO

DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'StartDate');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var81 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2019-12-19T13:30:49.6692179+00:00' FOR [StartDate];

GO

DECLARE @var82 sysname;
SELECT @var82 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'EndDate');
IF @var82 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var82 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [EndDate] datetimeoffset NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191219133050_CallCallPartRemoveEndDateRequiered', N'3.0.1');

GO