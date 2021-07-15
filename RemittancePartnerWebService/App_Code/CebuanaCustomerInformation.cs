using System.Collections.Generic;

/// <summary>
/// Summary description for CebuanaCustomerInformation
/// </summary>
public class CebuanaCustomerInformation
{
    #region Constructors

    public CebuanaCustomerInformation()
    { }

    #endregion

    #region Fields

    private string _customerNumber;
    private string _lastName;
    private string _firstName;
    private string _middleName;
    private string _city;
    private string _country;
    private List<CebuanaCustomerID> _registeredIDs;

    private List<CebuanaCustomerID> _secondaryIDs;
    private string _pickupDate;
    private string _reasonRemarks;
    private string _primaryIDIssuanceDate;
    private string _primaryIDExpirationdate;
    private string _secondaryIDCode;
    private string _secondaryIDType;
    private string _secondaryIDNumber;
    private string _secondaryIDIssuanceDate;
    private string _secondaryIDExpirationdate;
    private string _receiverStreet;
    private string _receiverBarangay;
    private string _receiverProvince;
    private string _receiverZipCode;
    private int _mobileNumber;
    private string _emailAddress;
    private string _dateOfBirth;

    #endregion

    #region Properties

    public string CustomerNumber
    {
        get { return _customerNumber; }
        set { _customerNumber = value; }
    }

    public string LastName
    {
        get { return _lastName; }
        set { _lastName = value; }
    }

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; }
    }

    public string MiddleName
    {
        get { return _middleName; }
        set { _middleName = value; }
    }

    public string City
    {
        get { return _city; }
        set { _city = value; }
    }

    public string Country
    {
        get { return _country; }
        set { _country = value; }
    }

    public List<CebuanaCustomerID> RegisteredIDs
    {
        get { return _registeredIDs; }
        set { _registeredIDs = value; }
    }

    public List<CebuanaCustomerID> SecondaryIDs
    {
        get { return _secondaryIDs; }
        set { _secondaryIDs = value; }
    }


    public string PickupDate
    {
        get { return _pickupDate; }
        set { _pickupDate = value; }
    }

    public string ReasonRemarks
    {
        get { return _reasonRemarks; }
        set { _reasonRemarks = value; }
    }

    public string PrimaryIDIssuanceDate
    {
        get { return _primaryIDIssuanceDate; }
        set { _primaryIDIssuanceDate = value; }
    }

    public string PrimaryIDExpirationdate
    {
        get { return _primaryIDExpirationdate; }
        set { _primaryIDExpirationdate = value; }
    }

    public string SecondaryIDCode
    {
        get { return _secondaryIDCode; }
        set { _secondaryIDCode = value; }
    }
    public string SecondaryIDType
    {
        get { return _secondaryIDType; }
        set { _secondaryIDType = value; }
    }

    public string SecondaryIDNumber
    {
        get { return _secondaryIDNumber; }
        set { _secondaryIDNumber = value; }
    }

    public string SecondaryIDIssuanceDate
    {
        get { return _secondaryIDIssuanceDate; }
        set { _secondaryIDIssuanceDate = value; }
    }

    public string SecondaryIDExpirationdate
    {
        get { return _secondaryIDExpirationdate; }
        set { _secondaryIDExpirationdate = value; }
    }

    public string ReceiverStreet
    {
        get { return _receiverStreet; }
        set { _receiverStreet = value; }
    }

    public string ReceiverBarangay
    {
        get { return _receiverBarangay; }
        set { _receiverBarangay = value; }
    }

    public string ReceiverProvince
    {
        get { return _receiverProvince; }
        set { _receiverProvince = value; }
    }

    public string ReceiverZipCode
    {
        get { return _receiverZipCode; }
        set { _receiverZipCode = value; }
    }

    public int MobileNumber
    {
        get { return _mobileNumber; }
        set { _mobileNumber = value; }
    }

    public string EmailAddress
    {
        get { return _emailAddress; }
        set { _emailAddress = value; }
    }

    public string DateOfBirth
    {
        get { return _dateOfBirth; }
        set { _dateOfBirth = value; }
    }
    #endregion
}
