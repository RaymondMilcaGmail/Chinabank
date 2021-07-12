using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using AppCryptor;
using ChinaBankControlLibrary.RemittancePartnerWebReference;
using System.Windows.Forms;
using System.Data;

namespace ChinaBankControlLibrary
{
    class RemittancePartnerConfiguration
    {
        internal static string PartnerName
        {
            get { return "ChinaBank"; }
        }

        internal static string PartnerCode
        {
            get { return "CHB"; }
        }

        internal static string WebServiceURL
        {
            get { return ConfigurationManager.AppSettings["ChinaBankWebServiceURL"]; }
        }

        internal static string DBRemittanceConnectionString
        {
            get
            {
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["DBRemittance_ConnectionString"].ToString();
                    string connectionPassword = ConfigurationManager.AppSettings["DBRemittance_ConnStrPwd"];
                    string decryptedConnectionPassword;
                    try
                    {
                        decryptedConnectionPassword = AESCrypt.EncryptDecrypt(connectionPassword, "Decrypt");
                    }
                    catch
                    {
                        decryptedConnectionPassword = connectionPassword;
                    }

                    return string.Format("{0}{1};", connectionString, decryptedConnectionPassword);
                }
                catch (Exception error)
                {
                    throw new RemittanceException("An error has occured while getting the Remittance database configuration. Please contact ICT Support Desk.", error);
                }
            }
        }

        internal static int WebServiceCallTimeout
        {
            get { return 100000; }
        }

