using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCUtils
{
	public static class MathUtility
	{
		public static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);
		public static float ToRadians(float degrees) => degrees * (float)(Math.PI / 180.0f);
		
		public static double ToDegrees(double radians) => radians * (180.0 / Math.PI);
		public static float ToDegrees(float radians) => radians * (float)(180.0f / Math.PI);

		public static string UIntToBinaryString(uint value, bool trimPadding = true)
		{
			if (value == 0 && trimPadding)
				return "0";

			string binary = string.Empty;
			int startIndex = trimPadding ? GetLeftMostSetBit(value) : (sizeof(uint) * 8);
			startIndex--;
			for (byte i = (byte)startIndex; i > 0; i--)
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

		public static bool IsLeftMostBitSet(uint value) => IsBitSet(value, (sizeof(uint) * 8) - 1);
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
			for (byte i = (sizeof(uint) * 8) - 1; i > 0; i--)
				if (IsBitSet(value, i))
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
