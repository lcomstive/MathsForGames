# LC ECS
##### *Lewis Comstive's Entity Component System*

## Example
```cs
using LCECS;
using System;

// Component can be any class, but not a struct.
// No methods get called, these are meant purely for holding data.
public class DebugNameComponent { public string Name = "Entity"; }

// Systems get created and called in the World, and are designed
// to be run once every frame.
// A system often checks or manipulates entities' components
public class PrintDebugNameSystem : LCECS.System
{
	// Preferred over constructor due to timing of the call
	protected override void Init() => Console.WriteLine("Initialized PrintDebugNameSystem");
	protected override void Destroy() => Console.WriteLine("Destroyed PrintDebugNameSystem");

	protected override void Update()
	{
		// Systems have access to the World that made them,
		//  granting easy access to entities and their components.

		// Get all entities with an attached DebugNameComponent
		// Returns a tuple of entities and their corresponding components
		(EntityID[] entities, DebugNameComponent[] debugNames) = World.GetEntitiesWithComponent<DebugNameComponent>();

		if(entities == null)
			return; // No entities were found with that component

		Console.WriteLine("Entities:");
		for (int i = 0; i < entities.Length; i++)
			Console.WriteLine($" [{entities[i]}] {debugNames[i].Name}");
	}
}

class Program
{
	static void Main(string[] args)
	{
		World world = new World();
		world.AddSystem<PrintDebugNameSystem>();

		Entity entity = world.CreateEntity();
		DebugNameComponent debugName = entity.AddComponent<DebugNameComponent>();
		debugName.Name = "Test Entity";

		// Update the world (usually done once per game-loop)
		world.Update();

		Console.WriteLine("\n\nPress ENTER to exit...");
		Console.Read();
	}
}

```

## License
This project is under the [MIT License](LICENSE.txt)