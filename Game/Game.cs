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

		/// CAMERA ///
		private const float CameraSpeed = 300;
		private Camera m_PlayerCamera;

		/// PLAYER ///
		private const float PlayerSpeed = 250;

		// Main Body
		private Entity m_Player;
		private TransformComponent m_PlayerTransform;
		private Rigidbody2DComponent m_PlayerRigidbody;

		public void Init()
		{
			m_World = new EntityWorld();
			m_Renderer = new Renderer();

			m_Stopwatch = new Stopwatch();
			m_Stopwatch.Start();

			m_World.AddSystem<Physics2DSystem>();

			/// CAMERA ///
			m_PlayerCamera = new Camera(new Vector3(0, 0, -1024));
			m_PlayerCamera.Far = 1250;

			/// PLAYER ///
			m_Player = m_World.CreateEntity();

			m_PlayerTransform = m_Player.AddComponent<TransformComponent>();
			m_PlayerTransform.Position = new Vector3(0, 0, 0);
			m_PlayerTransform.Scale = new Vector3(100, 100);
			
			m_PlayerRigidbody = m_Player.AddComponent<Rigidbody2DComponent>();
			m_Player.AddComponent<ColouredRectComponent>().Colour = Colour.Purple;
			m_Player.AddComponent<Box2DColliderComponent>().Size = m_PlayerTransform.Scale.xy;

			/// CREATE FLOOR ///
			Entity floor = m_World.CreateEntity();
			TransformComponent floorTransform = floor.AddComponent<TransformComponent>();
			floorTransform.Position = new Vector3(0, 250, 0);
			floorTransform.Scale = new Vector3(500, 5);
			floor.AddComponent<ColouredRectComponent>().Colour = new Colour(255, 255, 255, 100);
			floor.AddComponent<Box2DColliderComponent>().Size = floorTransform.Scale.xy;
			floor.AddComponent<Rigidbody2DComponent>().EnableForces = false;

			/// RANDOM CIRCLE ///
			Entity circle = m_World.CreateEntity();
			TransformComponent circleTransform = circle.AddComponent<TransformComponent>();
			circleTransform.Position = new Vector3(200, 200);
			circleTransform.Scale = new Vector3(30, 30);

			circle.AddComponent<Rigidbody2DComponent>();
			circle.AddComponent<ColouredCircleComponent>().Colour = Colour.Yellow;
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

			#region Input
			/// TOGGLE FULLSCREEN STATE ///
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
				ToggleFullscreen();

			/// PLAYER MOVEMENT ///
			if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) m_PlayerTransform.Position.x -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) m_PlayerTransform.Position.x += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) m_PlayerTransform.Position.y -= PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) m_PlayerTransform.Position.y += PlayerSpeed * deltaTime;

			/// CAMERA CONTROL ///
			Vector3 playerCameraPosition = new Vector3(m_PlayerCamera.Position);
			float cameraSpeed = CameraSpeed * deltaTime * (m_PlayerCamera.Orthographic ? 1f : -1f);
			if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))	 playerCameraPosition.x += cameraSpeed;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) playerCameraPosition.x -= cameraSpeed;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))	 playerCameraPosition.y += cameraSpeed;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))  playerCameraPosition.y -= cameraSpeed;
			m_PlayerCamera.Position = playerCameraPosition;

			// Change FOV (only affects perspective camera mode)
			if (Raylib.IsKeyDown(KeyboardKey.KEY_MINUS)) m_PlayerCamera.FOV += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_EQUAL)) m_PlayerCamera.FOV -= PlayerSpeed * deltaTime;

			// Switch between orthographic and perspective
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_ALT))
			{
				m_PlayerCamera.Orthographic = !m_PlayerCamera.Orthographic;

				// If perspective mode, set Camera.Up to Vector3.down to flip y axis (not exactly sure why not during orthographic?)
				m_PlayerCamera.Up = m_PlayerCamera.Orthographic ? Vector3.up : Vector3.down; // Because Raylib
			}

			/// TEST PHYSICS SYSTEM ///
			///      TEMPORARY      ///
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
			{
				m_PlayerRigidbody.Velocity += new Vector2(0, 10);
				Console.WriteLine($"Velocity: {m_PlayerRigidbody.Velocity}");
			}

			// Toggle drawing debug 2D colliders
			if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
				m_Renderer.DrawDebugAABBs = !m_Renderer.DrawDebugAABBs;
			#endregion

			// Rotate the player 10 degrees every second, testing rotation
			m_PlayerTransform.Rotation.z += deltaTime * 10f;

			/// UPDATE THE ENTITY WORLD & SYSTEMS ///
			m_World.Update(deltaTime);

			/// RENDER ENTITIES ///
			m_Renderer.Draw(m_PlayerCamera, m_World);

			/// DRAW FPS & DEBUG INFO ///
			DrawDebugInfo(deltaTime);
		}

		private void DrawDebugInfo(float deltaTime)
        {
			Vector2 screenSize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

			Vector3 screenTopLeft = m_PlayerCamera.ScreenToWorld(m_PlayerCamera.Position.xy);
			screenTopLeft += m_PlayerCamera.Position * (m_PlayerCamera.Orthographic ? -1f : 1f);
			screenTopLeft -= screenSize / 2f;
			
			// TODO: Get UI text to draw in screenspace, not world space

#if DEBUG
			if (GlobalSettings.ShowFPS)
			{
				Color textColor = new Color(255, 255, 255, 200);

				Raylib.DrawText($"Frame Time: {deltaTime} ({m_FPS} FPS)", (int)screenTopLeft.x, (int)screenTopLeft.y, 20, textColor);
				Raylib.DrawText($"Entities: {m_World.EntityCount}", (int)screenTopLeft.x, (int)screenTopLeft.y + 25, 20, textColor);
				Raylib.DrawText($"Cam Type: " + (m_PlayerCamera.Orthographic ? "Orth" : "Per"), (int)screenTopLeft.x, (int)screenTopLeft.y + 55, 20, textColor);
			}
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
