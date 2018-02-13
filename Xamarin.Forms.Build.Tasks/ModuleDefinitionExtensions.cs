using System;
using Mono.Cecil;
using System.Collections.Generic;

namespace Xamarin.Forms.Build.Tasks
{
	public static class ModuleDefinitionExtensions
	{
		class Cache {
			public Dictionary<string, MethodReference> MrMr { get; } = new Dictionary<string, MethodReference>();
			public Dictionary<string, MethodReference> MbMr { get; } = new Dictionary<string, MethodReference>();
			public Dictionary<string, FieldReference> FrFr { get; } = new Dictionary<string, FieldReference>();
			public Dictionary<Type, TypeReference> TTr { get; } = new Dictionary<Type, TypeReference>();
			public Dictionary<string, TypeReference> TrTr { get; } = new Dictionary<string, TypeReference>();
		}

		static Dictionary<ModuleDefinition, Cache> staticCache = new Dictionary<ModuleDefinition, Cache>();

		public static TypeReference GetOrImportReference(this ModuleDefinition module, TypeReference type)
		{
			using(Internals.Performance.StartNew())
				return module.ImportReference(type);
			//if (!staticCache.TryGetValue(module, out Cache cache))
			//	staticCache.Add(module, cache = new Cache());
			//if (cache.TrTr.TryGetValue(type.FullName, out TypeReference returnType))
			//	return returnType;
			//returnType = module.ImportReference(type);
			//cache.TrTr.Add(type.FullName, returnType);
			//return returnType;
		}

		public static TypeReference GetOrImportReference(this ModuleDefinition module, Type type)
		{
			using (Internals.Performance.StartNew())
				return module.ImportReference(type);
			//if (!staticCache.TryGetValue(module, out Cache cache))
			//	staticCache.Add(module, cache = new Cache());
			//if (cache.TTr.TryGetValue(type, out TypeReference returnType))
			//	return returnType;
			//returnType = module.ImportReference(type);
			//cache.TTr.Add(type, returnType);
			//return returnType;
		}

		public static FieldReference GetOrImportReference(this ModuleDefinition module, FieldReference field)
		{
			using (Internals.Performance.StartNew())
				return module.ImportReference(field);
			//if (!staticCache.TryGetValue(module, out Cache cache))
			//	staticCache.Add(module, cache = new Cache());
			//if (cache.FrFr.TryGetValue(field.FullName, out FieldReference returnField))
			//	return returnField;
			//returnField = module.ImportReference(field);
			//cache.FrFr.Add(field.FullName, returnField);
			//return returnField;
		}

		public static MethodReference GetOrImportReference(this ModuleDefinition module, MethodReference method)
		{
			using (Internals.Performance.StartNew())
				return module.ImportReference(method);
			//if (!staticCache.TryGetValue(module, out Cache cache))
			//	staticCache.Add(module, cache = new Cache());
			//if (cache.MrMr.TryGetValue(method.FullName, out MethodReference returnMethod))
			//	return returnMethod;
			//returnMethod = module.ImportReference(method);
			//cache.MrMr.Add(method.FullName, returnMethod);
			//return returnMethod;
		}

		public static MethodReference GetOrImportReference(this ModuleDefinition module, System.Reflection.MethodBase method)
		{
			using (Internals.Performance.StartNew())
				return module.ImportReference(method);
			//if (!staticCache.TryGetValue(module, out Cache cache))
			//	staticCache.Add(module, cache = new Cache());
			//if (cache.MbMr.TryGetValue(method.ToString(), out MethodReference returnMethod))
			//	return returnMethod;
			//returnMethod = module.ImportReference(method);
			//cache.MbMr.Add(method.ToString(), returnMethod);
			//return returnMethod;
		}
	}
}
