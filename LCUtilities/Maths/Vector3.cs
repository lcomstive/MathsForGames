﻿using System;

namespace LCUtils
{
	public class Vector3
	{
		private float[] m_Components;

		public float x { get => m_Components[0]; set => m_Components[0] = value; }
		public float y { get => m_Components[1]; set => m_Components[1] = value; }
		public float z { get => m_Components[2]; set => m_Components[2] = value; }

		public Vector3 normalized
		{
			get
			{
				Vector3 vector = new Vector3(this);
				vector.Normalize();
				return vector;
			}
		}

		public static Vector3 zero => new Vector3();
		public static Vector3 up => new Vector3(0, 1, 0);
		public static Vector3 down => new Vector3(0, -1, 0);
		public static Vector3 left => new Vector3(-1, 0, 0);
		public static Vector3 right => new Vector3(1, 0, 0);

		#region Constructors
		public Vector3(float x = 0, float y = 0, float z = 0) => m_Components = new float[] { x, y, z };

		// Copy constructors
		public Vector3(Vector2 input) : this(input.x, input.y) { }
		public Vector3(Vector3 input) : this(input.x, input.y, input.z) { }
		#endregion

		public float Dot(Vector3 b) => x * b.x + y * b.y + z * b.z;

		public Vector3 Cross(Vector3 b) => new Vector3(
			y * b.z - z * b.y,
			z * b.x - x * b.z,
			x * b.y - y * b.x
			);

		public float Magnitude() => (float)Math.Sqrt(x * x + y * y + z * z);

		public void Normalize()
		{
			float magnitude = Magnitude();
			for (int i = 0; i < m_Components.Length; i++)
				m_Components[i] /= magnitude;
		}

		#region Operator Overloads
		/// ADD ///
		public static Vector3 operator +(Vector3 a, Vector2 b) => new Vector3(a.x + b.x, a.y + b.y, a.z);
		public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

		public static Vector3 operator +(Vector2 a, Vector3 b) => b + a;

		/// SUBTRACT ///
		public static Vector3 operator -(Vector3 a, Vector2 b) => new Vector3(a.x - b.x, a.y - b.y, a.z);
		public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

		/// DIVIDE ///
		public static Vector3 operator /(Vector3 a, float value) => new Vector3(a.x / value, a.y / value, a.z / value);
		public static Vector3 operator /(float value, Vector3 a) => new Vector3(a.x / value, a.y / value, a.z / value);

		/// MULTIPLY ///
		public static Vector3 operator *(Vector3 a, float value) => new Vector3(a.x * value, a.y * value, a.z * value);
		public static Vector3 operator *(float value, Vector3 a) => new Vector3(a.x * value, a.y * value, a.z * value);

		/// CONVERSIONS ///
		public static implicit operator Vector3(System.Numerics.Vector3 v) => new Vector3(v.X, v.Y, v.Z);
		public static implicit operator System.Numerics.Vector3(Vector3 v) => new System.Numerics.Vector3(v.x, v.y, v.z);

		public float this[uint i]
		{
			get => i >= 3 ? 0 : m_Components[i];
			set
			{
				if (i >= 3)
					return;
				m_Components[i] = value;
			}
		}
		#endregion
	}
}