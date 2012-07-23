
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/18/2012 15:01:04
-- Generated from EDMX file: C:\Users\THANH BINH\Desktop\sgnmovies-server\SGNMovies.Server\Models\SGNMovies.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SGNMOVIE];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CinemaSessionTime]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SessionTimes] DROP CONSTRAINT [FK_CinemaSessionTime];
GO
IF OBJECT_ID(N'[dbo].[FK_CinemaProvider]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Providers] DROP CONSTRAINT [FK_CinemaProvider];
GO
IF OBJECT_ID(N'[dbo].[FK_MovieProvider]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Providers] DROP CONSTRAINT [FK_MovieProvider];
GO
IF OBJECT_ID(N'[dbo].[FK_MovieSessionTime]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SessionTimes] DROP CONSTRAINT [FK_MovieSessionTime];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Providers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Providers];
GO
IF OBJECT_ID(N'[dbo].[Cinemas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cinemas];
GO
IF OBJECT_ID(N'[dbo].[Movies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movies];
GO
IF OBJECT_ID(N'[dbo].[SessionTimes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SessionTimes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Providers'
CREATE TABLE [dbo].[Providers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [HostUrl] nvarchar(max)  NOT NULL,
    [Cinema_Id] int  NOT NULL,
    [Movie_Id] int  NOT NULL
);
GO

-- Creating table 'Cinemas'
CREATE TABLE [dbo].[Cinemas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DisplayName] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Latitude] float  NOT NULL,
    [Longitude] float  NOT NULL,
    [ImageUrl] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NOT NULL,
    [WebId] int  NOT NULL
);
GO

-- Creating table 'Movies'
CREATE TABLE [dbo].[Movies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Var] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Director] nvarchar(max)  NULL,
    [Duration] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [Genre] nvarchar(max)  NULL,
    [Cast] nvarchar(max)  NULL,
    [Language] nvarchar(max)  NULL,
    [Producer] nvarchar(max)  NULL,
    [Is3d] bit  NULL,
    [IsNowShowing] bit  NOT NULL,
    [InfoUrl] nvarchar(max)  NOT NULL,
    [ImageUrl] nvarchar(max)  NULL,
    [TrailerUrl] nvarchar(max)  NULL
);
GO

-- Creating table 'SessionTimes'
CREATE TABLE [dbo].[SessionTimes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] nvarchar(max)  NOT NULL,
    [Time] nvarchar(max)  NOT NULL,
    [Cinema_Id] int  NOT NULL,
    [Movie_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Providers'
ALTER TABLE [dbo].[Providers]
ADD CONSTRAINT [PK_Providers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cinemas'
ALTER TABLE [dbo].[Cinemas]
ADD CONSTRAINT [PK_Cinemas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Movies'
ALTER TABLE [dbo].[Movies]
ADD CONSTRAINT [PK_Movies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [PK_SessionTimes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Cinema_Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [FK_CinemaSessionTime]
    FOREIGN KEY ([Cinema_Id])
    REFERENCES [dbo].[Cinemas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CinemaSessionTime'
CREATE INDEX [IX_FK_CinemaSessionTime]
ON [dbo].[SessionTimes]
    ([Cinema_Id]);
GO

-- Creating foreign key on [Cinema_Id] in table 'Providers'
ALTER TABLE [dbo].[Providers]
ADD CONSTRAINT [FK_CinemaProvider]
    FOREIGN KEY ([Cinema_Id])
    REFERENCES [dbo].[Cinemas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CinemaProvider'
CREATE INDEX [IX_FK_CinemaProvider]
ON [dbo].[Providers]
    ([Cinema_Id]);
GO

-- Creating foreign key on [Movie_Id] in table 'Providers'
ALTER TABLE [dbo].[Providers]
ADD CONSTRAINT [FK_MovieProvider]
    FOREIGN KEY ([Movie_Id])
    REFERENCES [dbo].[Movies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MovieProvider'
CREATE INDEX [IX_FK_MovieProvider]
ON [dbo].[Providers]
    ([Movie_Id]);
GO

-- Creating foreign key on [Movie_Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [FK_MovieSessionTime]
    FOREIGN KEY ([Movie_Id])
    REFERENCES [dbo].[Movies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MovieSessionTime'
CREATE INDEX [IX_FK_MovieSessionTime]
ON [dbo].[SessionTimes]
    ([Movie_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------