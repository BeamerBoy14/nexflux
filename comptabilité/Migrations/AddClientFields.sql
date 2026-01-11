-- Migration pour ajouter les nouveaux champs à la table Clients
-- Exécutez ce script SQL directement sur votre base de données

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'NumeroIdd')
BEGIN
    ALTER TABLE [Clients] ADD [NumeroIdd] nvarchar(max) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'NumeroPermis')
BEGIN
    ALTER TABLE [Clients] ADD [NumeroPermis] nvarchar(max) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'DateNaissance')
BEGIN
    ALTER TABLE [Clients] ADD [DateNaissance] datetime2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = 'IdTel')
BEGIN
    ALTER TABLE [Clients] ADD [IdTel] nvarchar(max) NULL;
END
GO

