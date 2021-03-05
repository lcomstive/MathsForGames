namespace LCUtils
{
	public class Colour
	{
		private uint m_Components;

		public byte r { get => GetRed();	set => SetRed(value);   }
		public byte g { get => GetGreen();	set => SetGreen(value); }
		public byte b { get => GetBlue();	set => SetBlue(value);  }
		public byte a { get => GetAlpha();	set => SetAlpha(value); }

		public static Colour Black => new Colour(0, 0, 0);
		public static Colour White => new Colour(255, 255, 255);
		public static Colour Red => new Colour(255);
		public static Colour Green => new Colour(0, 255);
		public static Colour Blue => new Colour(0, 0, 255);

		public Colour(byte red = 0, byte green = 0, byte blue = 0, byte alpha = 255)
		{
			m_Components = (uint)(
				alpha		  |
				blue	<< 8  |
				green	<< 16 |
				red		<< 24
				);
		}

		public void SetRed(byte red)	 => m_Components = (m_Components & 0x00ffffff) | (uint)(red	  << 24);
		public void SetGreen(byte green) => m_Components = (m_Components & 0xff00ffff) | (uint)(green << 16);
		public void SetBlue(byte blue)	 => m_Components = (m_Components & 0xffff00ff) | (uint)(blue  << 8);
		public void SetAlpha(byte alpha) => m_Components = (m_Components & 0xffffff00) | alpha;

		public byte GetRed()	=> (byte)((m_Components & 0xff000000) >> 24);
		public byte GetGreen()	=> (byte)((m_Components & 0x00ff0000) >> 16);
		public byte GetBlue()	=> (byte)((m_Components & 0x0000ff00) >> 8);
		public byte GetAlpha()	=> (byte) (m_Components & 0x000000ff);

		public override string ToString() => $"({r}, {g}, {b}, {a})";
		public string ToHexString() => $"#{System.Convert.ToString(m_Components, 16)}";

		public static implicit operator Colour(Raylib_cs.Color c) => new Colour(c.r, c.g, c.b, c.a);
		public static implicit operator Raylib_cs.Color(Colour c) => new Raylib_cs.Color(c.r, c.g, c.b, c.a);
	}
}
