using System;

namespace LCUtils
{
	public static class MathUtility
	{
		private static Random m_Random = new Random();

		public static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
		public static float ToRadians(float degrees) => degrees * (float)(Math.PI / 180.0f);
		
		public static double ToDegrees(double radians) => radians * (180.0 / Math.PI);
		public static float ToDegrees(float radians) => radians * (float)(180.0f / Math.PI);

		public static int Random(int min = 0, int max = 100) => m_Random.Next(min, max);
		public static double Random(double min = 0.0, double max = 1.0) => m_Random.NextDouble() * (max - min) + min;
		public static float Random(float min = 0f, float max = 1f) => (float)m_Random.NextDouble() * (max - min) + min;

		public static T Random<T>(this T[] input, int min = 0, int max = -1)
			=> input[Random(Math.Max(min, 0), max == -1 ? input.Length : (Math.Max(max, input.Length)))];

		public static string ToBinaryString(Colour c, bool trimPadding = true) => ToBinaryString(c.Components, trimPadding);
		public static string ToBinaryString(uint value, bool trimPadding = true)
		{
			if (value == 0 && trimPadding) return "0";

			string binary = string.Empty;
			int startIndex = trimPadding ? GetLeftMostSetBit(value) : (sizeof(uint) * 8);
			for (byte i = (byte)startIndex; i > 0; i--)
				binary += IsBitSet(value, (byte)(i - 1)) ? "1" : "0";
			return binary;
		}

		public static string ToBinaryString(byte value)
		{
			if (value == 0) return "0";

			string binary = string.Empty;
			for (byte i = 8; i > 0; i--)
				binary += IsBitSet(value, (byte)(i - 1)) ? "1" : "0";
			return binary;
		}

		/// <summary>
		/// Returns true if the bit at bitIndex is set to 1.
		/// </summary>
		/// <param name="bitIndex">Zero-indexed bit, going right to left</param>
		public static bool IsBitSet(uint value, byte bitIndex)
			// Create bitfield at index, AND bitfield with the value and check for a non-zero value
			=> (value & 1u << bitIndex) != 0;

		public static bool IsLeftMostBitSet(uint value)  => IsBitSet(value, (sizeof(uint) * 8));
		public static bool IsRightMostBitSet(uint value) => IsBitSet(value, 0);

		/// <returns>Index of the right-most set bit, or -1 if none found</returns>
		public static int GetRightMostSetBit(uint value)
		{
			for (byte i = 0; i < (sizeof(uint) * 8); i++)
				if (IsBitSet(value, i))
					return i;
			return -1;
		}

		/// <returns>Index of the left-most set bit, or -1 if none found</returns>
		public static int GetLeftMostSetBit(uint value)
		{
			for (byte i = (sizeof(uint) * 8); i > 0; i--)
				if (IsBitSet(value, (byte)(i - 1)))
					return i;
			return -1;
		}

		public static bool IsPowerOf2(uint value)
		{
			for (int i = 0; i < sizeof(uint) * 8; i++)
				if ((value ^ (1u << i)) == 0)
					return true;
			return false;
		}
	}
}
