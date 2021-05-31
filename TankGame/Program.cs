using System;
using LCGF;
using Raylib_cs;
using LCUtils;
using System.Diagnostics;
using TankGame;

class Program
{
	static void Main(string[] args)
	{
		GlobalSettings.Load();
		Console.WriteLine(GlobalSettings.ToReadableString());

		ApplicationOptions options = new ApplicationOptions()
		{
			Name = "Tanks For The Memories",
			WindowSize = new Vector2(GlobalSettings.WindowWidth, GlobalSettings.WindowHeight),
			VSync = GlobalSettings.VSync
		};

		new Game(options).Run();
	}
}