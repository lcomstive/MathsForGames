using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MathUtils.Tests
{
	[TestClass()]
	public class UtilityTests
	{
		#region ToDegrees
		readonly (float, float)[] ToDegreesValues =
		{
			( 1.75f, 100.26762f ),
			( 0.40f, 22.918312f ),
			( 0.07f, 4.0107045f )
		};

		[TestMethod()]
		public void ToDegreesTest()
		{
			Console.WriteLine($"1 radian is approximately {MathUtility.ToDegrees(1.0)}°");
			foreach((float, float) pair in ToDegreesValues)
				Assert.AreEqual(MathUtility.ToDegrees(pair.Item1), pair.Item2);
		}
		#endregion

		#region ToRadians
		readonly (float, float)[] ToRadiansValues =
		{
			( 100.26762f, 1.75f ),
			( 22.918312f, 0.40f ),
			( 4.0107045f, 0.07f )
		};

		[TestMethod()]
		public void ToRadiansTest()
		{
			Console.WriteLine($"60° to radians: {MathUtility.ToRadians(60.0)}");
			foreach((float, float) pair in ToRadiansValues)
				Assert.AreEqual(MathUtility.ToRadians(pair.Item1), pair.Item2);
		}
		#endregion
	}
}