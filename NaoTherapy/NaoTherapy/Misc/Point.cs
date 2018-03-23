using System;

namespace Misc
{
	class Point
	{
		private long x;
		private long y;

		public Point ( long X, long Y )
		{
			this.x = X;
			this.y = Y;
		}

		public long X
		{
			get { return this.x; }
			set { this.x = value; }
		}

		public long Y
		{
			get { return this.y; }
			set { this.y = value; }
		}

		public override bool Equals ( object obj )
		{
			if ( obj is Point )
			{
				Point p = ( Point )obj;

				return this.x == p.x && +this.y == p.y;
			}
			else { return false; }
		}
	}
}