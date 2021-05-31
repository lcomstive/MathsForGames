using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCGF.GameObjects;
using LCGF.Physics;
using LCUtils;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Runtime.InteropServices;

namespace LCGF
{
	public class ApplicationOptions
	{
		public bool VSync { get; set; } = true;
		public string Name { get; set; } = "Game";
		public Vector2 WindowSize { get; set; } = new Vector2(800, 600);

		/// <summary>
		/// 0 Indicates refresh rate.
		/// < 0 Indicates unlimited.
		/// Values under 10 are ignored.
		/// </summary>
		public int FPSLimit { get; set; } = -1;
	}

	/// <summary>
	/// Base class to inherit from for running games using the LCGF
	/// </summary>
	public abstract class Application
	{
		public static Vector2 MousePos	 { get; private set; }
		public static Vector2 ScreenSize { get; private set; }

		private ApplicationOptions m_CreationOptions;

		public Application() : this(null) { }
		public Application(ApplicationOptions options) => m_CreationOptions = options ?? new ApplicationOptions();

		public void Run()
		{
#if !DEBUG
			HideConsoleWindow(); // Only hides console in Release configuration
#endif

			ScreenSize = m_CreationOptions.WindowSize;
			InitWindow((int)ScreenSize.x, (int)ScreenSize.y, m_CreationOptions.Name);
			InitAudioDevice();

			// Window hints
			if (m_CreationOptions.VSync) SetWindowState(ConfigFlag.FLAG_VSYNC_HINT);
			if (m_CreationOptions.FPSLimit == 0) SetTargetFPS(GetMonitorRefreshRate(0));
			if (m_CreationOptions.FPSLimit > 10) SetTargetFPS(m_CreationOptions.FPSLimit);

			Time.Init();
			PhysicsWorld.Init();
			OnInit();

			while(!WindowShouldClose())
			{
				if (IsWindowResized()) 
					ScreenSize = new Vector2(GetScreenWidth(), GetScreenHeight());
				MousePos = GetMousePosition();

				BeginDrawing();
				ClearBackground(Colour.Black);

				OnUpdate();
				PhysicsWorld.Update();
				OnDraw();

				EndDrawing();
				Time.Update();
			}

			OnDestroy();
			CloseAudioDevice();
			CloseWindow();
		}

		/// <summary>
		/// Called before the game loop starts
		/// </summary>
		protected virtual void OnInit() { }

		/// <summary>
		/// Called before GameObjects get rendered
		/// </summary>
		protected virtual void OnUpdate() { }

		protected virtual void OnDraw() { }

		/// <summary>
		/// Called before the application closes
		/// </summary>
		protected virtual void OnDestroy() { }

		#region Show & Hide Console
#if !DEBUG
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hwnd, int ncmdshow);

		private static void HideConsoleWindow() => ShowWindow(GetConsoleWindow(), SW_HIDE);
#endif
#endregion
	}
}
