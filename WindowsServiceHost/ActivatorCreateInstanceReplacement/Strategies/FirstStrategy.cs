using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivatorCreateInstanceReplacement
{
	internal sealed class FirstStrategy : IStrategy
	{
		public void Execute()
		{
			System.Diagnostics.Debug.WriteLine(nameof(FirstStrategy));
		}
	}
}
