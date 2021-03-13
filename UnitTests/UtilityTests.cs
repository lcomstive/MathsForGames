using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LCUtils;

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

	#region Matrices
	[TestMethod()]
	public void TestMatrix3MultiplyMatrix3()
	{
		Matrix3 inputA = new Matrix3(
				10, 20, 10,
				4, 5, 6,
				2, 3, 5
			);
		Matrix3 inputB = new Matrix3(
			3, 2, 4,
			3, 3, 9,
			4, 4, 2
			);
		Matrix3 expected = new Matrix3(
			130, 120, 240,
			51, 47, 73,
			35, 33, 45
			);

		Matrix3 output = inputA * inputB;

		Console.WriteLine($"InputA: {inputA}");
		Console.WriteLine($"InputB: {inputB}");
		Console.WriteLine($"Output: {output}");
		Console.WriteLine($"Expect: {expected}");

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				Assert.AreEqual(expected[i][j], output[i][j]);
	}

	[TestMethod()]
	public void TestMatrix4MultiplyMatrix4()
	{
		Matrix4 inputA = new Matrix4(
				5, 7, 9, 10,
				2, 3, 3, 8,
				8, 10, 2, 3,
				3, 3, 4, 8
			);
		Matrix4 inputB = new Matrix4(
			3, 10, 12, 18,
			12, 1, 4, 9,
			9, 10, 12, 2,
			3, 12, 4, 10
			);
		Matrix4 expected = new Matrix4(
			210, 267, 236, 271,
			93, 149, 104, 149,
			171, 146, 172, 268,
			105, 169, 128, 169
			);

		Matrix4 output = inputA * inputB;

		Console.WriteLine($"InputA: {inputA}");
		Console.WriteLine($"InputB: {inputB}");
		Console.WriteLine($"Output: {output}");
		Console.WriteLine($"Expect: {expected}");

		for (int i = 0; i < 4; i++)
			for (int j = 0; j < 4; j++)
				Assert.AreEqual(expected[i][j], output[i][j]);
	}
	#endregion
}