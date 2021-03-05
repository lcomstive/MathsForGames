using Raylib_cs;
using LCECS;
using LCUtils;
using Game.Components;

namespace Game.Systems
{
	public class DrawColouredRectSystem : LCECS.System
	{
		protected override void Update(float deltaTime)
		{
			(EntityID[] entities, TransformComponent[] transforms, ColouredRectComponent[] rects) = World.GetEntitiesWithComponents<TransformComponent, ColouredRectComponent>();

			if (entities == null)
				return; // No entities with components found

			TransformComponent transform;
			ColouredRectComponent colouredRect;
			for(int i = 0; i < entities.Length; i++)
			{
				transform = transforms[i];
				colouredRect = rects[i];

				Vector3 position = transform.Position - (transform.Size / 2f);
				if (transform.Parent != EntityID.InvalidID)
					position += World.GetComponent<TransformComponent>(transform.Parent)?.Position ?? Vector3.zero;

				Raylib.DrawRectanglePro(
					new Rectangle(position.x, position.y, transform.Size.x, transform.Size.y),
					Vector2.zero,
					transform.Rotation,
					colouredRect.Colour
					);
			}
		}
	}
}
