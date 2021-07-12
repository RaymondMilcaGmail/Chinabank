 USE [DBRemittance]
GO

SELECT
	 [fldControlNum]
	,[fldTUUID]
	,[fldInternalNum]
	,[fldSLastname]
	,[fldSFirstname]
	,[fldSFullname]
	,[fldBLastname]
	,[fldBFirstname]
	,[fldBFullname]
	,[fldBUID]
	,[fldCCY]
	,[fldCCY2]
	,[fldCTY]
	,[fldRate]
	,[fldAmount]
	,[fldOtherCharges]
	,[fldSendingBranch]
	,[fldPayoutBranchFinal]
	,[fldDiscount]
	,[fldRemarks]
	,[fldTDateCreated]
	,[fldTLastUpdated]
	,[fldARDate]
	,[fldPayoutDate]
	,[fldPayoutID1]
	,[fldID1Details]
	,[fldPayoutID2]
	,[fldID2Details]
	,[fldBeneIncomplete]
	,[fldTranStatus]
	,[fldReactivated]
	,[fldReceiverID]
	,[fldAuditTracker]
	,[fldFeedBackSent]
	,[fld_PartnerInternalRefNo]
	,[fld_ProcessedBy]
	,[fld_PartnerInternalRefNo2]
FROM
	[dbo].[TblPartnerRemittance]
WHERE
	[fldSendingBranch] = 'XOM'
ORDER BY
	[fldTDateCreated] DESC