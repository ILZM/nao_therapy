using Misc;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Main
{
    class Server
    {
        private			TcpListener     tcpListener		= null;
        private			Thread          serverThread	= null;
        private			Boolean         isConnected		= false;
        private			Logic           logic			= null;

		private			int				counter			= 0;

		private			NetworkStream   networkStreamSend	= null;
        private			NetworkStream   networkStream	= null;

		private static	ConsoleColor	consoleColor	= ConsoleColor.Red;

        public Server ( Logic logic )
        {
            this.logic			= logic;
            this.serverThread   = new Thread ( new ThreadStart ( startServerListening ) );

			// allocated new thread for server listening
            this.serverThread.Start ( );
        }

        private void startServerListening ( )
        {
			this.tcpListener = new TcpListener ( IPAddress.Any, Constant.SERVER_TCP_PORT );

			this.tcpListener.Start ( );

			this.Log ( "Started to listen on\n" +
				"\t\tIP:\t" + Constant.GetLocalIPAddress ( ) + "\n" +
				"\t\tPORT:\t" + Constant.SERVER_TCP_PORT + "\n" );

            while ( true )
            {
				this.Log ( "Waiting a NAO\n" );

                TcpClient client = this.tcpListener.AcceptTcpClient ( );

                Thread clientThread = new Thread ( new ParameterizedThreadStart ( startClientListening ) );

				// allocated new thread for client listening
                clientThread.Start ( client );
            }
        }

        private void startClientListening ( object client )
        {
            this.logic.OnConnect ( );

            TcpClient tcpClient = ( TcpClient ) client;

            this.isConnected	= true;
			this.networkStream	= tcpClient.GetStream ( );

			this.counter++;

			if ( this.networkStreamSend == null )
			{
				this.networkStreamSend = this.networkStream;

				this.Log ( "networkStreamSend set\n" );

				this.networkStreamSend.WriteTimeout = 99999;
			}

			// In our solution nao sends a single message and then closes the connection
            //while ( true )
            //{
				byte [ ] buf = new byte [ Constant.RECEIVE_MSG_LENGTH ];

                int msgLen = 0;

                try
                {
                    //this.Log ( "Waiting a message from NAO\n" );

                    msgLen = networkStream.Read ( buf, 0, buf.Length );
                }
                catch
                {
					this.OnConnectionLoss ( );

                    //break;
                }

                if ( msgLen == 0 )
                {
                    this.OnConnectionLoss ( );

                    //break;
                }

                string msg = Encoding.ASCII.GetString ( buf, 0, msgLen );

                this.OnMessageReceive ( msg );
            //}

			//tcpClient.Close ( );

			this.Log ( "Closing the connection\n" );
        }

		private void OnConnectionLoss ( )
		{
			this.isConnected = false;

			this.Log ( "Lost connection with NAO\n" );

			this.logic.OnConnectionLoss ( );
		}

		private void OnMessageReceive ( string msg )
        {
            this.logic.Broadcast ( msg );
        }

		// send a message to nao robot
        public bool SendToClient ( String msg )
        {
            /*if ( !this.isConnected )
            {
                this.Log ( "Trying to send without a connection\n" );

                return false;
            }*/

			if ( this.counter != 3 )
			{
				return false;
			}

            byte [ ] buf = Encoding.ASCII.GetBytes ( msg );

			this.Log ( "Sent a message to NAO '" + msg +"'\n" );

			this.networkStream.Write ( buf, 0, buf.Length );
            this.networkStream.Flush ( );

			return true;
        }

		private void Log ( string log )
		{
			Console.ForegroundColor = consoleColor;

			Constant.Log ( "[" + this.GetType ( ).Name + "] " + log );

			Console.ResetColor ( );
		}
    }
}