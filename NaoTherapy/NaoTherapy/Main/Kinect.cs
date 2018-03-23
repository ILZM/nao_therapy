using Microsoft.Kinect;
using System;
using Misc;
using System.Linq;

namespace Main
{
    class Kinect
    {
		private			Logic			logic			= null;
        private			KinectSensor	kinectSensor    = null;
        //private		Skeleton		kinectSkeleton	= null;
		
		private			TherapyExercise	therapyExercise	= null;

		private static	ConsoleColor	consoleColor	= ConsoleColor.DarkYellow;

        public Kinect ( Logic logic )
        {
			this.logic = logic;
        }

		// Initialize kinect
        public bool Init ( )
        {
            this.kinectSensor = KinectSensor.KinectSensors.Where ( s => s.Status == KinectStatus.Connected ).FirstOrDefault ( );

            if ( this.kinectSensor == null )
            {
                return false;
            }

			// some parameters to make smooth skeleton recognition. native method of kinect
			TransformSmoothParameters smoothingParam = new TransformSmoothParameters ( );
			{
				smoothingParam.Smoothing			= 0.7f;
				smoothingParam.Correction			= 0.3f;
				smoothingParam.Prediction			= 1.0f;
				smoothingParam.JitterRadius			= 1.0f;
				smoothingParam.MaxDeviationRadius	= 1.0f;
			};

            //this.kinectSensor.ColorStream.Enable ( ColorImageFormat.RgbResolution640x480Fps30 );

			this.kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

			this.kinectSensor.SkeletonStream.Enable ( smoothingParam );
			this.kinectSensor.Start ( );

			this.kinectSensor.SkeletonFrameReady += this.SkeletonFrameReady;

            return true;
        }

		// Set the exercise to do now
        public void SetExercise ( TherapyExercise therapyExercise )
        {
			this.therapyExercise = therapyExercise;
        }

		// Start the exercise recognition
		public void StartExercise ( )
		{
			if ( this.therapyExercise == null )
			{
				return;
			}

			this.therapyExercise.Start ( );
		}

		public void Finish ( )
		{
			this.kinectSensor.Stop ( );
		}

		// A callback from the KinectManager
        private void SkeletonFrameReady ( object sender, SkeletonFrameReadyEventArgs e )
        {
            using ( SkeletonFrame curSkeletonFrame = e.OpenSkeletonFrame ( ) )
            {
                if ( curSkeletonFrame != null )
                {
                    Skeleton [ ] tempSkeletons = new Skeleton [ curSkeletonFrame.SkeletonArrayLength ];

                    curSkeletonFrame.CopySkeletonDataTo ( tempSkeletons );
					curSkeletonFrame.Dispose ( );

					this.logic.DrawKinectSkeletons ( tempSkeletons );

                    if ( tempSkeletons.Length > 0 )
                    {
                        Skeleton curSkeleton = tempSkeletons.Where ( u => u.TrackingState == SkeletonTrackingState.Tracked ).FirstOrDefault ( );

                        if ( curSkeleton != null ) 
                        {
                            this.Process ( curSkeleton );
                        }
                        else  {  this.SensorError ( ); }
                    }
                    else { this.SensorError ( ); }
                }
                else { this.SensorError ( ); }
            }
        }

		// Estimate the therapy exercise
        private void Process ( Skeleton kinectSkeleton )
        {
			if ( this.therapyExercise == null )
			{
				return;
			}

			this.therapyExercise.Process ( kinectSkeleton );
        }

        private void SensorError ( )
        {

        }

		private void Log ( string log )
		{
			Console.ForegroundColor = consoleColor;

			Constant.Log ( "[" + this.GetType ( ).Name + "] " + log );

			Console.ResetColor ( );
		}
    }
}