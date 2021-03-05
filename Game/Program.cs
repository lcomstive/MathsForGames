using System;
using Raylib_cs;
using Game;

class Program
{
	static void Main(string[] args)
	{
		GlobalSettings.Load();

		Console.WriteLine(GlobalSettings.ToReadableString());

		/// INITIALIZE WINDOW ///
		string gameTitle = $"Game (v{GlobalSettings.CurrentGameVersion})";
		Raylib.InitWindow(GlobalSettings.WindowWidth, GlobalSettings.WindowHeight, gameTitle);
		Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(GlobalSettings.MonitorIndex));

		if(!GlobalSettings.PauseNoFocus) Raylib.SetWindowState(ConfigFlag.FLAG_WINDOW_ALWAYS_RUN);
		if (GlobalSettings.VSync)		 Raylib.SetWindowState(ConfigFlag.FLAG_VSYNC_HINT);

		/// CREATE THE GAME ///
		Application app = new Application();
		app.Init();

		/// GAME LOOP ///
		while(!Raylib.WindowShouldClose())
		{
			Raylib.BeginDrawing();
			Raylib.ClearBackground(Color.BLACK);

			app.Update();

			Raylib.EndDrawing();
		}

		app.Destroy();
		GlobalSettings.Save();
	}
}
