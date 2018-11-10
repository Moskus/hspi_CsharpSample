using HomeSeerAPI;

namespace hspi_CsharpSample
{
	public class Utils
	{
		public static string IFACE_NAME = "Hspi_CsharpSample";
		private static bool _isShuttingDown=false;

		public static bool IsShuttingDown
		{
			get => _isShuttingDown;
			set => _isShuttingDown = value;
		}
		public IAppCallbackAPI Callback{ get; set; }
		public IHSApplication Hs { get; set; }
	}
}