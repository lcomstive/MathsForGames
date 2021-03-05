using LCECS;
using Raylib_cs;

namespace Game.Systems
{
	public class DrawDebugInfoSystem : LCECS.System
	{
		protected override void Update(float deltaTime)
		{
			Color textColor = new Color(255, 255, 255, 150);
			int windowHeight = Raylib.GetScreenHeight();

			Raylib.DrawText($"Frame Time: {deltaTime}", 5, windowHeight - 65, 20, textColor);
			Raylib.DrawText($"Entities: {World.EntityCount}", 5, windowHeight - 45, 20, textColor);
			Raylib.DrawText($"Fullscreen Mode: {GlobalSettings.FullscreenState}", 5, windowHeight - 25, 20, textColor);
		}
	}
}
