using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PseudoNao
{
	class Program
	{
		private static	Thread			receiveThread	= null;
		private static	NetworkStream	networkStream	= null;

		private static readonly	string	IP_ADDRESS		= "127.0.0.1";
		private static readonly	int		SERVER_TCP_PORT	= 7200;

		public static void Main ( string [ ] args )
		{
			TcpClient tcpClient = new TcpClient ( );

			try
			{
				tcpClient.Connect ( IP_ADDRESS, SERVER_TCP_PORT);
			}
			catch ( Exception e )
			{
				Console.WriteLine ( "The server " + IP_ADDRESS + ":" + SERVER_TCP_PORT + " is offline" );
				Console.ReadKey ( );

				return;
			}

			Console.WriteLine ( "Connected to " + IP_ADDRESS + ":" + SERVER_TCP_PORT );

			networkStream	= tcpClient.GetStream ( );
			receiveThread	= new Thread ( new ThreadStart ( StartServerListening ) );

			while ( true )
			{
				String msg = Console.ReadLine ( );

				// enter q to quit
				if ( msg.ToLower ( ) == "q" )
				{
					break;
				}
				else
				// enter e to send the exercise specifications to the server
				if ( msg.ToLower ( ) == "e" )
				{
					msg = "E 4 0";
				}
				else
				// enter s to start the exercise after u send the exercise details
				if ( msg.ToLower ( ) == "s" )
				{
					msg = "S";
				}

				byte [ ] msgByte = Encoding.ASCII.GetBytes ( ( msg ) );

				Console.WriteLine ( "Transmitting '" + msg + "'" );
            
				try
				{
					networkStream.Write ( msgByte, 0, msgByte.Length );
				}
				catch ( Exception e )
				{
					Console.WriteLine ( "Lost connection with the server" );
					Console.ReadKey ( );

					break;
				}
			}

			tcpClient.Close ( );
		}

		public static void StartServerListening ( )
		{
			while ( true )
			{
				byte [ ] buf = new byte [ 128 ];

				int msgLen = 0;

				try
				{
					msgLen = networkStream.Read ( buf, 0, buf.Length );
				}
				catch ( Exception e )
				{
					Console.WriteLine ( "Lost connection with the server" );

					break;
				}
            
				if ( msgLen == 0 )
				{
					Console.WriteLine ( "Lost connection with the server" );

					break;
				}

				string msg = Encoding.ASCII.GetString ( buf, 0, msgLen );
				
				Console.WriteLine ( "Received '" + msg + "'" );
			}
		}
	}
}