        internal static CebuanaCustomerID[] GetCebuanaCustomerIDs()
        {

            //List<CebuanaCustomerID> cebuanaCustomerIDs = new List<CebuanaCustomerID>();

            #region Valid ID Types

            //CebuanaCustomerID driversLicense = new CebuanaCustomerID();
            //driversLicense.IDCode = "DRIVERS_LICENSE";
            //driversLicense.IDDescription = "Driver's License";
            //cebuanaCustomerIDs.Add(driversLicense);

            //CebuanaCustomerID passport = new CebuanaCustomerID();
            //passport.IDCode = "PASSPORT";
            //passport.IDDescription = "Passport";
            //cebuanaCustomerIDs.Add(passport);

            //CebuanaCustomerID universityID = new CebuanaCustomerID();
            //universityID.IDCode = "UNIVERSITY_ID";
            //universityID.IDDescription = "University ID";
            //cebuanaCustomerIDs.Add(universityID);

            //CebuanaCustomerID employmentID = new CebuanaCustomerID();
            //employmentID.IDCode = "EMPLOYMENT_IDENTIFICATION";
            //employmentID.IDDescription = "Employment ID";
            //cebuanaCustomerIDs.Add(employmentID);

            //CebuanaCustomerID alienID = new CebuanaCustomerID();
            //alienID.IDCode = "ALIEN_IDENTIFICATION";
            //alienID.IDDescription = "Alien ID";
            //cebuanaCustomerIDs.Add(alienID);

            //CebuanaCustomerID prcID = new CebuanaCustomerID();
            //prcID.IDCode = "PROFESSIONAL_REGULATIONS_COMISSION_IDENTIFICATION_CARD";
            //prcID.IDDescription = "PRC ID";
            //cebuanaCustomerIDs.Add(prcID);

            //CebuanaCustomerID afpID = new CebuanaCustomerID();
            //afpID.IDCode = "AFP_IDENTIFICATION_CARD";
            //afpID.IDDescription = "AFP ID";
            //cebuanaCustomerIDs.Add(afpID);

            //CebuanaCustomerID sssGsisID = new CebuanaCustomerID();
            //sssGsisID.IDCode = "SSS_GSIS_MEMBERSHIP_CARD";
            //sssGsisID.IDDescription = "SSS/GSIS";
            //cebuanaCustomerIDs.Add(sssGsisID);

            //CebuanaCustomerID ofwID = new CebuanaCustomerID();
            //ofwID.IDCode = "OFW_ID_VE_CARD";
            //ofwID.IDDescription = "OFW ID-VE Card";
            //cebuanaCustomerIDs.Add(ofwID);

            #endregion

            #region Invalid ID Types

            //CebuanaCustomerID immigrationCard = new CebuanaCustomerID();
            //immigrationCard.IDCode = "IMMIGRATION_CARD";
            //immigrationCard.IDDescription = "Immigration Card";
            //cebuanaCustomerIDs.Add(immigrationCard);

            //CebuanaCustomerID militaryID = new CebuanaCustomerID();
            //militaryID.IDCode = "MILITARY_IDENTIFICATION";
            //militaryID.IDDescription = "Military ID";
            //cebuanaCustomerIDs.Add(militaryID);

            //CebuanaCustomerID enrollmentMilitaryDocument = new CebuanaCustomerID();
            //enrollmentMilitaryDocument.IDCode = "ENROLLMENT_MILITARY_DOCUMENT";
            //enrollmentMilitaryDocument.IDDescription = "EnrollmentMilitaryDoc";
            //cebuanaCustomerIDs.Add(enrollmentMilitaryDocument);

            //CebuanaCustomerID policeMilitaryID = new CebuanaCustomerID();
            //policeMilitaryID.IDCode = "POLICE_OR_MILITARY_IDENTIFICATION";
            //policeMilitaryID.IDDescription = "Police/Military ID";
            //cebuanaCustomerIDs.Add(policeMilitaryID);

            //CebuanaCustomerID cmnd = new CebuanaCustomerID();
            //cmnd.IDCode = "CMND";
            //cmnd.IDDescription = "CMND";
            //cebuanaCustomerIDs.Add(cmnd);

            //CebuanaCustomerID voterID = new CebuanaCustomerID();
            //voterID.IDCode = "VOTER_IDENTIFICATION";
            //voterID.IDDescription = "Voter's ID";
            //cebuanaCustomerIDs.Add(voterID);

            //CebuanaCustomerID attestedBankPassbook = new CebuanaCustomerID();
            //attestedBankPassbook.IDCode = "ATTESTED_BANK_PASSBOOK";
            //attestedBankPassbook.IDDescription = "AttestedBankPassbook";
            //cebuanaCustomerIDs.Add(attestedBankPassbook);

            //CebuanaCustomerID tin = new CebuanaCustomerID();
            //tin.IDCode = "TAXPAYER_IDENTIFICATION";
            //tin.IDDescription = "TIN";
            //cebuanaCustomerIDs.Add(tin);

            //CebuanaCustomerID other = new CebuanaCustomerID();
            //other.IDCode = "OTHER";
            //other.IDDescription = "Other";
            //cebuanaCustomerIDs.Add(other);

            //CebuanaCustomerID governmentID = new CebuanaCustomerID();
            //governmentID.IDCode = "OTHER_GOVT_ID";
            //governmentID.IDDescription = "Government ID";
            //cebuanaCustomerIDs.Add(governmentID);

            //CebuanaCustomerID otherPhoto = new CebuanaCustomerID();
            //otherPhoto.IDCode = "OTHER_PHOTO";
            //otherPhoto.IDDescription = "Other Photo ID";
            //cebuanaCustomerIDs.Add(otherPhoto);

            #endregion

            //return cebuanaCustomerIDs.ToArray();

            SqlConnection dbRemittanceConnection = new SqlConnection();
            try
            {
                dbRemittanceConnection.ConnectionString = RemittancePartnerConfiguration.DBRemittanceConnectionString;
                SqlCommand dbRemittanceCommand = new SqlCommand("dbo.GetRequiredID", dbRemittanceConnection);
                dbRemittanceCommand.CommandType = System.Data.CommandType.StoredProcedure;
                List<CebuanaCustomerID> cebuanaCustomerIDs = new List<CebuanaCustomerID>();
                dbRemittanceConnection.Open();
                SqlDataReader dbRemittanceReader = dbRemittanceCommand.ExecuteReader();
                while (dbRemittanceReader.Read())
                {
                    CebuanaCustomerID cebuanaCustomerID = new CebuanaCustomerID();
                    cebuanaCustomerID.IDCode = dbRemittanceReader["fldIDCode"].ToString();
                    cebuanaCustomerID.IDDescription = dbRemittanceReader["fldIDDescription"].ToString();
                    cebuanaCustomerIDs.Add(cebuanaCustomerID);
                }

                return cebuanaCustomerIDs.ToArray();
            }
            catch (RemittanceException remittanceError)
            {
                throw remittanceError;
            }
            catch (Exception error)
            {
                throw new RemittanceException("An error has occured while getting the required IDs list from the database. Please contact ICT Support Desk.", error);
            }
            finally
            {
                dbRemittanceConnection.Close();
            }
        }
    }
}
