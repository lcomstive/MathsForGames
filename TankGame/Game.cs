using LCGF;
using System;
using LCUtils;
using Raylib_cs;
using LCGF.Physics;
using LCGF.GameObjects;
using LCGF.GameObjects.UI;
using TankGame.GameObjects;

using static Raylib_cs.Raylib;

namespace TankGame
{
	public class Game : LCGF.Application
	{
		#region Asset Paths
		/// BACKGROUND
		private const string BackgroundCrate = "Tank Sprites/Environment/crateWood.png";
		private const string BackgroundGrassBase = "Tank Sprites/Tiles/tileGrass";
		private const string BackgroundGrass1 = BackgroundGrassBase + "1.png";
		private const string BackgroundGrass2 = BackgroundGrassBase + "2.png";
		private const string BackgroundTileEast = BackgroundGrassBase + "_roadEast.png";
		private const string BackgroundTileNorth = BackgroundGrassBase + "_roadNorth.png";
		private const string BackgroundTileCornerLL = BackgroundGrassBase + "_roadLL.png";
		private const string BackgroundTileCornerLR = BackgroundGrassBase + "_roadLR.png";
		private const string BackgroundTileCornerUL = BackgroundGrassBase + "_roadUL.png";
		private const string BackgroundTileCornerUR = BackgroundGrassBase + "_roadUR.png";
		private const string BackgroundTileCross = BackgroundGrassBase + "_roadCrossing.png";
		#endregion

		private const float BulletSpeed = 150f;
		private const float TankMoveSpeed = 150f;
		private const float TurretRotateSpeed = 50f;
		private const string Player1TankColour = "Red";
		private const string Player2TankColour = "Blue";

		private Tank m_Player1;
		private Tank m_Player2;
		private GameObject m_SceneRoot;

		private SpriteObject[,] m_Background;
		private Vector2 m_BackgroundSize;

		public enum State { InGame, GameOver }
		public State GameState { get; private set; } = State.GameOver;

		private UIText m_GameOverText, m_GameOverSubText;
		private float m_GameOverLerpTime = 0f;

		public Game(ApplicationOptions options = null) : base(options) { }

		/// <summary>
		/// Called before the game loop starts
		/// </summary>
		protected override void OnInit()
		{
			PhysicsWorld.Gravity = Vector2.zero; // Top-down game, no gravity

			ChangeGameState(State.InGame);

			Console.WriteLine();
			PrintHierarchy(m_SceneRoot);
		}

		private void CreateTanks()
		{
			if(m_Player1 != null)
			{
				m_Player1.Destroyed -= OnTankDestroyed;
				m_Player1.Destroy();
			}
			if(m_Player2 != null)
			{
				m_Player2.Destroyed -= OnTankDestroyed;
				m_Player2.Destroy();
			}

			Vector2 positionOffset = new Vector2(MathUtility.Random(0, ScreenSize.x / 3f), MathUtility.Random(0, ScreenSize.y / 4f));
			m_Player1 = new Tank(positionOffset, Player1TankColour, m_SceneRoot);
			m_Player2 = new Tank(-positionOffset, Player2TankColour, m_SceneRoot);

			m_Player1.Destroyed += OnTankDestroyed;
			m_Player2.Destroyed += OnTankDestroyed;
		}

		private void OnTankDestroyed() => ChangeGameState(State.GameOver);

		private void CreateBackground()
		{
			// Destroy existing background
			if(m_Background != null)
				for(int x = 0; x < m_BackgroundSize.x; x++)
					for (int y = 0; y < m_BackgroundSize.y; y++)
						m_Background[x, y].Destroy();

			// Create new background
			const int TileSize = 50;
			int width  = (int)Math.Round(ScreenSize.x + TileSize) / TileSize;
			int height = (int)Math.Round(ScreenSize.y + TileSize) / TileSize;
			int horizontalTile = MathUtility.Random(1, width - 1);
			int verticalTile   = MathUtility.Random(1, height - 1);
			m_Background = new SpriteObject[width, height];
			m_BackgroundSize = new Vector2(width, height);
			Random random = new Random();
			for(int w = 0; w < width; w++)
			{
				for(int h = 0; h < height; h++)
				{
					string sprite = BackgroundGrass1;
					Vector2 position = new Vector2(w * TileSize, h * TileSize) - ScreenSize / 2f;
					Vector2 size = new Vector2(TileSize, TileSize);
					if (w == 0 || h == 0 || w == width - 1 || h == height - 1)
					{
						sprite = BackgroundCrate;

						Rigidbody crateCollider = new Rigidbody("Background Crate Collider", m_SceneRoot);
						crateCollider.Position = position;
						crateCollider.IsKinematic = true;
						crateCollider.Size = size;
						crateCollider.SetCollider(BoxCollider.FromCenter(position, size));
					}
					else if (w == verticalTile && h == horizontalTile)
						sprite = BackgroundTileCross;
					else if (h == horizontalTile)
						sprite = BackgroundTileEast;
					else if (w == verticalTile)
						sprite = BackgroundTileNorth;
					else
						sprite = random.NextDouble() < 0.5f ? BackgroundGrass1 : BackgroundGrass2;
					m_Background[w, h] = new SpriteObject(sprite, 0f, "Background Tile", m_SceneRoot);
					m_Background[w, h].Size = size;
					m_Background[w, h].Position = position;
					m_Background[w, h].ShouldDrawDebugInfo = false;
				}
			}
		}

