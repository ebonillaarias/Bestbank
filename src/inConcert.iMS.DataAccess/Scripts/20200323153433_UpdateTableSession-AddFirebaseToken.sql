DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sessions]') AND [c].[name] = N'StartDate');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Sessions] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Sessions] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Sessions] ADD DEFAULT '2020-03-23T15:34:32.7521072+00:00' FOR [StartDate];

GO

ALTER TABLE [Sessions] ADD [FirebaseToken] nvarchar(max) NULL;

GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'StartDate');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Calls] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2020-03-23T15:34:32.7119145+00:00' FOR [StartDate];

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'StartDate');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2020-03-23T15:34:32.7252277+00:00' FOR [StartDate];

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200323153433_UpdateTableSession-AddFirebaseToken', N'3.0.1');

GO

