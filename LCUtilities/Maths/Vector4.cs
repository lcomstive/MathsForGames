using System;

namespace LCUtils
{
	public class Vector4
	{
		private float[] m_Components;

		public float x => m_Components[0];
		public float y => m_Components[1];
		public float z => m_Components[2];
		public float w => m_Components[3];
		public Vector2 xy => new Vector2(x, y);
		public Vector3 xyz => new Vector3(x, y, z);

		public static Vector4 zero => new Vector4();

		#region Constructors
		public Vector4(float x = 0, float y = 0, float z = 0, float w = 0) => m_Components = new float[] { x, y, z, w };

		// Copy constructors
		public Vector4(Vector2 input, float z = 0, float w = 0) : this(input.x, input.y, z, w) { }
		public Vector4(Vector3 input, float w = 0) : this(input.x, input.y, input.z, w) { }
		public Vector4(Vector4 input) : this(input.x, input.y, input.z, input.w) { }
		#endregion

		public float Dot(Vector4 b) => x * b.x + y * b.y + z * b.z + w * b.w;

		public float Magnitude() => (float)Math.Sqrt(x * x + y * y + z * z + w * w);

		public void Normalize()
		{
			float magnitude = Magnitude();
			for (int i = 0; i < m_Components.Length; i++)
				m_Components[i] /= magnitude;
		}

		public Vector4 Cross(Vector4 b) => new Vector4(
				y * b.z - z * b.y,
				z * b.x - x * b.z,
				x * b.y - y * b.x,
				0
			);

		#region Operator Overloads
		/// ADD ///
		public static Vector4 operator +(Vector4 a, Vector2 b) => new Vector4(a.x + b.x, a.y + b.y, a.z, a.w);
		public static Vector4 operator +(Vector4 a, Vector3 b) => new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w);
		public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

		public static Vector4 operator +(Vector2 a, Vector4 b) => b + a;
		public static Vector4 operator +(Vector3 a, Vector4 b) => b + a;

		/// SUBTRACT ///
		public static Vector4 operator -(Vector4 a) => new Vector4(-a.x, -a.y, -a.z, -a.w);
		public static Vector4 operator -(Vector4 a, Vector2 b) => new Vector4(a.x - b.x, a.y - b.y, a.z, a.w);
		public static Vector4 operator -(Vector4 a, Vector3 b) => new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w);
		public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

		/// DIVIDE ///
		public static Vector4 operator /(Vector4 a, float value) => new Vector4(a.x / value, a.y / value, a.z / value, a.w / value);
		public static Vector4 operator /(float value, Vector4 a) => new Vector4(a.x / value, a.y / value, a.z / value, a.w / value);

		/// MULTIPLY ///
		public static Vector4 operator *(Vector4 a, float value) => new Vector4(a.x * value, a.y * value, a.z * value, a.w * value);
		public static Vector4 operator *(float value, Vector4 a) => new Vector4(a.x * value, a.y * value, a.z * value, a.w * value);

		/// CONVERSIONS ///
		public static implicit operator Vector4(System.Numerics.Vector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);
		public static implicit operator System.Numerics.Vector4(Vector4 v) => new System.Numerics.Vector4(v.x, v.y, v.z, v.w);

		public override string ToString() => $"({x}, {y}, {z}, {w})";

		public float this[int i]
		{
			get => i >= 4 || i < 0 ? 0 : m_Components[i];
			set
			{
				if (i >= 4)
					return;
				m_Components[i] = value;
			}
		}
		#endregion
	}
}
