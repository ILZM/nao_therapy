using Microsoft.Kinect;
using Misc;
using NaoTherapy;
using System;

namespace Main
{
	class Logic
	{
		private			MainWindow		mainWindow		= null;
		private			Kinect			kinect			= null;
		private			Server			server			= null;

		private static	ConsoleColor	consoleColor	= ConsoleColor.Blue;

		// 1. initialized kinect: no kinect?-> abort
		// 2. created dedicated server
		// 2. nao has been turned on, connection with server has been established
		// 3. exercise has been chosen, sent the information to the server
		// 4. kinect selected the exercise therapy data, started to recognize 
		// 5. wait for exercise finish
		// 6. calculate fails and result based on the fails
		// 7. send the result to the client (robot)

		public Logic ( MainWindow mainWindow )
		{
			this.mainWindow	= mainWindow;
			this.kinect		= new Kinect ( this );
			
			if ( !this.kinect.Init ( ) )
			{
				this.Log ( "Failed to initialize the kinect sensor\n" );

				return;
			}

            this.server = new Server ( this );

            this.Log ( "Kinect and server have been initialized successfully\n" );

			// the program doesnt finish, because main thread is busy on manWindow rendering
			// this is not the program end
        }

        public void OnConnect ( )
        {
            this.Log ( "NAO has been been connected\n" );
        }

		public void OnConnectionLoss ( )
        {
            this.Log ( "NAO has been been disconnected\n" );
        }

		public void Result ( int limExcessNum, int limShortageNum, int fastWorkNum,
			int longWorkNum, bool fastExercise, bool longExercise )
		{
			this.Log ( "Result of therapy: " + "\n" +
				"\t\tLimit excess number " + limExcessNum + "\n" +
				"\t\tLimit shortage number " + limShortageNum + "\n" +
				"\t\tFast work number " + fastWorkNum + "\n" +
				"\t\tLong work number " + longWorkNum + "\n" +
				"\t\tFast exercise perfomance " + fastExercise + "\n" + 
				"\t\tLong exercise perfomance " + longExercise + "\n" );

			// Selection of the most foreground fail
			string result;

			if ( longExercise )
			{
				// too long whole exercise doing
				result = "6";
			}
			else
			if ( fastExercise )
			{
				// too fast whole exercise doing
				result = "5";
			}
			else
			if ( longWorkNum > 0 )
			{
				// too long exercise work loop doing
				result = "4";
			}
			else
			if ( fastWorkNum > 0 )
			{
				// too fast exercise work loop doing
				result = "3";
			}
			else
			if ( limShortageNum > 0 )
			{
				// didnt raise or drop arm enough
				result = "2";
			}
			else
			{
				// raised too high or droped too low arm
				result = "1";
			}

			// Saiting for a third NAO Robot connection to the server (Hardcoded)
			while ( this.Send ( Convert.ToString ( result ) ) == false ) { }
		}

		// Send the message to the client (robot)
		public bool Send ( string msg )
        {
			return this.server.SendToClient ( msg );
        }

		// Nao robot sent a message
		public void Broadcast ( string msg )
        {
            this.Log ( "NAO says '" + msg +"'\n" );

			Scanner	sc		= new Scanner ( msg );
			string	method	= sc.Next ( );

			switch ( method.ToLower ( ) )
			{
				// method of exercise choice
				case "e":
				{
					int exerciseNum		= sc.NextInt ( );
					int armOrientation	= sc.NextInt ( );

					this.OnExerciseReceive ( exerciseNum, armOrientation );
					
					break;
				}
				// method of exercise start
				case "s":
				{
					this.kinect.StartExercise ( );

					break;
				}
			}
        }

		// Draw patient skeleton on the main window
		public void DrawKinectSkeletons ( Skeleton [ ] kinectSkeletons )
		{
			this.mainWindow.DrawKinectSkeletons( kinectSkeletons );
		}

		// Received an exercise to do from the robot
		private void OnExerciseReceive ( int exerciseNum, int armOrientation )
		{
			// TODO: check exerciseNum

			this.Log ( "Received exercise number " + exerciseNum + "\n" );

			TherapyExercise te = new TherapyExercise ( this, armOrientation );

			// TODO: expand TherapyExercise class

			this.kinect.SetExercise ( te );
		}

		// method to debug
		private void Log ( string log )
		{
			Console.ForegroundColor = consoleColor;

			Constant.Log ( "[" + this.GetType ( ).Name + "] " + log );

			Console.ResetColor ( );
		}
    }
}