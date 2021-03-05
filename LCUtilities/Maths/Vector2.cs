﻿using System;

namespace LCUtils
{
	public class Vector2
	{
		private float[] m_Components;

		public float x => m_Components[0];
		public float y => m_Components[1];

		public static Vector2 zero	=> new Vector2();
		public static Vector2 up	=> new Vector2(0,  1);
		public static Vector2 down	=> new Vector2(0, -1);
		public static Vector2 left	=> new Vector2(-1, 0);
		public static Vector2 right => new Vector2( 1, 0);

		public Vector2 normalized
		{
			get
			{
				Vector2 vector = new Vector2(this);
				vector.Normalize();
				return vector;
			}
		}

		#region Constructors
		public Vector2(float x = 0, float y = 0) => m_Components = new float[] { x, y };

		// Copy constructor
		public Vector2(Vector2 other) : this(other.x, other.y) { }
		#endregion
		
		public float Dot(Vector2 b) => x * b.x + y * b.y;

		public float Magnitude() => (float)Math.Sqrt(x * x + y * y);

		public void Normalize()
		{
			float magnitude = Magnitude();
			m_Components[0] /= magnitude;
			m_Components[1] /= magnitude;
		}

		#region Operator Overloads
		/// ADD ///
		public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

		/// SUBTRACT ///
		public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

		/// DIVIDE ///
		public static Vector2 operator /(Vector2 a, float value) => new Vector2(a.x / value, a.y / value);
		public static Vector2 operator /(float value, Vector2 a) => new Vector2(a.x / value, a.y / value);

		/// MULTIPLY ///
		public static Vector2 operator *(Vector2 a, float value) => new Vector2(a.x * value, a.y * value);
		public static Vector2 operator *(float value, Vector2 a) => new Vector2(a.x * value, a.y * value);

		/// CONVERSIONS ///
		public static implicit operator Vector2(System.Numerics.Vector2 v) => new Vector2(v.X, v.Y);
		public static implicit operator System.Numerics.Vector2(Vector2 v) => new System.Numerics.Vector2(v.x, v.y);

		public float this[uint i]
		{
			get => i >= 2 ? 0 : m_Components[i];
			set
			{
				if (i >= 2)
					return;
				m_Components[i] = value;
			}
		}
		#endregion
	}
}
