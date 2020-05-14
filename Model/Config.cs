namespace SerialPortToUDP.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class UDP
	{
		/// <summary>
		/// 
		/// </summary>
		public int Listen = 0;
		/// <summary>
		/// 
		/// </summary>
		public string SendToIP = null;
		/// <summary>
		/// 
		/// </summary>
		public int SendToPort = 0;
	}

	/// <summary>
	/// 
	/// </summary>
	public class Serial
	{
		/// <summary>
		/// 
		/// </summary>
		public string Port = null;
		/// <summary>
		/// 
		/// </summary>
		public int Baudrate = 0;
	}

	/// <summary>
	/// 
	/// </summary>
	public class Config
	{
		/// <summary>
		/// 
		/// </summary>
		public Serial Serial = null;
		/// <summary>
		/// 
		/// </summary>
		public UDP UDP = null;
	}
}