		/// <summary>
		/// Called before GameObjects get rendered
		/// </summary>
		protected override void OnUpdate()
		{
			switch(GameState)
			{
				default:
				case State.InGame:
					InGameUpdate();
					break;
				case State.GameOver:
					GameOverUpdate();
					break;
			}

			m_SceneRoot.Update();
		}

		private void InGameUpdate()
		{
			#region Player Input
			// Player 1
			if (IsKeyDown(KeyboardKey.KEY_W)) m_Player1.Position += m_Player1.GlobalForward * TankMoveSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_S)) m_Player1.Position -= m_Player1.GlobalForward * TankMoveSpeed * Time.DeltaTime;

			if (IsKeyDown(KeyboardKey.KEY_D)) m_Player1.Rotation -= TurretRotateSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_A)) m_Player1.Rotation += TurretRotateSpeed * Time.DeltaTime;

			if (IsKeyDown(KeyboardKey.KEY_Q)) m_Player1.Turret.Rotation += TurretRotateSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_E)) m_Player1.Turret.Rotation -= TurretRotateSpeed * Time.DeltaTime;

			if (IsKeyPressed(KeyboardKey.KEY_SPACE)) m_Player1.ShootBullet(BulletSpeed);

			// Player 2
			if (IsKeyDown(KeyboardKey.KEY_UP))   m_Player2.Position += m_Player2.GlobalForward * TankMoveSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_DOWN)) m_Player2.Position -= m_Player2.GlobalForward * TankMoveSpeed * Time.DeltaTime;

			if (IsKeyDown(KeyboardKey.KEY_RIGHT)) m_Player2.Rotation -= TurretRotateSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_LEFT))  m_Player2.Rotation += TurretRotateSpeed * Time.DeltaTime;

			if (IsKeyDown(KeyboardKey.KEY_L))		   m_Player2.Turret.Rotation += TurretRotateSpeed * Time.DeltaTime;
			if (IsKeyDown(KeyboardKey.KEY_APOSTROPHE)) m_Player2.Turret.Rotation -= TurretRotateSpeed * Time.DeltaTime;

			if (IsKeyPressed(KeyboardKey.KEY_SEMICOLON)) m_Player2.ShootBullet(BulletSpeed);
			#endregion
		}

		private void GameOverUpdate()
		{
			if(m_GameOverLerpTime < 1.25f) // Fade in over 1.25 seconds
			{
				m_GameOverLerpTime += Time.DeltaTime;
				m_GameOverText.Colour.a = m_GameOverText.Colour.a.Lerp(255, Math.Min(m_GameOverLerpTime, 1f)); // Fade in over 1 second
				m_GameOverSubText.Colour.a = m_GameOverSubText.Colour.a.Lerp(255, Math.Max(m_GameOverLerpTime - 0.2f, 0)); // Delay a small amount for cool effect 👌
			}
			if (IsKeyPressed(KeyboardKey.KEY_ENTER))
				ChangeGameState(State.InGame);
		}

		private void CreateGameOverText()
		{
			m_GameOverLerpTime = 0f;
			m_GameOverText = new UIText("GAME OVER", 64, m_SceneRoot);
			m_GameOverText.Colour = new Colour(255, 255, 255, 0);
			m_GameOverSubText = new UIText("Press ENTER to restart", Vector2.up * 50, m_GameOverText);
			m_GameOverSubText.Colour = new Colour(230, 230, 230, 0);
		}

		public void ChangeGameState(State newState)
		{
			if (newState == State.GameOver)
				CreateGameOverText();
			else
				m_GameOverText?.Destroy();

			if(newState == State.InGame && newState != GameState)
			{
				if(m_Player1 != null)
					m_Player1.Destroyed -= OnTankDestroyed;
				if (m_Player2 != null)
					m_Player2.Destroyed -= OnTankDestroyed;

				m_SceneRoot?.Destroy();
				m_SceneRoot = new GameObject(ScreenSize / 2f, "Scene Root");

				CreateBackground();
				CreateTanks();
			}

			GameState = newState;
		}

		protected override void OnDraw()
		{
			m_SceneRoot.Draw();
			DrawFPS(10, 10);
		}

		/// <summary>
		/// Called before the application closes
		/// </summary>
		protected override void OnDestroy() => GlobalSettings.Save();

		private void PrintHierarchy(GameObject topLevel, string prefix = "")
		{
#if DEBUG
			Console.WriteLine($"{prefix} {topLevel.Name}");
			foreach (GameObject child in topLevel.Children)
				PrintHierarchy(child, prefix + "-");
#endif
		}
	}
}
