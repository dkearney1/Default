﻿using DKK.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.ServiceHostCommands
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Nack")]
	public sealed class PauseComponentNack : CommandBase
	{
		public PauseComponentNack()
		{
		}

		public PauseComponentNack(ICommand source)
			:base(source)
		{
		}
	}
}
