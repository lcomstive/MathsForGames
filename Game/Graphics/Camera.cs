using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCECS;
using LCUtils;
using Raylib_cs;

namespace Game.Graphics
{
	public class Camera
	{
		public const int DefaultFOV = 60;

		public Matrix4 ViewMatrix => m_ViewMatrix;
		public Matrix4 ProjectionMatrix => m_ProjectionMatrix;

		#region Getter Setters
		public Vector3 Position
		{
			get => m_Position;
			set
			{
				if (m_Position == value)
					return;
				m_Dirty = true;
				m_Position = value;
			}
		}

		public bool Orthographic
		{
			get => m_Orthographic;
			set
			{
				if (m_Orthographic == value)
					return; // Don't recalculate matrices if same
				m_Dirty = true;
				m_Orthographic = value;
			}
		}

		public float FOV
		{
			get => m_FOV;
			set
			{
				if (m_FOV == value)
					return; // No change
				m_FOV = value;

				// Only re-calculate if perspective mode, orthographic doesn't use FOV
				m_Dirty = !m_Orthographic;
			}
		}

		public float Near
		{
			get => m_Near;
			set
			{
				if (m_Near == value)
					return; // No change
				m_Near = value;
				m_Dirty = !m_Orthographic;
			}
		}

		public float Far
		{
			get => m_Far;
			set
			{
				if (m_Far == value)
					return; // No change
				m_Far = value;
				m_Dirty = !m_Orthographic;
			}
		}

		public Vector3 Up
		{
			get => m_Up;
			set
			{
				if (m_Up == value)
					return; // No change
				m_Up = value;
			}
		}
		#endregion

		private float m_Far;
		private float m_Near;

		private float m_FOV;
		private bool m_Dirty;
		private Vector3 m_Position;
		private bool m_Orthographic;

		private Vector3 m_Up;
		private Vector3 m_Forward;

		private Matrix4 m_ViewMatrix;
		private Matrix4 m_ProjectionMatrix;

		public Camera(bool orthographic = false) : this(new Vector3(0, 0, -10), DefaultFOV, true) { }
		public Camera(Vector3 position, float fov = DefaultFOV, bool orthographic = true)
		{
			m_Near = 0.1f;
			m_Far = 1000.0f;

			m_FOV = fov;
			m_Position = position;
			m_Orthographic = orthographic;

			m_Up = Vector3.up;
			m_Forward = Vector3.forward;

			m_Dirty = true;
			CalculateMatrices();
		}

		public Vector2 WorldToScreen(Vector2 position) => WorldToScreen(new Vector3(position, Position.z));
		public Vector2 WorldToScreen(Vector3 position)
		{
			Vector4 worldToScreen = ProjectionMatrix * ViewMatrix * new Vector4(position, 1);
			return new Vector2(
				(worldToScreen.x + 1.0f) / 2f * Raylib.GetScreenWidth(),
				(1.0f - worldToScreen.y) / 2f * Raylib.GetScreenHeight()
				);
		}

		public Vector3 ScreenToWorld(Vector2 position)
		{
			float x = 2f * position.x / Raylib.GetScreenWidth()  - 1.0f;
			float y = -2f * position.y / Raylib.GetScreenHeight() + 1.0f;

			Matrix4 viewProjectionInverse = (ProjectionMatrix * ViewMatrix).inverted;
			Vector4 worldSpace = new Vector4(x, y, 0, 1);

			return (worldSpace * viewProjectionInverse).xyz;
		}

		internal void CalculateMatrices(bool forceUpdate = false)
		{
			if (!m_Dirty && !forceUpdate)
				return; // Nothing to change, don't calculate anything
			m_Dirty = false;

			Vector3 cameraRight = -m_Forward.Cross(m_Up);

			Vector3 forward = m_Forward;
			m_ViewMatrix = new Matrix4(
				cameraRight.x, m_Up.x, forward.x, m_Position.x,
				cameraRight.y, m_Up.y, forward.y, m_Position.y,
				cameraRight.z, m_Up.z, forward.z, m_Position.z,
				0, 0, 0, 1
				);

			Vector2 screenSize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
			Vector2 halfScreenSize = screenSize / 2f;
			if (m_Orthographic)
				m_ProjectionMatrix = Matrix4.CreateOrthographic(
					-halfScreenSize.x,
					 halfScreenSize.x,
					-halfScreenSize.y,
					 halfScreenSize.y,
					 m_Near, m_Far);
			else
				m_ProjectionMatrix = Matrix4.CreatePerspective(screenSize.x, screenSize.y, m_FOV, m_Near, m_Far);
		}
	}
}
