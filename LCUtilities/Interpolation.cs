using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCUtils
{
	public static class Interpolation
	{
		#region Linear
		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static int Lerp(this int a, int b, float t) => (int)Math.Floor(Lerp((float)a, b, t));

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static int Lerp(ref int a, int b, float t) => a = Lerp(a, b, t);

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static byte Lerp(this byte a, byte b, float t) => (byte)Lerp((int)a, b, t);

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static byte Lerp(ref byte a, byte b, float t) => a = Lerp(a, b, t);

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static float Lerp(this float a, float b, float t)
			=> (1f - (t = Math.Clamp(t, 0f, 1f))) * a + t * b;
		
		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static float Lerp(ref float a, float b, float t)
			=> a = (1f - (t = Math.Clamp(t, 0f, 1f))) * a + t * b;

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static Vector2 Lerp(this Vector2 a, Vector2 b, float t)
			=> (1f - (t = Math.Clamp(t, 0f, 1f))) * a + t * b;
		
		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static Vector2 Lerp(ref Vector2 a, Vector2 b, float t)
			=> a = (1f - (t = Math.Clamp(t, 0f, 1f))) * a + t * b;

		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static Colour Lerp(this Colour a, Colour b, float t)
			=> new Colour(
				a.r.Lerp(b.r, t),
				a.g.Lerp(b.g, t),
				a.b.Lerp(b.b, t),
				a.a.Lerp(b.a, t)
				);
		
		/// <param name="a">Initial value</param>
		/// <param name="b">Final value</param>
		/// <param name="t">0.0-1.0 ranged float, representing a percentage</param>
		public static Colour Lerp(ref Colour a, Colour b, float t)
			=>  a = new Colour(
				a.r.Lerp(b.r, t),
				a.g.Lerp(b.g, t),
				a.b.Lerp(b.b, t),
				a.a.Lerp(b.a, t)
				);
		#endregion

		#region Spline
		public static Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
			=> Lerp(a, b, t).Lerp(Lerp(b, c, t), t);

		public static Vector2 Hermite(Vector2 a, Vector2 tangentA, Vector2 b, Vector2 tangentB, float t)
		{
			t = Math.Clamp(t, 0f, 1f);

			float tsqr = t * t;
			float tcub = tsqr * t;

			float h00 = 2f * tcub - 3 * tsqr + 1;
			float h01 = -2f * tcub + 3 * tsqr;
			float h10 = tcub - 2f * tsqr + t;
			float h11 = tcub - tsqr;

			return h00 * a + h10 * tangentA + h01 * b + h11 * tangentB;
		}

		public static Vector2 CardinalSpline(Vector2 a, Vector2 b, Vector2 c, float t)
		{
			Vector2 tangentA = b - a;
			Vector2 tangentB = c - b;
			return Hermite(a, tangentA, b, tangentB, t);
		}

		public static Vector2 CatmullRomSpline(Vector2 a, Vector2 b, Vector2 c, float t)
		{
			Vector2 tangentA = (b - a) * 0.5f;
			Vector2 tangentB = (c - b) * 0.5f;
			return Hermite(a, tangentA, b, tangentB, t);
		}
		#endregion
	}
}
