﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoPos
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="dbBeautyCommsupport")]
	public partial class POSULDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPOS_UL(POS_UL instance);
    partial void UpdatePOS_UL(POS_UL instance);
    partial void DeletePOS_UL(POS_UL instance);
    partial void Inserttrn_log_pos(trn_log_pos instance);
    partial void Updatetrn_log_pos(trn_log_pos instance);
    partial void Deletetrn_log_pos(trn_log_pos instance);
    #endregion
		
		public POSULDataContext() : 
				base(global::AutoPos.Properties.Settings.Default.dbBeautyCommsupportConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public POSULDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public POSULDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public POSULDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public POSULDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<POS_UL> POS_ULs
		{
			get
			{
				return this.GetTable<POS_UL>();
			}
		}
		
		public System.Data.Linq.Table<trn_log_pos> trn_log_pos
		{
			get
			{
				return this.GetTable<trn_log_pos>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.POS_UL")]
	public partial class POS_UL : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _WH_ID;
		
		private string _ABBNO;
		
		private System.DateTime _PTDATE;
		
		private System.DateTime _WORKDATE;
		
		private string _TMCODE;
		
		private string _UFLAG;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnWH_IDChanging(int value);
    partial void OnWH_IDChanged();
    partial void OnABBNOChanging(string value);
    partial void OnABBNOChanged();
    partial void OnPTDATEChanging(System.DateTime value);
    partial void OnPTDATEChanged();
    partial void OnWORKDATEChanging(System.DateTime value);
    partial void OnWORKDATEChanged();
    partial void OnTMCODEChanging(string value);
    partial void OnTMCODEChanged();
    partial void OnUFLAGChanging(string value);
    partial void OnUFLAGChanged();
    #endregion
		
		public POS_UL()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WH_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int WH_ID
		{
			get
			{
				return this._WH_ID;
			}
			set
			{
				if ((this._WH_ID != value))
				{
					this.OnWH_IDChanging(value);
					this.SendPropertyChanging();
					this._WH_ID = value;
					this.SendPropertyChanged("WH_ID");
					this.OnWH_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ABBNO", DbType="VarChar(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string ABBNO
		{
			get
			{
				return this._ABBNO;
			}
			set
			{
				if ((this._ABBNO != value))
				{
					this.OnABBNOChanging(value);
					this.SendPropertyChanging();
					this._ABBNO = value;
					this.SendPropertyChanged("ABBNO");
					this.OnABBNOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PTDATE", DbType="DateTime NOT NULL", IsPrimaryKey=true)]
		public System.DateTime PTDATE
		{
			get
			{
				return this._PTDATE;
			}
			set
			{
				if ((this._PTDATE != value))
				{
					this.OnPTDATEChanging(value);
					this.SendPropertyChanging();
					this._PTDATE = value;
					this.SendPropertyChanged("PTDATE");
					this.OnPTDATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WORKDATE", DbType="DateTime NOT NULL", IsPrimaryKey=true)]
		public System.DateTime WORKDATE
		{
			get
			{
				return this._WORKDATE;
			}
			set
			{
				if ((this._WORKDATE != value))
				{
					this.OnWORKDATEChanging(value);
					this.SendPropertyChanging();
					this._WORKDATE = value;
					this.SendPropertyChanged("WORKDATE");
					this.OnWORKDATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TMCODE", DbType="VarChar(20) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string TMCODE
		{
			get
			{
				return this._TMCODE;
			}
			set
			{
				if ((this._TMCODE != value))
				{
					this.OnTMCODEChanging(value);
					this.SendPropertyChanging();
					this._TMCODE = value;
					this.SendPropertyChanged("TMCODE");
					this.OnTMCODEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UFLAG", DbType="VarChar(1)")]
		public string UFLAG
		{
			get
			{
				return this._UFLAG;
			}
			set
			{
				if ((this._UFLAG != value))
				{
					this.OnUFLAGChanging(value);
					this.SendPropertyChanging();
					this._UFLAG = value;
					this.SendPropertyChanged("UFLAG");
					this.OnUFLAGChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.trn_log_pos")]
	public partial class trn_log_pos : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _whcode;
		
		private System.Nullable<System.DateTime> _workdate;
		
		private string _sms;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnwhcodeChanging(string value);
    partial void OnwhcodeChanged();
    partial void OnworkdateChanging(System.Nullable<System.DateTime> value);
    partial void OnworkdateChanged();
    partial void OnsmsChanging(string value);
    partial void OnsmsChanged();
    #endregion
		
		public trn_log_pos()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_whcode", DbType="NVarChar(10)")]
		public string whcode
		{
			get
			{
				return this._whcode;
			}
			set
			{
				if ((this._whcode != value))
				{
					this.OnwhcodeChanging(value);
					this.SendPropertyChanging();
					this._whcode = value;
					this.SendPropertyChanged("whcode");
					this.OnwhcodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_workdate", DbType="DateTime")]
		public System.Nullable<System.DateTime> workdate
		{
			get
			{
				return this._workdate;
			}
			set
			{
				if ((this._workdate != value))
				{
					this.OnworkdateChanging(value);
					this.SendPropertyChanging();
					this._workdate = value;
					this.SendPropertyChanged("workdate");
					this.OnworkdateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sms", DbType="NVarChar(MAX)")]
		public string sms
		{
			get
			{
				return this._sms;
			}
			set
			{
				if ((this._sms != value))
				{
					this.OnsmsChanging(value);
					this.SendPropertyChanging();
					this._sms = value;
					this.SendPropertyChanged("sms");
					this.OnsmsChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
