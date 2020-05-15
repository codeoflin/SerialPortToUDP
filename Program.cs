using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortToUDP
{
	class Program
	{
		static int Main(string[] args)
		{
			string remoteip = null;
			#region 参数判断
			if (args == null)
			{
				Console.WriteLine("请传入有效的配置文件路径!");
				return -1;
			}
			if (args.Length == 0)
			{
				Console.WriteLine("请传入有效的配置文件路径!");
				return -1;
			}
			if (!File.Exists(args[0]))
			{
				Console.WriteLine("请传入有效的配置文件路径!");
				return -1;
			}
			#endregion
			var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Config>(File.ReadAllText(args[0]));
			UdpClient udp = null;
			SerialPort serial = null;
			#region 监听端口与打开串口
			try
			{
				udp = new UdpClient(config.UDP.Listen);
			}
			catch (Exception err)
			{
				Console.WriteLine($"监听端口 {config.UDP.Listen} 失败!!");
				return -1;
			}
			try
			{
				serial = new SerialPort(config.Serial.Port, config.Serial.Baudrate);
				serial.Open();
			}
			catch (Exception err)
			{
				Console.WriteLine($"打开串口 {config.Serial.Port} 失败!!");
				return -1;
			}
			#endregion
			bool running = true;
			Task.Factory.StartNew(async () =>
			{
				while (running)
				{
					var udprecv = (await udp.ReceiveAsync());
					var buff = udprecv.Buffer;
					if (config.UDP.SendToIP.ToUpper() == "AUTO") remoteip = udprecv.RemoteEndPoint.Address.ToString();
					serial.Write(buff, 0, buff.Length);
				}
			});
			Task.Factory.StartNew(() =>
						{
							var listbuff = new List<byte>();
							while (running)
							{
								listbuff.Clear();
								while (serial.BytesToRead == 0 && running) Thread.Sleep(10);//等待第一个字节
								Thread.Sleep(50);
								while (serial.BytesToRead > 0)//等待后续字节
								{
									var buff = new byte[serial.BytesToRead];
									serial.Read(buff, 0, buff.Length);
									listbuff.AddRange(buff);
									Thread.Sleep(50);
								}
								if (listbuff.Count > 0)
								{
									if (config.UDP.SendToIP.ToUpper() != "AUTO") udp.Send(listbuff.ToArray(), listbuff.Count, config.UDP.SendToIP, config.UDP.SendToPort);
									if (!string.IsNullOrWhiteSpace(remoteip)) udp.Send(listbuff.ToArray(), listbuff.Count, remoteip, config.UDP.SendToPort);
								}
							}
						});
			//Console.ReadKey();
			while (true) Thread.Sleep(50);
			running = false;
			Thread.Sleep(50);
			return 0;
		}
	}//End Class
}
