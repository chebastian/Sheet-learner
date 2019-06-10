using System;
using System.Collections.Generic;

namespace MVVMCore.Utils
{
	public class IOCContainer
	{
		private static Dictionary<Type, Func<object>> classes = new Dictionary<Type, Func<object>>();
		private static Dictionary<Type, Type> InterfaceBindings = new Dictionary<Type, Type>();

		public static TypeName Register<TypeName>(Func<TypeName> creator) where TypeName : class, new()
		{
			return creator();
		}

		public static void Bind(Type t, Type impl) => InterfaceBindings[t] = impl;

		public static void Bind(Type t, Func<object> creator)
		{
			if (classes == null)
				classes = new Dictionary<Type, Func<object>>();

			classes[t] = creator;
		}

		public static T Resolve<T>() where T : class
		{
			var type = typeof(T);
			T instance = null;
			if (classes.ContainsKey(type))
			{
				instance = (T)classes[type]();

				var ctr = type.GetConstructors();
				var frst = ctr[0];
				var para = frst.GetParameters();

				foreach (var p in para)
				{
					var pt = p.GetType();
					var resolevedP = Resolve(p.GetType());
				}

			}

			return instance;
		}

		public static T Create<T>()
		{
			return (T)Resolve(typeof(T));
		}

		public static object Resolve(Type type)
		{
			if (type.IsInterface)
				return Resolve(InterfaceBindings[type]);

			if (!classes.ContainsKey(type))
			{
				var ctr = type.GetConstructors();
				var frst = ctr[0];
				var para = frst.GetParameters();
				var constructedParams = new List<object>();

				foreach (var p in para)
				{
					var pt = p.GetType();
					var resp = Resolve(p.ParameterType);
					constructedParams.Add(resp);
				}

				return frst.Invoke(constructedParams.ToArray());
			}

			return classes[type]();
		}
	}

	internal class ClassA : ISomeClass
	{
		public void dostuff()
		{
			Console.WriteLine("STuff");
		}
	}

	class ClassB : ISomeClass
	{
		public ClassB(ClassA dependency)
		{

		}

		public void dostuff()
		{
			Console.WriteLine("Stuff B ");
		}
	}

	internal interface ISomeClass
	{
		void dostuff();
	}
}
