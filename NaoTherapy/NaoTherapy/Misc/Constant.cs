using System;
using System.Net;
using System.Net.Sockets;

namespace Misc
{
	class Constant
	{
		public static readonly  int         SERVER_TCP_PORT		= 7200;
		public static readonly	int			RECEIVE_MSG_LENGTH	= 48;
        public static readonly  DateTime    epoch				= new DateTime ( 1970, 1 , 1, 0, 0, 0, DateTimeKind.Utc );

		public static long GetCurrentMilliTime ( )
		{
			long ct = ( long )( DateTime.UtcNow - epoch ).TotalMilliseconds;

			return ct;
		}

		public static string GetLocalIPAddress ( )
		{
			var host = Dns.GetHostEntry ( Dns.GetHostName ( ) );

			foreach ( var ip in host.AddressList )
			{
				if ( ip.AddressFamily == AddressFamily.InterNetwork )
				{
					return ip.ToString ( );
				}
			}
			throw new Exception ( "Local IP Address Not Found!" );
		}

		public static void Log ( string log )
		{
			Console.Write ( log );
		}
	}
}