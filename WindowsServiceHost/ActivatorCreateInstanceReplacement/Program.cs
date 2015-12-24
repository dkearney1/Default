using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace ActivatorCreateInstanceReplacement
{
	class Program
	{
		static void Main(string[] args)
		{
			var theAssemblyName = "ExternalAssembly"; // Assembly.GetExecutingAssembly().GetName().Name;
			var theClassName = "ExternalAssembly.ThirdStrategy"; // "ActivatorCreateInstanceReplacement.FirstStrategy";
			var fullTypeName = $"{theClassName}, {theAssemblyName}";

			//var theType = Type.GetType(fullTypeName);
			var theType = Type.GetType(fullTypeName,
					 (aName) => Assembly.LoadFrom($".\\{aName}.dll"),
					 null //(assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) : assem.GetType(name, false, ignore)
					 );

			var theInstance = theType.GetInstance() as IStrategy;

			System.Diagnostics.Debug.WriteLine(fullTypeName);

			theInstance.Execute();
		}
	}
}
