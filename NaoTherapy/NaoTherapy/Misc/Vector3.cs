using System;
using Microsoft.Kinect;

namespace Misc
{
	// custom 3D Vector class
	public class Vector3
	{
		public static Vector3 Zero = GetZero ( );

		public float X, Y, Z;

		public Vector3 ( )
		{
			this.X = this.Y = this.Z = 0;
		}
		
		public Vector3 ( float x, float y, float z )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		public Vector3 ( float xyz )
		{
			this.X = this.Y = this.Z = xyz;
		}
		
		public Vector3 ( Vector3 v )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
		}
		
		public Vector3 ( SkeletonPoint sp )
		{
			this.X = sp.X * 100;
			this.Y = sp.Y * 100;
			this.Z = sp.Z * 100;
		}
		
		public static Vector3 GetZero ( )
		{
			return new Vector3 ( 0 );
		}
		
		public float DotProduct ( Vector3 other )
		{
			return this.X * other.X + this.Y * other.Y + this.Z * other.Z;
		}
		
		public static Vector3 operator + ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 ( v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z );
		}

		public static Vector3 operator - ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 ( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
		}
		
		public static Vector3 operator - ( Vector3 v )
		{
			return new Vector3 ( -v.X, -v.Y, -v.Z );
		}
		
		public static Vector3 operator * ( Vector3 v, float scalar )
		{
			return new Vector3 ( v.X * scalar, v.Y * scalar, v.Z * scalar );
		}
		
		public static Vector3 operator / ( Vector3 v, float scalar )
		{
			return new Vector3 ( v.X / scalar, v.Y / scalar, v.Z / scalar );
		}
		
		public static bool operator == ( Vector3 v1, Vector3 v2 )
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
		}
		
		public static bool operator != ( Vector3 v1, Vector3 v2 )
		{
			return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
		}
		
		public static Vector3 CrossProduct ( Vector3 a, Vector3 b )
		{
			float nx = a.Y * b.Z - a.Z * b.Y;
			float ny = a.Z * b.X - a.X * b.Z;
			float nz = a.X * b.Y - a.Y * b.X;
			
			return new Vector3 ( nx, ny, nz );
		}
		
		public Vector3 Add ( Vector3 v )
		{
			this.X += v.X;
			this.Y += v.Y;
			this.Z += v.Z;
			
			return this;
		}
		
		public float DistanceTo ( Vector3 v )
		{
			float dx = this.X - v.X;
			float dy = this.Y - v.Y;
			float dz = this.Z - v.Z;
			
			return ( float )Math.Sqrt ( dx * dx + dy * dy + dz * dz );
		}
		
		public float Length ( )
		{
			return this.DistanceTo ( Vector3.Zero );
		}
		
		public Vector3 Clone ( )
		{
			return new Vector3 ( this );
		}

		public override string ToString ( )
        {
            return ( int )Math.Round ( X ) + "\t" + ( int )Math.Round ( Y ) + "\t" + ( int )Math.Round ( Z );
        }

		public string ToString ( bool temp )
        {
            return Math.Round ( X, 3 ) + "\t" + Math.Round ( Y, 3 ) + "\t" + Math.Round ( Z, 3 );
        }

		public override bool Equals ( object obj )
		{
 			 return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
    }
}