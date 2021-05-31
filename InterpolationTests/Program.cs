using System;
using LCUtils;
using LCGF;
using Raylib_cs;
using LCGF.GameObjects;
using LCGF.GameObjects.UI;
using System.Collections.Generic;

namespace InterpolationTests
{
	class MoveButton : UIButton
	{
		public MoveButton(Vector2 position, Vector2 size, GameObject parent = null)
			: base(position, size, parent)
		{
			Console.WriteLine($"LocalPos: {Position} | GlobalPos: {GlobalPosition}");

			Tint = Colour.White;
			HoverTint = new Colour(230, 230, 230);
			PressTint = new Colour(200, 200, 200);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (IsMouseInside && (
				Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) ||
				Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON)
				))
				Position = Application.MousePos - Parent.GlobalPosition;
		}
	}

	class App : Application
	{
		public App(ApplicationOptions options = null) : base(options) { }

		private GameObject m_SceneRoot;
		private static readonly Vector2 m_ButtonSize = new Vector2(30, 30);

		private List<Vector2> m_InterpolationPoints = new List<Vector2>();
		private List<MoveButton> m_InterpolationPointButtons = new List<MoveButton>();

		protected override void OnInit()
		{
			m_SceneRoot = new GameObject(ScreenSize / 2f, "Root");

			AddPoint(-150,  150);
			AddPoint(-200, -145);
			AddPoint( 160, -175);

			AddPoint(150, -150);
			AddPoint(200,  145);
			AddPoint(-160, 175);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			for (int i = 0; i < m_InterpolationPoints.Count; i++)
				m_InterpolationPoints[i] = m_InterpolationPointButtons[i].GlobalPosition;

			m_SceneRoot.Update();
		}

		protected override void OnDraw()
		{
			base.OnDraw();

			const float Interval = 0.05f;
			for (float t = 0f; t <= 1.0f; t += Interval)
			{
				for(int i = 0; i < m_InterpolationPoints.Count; i += 3)
				{
					Vector2 v = Interpolation.QuadraticBezier(
							m_InterpolationPoints[i],
							m_InterpolationPoints[i + 1],
							m_InterpolationPoints[i + 2],
							t
							);
					Raylib.DrawPixel((int)v.x, (int)v.y, Colour.Green);
				}
			}

			m_SceneRoot.Draw();
		}

		public void AddPoint(float x, float y) => AddPoint(new Vector2(x, y));
		public void AddPoint(Vector2 v)
		{
			m_InterpolationPoints.Add(v);
			m_InterpolationPointButtons.Add(new MoveButton(v, m_ButtonSize, m_SceneRoot));
		}

		public void RemovePoint(int index)
		{
			if (index < 0 || index >= m_InterpolationPoints.Count) return;

			m_InterpolationPoints.RemoveAt(index);
			m_InterpolationPointButtons.RemoveAt(index);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			new App(new ApplicationOptions()
			{
				VSync = false,
				FPSLimit = -1,
				WindowSize = new Vector2(800, 600)
			}).Run();
		}
	}
}
