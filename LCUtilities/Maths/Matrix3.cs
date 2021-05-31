using System;

namespace LCUtils
{
	public class Matrix3
	{
		private float[] m_Components;

		public float m1 { get => m_Components[0]; set => m_Components[0] = value; }
		public float m2 { get => m_Components[1]; set => m_Components[1] = value; }
		public float m3 { get => m_Components[2]; set => m_Components[2] = value; }
		public float m4 { get => m_Components[3]; set => m_Components[3] = value; }
		public float m5 { get => m_Components[4]; set => m_Components[4] = value; }
		public float m6 { get => m_Components[5]; set => m_Components[5] = value; }
		public float m7 { get => m_Components[6]; set => m_Components[6] = value; }
		public float m8 { get => m_Components[7]; set => m_Components[7] = value; }
		public float m9 { get => m_Components[8]; set => m_Components[8] = value; }

		public Matrix3 transposed
		{
			get
			{
				Matrix3 value = new Matrix3(this);
				value.Tranpose();
				return value;
			}
		}

		public static Matrix3 Identity => new Matrix3();

		public static Matrix3 Zero => new Matrix3(
			0, 0, 0,
			0, 0, 0,
			0, 0, 0
			);

		public Matrix3()
		{
			m_Components = new float[] {
				1, 0, 0,
				0, 1, 0,
				0, 0, 1
			};
		}

