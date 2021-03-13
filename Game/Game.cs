using LCECS;
using System;
using LCUtils;
using Game.Systems;
using Game.Graphics;
using Game.Components;
using System.Diagnostics;
using Game.Components.Physics;
using Game.Components.Physics2D;
using Raylib_cs;

namespace Game
{
	public class Application
	{
		private EntityWorld m_World;
		private Renderer m_Renderer;

		/// FPS TIMING ///
		private int m_FrameCount = 0; // Frames this second
		private float m_FrameCountTime = 0; // Total frame time in the current second
		private float m_FPS = 0; // Last FPS recorded
		private float m_LastTime = 0; // Time of last frame
		private float m_GameTime = 0; // Time since start of game
		private Stopwatch m_Stopwatch;

		/// PLAYER ///
		private const int PlayerSpeed = 100;
		private Camera m_PlayerCamera = new Camera(new Vector3(0, 0, -10));

		// Main Body
		private Entity m_Player;
		private TransformComponent m_PlayerTransform;
		private Rigidbody2DComponent m_PlayerRigidbody;

#if DEBUG
		DrawDebug2DCollidersSystem m_DrawDebug2DCollidersSystem;
#endif

		public void Init()
		{
			m_World = new EntityWorld();
			m_Renderer = new Renderer();

			m_Stopwatch = new Stopwatch();
			m_Stopwatch.Start();

			m_World.AddSystem<Physics2DSystem>();

#if DEBUG
			m_DrawDebug2DCollidersSystem = m_World.AddSystem<DrawDebug2DCollidersSystem>(); // Debug physics colliders
			m_DrawDebug2DCollidersSystem.Draw = false; // Disable by default
#endif

			/// PLAYER ///
			m_Player = m_World.CreateEntity();

			m_PlayerTransform = m_Player.AddComponent<TransformComponent>();
			m_PlayerTransform.Position = new Vector3(0, 0, 0);
			m_PlayerTransform.Scale = new Vector3(100, 100, 100);
			
			m_PlayerRigidbody = m_Player.AddComponent<Rigidbody2DComponent>();
			m_Player.AddComponent<ColouredRectComponent>().Colour = Colour.Red;
			m_Player.AddComponent<Box2DColliderComponent>().Size = m_PlayerTransform.Scale.xy;

			/// CREATE FLOOR ///
			Entity floor = m_World.CreateEntity();
			TransformComponent floorTransform = floor.AddComponent<TransformComponent>();
			floorTransform.Position = new Vector3(100, 0, 0);
			floorTransform.Scale = new Vector3(50, 1, 50);
			// floor.AddComponent<ColouredRectComponent>().Colour = new Colour(255, 255, 255, 100);
			floor.AddComponent<Box2DColliderComponent>().Size = floorTransform.Scale.xy;
			floor.AddComponent<Rigidbody2DComponent>().EnableForces = false;

			/// RANDOM CIRCLE ///
			Entity circle = m_World.CreateEntity();
			TransformComponent circleTransform = circle.AddComponent<TransformComponent>();
			circleTransform.Scale = new Vector3(25, 25, 25);
			circleTransform.Position = new Vector3(200, 200);

			circle.AddComponent<Rigidbody2DComponent>();
			circle.AddComponent<ColouredCircleComponent>().Colour = Colour.Blue;
			circle.AddComponent<Circle2DColliderComponent>().Radius = circleTransform.Scale.x;
		}

		public void Destroy() => m_World.Dispose();

