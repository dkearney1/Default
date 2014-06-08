using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.Commands
{
	[Serializable]
	public abstract class CommandBase : ICommand
	{
		protected List<Type> _responseTypes;

		public CommandBase()
		{
			this._responseTypes = new List<Type>();

			this.Created = DateTimeOffset.Now;
			this.FromMachine = Environment.MachineName;
			this.FromProcess = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
			this.FromUserName = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
		}

		public CommandBase(ICommand src)
		{
			this._responseTypes = new List<Type>();

			this.CorrelationId = src.CorrelationId;
			this.Created = DateTimeOffset.Now;
			this.FromMachine = src.FromMachine;
			this.FromProcess = src.FromProcess;
			this.FromUserName = src.FromUserName;
			this.ToMachine = Environment.MachineName;
			this.ToProcess = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
		}

		public DateTimeOffset Created { get; protected set; }
		public string FromMachine { get; protected set; }
		public string FromProcess { get; protected set; }
		public string FromUserName { get; protected set; }
		public string ToMachine { get; protected set; }
		public string ToProcess { get; protected set; }
		public string RoutingKey { get { return this.GetType().Name; } }
		public Guid? CorrelationId { get; set; }
		[JsonIgnore] public IEnumerable<Type> ResponseTypes { get { return this._responseTypes; } }
	}
}
