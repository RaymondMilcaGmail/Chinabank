using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for CebuanaCustomerID
/// </summary>
public class CebuanaCustomerID
{
    #region Constructors

    public CebuanaCustomerID()
    {
    }

    #endregion

    #region Fields

    private string _iDCode;
    private string _iDDescription;
    //private string _iDNumber;

    #endregion

    #region Properties

    public string IDCode
    {
        get { return _iDCode; }
        set { _iDCode = value; }
    }

    public string IDDescription
    {
        get { return _iDDescription; }
        set { _iDDescription = value; }
    }

    //public string IDNumber
    //{
    //    get { return _iDNumber; }
    //    set { _iDNumber = value; }
    //}

    

    #endregion
}
