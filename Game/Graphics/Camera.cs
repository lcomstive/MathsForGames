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

		public Vector3 Position
		{
			get => m_Position;
			set
			{
				if (m_Position == value)
					return;
				m_Dirty = true;
				m_Position = value;
				CalculateMatrices();
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
				CalculateMatrices();
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
				CalculateMatrices();
			}
		}

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
			m_Far = 100.0f;

			m_FOV = fov;
			m_Position = position;
			m_Orthographic = orthographic;

			m_Up = Vector3.up;
			m_Forward = Vector3.forward;

			m_Dirty = true;
			CalculateMatrices();
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

			float aspectRatio = screenSize.y / screenSize.x;
			Vector2 screenSizeRatiod = screenSize * aspectRatio;
			if (m_Orthographic)
				m_ProjectionMatrix = Matrix4.CreateOrthographic(screenSizeRatiod.x, screenSizeRatiod.y, m_Near, m_Far);
			else
				m_ProjectionMatrix = Matrix4.CreatePerspective(aspectRatio, m_FOV, m_Near, m_Far);

			Console.WriteLine("Matrix recalculation");
		}
	}
}
