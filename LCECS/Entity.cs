using System;

namespace LCECS
{
	/// <summary>An identifier for individual entities, represented by an unsigned integer</summary>
	public struct EntityID
	{
		public const uint InvalidID = uint.MaxValue;

		private uint m_ID;

		/// <summary>
		/// Private constructor so that it may only be initialized
		/// by implicitly converting from a uint
		/// </summary>
		private EntityID(uint id) => m_ID = id;

		/// Conversion to and from uint
		public static implicit operator uint(EntityID e) => e.m_ID;
		public static implicit operator EntityID(uint value) => new EntityID(value);

		public override string ToString() => m_ID.ToString();
	}

	/// <summary>
	/// Representation of an object in a game world
	/// </summary>
	public class Entity : IDisposable
	{
		/// <summary>Unique identifier</summary>
		public EntityID ID { get; private set; }

		/// <summary>Reference to world that this entity belongs to</summary>
		internal EntityWorld m_World;

		// Constructor being internal prevents outside code from incorrectly instantiating an entity
		/// <param name="parent">World that instantiated this entity</param>
		/// <param name="id">Unique identifier</param>
		internal Entity(EntityWorld parent, EntityID id)
		{
			ID = id;
			m_World = parent;
		}

		/// Component Proxy ///
		/// <returns>Reference to created component</returns>
		public T AddComponent<T>() where T : new() => m_World.AddComponent<T>(ID);

		/// <returns>Tuple of reference to created components</returns>
		public (T1, T2) AddComponents<T1, T2>() where T1 : new() where T2 : new() =>
			m_World.AddComponents<T1, T2>(ID);

		/// <returns>Tuple of reference to created components</returns>
		public (T1, T2, T3) AddComponents<T1, T2, T3>() where T1 : new() where T2 : new() where T3 : new() =>
			m_World.AddComponents<T1, T2, T3>(ID);

		/// <returns>Tuple of reference to created components</returns>
		public (T1, T2, T3, T4) AddComponents<T1, T2, T3, T4>() where T1 : new() where T2 : new() where T3 : new() where T4 : new() =>
			m_World.AddComponents<T1, T2, T3, T4>(ID);

		/// <summary>
		/// Retrieves a component from the given entity
		/// </summary>
		/// <returns>Reference to component, or null if not found</returns>
		public T GetComponent<T>() where T : class, new() => m_World.GetComponent<T>(ID);

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2) GetComponents<T1, T2>() where T1 : class, new() where T2 : class, new() =>
			m_World.GetComponents<T1, T2>(ID);

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2, T3) GetComponents<T1, T2, T3>() where T1 : class, new() where T2 : class, new() where T3 : class, new() =>
			m_World.GetComponents<T1, T2, T3>(ID);

		/// <summary>
		/// Retrieves components from the given entity
		/// </summary>
		/// <returns>Tuple of components (null where not found)</returns>
		public (T1, T2, T3, T4) GetComponents<T1, T2, T3, T4>() where T1 : class, new() where T2 : class, new() where T3 : class, new() where T4 : class, new() =>
			m_World.GetComponents<T1, T2, T3, T4>(ID);

		/// <summary>
		/// Checks an entity for existing component
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity contains component, otherwise false</returns>
		public bool HasComponent<T>() where T : new() => m_World.HasComponent<T>(ID);

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2>() where T1 : new() where T2 : new() =>
			m_World.HasComponents<T1, T2>(ID);

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2, T3>() where T1 : new() where T2 : new() where T3 : new() =>
			m_World.HasComponents<T1, T2, T3>(ID);

		/// <summary>
		/// Checks an entity for existing components
		/// </summary>
		/// <param name="id">Valid EntityID</param>
		/// <returns>True if entity has all components, otherwise false</returns>
		public bool HasComponents<T1, T2, T3, T4>() where T1 : new() where T2 : new() where T3 : new() where T4 : new() =>
			m_World.HasComponents<T1, T2, T3, T4>(ID);

		public void Dispose() => m_World.DestroyEntity(ID);

		public void RemoveComponent<T>() where T : new()  => m_World.RemoveComponent<T>(ID);

		public override string ToString() => $"{{{ID}}}"; // e.g. ID of 2 is "{2}"

		// Allow implicit conversion to an EntityID or uint, purely for easier code-reading
		public static implicit operator uint(Entity e) => e.ID;
		public static implicit operator EntityID(Entity e) => e.ID;
	}
}
