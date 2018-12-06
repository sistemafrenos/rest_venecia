CREATE TABLE [dbo].[Impuestos] (
    [Id]                 INT IDENTITY(1,1) NOT NULL,
    [TipoImpuesto]       VARCHAR (50)    DEFAULT ('EXCLUIDO') NULL,
    [TasaNormal]         DECIMAL (18, 2) DEFAULT ((12)) NULL,
    [TasaReducida]       DECIMAL (18, 2) DEFAULT ((9)) NULL,
    [InicialesReducido]  VARCHAR (50)    DEFAULT ('VEJG') NULL,
    [TopeReduccion]      DECIMAL (18, 2) DEFAULT ((2000000)) NULL,
    [TasaReducida2]      DECIMAL (18, 2) DEFAULT ((7)) NULL,
    [InicialesReducido2] VARCHAR (50)    DEFAULT ('VEJG') NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);