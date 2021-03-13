using System;
using System.Collections.Generic;
using System.Linq;

namespace LCECS
{
	// Abstract class for different templates in generic list
	internal abstract class ComponentContainer
	{
		internal Type ComponentType;

		protected ComponentContainer(Type type) => ComponentType = type;
	}

	internal class ComponentContainer<T> : ComponentContainer where T : new()
	{
		internal EntityID[] Entities => m_Components.Keys.ToArray();
		internal T[] Components => m_Components.Values.ToArray();

		private Dictionary<EntityID, T> m_Components;

		internal ComponentContainer() : base(typeof(T)) => m_Components = new Dictionary<EntityID, T>();

		internal bool HasEntity(EntityID id) => m_Components.ContainsKey(id);

		internal T GetComponent(EntityID id) => HasEntity(id) ? m_Components[id] : CreateComponent(id);

		internal T CreateComponent(EntityID id)
		{
			if (HasEntity(id))
				return m_Components[id];

			m_Components.Add(id, new T());
			return m_Components[id];
		}

		internal void RemoveComponent(EntityID id)
		{
			if (HasEntity(id))
				m_Components.Remove(id);
		}

		internal T this[EntityID id]
		{
			get => GetComponent(id);
			set
			{
				if (!m_Components.ContainsKey(id))
					return;
				m_Components[id] = value;
			}
		}
	}
}
