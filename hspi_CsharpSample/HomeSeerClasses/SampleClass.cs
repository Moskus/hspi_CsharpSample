using System;
using System.Threading;

namespace hspi_CsharpSample.HomeSeerClasses
{
	///<summary>
    ///HSPI_SAMPE_BASIC class
    ///</summary>
    [Serializable()]
	public class SampleClass
    {
	    private string _housecode;

	    private string _deviceCode;

	    public string HouseCode
	    {
		    get=>_housecode;
		    set=>_housecode=value;
	    }
	    public string DeviceCode
	    {
		    get => _deviceCode;
		    set => _deviceCode = value;
	    }
	}

}