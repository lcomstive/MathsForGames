using System;
using System.Linq;
using System.Collections.Generic;

namespace LCECS
{
	public class World : IDisposable
	{
		internal const int MaxEntities = 1024 * 1024; // Just over 1 million

		public int EntityCount => m_EntityCount;

		private Entity[] m_Entities;
		private bool[] m_FreeEntities;
		private int m_EntityCount = 0;

		private List<System> m_Systems;
		private List<ComponentContainer> m_Components;

		/// <summary>
		/// Creates a world instance, ready for systems and entities
		/// </summary>
		public World()
		{
			m_Entities = new Entity[MaxEntities];
			m_FreeEntities = new bool[MaxEntities];

			m_Systems = new List<System>();
			m_Components = new List<ComponentContainer>();

			for(int i = 0; i < MaxEntities; i++)
			{
				m_Entities[i] = new Entity(this, (uint)i);
				m_FreeEntities[i] = true;
			}
		}

		public void Update(float deltaTime)
		{
			for (int i = 0; i < m_Systems.Count; i++)
				m_Systems[i].Update(deltaTime);
		}

		#region Entity
		/// <returns>Valid entity ready for components</returns>
		public Entity CreateEntity()
		{
			EntityID id = GetNextAvailableEntityID();
			m_FreeEntities[id] = false;
			m_EntityCount++;
			return m_Entities[id];
		}

		public Entity CreateEntity<T>() where T : new()
		{
			Entity entity = CreateEntity();
			entity.AddComponent<T>();
			return entity;
		}

		public void DestroyEntity(EntityID id)
		{
			if (id >= m_Entities.Length)
				return;
			m_FreeEntities[id] = true;
			m_EntityCount--;
		}

		/// <param name="id">ID of entity to search for</param>
		/// <returns>Found entity, otherwise null</returns>
		public Entity GetEntity(EntityID id) => EntityExists(id) ? m_Entities[id] : null;

		/// <returns>True for a valid EntityID and a non-null entity value</returns>
		public bool EntityExists(EntityID id) => id < m_Entities.Length - 1 && m_Entities[id] != null;
		#endregion

		#region Component

		#region Add Components
		/// <param name="id">Valid EntityID</param>
		/// <returns>Reference to created component</returns>
		public T AddComponent<T>(EntityID id) where T : new()
		{
			if (!EntityExists(id))
				throw new Exception("Tried adding component to non-existent entity");

			ComponentContainer<T> container = GetComponentContainer<T>();
			if(container == null)
				m_Components.Add(container = new ComponentContainer<T>());
			return container.CreateComponent(id);
		}

		/// <param name="id">Valid EntityID</param>
		/// <returns>References to created component</returns>
		public (T1, T2) AddComponents<T1, T2>(EntityID id) where T1 : new() where T2 : new() =>
			(AddComponent<T1>(id), AddComponent<T2>(id));

		/// <param name="id">Valid EntityID</param>
		/// <returns>References to created component</returns>
		public (T1, T2, T3) AddComponents<T1, T2, T3>(EntityID id) where T1 : new() where T2 : new() where T3 : new() =>
			(AddComponent<T1>(id), AddComponent<T2>(id), AddComponent<T3>(id));

		/// <param name="id">Valid EntityID</param>
		/// <returns>References to created component</returns>
		public (T1, T2, T3, T4) AddComponents<T1, T2, T3, T4>(EntityID id) where T1 : new() where T2 : new() where T3 : new() where T4 : new() =>
			(AddComponent<T1>(id), AddComponent<T2>(id), AddComponent<T3>(id), AddComponent<T4>(id));
		#endregion

		public void RemoveComponent<T>(EntityID id) where T : new()
		{
			if (!EntityExists(id))
				return;
			GetComponentContainer<T>()?.RemoveComponent(id);
		}

