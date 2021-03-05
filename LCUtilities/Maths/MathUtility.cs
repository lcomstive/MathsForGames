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
	}
}
