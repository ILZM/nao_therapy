using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using Misc;

namespace Misc
{
	class CustomMath
	{
		public static float RADTODEG = ( float )( 180.0 / Math.PI );
		public static float DEGTORAD = ( float )( Math.PI / 180.0 );

		public static void NormalizeVector ( Vector3 v )
		{
			float len = v.Length ( );

			v.X /= len;
			v.Y /= len;
			v.Z /= len;
		}

		public static double GetSignedAngle ( Vector3 a, Vector3 b )
		{
			double cos		= a.DotProduct ( b ) / ( a.Length ( ) * b.Length ( ) );
			double ang		= Math.Acos ( cos ) * 180 / Math.PI;
			double crossp	= a.Y * b.Z - a.Z * b.Y + a.Z * b.X - a.X * b.Z + a.X * b.Y - a.Y * b.X;
			
			if ( crossp < 0 )
			{
				ang = -ang;
			}

			return ang;
		}
		
		public static double GetAngle ( Vector3 a1, Vector3 b1 )
		{
			Vector3 a = new Vector3 ( a1 );
			Vector3 b = new Vector3 ( b1 );
			
			CustomMath.NormalizeVector ( a );
			CustomMath.NormalizeVector ( b );
			
			double cos	= a.DotProduct ( b ) / ( a.Length ( ) * b.Length ( ) );
			double ang	= Math.Acos ( cos ) * CustomMath.RADTODEG;
			
			return ang;
		}
		
		public static List < Point > DouglasPeuckerReduction ( List < Point > points, Double tolerance )
        {
            if ( points == null || points.Count < 3 )
			{
                return points;
			}

            Int32 firstPoint	= 0;
            Int32 lastPoint		= points.Count - 1;

            List < Int32 > pointIndexsToKeep = new List < Int32 > ( );

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add ( firstPoint );
            pointIndexsToKeep.Add ( lastPoint );

            //The first and the last point can not be the same
            while ( points [ firstPoint ].Equals ( points [ lastPoint ] ) )
            {
                lastPoint--;
            }

            CustomMath.DouglasPeuckerReduction ( points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep );

            List<Point> returnPoints = new List < Point > ( );

            pointIndexsToKeep.Sort ( );

            foreach ( Int32 index in pointIndexsToKeep )
            {
                returnPoints.Add ( points [ index ] );
            }

            return returnPoints;
        }

		private static void DouglasPeuckerReduction ( List < Point > points,
			Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List < Int32 > pointIndexsToKeep )
        {
            Double	maxDistance		= 0;
            Int32	indexFarthest	= 0;

            for ( Int32 index = firstPoint; index < lastPoint; index++ )
            {
                Double distance = CustomMath.PerpendicularDistance ( points [ firstPoint ], points [ lastPoint ], points [ index ] );

                if ( distance > maxDistance)
                {
                    maxDistance		= distance;
                    indexFarthest	= index;
                }
            }

            if ( maxDistance > tolerance && indexFarthest != 0 )
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add ( indexFarthest );

                CustomMath.DouglasPeuckerReduction ( points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep );
                CustomMath.DouglasPeuckerReduction ( points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep );
            }
        }

        public static Double PerpendicularDistance ( Point Point1, Point Point2, Point Point )
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            Double area		= Math.Abs (.5 * ( Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y -
				Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y ) );

            Double bottom	= Math.Sqrt ( Math.Pow ( Point1.X - Point2.X, 2 ) + Math.Pow ( Point1.Y - Point2.Y, 2 ) );
            Double height	= area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else
			//if ( param > 1 )
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane ( Point, new Point ( xx, yy ) );

        }

		/*
		 * https://github.com/ValveSoftware/source-sdk-2013/blob/master/sp/src/mathlib/mathlib_base.cpp
		 */

		/*
		public static Vector3 VectorAngles ( Vector3 fwd )
		{
			float tmp, yaw, pitch;

			yaw = ( float )( Math.Atan2 ( fwd.Y, fwd.X ) * 180 / Math.PI );

			tmp = ( float )Math.Sqrt ( fwd.X * fwd.X + fwd.Y * fwd.Y );

			pitch = ( float )( Math.Atan2 ( -fwd.Z, tmp ) * 180 / Math.PI );

			Vector3 v = new Vector3 ( pitch, yaw, 0 );

			//CustomMath.NormalizeAngle ( v );

			return v;
		}
		
		public static void FromQ ( Vector4 q, Vector3 v )
		{
			float sqw = q.W * q.W;
			float sqx = q.X * q.X;
			float sqy = q.Y * q.Y;
			float sqz = q.Z * q.Z;

			float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			float test = q.X * q.W - q.Y * q.Z;

			// singularity at north pole
			if (test>0.4995f*unit)
			{
				v.Y = 2f * ( float )Math.Atan2 ( q.Y, q.X );
				v.X = ( float )Math.PI / 2;
				v.Z = 0;

				CustomMath.NormalizeAngles ( v * RADTODEG );
			}

			// singularity at south pole
			if (test<-0.4995f*unit)
			{
				v.Y = -2f * ( float )Math.Atan2 (q.Y, q.X);
				v.X = -( float )Math.PI / 2;
				v.Z = 0;

				CustomMath.NormalizeAngles ( v * RADTODEG );
			}

			Vector4 nq = new Vector4 ( );

			nq.W = q.W;
			nq.X = q.X;
			nq.Y = q.Y;
			nq.Z = q.Z;

			v.Y = ( float )Math.Atan2 ( 2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * ( q.Z * q.Z + q.W * q.W ) );     // Yaw
			v.X = ( float )Math.Asin ( 2f * ( q.X * q.Z - q.W * q.Y ) );                             // Pitch
			v.Z = ( float )Math.Atan2 ( 2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * ( q.Y * q.Y + q.Z * q.Z ) );      // Roll

			CustomMath.NormalizeAngles ( v * RADTODEG );
		}

		public static void NormalizeAngles ( Vector3 ang )
		{
			ang.X = NormalizeAngle ( ang.X );
			ang.Y = NormalizeAngle ( ang.Y );
			ang.Z = NormalizeAngle ( ang.Z );
		}

		public static float NormalizeAngle ( float ang )
		{
			while ( ang > 360 ) { ang -= 360; }
			
			while ( ang < 0 ) { ang += 360; }
			
			return ang;
		}*/
	}
}