using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace DKK.POCOs
{
	public sealed class ServiceHost : PocoBase
	{
		public ServiceHost()
			:base()
		{
			this._components = new List<ServiceComponent>();
		}

		private string _machine;
		private string _cmdMsgQueue;
		private List<ServiceComponent> _components;

		[BsonElement("m")]
		[BsonRequired]
		public string Machine
		{
			get { return this._machine; }
			set
			{
				SetProperty<string>(ref this._machine, value);
			}
		}

		[BsonElement("cmq")]
		public string CommandMessageQueue
		{
			get { return this._cmdMsgQueue; }
			set
			{
				SetProperty<string>(ref this._cmdMsgQueue, value);
			}
		}

		[BsonElement("scs")]
		public List<ServiceComponent> Components
		{
			get { return this._components; }
			set
			{
				SetProperty<List<ServiceComponent>>(ref this._components, value);
			}
		}
	}
}