		public Matrix3(Matrix3 other) : this(other.m_Components) { }
		public Matrix3(float[] components)
		{
			m_Components = new float[3 * 3];

			// Copy components into m_Components
			Array.Copy(components, m_Components, Math.Min(components.Length, m_Components.Length));

			// Fill remainder of array if input components are incomplete
			for (int i = components.Length; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix3(float m00, float m10, float m20,
					   float m01, float m11, float m21,
					   float m02, float m12, float m22)
		{
			m_Components = new float[]
			{
				m00, m10, m20,
				m01, m11, m21,
				m02, m12, m22,
			};
		}

		/// <summary>
		/// Converts a column-major matrix to a row-major, and vice versa
		/// </summary>
		public void Tranpose()
		{
			Matrix3 temp = new Matrix3(this);
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					this[i, j] = temp[j, i];
		}

		#region Translate
		public void Translate(Vector2 offset) => Translate(offset.x, offset.y);
		public void Translate(float x, float y)
		{
			this[2, 0] += x;
			this[2, 1] += y;
		}

		public void SetTranslation(float x, float y) => SetTranslate(x, y);
		public void SetTranslation(Vector2 position) => SetTranslate(position.x, position.y);

		public void SetTranslate(Vector2 position) => SetTranslate(position.x, position.y);
		public void SetTranslate(float x, float y)
		{
			this[2, 0] = x;
			this[2, 1] = y;
		}

		public static Matrix3 FromTranslation(float x, float y) => FromTranslation(new Vector2(x, y));
		public static Matrix3 FromTranslation(Vector2 position)
		{
			Matrix3 matrix = new Matrix3();
			matrix.SetTranslate(position);
			return matrix;
		}
		#endregion

		#region Scale
		public void Scale(Vector2 v) => Scale(v.x, v.y);
		public void Scale(float x, float y) => Scale(x, y, 1);
		public void Scale(float x, float y, float z)
		{
			this[0, 0] *= x;
			this[1, 1] *= y;
			this[2, 2] *= z;
		}

		public void SetScaled(Vector2 v) => SetScaled(v.x, v.y);
		public void SetScaled(float x, float y) => SetScaled(x, y, 1);
		public void SetScaled(float x, float y, float z)
		{
			this[0, 0] = x;
			this[1, 1] = y;
			this[2, 2] = z;
		}
		#endregion

		#region Rotate
		/// <summary>
		/// Rotate around the X axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateX(float rotation) => m_Components = (FromRotationX(rotation) * this).m_Components;
		public void SetRotateX(float rotation) => m_Components = FromRotationX(rotation).m_Components;

		/// <summary>
		/// Rotate around the Y axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateY(float rotation) => m_Components = (FromRotationY(rotation) * this).m_Components;
		public void SetRotateY(float rotation) => m_Components = FromRotationY(rotation).m_Components;

		/// <summary>
		/// Rotate around the Z axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateZ(float rotation) => m_Components = (FromRotationZ(rotation) * this).m_Components;
		public void SetRotateZ(float rotation) => m_Components = FromRotationZ(rotation).m_Components;

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix3 FromRotationX(float rotation)
		{
			Matrix3 value = new Matrix3();
			value[1, 1] = (float)Math.Cos(rotation);
			value[1, 2] = -(float)Math.Sin(rotation);
			value[2, 1] = (float)Math.Sin(rotation);
			value[2, 2] = (float)Math.Cos(rotation);

			return value;
		}

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix3 FromRotationY(float rotation)
		{
			Matrix3 value = new Matrix3();
			value[0, 0] = (float)Math.Cos(rotation);
			value[0, 2] = (float)Math.Sin(rotation);
			value[2, 0] = -(float)Math.Sin(rotation);
			value[2, 2] = (float)Math.Cos(rotation);

			return value;
		}

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix3 FromRotationZ(float rotation)
		{
			Matrix3 value = new Matrix3();
			value[0, 0] = (float)Math.Cos(rotation);
			value[0, 1] = -(float)Math.Sin(rotation);
			value[1, 0] = (float)Math.Sin(rotation);
			value[1, 1] = (float)Math.Cos(rotation);

			return value;
		}

		/// <summary>
		/// Rotate around the X, Y & Z axes
		/// </summary>
		/// <param name="eulerRotation">Eulers rotations, in degrees</param>
		public void Rotate(Vector3 eulerRotation) => m_Components = (FromRotation(eulerRotation) * this).m_Components;

		public static Matrix3 FromRotation(Vector3 eulerRotation) =>
				FromRotationZ(eulerRotation.z * ((float)Math.PI / 180f)) *
				FromRotationY(eulerRotation.y * ((float)Math.PI / 180f)) *
				FromRotationX(eulerRotation.x * ((float)Math.PI / 180f));

		#endregion

		#region Conversions & Operators
		public static Vector3 operator *(Matrix3 m, Vector3 v) => new Vector3(
			m[0, 0] * v[0] + m[0, 1] * v[1] + m[0, 2] * v[2],
			m[1, 0] * v[0] + m[1, 1] * v[1] + m[1, 2] * v[2],
			m[2, 0] * v[0] + m[2, 1] * v[1] + m[2, 2] * v[2]
			);

		public static Vector3 operator *(Vector3 v, Matrix3 m) => new Vector3(
			 m[0, 0] * v[0] + m[0, 1] * v[1] + m[0, 2] * v[2],
			 m[1, 0] * v[0] + m[1, 1] * v[1] + m[1, 2] * v[2],
			 m[2, 0] * v[0] + m[2, 1] * v[1] + m[2, 2] * v[2]
			 );

		public static Matrix3 operator *(Matrix3 a, Matrix3 b)
		{
			float[] components = new float[3 * 3];
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 3; k++)
						components[i + j * 3] += a.m_Components[i + k * 3] * b.m_Components[k + j * 3];
			return new Matrix3(components);
		}


		public string ToString(bool useNewLine)
		{
			string s = useNewLine ? Environment.NewLine : string.Empty;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
					s += m_Components[i + j * 3] + (j <= 2 ? ", " : "");
				if (useNewLine)
					s += Environment.NewLine;
			}
			return $"[{s}]";
		}

		public override string ToString() => ToString(true);

		public Vector3 this[int i]
		{
			get => i >= 3 || i < 0 ? Vector3.zero : new Vector3(m_Components[i], m_Components[i + 3], m_Components[i + 3]);
			set
			{
				if (i >= 3 || i < 0)
					return;
				m_Components[i] = value.x;
				m_Components[i + 3] = value.y;
				m_Components[i + 6] = value.z;
			}
		}

		public float this[int i, int j]
		{
			get => (i >= 3 || j >= 3 || i < 0 || j < 0) ? 0 : m_Components[i + (j * 3)];
			set
			{
				if (i >= 3 || j >= 3 || i < 0 || j < 0)
					return;
				m_Components[i + (j * 3)] = value;
			}
		}
		#endregion
	}
}