		#region Get Components
		/// <summary>
		/// Retrieves a component from the given entity
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>Reference to component, or null if not found</returns>
		public T GetComponent<T>(EntityID id) where T : class, new() => GetComponentContainer<T>()?.GetComponent(id) ?? null;

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2) GetComponents<T1, T2>(EntityID id) where T1 : class, new() where T2 : class, new()
			=> (GetComponent<T1>(id), GetComponent<T2>(id));

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2, T3) GetComponents<T1, T2, T3>(EntityID id) where T1 : class, new() where T2 : class, new() where T3 : class, new()
			=> (GetComponent<T1>(id), GetComponent<T2>(id), GetComponent<T3>(id));

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2, T3, T4) GetComponents<T1, T2, T3, T4>(EntityID id) where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new()
			=> (GetComponent<T1>(id), GetComponent<T2>(id), GetComponent<T3>(id), GetComponent<T4>(id));
		#endregion

		/// <summary>
		/// Checks an entity for existing component
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity contains component, otherwise false</returns>
		public bool HasComponent<T>(EntityID id) where T : new() => GetComponentContainer<T>()?.HasEntity(id) ?? false;

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2>(EntityID id) where T1 : new() where T2 : new()
			=> HasComponent<T1>(id) && HasComponent<T2>(id);

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2, T3>(EntityID id) where T1 : new() where T2 : new() where T3 : new()
			=> HasComponent<T1>(id) && HasComponent<T2>(id) && HasComponent<T3>(id);

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2, T3, T4>(EntityID id) where T1 : new() where T2 : new() where T3 : new() where T4 : new()
			=> HasComponent<T1>(id) && HasComponent<T2>(id) && HasComponent<T3>(id) && HasComponent<T4>(id);

		/// <summary>
		/// Search for all entities with the given component
		/// </summary>
		/// <returns>Tuple containing entity IDs and an equal length array of matching found components</returns>
		public (EntityID[], T[]) GetEntitiesWithComponent<T>() where T : new()
		{
			var container = GetComponentContainer<T>();
			return (container.Entities, container.Components);
		}

		/// <summary>
		/// Search for all entities with the given components
		/// </summary>
		/// <returns>Tuple containing entity IDs and equal length arrays of matching found components</returns>
		public (EntityID[], T1[], T2[]) GetEntitiesWithComponents<T1, T2>() where T1 : new() where T2 : new()
		{
			var containerT1 = GetComponentContainer<T1>();
			var containerT2 = GetComponentContainer<T2>();

			if (containerT1 == null || containerT2 == null)
				return (null, null, null);

			IEnumerable<EntityID> common = containerT1.Entities.Intersect(containerT2.Entities);
			EntityID[] entities = new EntityID[common.Count()];
			T1[] components1 = new T1[entities.Length];
			T2[] components2 = new T2[entities.Length];
			int index = 0;
			foreach(EntityID id in common)
			{
				entities[index] = id;
				components1[index] = containerT1.GetComponent(id);
				components2[index] = containerT2.GetComponent(id);
				index++;
			}

			return (entities, components1, components2);
		}

		/// <summary>
		/// Scan through all component containers to find matching component type
		/// </summary>
		/// <returns>ComponentContainer matching component type if found, otherwise null</returns>
		private ComponentContainer<T> GetComponentContainer<T>() where T : new()
		{
			Type componentType = typeof(T);
			for (int i = 0; i < m_Components.Count; i++)
				if (m_Components[i].ComponentType == componentType)
					return (ComponentContainer<T>)m_Components[i];
			return null;
		}
		#endregion

		#region System
		/// <summary>
		/// Creates a new system, ready for processing entities
		/// </summary>
		/// <returns>Existing system if same type exists, otherwise new instance</returns>
		public T AddSystem<T>() where T : System, new()
		{
			// Check for existing system
			T system = GetSystem<T>();
			if (system != null)
				return system;

			// Create new system
			system = new T();
			system.PreInit(this);
			m_Systems.Add(system);
			return system;
		}

		/// <summary>
		/// Destroys a system
		/// </summary>
		public void RemoveSystem<T>() where T : System
		{
			int systemIndex = GetSystemIndex<T>();
			if (systemIndex < 0)
				return;
			m_Systems[systemIndex].Destroy();
			m_Systems.RemoveAt(systemIndex);
		}

		/// <summary>
		/// Searches for an existing System
		/// </summary>
		/// <returns>Existing system if found, otherwise null</returns>
		public T GetSystem<T>() where T : System
		{
			int systemIndex = GetSystemIndex<T>();
			return systemIndex >= 0 ? (T)m_Systems[systemIndex] : null;
		}

		/// <summary>
		/// checks for an existing system
		/// </summary>
		/// <returns>True if system exists, otherwise false</returns>
		public bool SystemExists<T>() where T : System => GetSystemIndex<T>() >= 0;

		/// <summary>
		/// Searches existing systems by type.
		/// Returns index of system if exists, otherwise -1
		/// </summary>
		/// <typeparam name="T">Type of system to search for</typeparam>
		/// <returns>Index of system, or -1 if not found</returns>
		private int GetSystemIndex<T>() where T : System
		{
			Type type = typeof(T);
			for (int i = 0; i < m_Systems.Count; i++)
				if (m_Systems[i].GetType() == type)
					return i;
			return -1;
		}
		#endregion

		public void Dispose()
		{
			for (int i = 0; i < m_Systems.Count; i++)
				m_Systems[i].Destroy();
		}

		/// <summary>
		/// Scans through all entities and returns the first available ID
		/// </summary>
		/// <returns>Valid entity ID for use</returns>
		/// TODO: Make this more efficient than scanning from start to finish.. Maybe a tree-like search?
		private EntityID GetNextAvailableEntityID()
		{
			EntityID id = 0;
			while (!m_FreeEntities[id])
				id++;

			if (id >= MaxEntities)
				throw new Exception("Exceeded maximum entity count!");

			return id;
		}
	}
}
