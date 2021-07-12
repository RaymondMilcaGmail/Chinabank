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

    #endregion
}