		public void Update()
		{
			/// CALCULATE DELTA TIME ///
			long currentTime = m_Stopwatch.ElapsedMilliseconds;
			float deltaTime = (currentTime - m_LastTime) / 1000.0f;
			m_LastTime = currentTime;
			m_FrameCountTime += deltaTime;
			m_GameTime += deltaTime;

			if(m_FrameCountTime >= 1.0f) // 1 second has passed
			{
				m_FPS = m_FrameCount;
				m_FrameCount = 0;
				m_FrameCountTime -= 1.0f;
			}
			m_FrameCount++;

			/// TOGGLE FULLSCREEN STATE ///
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
				ToggleFullscreen();

			/// PLAYER MOVEMENT ///
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) m_PlayerTransform.Position.x -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) m_PlayerTransform.Position.x += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) m_PlayerTransform.Position.y -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) m_PlayerTransform.Position.y += PlayerSpeed * deltaTime;

			if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))  m_PlayerCamera.Position.x += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) m_PlayerCamera.Position.x -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))	 m_PlayerCamera.Position.y += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))  m_PlayerCamera.Position.y -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_MINUS)) m_PlayerCamera.Position.z += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_EQUAL)) m_PlayerCamera.Position.z -= PlayerSpeed * deltaTime;

			if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
			{
				m_PlayerRigidbody.Velocity += new Vector2(0, 10);
				Console.WriteLine($"Velocity: {m_PlayerRigidbody.Velocity}");
			}

			m_PlayerTransform.Rotation.z += deltaTime * 10f;

			m_World.Update(deltaTime);

			m_Renderer.Draw(m_PlayerCamera, m_World);

			DrawDebugInfo(deltaTime);
		}

		private void DrawDebugInfo(float deltaTime)
        {
#if DEBUG
			if (GlobalSettings.ShowFPS)
			{
				Color textColor = new Color(255, 255, 255, 150);

				Raylib.DrawText($"Frame Time: {deltaTime} ({m_FPS} FPS)", 5, 5, 20, textColor);
				Raylib.DrawText($"Entities: {m_World.EntityCount}", 5, 25, 20, textColor);
			}

			if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB)) // Toggle drawing debug 2D colliders
				m_DrawDebug2DCollidersSystem.Draw = !m_DrawDebug2DCollidersSystem.Draw;

#else
			if (GlobalSettings.ShowFPS)
				Raylib.DrawText($"FPS: {m_FPS}", 10, 10, 16, Color.GREEN);
#endif
		}

		private void ToggleFullscreen()
		{
			FullscreenState fullscreenState = GlobalSettings.FullscreenState;
			if (fullscreenState == FullscreenState.Fullscreen)
				UpdateFullscreenState(FullscreenState.Windowed);
			else
				UpdateFullscreenState(FullscreenState.Fullscreen);
		}

		private void UpdateFullscreenState(FullscreenState newState)
		{
			GlobalSettings.FullscreenState = newState;
			int monitorWidth = Raylib.GetMonitorWidth(GlobalSettings.MonitorIndex);
			int monitorHeight = Raylib.GetMonitorHeight(GlobalSettings.MonitorIndex);
			switch (newState)
			{
				default:
				case FullscreenState.Windowed:
					Raylib.ClearWindowState(ConfigFlag.FLAG_WINDOW_UNDECORATED);
					Raylib.ClearWindowState(ConfigFlag.FLAG_FULLSCREEN_MODE);

					Raylib.SetWindowSize(GlobalSettings.WindowWidth, GlobalSettings.WindowHeight);
					Raylib.SetWindowPosition(
						(monitorWidth / 2) - (GlobalSettings.WindowWidth / 2),
						(monitorHeight / 2) - (GlobalSettings.WindowHeight / 2)
						);
					break;
				case FullscreenState.Borderless:
					Raylib.SetWindowState(ConfigFlag.FLAG_WINDOW_UNDECORATED);
					Raylib.ClearWindowState(ConfigFlag.FLAG_FULLSCREEN_MODE);

					Raylib.SetWindowSize(monitorWidth, monitorHeight);
					Raylib.SetWindowPosition(0, 0);
					break;
				case FullscreenState.Fullscreen:
					Raylib.SetWindowSize(monitorWidth, monitorHeight);
					Raylib.SetWindowPosition(0, 0);
					Raylib.SetWindowState(ConfigFlag.FLAG_FULLSCREEN_MODE);
					break;
			}
		}
	}
}
