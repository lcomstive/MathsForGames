using System;
using System.Runtime.CompilerServices;

namespace LCUtils
{
	public class Matrix4
	{
		private float[] m_Components;

		public static Matrix4 Identity => new Matrix4(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
			);

		public static Matrix4 Zero = new Matrix4(
			0, 0, 0, 0,
			0, 0, 0, 0,
			0, 0, 0, 0,
			0, 0, 0, 0
			);

		public Matrix4 transposed
		{
			get
			{
				Matrix4 value = new Matrix4(this);
				value.Tranpose();
				return value;
			}
		}

		public Matrix4()
		{
			m_Components = new float[4 * 4];
			for (int i = 0; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix4(Matrix4 other) : this(other.m_Components) { }
		public Matrix4(float[] components)
		{
			m_Components = new float[4 * 4];

			// Copy components into m_Components
			Array.Copy(components, m_Components, Math.Min(components.Length, m_Components.Length));

			// Fill remainder of array if input components are incomplete
			for (int i = components.Length; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix4(float m00, float m10, float m20, float m30,
					   float m01, float m11, float m21, float m31,
					   float m02, float m12, float m22, float m32,
					   float m03, float m13, float m23, float m33)
		{
			m_Components = new float[]
			{
				m00, m10, m20, m30,
				m01, m11, m21, m31,
				m02, m12, m22, m32,
				m03, m13, m23, m33
			};
		}

		#region Translate
		public void Translate(Vector3 position)
		{
			TranslateX(position.x);
			TranslateY(position.y);
			TranslateZ(position.z);
		}

		public void SetTranslate(Vector3 position)
		{
			this[3, 0] = position.x;
			this[3, 1] = position.y;
			this[3, 2] = position.z;
		}

		public void TranslateX(float x) => this[3, 0] += x;
		public void TranslateY(float y) => this[3, 1] += y;
		public void TranslateZ(float z) => this[3, 2] += z;

		public static Matrix4 FromTranslation(Vector3 position)
		{
			Matrix4 value = Identity;
			value.Translate(position);
			return value;
		}
		#endregion

		#region Scale
		public void Scale(float x, float y, float z) => Scale(new Vector3(x, y, z));
		public void Scale(Vector3 scalar) => m_Components = (this * FromScale(scalar)).m_Components;

		public void SetScale(Vector3 scalar) => SetScale(scalar.x, scalar.y, scalar.z);
		public void SetScale(float x, float y, float z)
		{
			this[0, 0] = x;
			this[1, 1] = y;
			this[2, 2] = z;
		}

		public void ScaleX(float scale) => Scale(scale, 0, 0);
		public void ScaleY(float scale) => Scale(0, scale, 0);
		public void ScaleZ(float scale) => Scale(0, 0, scale);

		public void SetScaleX(float scale) => this[0, 0] = scale;
		public void SetScaleY(float scale) => this[1, 1] = scale;
		public void SetScaleZ(float scale) => this[2, 2] = scale;

		public static Matrix4 FromScale(float x, float y, float z) => FromScale(new Vector3(x, y, z));
		public static Matrix4 FromScale(Vector3 scale)
		{
			Matrix4 value = Identity;
			value.SetScale(scale);
			return value;
		}
		#endregion

		#region Rotate
		/// <summary>
		/// Rotate around the X axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateX(float rotation) => m_Components = (this * FromRotationX(rotation)).m_Components;
		public void SetRotateX(float rotation) => m_Components = FromRotationX(rotation).m_Components;

		/// <summary>
		/// Rotate around the Y axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateY(float rotation) => m_Components = (this * FromRotationY(rotation)).m_Components;
		public void SetRotateY(float rotation) => m_Components = FromRotationY(rotation).m_Components;

		/// <summary>
		/// Rotate around the Z axis
		/// </summary>
		/// <param name="rotation">Amount to rotate, in radians</param>
		public void RotateZ(float rotation) => m_Components = (this * FromRotationZ(rotation)).m_Components;
		public void SetRotateZ(float rotation) => m_Components = FromRotationZ(rotation).m_Components;

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix4 FromRotationX(float rotation)
		{
			Matrix4 value = Identity;
			value[1, 1] =  (float)Math.Cos(rotation);
			value[1, 2] = -(float)Math.Sin(rotation);
			value[2, 1] =  (float)Math.Sin(rotation);
			value[2, 2] =  (float)Math.Cos(rotation);

			return value;
		}

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix4 FromRotationY(float rotation)
		{
			Matrix4 value = Identity;
			value[0, 0] =  (float)Math.Cos(rotation);
			value[0, 2] =  (float)Math.Sin(rotation);
			value[2, 0] = -(float)Math.Sin(rotation);
			value[2, 2] =  (float)Math.Cos(rotation);

			return value;
		}

		/// <param name="rotation">Rotation, in radians</param>
		public static Matrix4 FromRotationZ(float rotation)
		{
			Matrix4 value = Identity;
			value[0, 0] =  (float)Math.Cos(rotation);
			value[0, 1] = -(float)Math.Sin(rotation);
			value[1, 0] =  (float)Math.Sin(rotation);
			value[1, 1] =  (float)Math.Cos(rotation);

			return value;
		}

		/// <summary>
		/// Rotate around the X, Y & Z axes
		/// </summary>
		/// <param name="eulerRotation">Eulers rotations, in degrees</param>
		public void Rotate(Vector3 eulerRotation) => m_Components = (FromRotation(eulerRotation) * this).m_Components;

		public static Matrix4 FromRotation(Vector3 eulerRotation) =>
				FromRotationZ(MathUtility.ToRadians(eulerRotation.z)) *
				FromRotationY(MathUtility.ToRadians(eulerRotation.y)) *
				FromRotationX(MathUtility.ToRadians(eulerRotation.x));
		#endregion

		/// <summary>
		/// Converts a column-major matrix to a row-major, and vice versa
		/// </summary>
		public void Tranpose()
		{
			Matrix4 temp = new Matrix4(this);
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					this[i, j] = temp[j, i];
		}

		#region Conversions & Operators
		public static Vector4 operator *(Matrix4 m, Vector4 v) => new Vector4(
			 m[0, 0] * v[0] + m[0, 1] * v[1] + m[0, 2] * v[2] + m[0, 3] * v[3],
			 m[1, 0] * v[0] + m[1, 1] * v[1] + m[1, 2] * v[2] + m[1, 3] * v[3],
			 m[2, 0] * v[0] + m[2, 1] * v[1] + m[2, 2] * v[2] + m[2, 3] * v[3],
			 m[3, 0] * v[0] + m[3, 1] * v[1] + m[3, 2] * v[2] + m[3, 3] * v[3]
			 );

		public static Vector4 operator *(Vector4 v, Matrix4 m) => new Vector4(
			m[0, 0] * v[0] + m[1, 0] * v[1] + m[2, 0] * v[2] + m[3, 0] * v[3],
			m[0, 1] * v[0] + m[1, 1] * v[1] + m[2, 1] * v[2] + m[3, 1] * v[3],
			m[0, 2] * v[0] + m[1, 2] * v[1] + m[2, 2] * v[2] + m[3, 2] * v[3],
			m[0, 3] * v[0] + m[1, 3] * v[1] + m[2, 3] * v[2] + m[3, 3] * v[3]
			);

		public static Matrix4 operator *(Matrix4 a, Matrix4 b)
		{
			float[] components = new float[4 * 4];
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					for (int k = 0; k < 4; k++)
						components[i + j * 4] += a.m_Components[i + k * 4] * b.m_Components[k + j * 4];
			return new Matrix4(components);
		}

		public static implicit operator Matrix4(System.Numerics.Matrix4x4 m) =>
			new Matrix4(
				m.M11, m.M21, m.M31, m.M41,
				m.M12, m.M22, m.M32, m.M42,
				m.M13, m.M23, m.M33, m.M43,
				m.M14, m.M24, m.M34, m.M44
				);

		public static implicit operator System.Numerics.Matrix4x4(Matrix4 m) =>
			new System.Numerics.Matrix4x4(
					m[0, 0], m[1, 0], m[2, 0], m[3, 0],
					m[0, 1], m[1, 1], m[2, 1], m[3, 1],
					m[0, 2], m[1, 2], m[2, 2], m[3, 2],
					m[0, 3], m[1, 3], m[2, 3], m[3, 3]
					);

		public string ToString(bool useNewLine)
		{
			string s = useNewLine ? Environment.NewLine : string.Empty;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
					s += m_Components[j + i * 4] + (j < 3 ? ", " : "");
				if (useNewLine)
					s += Environment.NewLine;
			}
			return $"[{s}]";
		}

		public override string ToString() => ToString(true);

		public Vector4 this[int i]
		{
			get => i >= 4 || i < 0 ? Vector4.zero : new Vector4(m_Components[i], m_Components[i + 4], m_Components[i + 8], m_Components[i + 12]);
			set
			{
				if (i >= 4 || i < 0)
					return;
				m_Components[i] = value.x;
				m_Components[i + 4] = value.y;
				m_Components[i + 8] = value.z;
				m_Components[i + 12] = value.w;
			}
		}

		public float this[int i, int j]
		{
			get => (i >= 4 || j >= 4 || i < 0 || j < 0) ? 0 : m_Components[i + (j * 4)];
			set
			{
				if (i >= 4 || j >= 4 || i < 0 || j < 0)
					return;
				m_Components[i + (j * 4)] = value;
			}
		}
		#endregion
	}
}
