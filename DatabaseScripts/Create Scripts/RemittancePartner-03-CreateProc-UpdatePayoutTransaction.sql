USE [DBRemittance]
GO

CREATE PROCEDURE [ChinaBank].[proc_UpdatePayoutTransaction]
	 @PartnerCode VARCHAR(50)
	,@TransactionID BIGINT
	,@ControlNumber VARCHAR(25)
	,@TransactionStatusID INT
	,@TransactionStatusDescription VARCHAR(20)
	
	,@SenderLastName VARCHAR(50)
	,@SenderFirstName VARCHAR(50)
	
	,@BeneficiaryCustomerNumber BIGINT
	,@BeneficiaryLastName VARCHAR(50)
	,@BeneficiaryFirstName VARCHAR(50)
	,@BeneficiaryIDType VARCHAR(20)
	,@BeneficiaryIDDetails VARCHAR(MAX)
	
	,@PayoutAmount FLOAT
	,@SendingCurrency VARCHAR(3)
	,@PayoutCurrency VARCHAR(3)
	,@CurrencyConversionRate DECIMAL(18,2)
	,@PayoutCountry VARCHAR(3)
	
	--,@Remarks VARCHAR(MAX)
	,@PartnerInternalReferenceNumber NVARCHAR(50)
	,@PartnerInternalReferenceNumber2 NVARCHAR(50)
	
	,@PayoutBranchCode VARCHAR(50)
	,@ReceiverBranchUserID VARCHAR(15)
	,@AuditTracker VARCHAR(MAX)
AS
BEGIN		
--	DECLARE
--		 @PartnerCode VARCHAR(50)
--		,@AuditTrackerXMLHandle INTEGER
--		,@AuditTrackerXMLDocument XML
--		,@AuditTrackerXMLHandleStatus INT
	
--	SELECT
--		 @PartnerCode = 'MBK'
--		,@AuditTrackerXMLHandleStatus = 0 -- XMLDOC status is closed
--		,@AuditTrackerXMLDocument = @AuditTracker
		
	BEGIN TRANSACTION
		BEGIN TRY
			UPDATE [dbo].[TblPartnerRemittance]
			SET 
				 [fldControlNum] = @ControlNumber
				,[fldSendingBranch] = @PartnerCode
				,[fldTranStatus] = @TransactionStatusID
				
				,[fldSLastname] = @SenderLastName
				,[fldSFirstname] = @SenderFirstName
				,[fldSFullname] =
					CASE @SenderFirstName
						WHEN ''
						THEN @SenderLastName
						ELSE @SenderLastName + ', ' + @SenderFirstName
					END
				
				,[fldBUID] = @BeneficiaryCustomerNumber
				,[fldBLastname] = @BeneficiaryLastName
				,[fldBFirstname] = @BeneficiaryFirstName
				,[fldBFullname] =
					CASE @BeneficiaryFirstName
						WHEN ''
						THEN @BeneficiaryLastName
						ELSE @BeneficiaryLastName + ', ' + @BeneficiaryFirstName
					END
				,[fldPayoutID1] = @BeneficiaryIDType
				,[fldID1Details] = @BeneficiaryIDDetails
				
				,[fldAmount] = @PayoutAmount
				,[fldCCY] = @SendingCurrency
				,[fldCCY2] = @PayoutCurrency
				,[fldRate] = @CurrencyConversionRate
				,[fldCTY] = @PayoutCountry
				
				--,[fldRemarks] = @Remarks
				,[fld_PartnerInternalRefNo] = @PartnerInternalReferenceNumber
				,[fld_PartnerInternalRefNo2] = @PartnerInternalReferenceNumber2
				
				,[fldPayoutBranchFinal] = @PayoutBranchCode
				,[fldBeneIncomplete] = 0
				,[fldReceiverID] = @ReceiverBranchUserID
				,[fldPayoutDate] = CURRENT_TIMESTAMP
				,[fldTLastUpdated] = CURRENT_TIMESTAMP
				,[fldAuditTracker] = @AuditTracker
			WHERE
				 [fldTUUID] = @TransactionID
				
--			EXEC SP_XML_PREPAREDOCUMENT
--				 @idoc = @AuditTrackerXMLHandle OUTPUT
--				,@x = @AuditTrackerXMLDocument
--				
--			SELECT @AuditTrackerXMLHandleStatus = 1 -- XMLDOC status is open
--
--			INSERT INTO [dbo].[TblAuditTrail] (
--				 [AuditDateTime]
--				,[Action]
--				,[AuditDetails]
--				,[ValueBeforeUpdate]
--				,[ValueAfterUpdate]
--				,[BranchID]
--				,[UserID]
--				,[RoleID]
--				,[ModuleID]
--				,[MACAddress]
--				,[IPAddress])
--			SELECT
--				 CURRENT_TIMESTAMP
--				,@TransactionStatusDescription --COALESCE([STATUS].[fldTranStatusDesc], 'UNKNOWN STATUS')
--				,UPPER(LTRIM(RTRIM(@ControlNumber)))
--				,'<NONE>'
--				,'<NONE>'
--				,[BRID]
--				,[UserID]
--				,[RoleID]
--				,[ModuleID]
--				,[MACADD]
--				,[IPAdd]
--			FROM
--				OPENXML(@AuditTrackerXMLHandle, 'DTRACKER/TRACKER', 2)
--					WITH (
--						 [BRID] BIGINT
--						,[UserID] BIGINT
--						,[RoleID] BIGINT
--						,[ModuleID] BIGINT
--						,[IPAdd] VARCHAR(15)
--						,[MACADD] VARCHAR(20)
--						)
----			JOIN dbo.TblTransactionStatusReference [Status]
----				ON [STATUS].[fldTranStatusCode] = @TransactionStatusID
			COMMIT TRANSACTION
--			EXEC SP_XML_REMOVEDOCUMENT @AuditTrackerXMLHandle
		END TRY
		BEGIN CATCH
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;

			SELECT 
				@ErrorMessage = ERROR_MESSAGE(),
				@ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState = ERROR_STATE();
			
			ROLLBACK TRANSACTION
				
--			IF @AuditTrackerXMLHandleStatus = 1 --if open, close
--			BEGIN
--				EXEC SP_XML_REMOVEDOCUMENT @AuditTrackerXMLHandle
--			END
			
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		END CATCH
END