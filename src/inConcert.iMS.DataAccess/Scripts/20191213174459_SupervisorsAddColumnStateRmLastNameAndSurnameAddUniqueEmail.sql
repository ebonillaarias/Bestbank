DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Supervisors]') AND [c].[name] = N'LastName');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [Supervisors] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [Supervisors] DROP COLUMN [LastName];

GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Supervisors]') AND [c].[name] = N'Surname');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [Supervisors] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [Supervisors] DROP COLUMN [Surname];

GO

ALTER TABLE [Supervisors] ADD [State] int NOT NULL DEFAULT 0;

GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Sessions]') AND [c].[name] = N'StartDate');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [Sessions] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [Sessions] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Sessions] ADD DEFAULT '2019-12-13T17:44:58.6811669+00:00' FOR [StartDate];

GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'StartDate');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [Calls] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2019-12-13T17:44:58.6703721+00:00' FOR [StartDate];

GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Calls]') AND [c].[name] = N'EndDate');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [Calls] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [Calls] ALTER COLUMN [EndDate] datetimeoffset NOT NULL;
ALTER TABLE [Calls] ADD DEFAULT '2019-12-13T17:44:58.6706047+00:00' FOR [EndDate];

GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'StartDate');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [StartDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2019-12-13T17:44:58.6741928+00:00' FOR [StartDate];

GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CallParts]') AND [c].[name] = N'EndDate');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [CallParts] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [CallParts] ALTER COLUMN [EndDate] datetimeoffset NOT NULL;
ALTER TABLE [CallParts] ADD DEFAULT '2019-12-13T17:44:58.6742238+00:00' FOR [EndDate];

GO

CREATE UNIQUE INDEX [IX_Supervisors_Email] ON [Supervisors] ([Email]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20191213174459_SupervisorsAddColumnStateRmLastNameAndSurnameAddUniqueEmail', N'3.0.1');

GO

