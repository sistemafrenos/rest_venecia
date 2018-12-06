CREATE TABLE [dbo].[Impuestos]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [TipoImpuesto] VARCHAR(50) NULL DEFAULT 'EXCLUIDO', 
    [TasaNormal] DECIMAL(18, 2) NULL DEFAULT 12, 
    [TasaReducida] DECIMAL(18, 2) NULL DEFAULT 9, 
    [InicialesReducido] VARCHAR(50) NULL DEFAULT 'VEJG', 
    [TopeReduccion] DECIMAL(18, 2) NULL DEFAULT 2000000, 
    [TasaReducida2] DECIMAL(18, 2) NULL DEFAULT 7, 
    [InicialesReducido2] VARCHAR(50) NULL DEFAULT 'VEJG'
)