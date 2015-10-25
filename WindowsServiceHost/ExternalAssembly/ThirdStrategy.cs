using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivatorCreateInstanceReplacement;

namespace ExternalAssembly
{
	public class ThirdStrategy : IStrategy
	{
		public void Execute()
		{
			System.Diagnostics.Debug.WriteLine(nameof(ThirdStrategy));
		}
	}
}
