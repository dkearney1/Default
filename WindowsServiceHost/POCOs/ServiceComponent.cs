using MongoDB.Bson.Serialization.Attributes;

namespace DKK.POCOs
{
	public sealed class ServiceComponent : PocoBase
	{
		public ServiceComponent()
			:base()
		{
		}

		private string _assembly;
		private string _class;
		private string _paramsAssembly;
		private string _paramsClass;
		private string _config;
		private string _friendlyName;
		private bool _isActive;
		private bool _isPaused;

		[BsonElement("a")]
		[BsonRequired]
		public string Assembly
		{
			get { return this._assembly; }
			set
			{
				SetProperty<string>(ref this._assembly, value);
			}
		}

		[BsonElement("c")]
		[BsonRequired]
		public string Class
		{
			get { return this._class; }
			set
			{
				SetProperty<string>(ref this._class, value);
			}
		}

		[BsonElement("pa")]
		public string ParamsAssembly
		{
			get { return this._paramsAssembly; }
			set
			{
				SetProperty<string>(ref this._paramsAssembly, value);
			}
		}

		[BsonElement("pc")]
		public string ParamsClass
		{
			get { return this._paramsClass; }
			set
			{
				SetProperty<string>(ref this._paramsClass, value);
			}
		}

		[BsonElement("cf")]
		public string Config
		{
			get { return this._config; }
			set
			{
				SetProperty<string>(ref this._config, value);
			}
		}

		[BsonElement("fn")]
		public string FriendlyName
		{
			get { return this._friendlyName; }
			set
			{
				SetProperty<string>(ref this._friendlyName, value);
			}
		}

		[BsonElement("ac")]
		public bool IsActive
		{
			get { return this._isActive; }
			set
			{
				SetProperty<bool>(ref this._isActive, value);
			}
		}

		[BsonElement("ps")]
		public bool IsPaused
		{
			get { return this._isPaused; }
			set
			{
				SetProperty<bool>(ref this._isPaused, value);
			}
		}
	}
}
