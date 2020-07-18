DECLARE @var88 sysname;
SELECT @var88 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sessions]') AND [c].[name] = N'StartDate');
IF @var88 IS NOT NULL EXEC(N'ALTER TABLE [Sessions] DROP CONSTRAINT [' + @var88 + '];');
ALTER TABLE [Sessions] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Sessions] ADD DEFAULT '2019-12-19T17:53:24.8770371+00:00' FOR [StartDate];

GO

DECLARE @var89 sysname;
SELECT @var89 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'StartDate');
IF @var89 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var89 + '];');
ALTER TABLE [Calls] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2019-12-19T17:53:24.8569493+00:00' FOR [StartDate];

GO

DECLARE @var90 sysname;
SELECT @var90 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'StartDate');
IF @var90 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var90 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2019-12-19T17:53:24.8655245+00:00' FOR [StartDate];

GO

DECLARE @var91 sysname;
SELECT @var91 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'RedirectChannel');
IF @var91 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var91 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [RedirectChannel] nvarchar(100) NOT NULL;

GO

DECLARE @var92 sysname;
SELECT @var92 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'OrigChannel');
IF @var92 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var92 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [OrigChannel] nvarchar(100) NOT NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191219175325_CallPartExtendOrigRedirectChannel', N'3.0.1');

GO