USE [DBRemittance]
GO

/*
UPDATE [dbo].[TblPartnerList]
SET [fldIsAPI] = 1
WHERE [fldPartnerCode] = CBC-PL
*/


INSERT INTO [dbo].[TblPartnerList]
           ([fldPartnerCode]
           ,[fldPartnerName]
           ,[fldIsAPI]
           ,[fldIsPushAPI]
           ,[fldAPIForm]
           ,[fldResponseType]
           ,[fldEmail]
           ,[fldUsername]
           ,[fldPassword])
     VALUES
           ('CBC-PL'
           ,'ChinaBank'
           ,1
           ,0
           ,'CN'
           ,4
           ,NULL
           ,NULL
           ,NULL)

GO