CREATE UNIQUE INDEX [IX_Commercials_SiebelId] ON [Commercials] ([SiebelId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200121143348_AddUniqueIndex-CommercialsSiebelID', N'3.0.1');
GO