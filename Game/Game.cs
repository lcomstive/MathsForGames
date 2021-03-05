using LCECS;
using LCUtils;
using Raylib_cs;
using System.Diagnostics;
using Game.Components;
using Game.Systems;

namespace Game
{
	public class Application
	{
		private World m_World;

		/// FPS TIMING ///
		private int m_FrameCount = 0; // Frames this second
		private float m_FrameCountTime = 0; // Total frame time in the current second
		private float m_FPS = 0; // Last FPS recorded
		private float m_LastTime = 0; // Time of last frame
		private float m_GameTime = 0; // Time since start of game
		private Stopwatch m_Stopwatch;

		/// PLAYER ///
		private const int PlayerSpeed = 100;

		// Main Body
		private Entity m_Player;
		private TransformComponent m_PlayerTransform;

		// Player Spinner
		private const float PlayerSpinnerSpeed = 2.5f;
		private const float PlayerSpinnerDistance = 75;
		private TransformComponent m_PlayerSpinnerTransform;

		public void Init()
		{
			m_World = new World();
			m_Stopwatch = new Stopwatch();
			m_Stopwatch.Start();

			m_World.AddSystem<DrawColouredRectSystem>();
#if DEBUG
			m_World.AddSystem<DrawDebugInfoSystem>();
#endif

			m_Player = m_World.CreateEntity();
			m_PlayerTransform = m_Player.AddComponent<TransformComponent>();
			m_PlayerTransform.Size = new Vector2(50, 50);
			m_PlayerTransform.Position = new Vector3(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2, -1);
			m_Player.AddComponent<ColouredRectComponent>().Colour = Colour.Red;

			Entity playerSpinner = m_World.CreateEntity();
			m_PlayerSpinnerTransform = playerSpinner.AddComponent<TransformComponent>();
			m_PlayerSpinnerTransform.Parent = m_Player;
			m_PlayerSpinnerTransform.Size = new Vector2(30, 30);
			m_PlayerSpinnerTransform.Position = new Vector3(0, -75);
			playerSpinner.AddComponent<ColouredRectComponent>().Colour = Colour.Blue;
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
			if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) m_PlayerTransform.Position.y += PlayerSpeed * deltaTime;
			if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) m_PlayerTransform.Position.y -= PlayerSpeed * deltaTime;

			m_PlayerSpinnerTransform.Position.x = (float)System.Math.Sin(m_GameTime * PlayerSpinnerSpeed) * PlayerSpinnerDistance;
			m_PlayerSpinnerTransform.Position.y = (float)System.Math.Cos(m_GameTime * PlayerSpinnerSpeed) * PlayerSpinnerDistance;

			m_World.Update(deltaTime);

			if (GlobalSettings.ShowFPS)
				Raylib.DrawText($"FPS: {m_FPS}", 10, 10, 20, Color.GREEN);
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
