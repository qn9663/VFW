﻿using System;
using System.Collections.Generic;
using FullSerializer;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    /// <summary>
    /// The hack that's used to persist UnityEngine.Object references
    /// Whenever the serializer comes across a Unity object it stores it to a list
    /// of Unity objects (which Unity serializes) and serializes the index of where
    /// that storage took place.
    /// </summary>
	public class UnityObjectConverter : fsConverter
	{
		private List<UnityObject> serializedObjects
		{
			get { return Serializer.Context.Get<List<UnityObject>>(); }
		}

		public override bool CanProcess(Type type)
		{
			return typeof(UnityObject).IsAssignableFrom(type);
		}

		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = new fsData(serializedObjects.Count);
			serializedObjects.Add(instance as UnityObject);
			return fsResult.Success;
		}

		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			instance = serializedObjects[(int)data.AsInt64];
			return fsResult.Success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			int index = (int)data.AsDictionary["$content"].AsInt64;
			return serializedObjects[index];
		}
	}
}