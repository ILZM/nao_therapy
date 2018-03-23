using System;
using System.Text;

namespace Misc
{
	class Scanner
	{
		private string	str			= "";
		private int		iterator	= -1;

		public Scanner ( string str )
		{
			this.str		= str;
			this.iterator	= 0;
		}

		public string Next ( )
		{
			string res = "";

			while
			(
				this.iterator < this.str.Length &&
				this.str [ this.iterator ] != ' ' &&
				this.str [ this.iterator ] != '\n'
			)
			{
				res += this.str [ this.iterator ];

				this.iterator++;
			}

			if ( res.Length < this.str.Length )
			{
				this.str = this.str.Replace ( res + " ", "" );

				this.iterator = 0;
			}

			return res;
		}

		public int NextInt ( )
		{
			return Int32.Parse ( this.Next ( ) );
		}
	}
}