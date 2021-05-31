using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LCUtils;

namespace Tests
{
	[TestClass]
	public class Matrix
	{
		private const float TestTolerance = 0.0001f;

		private static bool Compare(Vector3 value, Vector3 expected)
		{
			for (int i = 0; i < 3; i++)
				if (Math.Abs(value[i] - expected[i]) > TestTolerance)
					return false;
			return true;
		}

		private static bool Compare(Vector4 value, Vector4 expected)
		{
			for (int i = 0; i < 4; i++)
				if (Math.Abs(value[i] - expected[i]) > TestTolerance)
					return false;
			return true;
		}

		private static bool Compare(Matrix3 value, Matrix3 expected)
		{
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					if (Math.Abs(value[i, j] - expected[i, j]) > TestTolerance)
						return false;
			return true;
		}

		private static bool Compare(Matrix4 value, Matrix4 expected)
		{
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					if (Math.Abs(value[i, j] - expected[i, j]) > TestTolerance)
						return false;
			return true;
		}

		[TestMethod]
		public void TestMatrix3MultiplyMatrix3()
		{
			Matrix3 inputA = new Matrix3(
				0, 3, 1,
				2, -2, 4,
				5, 6, -1
				);
			Matrix3 inputB = new Matrix3(
				7, 3, 0,
				1, 8, -4,
				4, 2, 5
				);
			Matrix3 expected = new Matrix3(
				7, 26, -7,
				28, -2, 28,
				37, 61, -29
				);

			Matrix3 output = inputB * inputA;

			Console.WriteLine($"InputA: {inputA}");
			Console.WriteLine($"InputB: {inputB}");
			Console.WriteLine($"Output: {output}");
			Console.WriteLine($"Expect: {expected}");

			Assert.IsTrue(Compare(output, expected));
		}

		[TestMethod]
		public void Vector4MatrixTransform2()
		{
			Matrix4 m4c = new Matrix4();
			m4c.SetRotateZ(0.72f);

			Vector4 v4a = new Vector4(13.5f, -48.23f, 862, 0);
			Vector4 v4b = m4c * v4a;

			Assert.IsTrue(Compare(v4b,
				new Vector4(41.951499939f, -27.3578968048f, 862, 0)));
		}

		[TestMethod]
		public void Matrix3SetRotateX()
		{
			Matrix3 input = new Matrix3();
			input.SetRotateX(3.98f);

			Matrix3 expected = new Matrix3(1, 0, 0, 0, -0.668648f, -0.743579f, 0, 0.743579f, -0.668648f);

			Console.WriteLine($"Input: {input}");
			Console.WriteLine($"Expected: {expected}");

			Assert.IsTrue(Compare(input, expected));
		}

		[TestMethod]
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

			Matrix4 output = inputB * inputA;

			Console.WriteLine($"InputA: {inputA}");
			Console.WriteLine($"InputB: {inputB}");
			Console.WriteLine($"Output: {output}");
			Console.WriteLine($"Expect: {expected}");

			Assert.IsTrue(Compare(output, expected));
		}

		[TestMethod]
		public void Matrix3Multiply_2()
		{
			Matrix3 m3a = new Matrix3();
			m3a.SetRotateX(3.98f);

			Matrix3 m3c = new Matrix3();
			m3c.SetRotateZ(9.62f);

			Matrix3 m3d = m3a * m3c;
			Matrix3 expected = new Matrix3(-0.981004655361f, 0.129707172513f, 0.14424264431f, 0.193984255195f, 0.655946731567f, 0.729454636574f, 0, 0.743579149246f, -0.668647944927f);

			Console.WriteLine($"Input A: {m3a}");
			Console.WriteLine($"Input B: {m3c}");
			Console.WriteLine($"Result: {m3d}");
			Console.WriteLine($"Expected: {expected}");

			Assert.IsTrue(Compare(m3d, expected));
		}

		[TestMethod]
		public void Matrix4Multiply_2()
		{
			Matrix4 m4b = new Matrix4();
			m4b.SetRotateY(-2.6f);

			Matrix4 m4c = new Matrix4();
			m4c.SetRotateZ(0.72f);

			Matrix4 m4d = m4c * m4b;
			Matrix4 expected = new Matrix4(-0.644213855267f, -0.565019249916f, 0.515501439571f, 0, -0.659384667873f, 0.751805722713f, 0, 0, -0.387556940317f, -0.339913755655f, -0.856888711452f, 0, 0, 0, 0, 1);

			Console.WriteLine($"Input A: {m4b}");
			Console.WriteLine($"Input B: {m4c}");
			Console.WriteLine($"Result: {m4d}");
			Console.WriteLine($"Expected: {expected}");

			Assert.IsTrue(Compare(m4d, expected));
		}
	}
}