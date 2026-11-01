-- ============================================================================
-- VIEWS AND REPORTING FOR GIVE-AID NGO PLATFORM
-- Database: IdentityDb
-- ============================================================================

USE [IdentityDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'[dbo].[vw_DonationStatistics]', N'V') IS NOT NULL
    DROP VIEW [dbo].[vw_DonationStatistics];
GO

CREATE VIEW [dbo].[vw_DonationStatistics]
AS
SELECT 
    c.[Id] AS CauseId,
    c.[Name] AS CauseName,
    c.[Slug] AS CauseSlug,
    COUNT(d.[Id]) AS TotalDonations,
    COUNT(CASE WHEN d.[Status] = N'Successful' THEN 1 END) AS SuccessfulDonations,
    ISNULL(SUM(CASE WHEN d.[Status] = N'Successful' THEN d.[Amount] ELSE 0 END), 0) AS TotalAmountRaised,
    MIN(CASE WHEN d.[Status] = N'Successful' THEN d.[Amount] END) AS MinDonationAmount,
    MAX(CASE WHEN d.[Status] = N'Successful' THEN d.[Amount] END) AS MaxDonationAmount
FROM [dbo].[Causes] c
LEFT JOIN [dbo].[Donations] d ON c.[Id] = d.[CauseId]
GROUP BY c.[Id], c.[Name], c.[Slug];
GO

PRINT N'[OK] Đã tạo View [vw_DonationStatistics].';
GO
