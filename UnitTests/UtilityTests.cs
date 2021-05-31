using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LCUtils;

namespace Tests
{
	[TestClass]
	public class Utility
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
			foreach ((float, float) pair in ToDegreesValues)
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
			foreach ((float, float) pair in ToRadiansValues)
				Assert.AreEqual(MathUtility.ToRadians(pair.Item1), pair.Item2);
		}
		#endregion

		#region Colours
		[TestMethod]
		public void ColourSetRed()
		{
			Colour c = new Colour();
			c.SetRed(0x12);

			Assert.AreEqual(0x12000000u, c.Components);
		}

		[TestMethod]
		public void ColourSetGreen()
		{
			Colour c = new Colour();
			c.SetGreen(0x34);

			Assert.AreEqual(0x00340000u, c.Components);
		}

		[TestMethod]
		public void ColourSetBlue()
		{
			Colour c = new Colour();
			c.SetBlue(0x56);

			Assert.AreEqual(0x00005600u, c.Components);
		}

		[TestMethod]
		public void ColourSetAlpha()
		{
			Colour c = new Colour();
			c.SetAlpha(0x78);

			Assert.AreEqual(0x00000078u, c.Components);
		}

		[TestMethod]
		public void ColourShiftRedToGreen()
		{
			Colour colour = new Colour(12, 34, 56, 78);

			uint components =
				(colour.Components & 0x0000ffff) |      // Get blue and alpha components
				(colour.Components & 0xff000000) >> 8;  // Get red value, shift to green. Bitwise-or with other components
			colour = new Colour(components);

			Colour expectedColour = new Colour(0, 12, 56, 78);
			Console.WriteLine(colour.Components);
			Assert.AreEqual(expectedColour, colour);
		}
		#endregion
	}
}