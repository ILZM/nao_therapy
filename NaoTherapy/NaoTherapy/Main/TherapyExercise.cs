using Microsoft.Kinect;
using Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Main
{
	class TherapyExercise
	{
		private	Logic		logic			= null;
		private	ArrayList	leftArmJoints	= null;
		private	ArrayList	rightArmJoints	= null;
		private	Skeleton	kinectSkeleton	= null;
		private	ArmState	armState		= ArmState.NOINFO;

		private static ConsoleColor consoleColor = ConsoleColor.Yellow;

		private static	int	maxExcessAng	= 100;
		private static	int	maxShortageAng	= 80;
		private static	int	minExcessAng	= 0;
		private static	int	minShortageAng	= 30;
		private static	int	difShortageAng	= 30;
		private static	int	startAngle		= 0;
		private static	int	minWorkTime		= ( int )1.5 * 1000;
		private static	int	maxWorkTime		= ( int )2.8 * 1000;
		private static	int	minExerciseTime	= ( int )15 * 1000;
		private static	int	maxExerciseTime	= ( int )30 * 1000;
		private static	int	maxWorkNum		= 4;

		private bool	isWorking		= false;
		private bool	directionUp		= false;
		private int		lastAng			= -1;
		private long	lastExtremaTime	= -1;
		private int		lastExtremaAng	= -1;
		private int		limExcessNum	= -1;
		private int		limShortageNum	= -1;
		private int		fastWorkNum		= -1;
		private int		longWorkNum		= -1;
		private int		workNum			= -1;

		private	StreamWriter streamWriter = null;

		private	long			exerciseStartTime	= -1;
		private	List < Point >	pointList			= null;

		private enum ArmState
		{
			NOINFO = 0,
			LEFT,
			RIGHT
		};

		public TherapyExercise ( Logic logic, int armOrientation )
		{
			this.logic			= logic;
			this.leftArmJoints	= new ArrayList ( );
			this.rightArmJoints	= new ArrayList ( );
			this.pointList		= new List < Point > ( );

			if ( armOrientation == 0 )
			{
				this.armState = ArmState.LEFT;

				this.Log ( "Selected therapy exercise for left arm\n" );
			}
			else
			{
				this.armState = ArmState.RIGHT;

				this.Log ( "Selected therapy exercise for right arm\n" );
			}

			this.streamWriter = new StreamWriter ( "data.txt" );

			this.isWorking			= false;
			this.directionUp		= true;
			this.lastAng			= startAngle;
			this.lastExtremaAng		= startAngle;
			this.lastExtremaTime	= 0;
			this.limExcessNum		= 0;
			this.limShortageNum		= 0;
			this.fastWorkNum		= 0;
			this.longWorkNum		= 0;
			this.workNum			= 0;
		}

		// Arm rehabilitation calculations dependent on arm orientation
		public void Process ( Skeleton kinectSkeleton )
		{
			if ( !this.isWorking )
			{
				return;
			}

			this.kinectSkeleton = kinectSkeleton;

			switch ( this.armState )
			{
				case ArmState.NOINFO:
				{
					//this.armState = this.GetWorkingArm ( );
					
					break;
				}
				case ArmState.LEFT:
				{
					Vector3 orgGlobalLeftWrist		= new Vector3 ( kinectSkeleton.Joints [ JointType.WristLeft ].Position );
					Vector3 orgGlobalLeftElbow		= new Vector3 ( kinectSkeleton.Joints [ JointType.ElbowLeft ].Position );
					Vector3 orgGlobalLeftShoulder	= new Vector3 ( kinectSkeleton.Joints [ JointType.ShoulderLeft ].Position );

					Vector3 orgLocalLeftElbow		= orgGlobalLeftWrist - orgGlobalLeftElbow;
					Vector3 orgLocalLeftShoulder	= orgGlobalLeftElbow - orgGlobalLeftShoulder;
					
					int leftAng = ( int )Math.Round ( CustomMath.GetAngle ( orgLocalLeftElbow, orgLocalLeftShoulder ) );
				
					this.CalculateAngle ( leftAng );

					break;
				}
				case ArmState.RIGHT:
				{
					Vector3 orgGlobalRightWrist		= new Vector3 ( kinectSkeleton.Joints [ JointType.WristRight ].Position );
					Vector3 orgGlobalRightElbow		= new Vector3 ( kinectSkeleton.Joints [ JointType.ElbowRight ].Position );
					Vector3 orgGlobalRightShoulder	= new Vector3 ( kinectSkeleton.Joints [ JointType.ShoulderRight ].Position );

					Vector3 orgLocalRightElbow		= orgGlobalRightWrist - orgGlobalRightElbow;
					Vector3 orgLocalRightShoulder	= orgGlobalRightElbow - orgGlobalRightShoulder;

					int rightAng = ( int )Math.Round ( CustomMath.GetAngle ( orgLocalRightElbow, orgLocalRightShoulder ) );
					
					this.CalculateAngle ( rightAng );

					break;
				}
			}
		}

		public void Start ( )
		{
			this.isWorking = true;
		}

		// Some legacy code to create a data for visual graph of exercise
		private void CreateCurvePoint ( int y )
		{
			long curMilliTime = Constant.GetCurrentMilliTime ( );

			if ( this.pointList.Count == 0 )
			{
				this.pointList.Add ( new Point ( 0, y ) );
			}
			else
			{
				this.pointList.Add ( new Point ( curMilliTime - this.exerciseStartTime, y ) );
			}
		}

		// Calculate the fails or success of exercise based on curve extremas
		private void CalculateAngle ( int curAng )
		{
			//this.Log ( "Angle " + curAng + "\n" );
			this.CreateCurvePoint ( curAng );

			this.streamWriter.WriteLine ( curAng );

			long curMilliTime = Constant.GetCurrentMilliTime ( );

			// Exercise start time
			if ( this.exerciseStartTime == -1 )
			{
				this.exerciseStartTime = curMilliTime;
			}
			
			// Too long exercise doing
			if ( curMilliTime - exerciseStartTime >= maxExerciseTime )
			{
				this.ExerciseEnd ( true );
			}

			// Difference of angle between current and last angles
			int dif = curAng - this.lastAng;

			// Last peak time
			if ( this.lastExtremaTime == 0 )
			{
				this.lastExtremaTime = Constant.GetCurrentMilliTime ( );
			}

			// Going to raise hand
			if ( this.directionUp )
			{
				if ( dif >= 0 )
				{
					// Everything is ok, going higher
				}
				else
				if ( Math.Abs ( curAng - this.lastExtremaAng ) >= difShortageAng )
				{
					// direction change from up to down
					this.directionUp	= false;
					this.lastExtremaAng	= curAng;

					this.workNum++;

					if ( curMilliTime - this.lastExtremaTime < minWorkTime )
					{
						this.fastWorkNum++;

						this.Log ( ( curMilliTime - this.lastExtremaTime ) +
							" Too fast on direction up. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curAng > maxExcessAng )
					{
						this.limExcessNum++;

						this.Log ( curAng + " Exceed max angle on direction up. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curAng < maxShortageAng )
					{
						this.limShortageNum++;

						this.Log ( curAng + " Lack of max angle on direction up. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curMilliTime - this.lastExtremaTime > maxWorkTime )
					{
						this.longWorkNum++;

						this.Log ( ( curMilliTime - this.lastExtremaTime ) +
							" Too long on direction up. Remaining number " + this.workNum + "\n" );
					}
					else
					{
						this.Log ( curAng + " OK on direction up. Remaining number " + this.workNum + "\n" );
					}

					this.lastExtremaTime = curMilliTime;
				}
			}
			else
			{
				if ( dif <= 0 )
				{
					// ok, going lower
				}
				else
				if ( Math.Abs ( curAng - this.lastExtremaAng ) >= difShortageAng )
				{
					//local minimum extrema
					this.directionUp	= true;
					this.lastExtremaAng	= curAng;

					this.workNum++;

					if ( curMilliTime - this.lastExtremaTime < minWorkTime )
					{
						this.fastWorkNum++;

						this.Log ( ( curMilliTime - this.lastExtremaTime ) +
							" Too fast on direction down. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curAng < minExcessAng )
					{
						this.limExcessNum++;

						this.Log ( curAng + " Exceed min angle on direction down. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curAng > minShortageAng )
					{
						this.limShortageNum++;

						this.Log ( curAng + " Lack of min angle on direction down. Remaining number " + this.workNum + "\n" );
					}
					else
					if ( curMilliTime - this.lastExtremaTime > maxWorkTime )
					{
						this.longWorkNum++;

						this.Log ( ( curMilliTime - this.lastExtremaTime ) +
							" Too long on direction down. Remaining number " + this.workNum + "\n" );
					}
					else
					{
						this.Log ( curAng + " OK on direction down. Remaining number " + this.workNum + "\n" );
					}

					this.lastExtremaTime = curMilliTime;
				}
			}

			// Reached the exercise loop limit
			if ( this.workNum >= maxWorkNum * 2 )
			{
				this.ExerciseEnd ( false );
			}
			
			this.lastAng = curAng;
		}

		private void ExerciseEnd ( bool longExercise )
		{
			this.streamWriter.Close ( );

			this.isWorking = false;
			this.pointList = CustomMath.DouglasPeuckerReduction ( this.pointList, 1.0 );
				
			this.streamWriter = new StreamWriter ( "dataSmoothed.txt" );

			for ( int i = 0; i < this.pointList.Count; i++ )
			{
				this.streamWriter.WriteLine ( this.pointList [ i ].Y );
			}

			this.streamWriter.Close ( );

			bool fastExercise = false;

			if ( Constant.GetCurrentMilliTime ( ) - exerciseStartTime <= minExerciseTime )
			{
				fastExercise = true;
			}

			this.logic.Result ( this.limExcessNum, this.limShortageNum,
				this.fastWorkNum, this.longWorkNum, fastExercise, longExercise );
		}

		// legacy code of arm orientation recognition. no need anymore
		/*
		private ArmState GetWorkingArm ( )
		{
			int leftNotTrackedNum = 0, rightNotTrackedNum = 0;

			this.RefreshArmJointList ( );

			for ( int i = 0; i < this.leftArmJoints.Count; i++ )
			{
				Joint j = ( Joint )this.leftArmJoints [ i ];

				if ( j.TrackingState != JointTrackingState.Tracked )
				{
					leftNotTrackedNum++;
				}
			}

			for ( int i = 0; i < this.rightArmJoints.Count; i++ )
			{
				Joint j = ( Joint )this.rightArmJoints [ i ];

				if ( j.TrackingState != JointTrackingState.Tracked )
				{
					rightNotTrackedNum++;
				}
			}

			this.Log ( "left = " + leftNotTrackedNum + "\n" );
			this.Log ( "right = " + rightNotTrackedNum + "\n" );
			
			//return ArmState.NOINFO;
			
			if ( leftNotTrackedNum > rightNotTrackedNum )
			{
				this.Log ( "selected left hand - " + leftNotTrackedNum + " vs " + rightNotTrackedNum  + "\n" );

				return ArmState.LEFT;
			}
			else
			{
				this.Log ( "selected right hand - " + rightNotTrackedNum + " vs " + leftNotTrackedNum  + "\n" );

				return ArmState.RIGHT;
			}
		}

		private void RefreshArmJointList ( )
		{
			this.leftArmJoints.Clear ( );
			this.leftArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.WristLeft ] );
			this.leftArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.ElbowLeft ] );
			this.leftArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.ShoulderLeft ] );
			
			this.rightArmJoints.Clear ( );
			this.rightArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.WristRight ] );
			this.rightArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.ElbowRight ] );
			this.rightArmJoints.Add ( this.kinectSkeleton.Joints [ JointType.ShoulderRight ] );
		}*/

		private void Log ( string log )
		{
			Console.ForegroundColor = consoleColor;

			Constant.Log ( "[" + this.GetType ( ).Name + "] " + log );

			Console.ResetColor ( );
		}
	}
}