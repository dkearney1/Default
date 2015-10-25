using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DKK.POCOs
{
	public abstract class PocoBase
	{
		protected PocoBase()
		{
			//this.Id = this.NewGuidComb();
			this.CreateDate = DateTime.UtcNow;
			this.Creator = $"{Environment.UserDomainName}\\{Environment.UserName}";
			this.RowVersion = 0;
		}

		protected Guid _id;
		protected string _creator;
		protected DateTime _createDate;
		protected string _lastUpdater;
		protected DateTime? _lastUpdate;
		protected int _rowVersion;

		[Key]
		[BsonId(IdGenerator = typeof(CombGuidGenerator))]
		public Guid Id
		{
			get { return this._id; }
			set
			{
				SetProperty(ref this._id, value);
			}
		}

		[BsonElement("ct")]
		public string Creator
		{
			get { return this._creator; }
			set
			{
				SetProperty<string>(ref this._creator, value);
			}
		}

		[BsonElement("cd")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime CreateDate
		{
			get { return this._createDate; }
			set
			{
				SetProperty<DateTime>(ref this._createDate, value);
			}
		}

		[BsonElement("lt")]
		public string LastUpdater
		{
			get { return this._lastUpdater; }
			set
			{
				SetProperty<string>(ref this._lastUpdater, value);
			}
		}

		[BsonElement("ld")]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime? LastUpdate
		{
			get { return this._lastUpdate; }
			set
			{
				SetProperty<DateTime?>(ref this._lastUpdate, value);
			}
		}

		[BsonElement("rv")]
		public int RowVersion
		{
			get { return this._rowVersion; }
			set
			{
				SetProperty<int>(ref this._rowVersion, value);
			}
		}

		#region INotifyPropertyChanged Members
		/// <summary>
		/// Multicast event for property change notifications.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Checks if a property already matches a desired value.  Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(storage, value))
				return false;

			storage = value;
			this._lastUpdate = DateTime.UtcNow;
			this._lastUpdater = $"{Environment.UserDomainName}\\{Environment.UserName}";
			this.OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// Notifies listeners that a property value has changed.
		/// </summary>
		/// <param name="propertyName">Name of the property used to notify listeners.  This
		/// value is optional and can be provided automatically when invoked from compilers
		/// that support <see cref="CallerMemberNameAttribute"/>.</param>
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
