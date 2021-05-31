using System;
using LCUtils;
using LCGF;
using LCGF.Physics;
using LCGF.GameObjects;
using Raylib_cs;
	
namespace PhysicsTest
{
	public class CircleSprite : GameObject
	{
		public Colour Colour { get; set; } = Colour.Yellow;

		public CircleSprite(Vector2 position, Vector2 size, Colour colour) : base(position)
		{
			Size = size;
			Colour = colour;
		}

		protected override void OnDraw()
		{
			Raylib.DrawCircle(
				(int)GlobalPosition.x,
				(int)GlobalPosition.y,
				Size.x,
				Colour);
		}
	}

	public class RectSprite : GameObject
	{
		public Colour Colour { get; set; } = Colour.Yellow;

		public RectSprite(Vector2 position, Vector2 size, float rotation, Colour colour) : base(position, rotation)
		{
			Size = size;
			Colour = colour;
		}

		protected override void OnDraw()
		{
			Rectangle rec = new Rectangle(
				GlobalPosition.x,
				GlobalPosition.y,
				Size.x, Size.y
				);
			Raylib.DrawRectanglePro(
				rec,
				Size / 2f,
				GlobalRotation,
				Colour
				);
		}
	}

	class App : Application
	{
		private const int SpawnedObjectCount = 50;

		private static readonly Colour[] colours = new Colour[]
		{
			Colour.Red,
			Colour.Blue,
			Colour.Green,
			Colour.Yellow,
			Colour.Purple,
			Colour.Magenta,
			Colour.White
		};

		private GameObject m_SceneRoot;
		private Rigidbody m_MouseRect;

		public App(ApplicationOptions options = null) : base(options) { }

		protected override void OnInit()
		{
			PhysicsWorld.Gravity = Vector2.zero;
			PhysicsWorld.Init();

			m_SceneRoot = new GameObject(ScreenSize / 2f, "Root");

			Vector2 randSize = new Vector2(ScreenSize.x / 50, ScreenSize.x / 15);
			for (int i = 0; i < SpawnedObjectCount; i++)
			{
				Rigidbody circle = new Rigidbody($"Circle {i}", m_SceneRoot);
				float radius = MathUtility.Random(randSize.x, randSize.y);

				circle.Position = new Vector2(
					MathUtility.Random(-ScreenSize.x / 2f, ScreenSize.x / 2f),
					MathUtility.Random(-ScreenSize.y / 2f, ScreenSize.y / 2f)
					);

				circle.Mass = radius * 10f;
				circle.Size = new Vector2(radius, radius);
				circle.SetCollider(new CircleCollider(circle.GlobalPosition, radius));
				circle.AddChild(new CircleSprite(Vector2.zero, circle.Size, colours.Random()));
				circle.Velocity = new Vector2(MathUtility.Random(-100, 100), MathUtility.Random(-100, 100));
			}

			const float outerPadding = 10f;

			Rigidbody roof = new Rigidbody("Roof", m_SceneRoot);
			roof.IsKinematic = true;
			roof.Size = new Vector2(ScreenSize.x - outerPadding * 2.5f, 100f);
			roof.Position = new Vector2(0, -ScreenSize.y / 2f - roof.Size.y / 2f + outerPadding);
			roof.SetCollider(BoxCollider.FromCenter(roof.GlobalPosition, roof.Size));

			Rigidbody floor = new Rigidbody("Floor", m_SceneRoot);
			floor.IsKinematic = true;
			floor.Size = roof.Size;
			floor.Position = roof.Position * -1f;
			floor.SetCollider(BoxCollider.FromCenter(floor.GlobalPosition, floor.Size));

			Rigidbody leftWall = new Rigidbody("Left Wall", m_SceneRoot);
			leftWall.IsKinematic = true;
			leftWall.Size = new Vector2(100f, ScreenSize.y - outerPadding * 2.5f);
			leftWall.Position = new Vector2(-ScreenSize.x / 2f - leftWall.Size.x / 2f + outerPadding, 0);
			leftWall.SetCollider(BoxCollider.FromCenter(leftWall.GlobalPosition, leftWall.Size));

			Rigidbody rightWall = new Rigidbody("Right Wall", m_SceneRoot);
			rightWall.IsKinematic = true;
			rightWall.Position = leftWall.Position * -1f;
			rightWall.Size = leftWall.Size;
			rightWall.SetCollider(BoxCollider.FromCenter(rightWall.GlobalPosition, rightWall.Size));

			m_MouseRect = new Rigidbody("Mouse Rect", m_SceneRoot);
			m_MouseRect.Size = new Vector2(randSize.x / 2f, randSize.x / 2f);
			m_MouseRect.Mass = 1000f;
			m_MouseRect.IsKinematic = false;
			m_MouseRect.SetCollider(BoxCollider.FromCenter(Vector2.zero, m_MouseRect.Size));
		}

		protected override void OnDraw()
		{
			m_SceneRoot.Draw();
			Raylib.DrawFPS(10, 10);
		}

		protected override void OnUpdate()
		{
			m_MouseRect.Position = (Vector2)Raylib.GetMousePosition() - m_SceneRoot.Position;
			Raylib.DrawRectangleV(m_MouseRect.GlobalPosition - m_MouseRect.Size / 2f, m_MouseRect.Size, Colour.White);

			m_SceneRoot.Update();
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
