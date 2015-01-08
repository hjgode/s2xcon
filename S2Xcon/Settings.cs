using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace S2Xcon.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance;

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("ScanNGo")]
		public string AppDisplayName
		{
			get
			{
				return (string)this["AppDisplayName"];
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("Server={0}; Database={1}; Integrated Security=SSPI; MultipleActiveResultSets=True;")]
		[UserScopedSetting]
		public string ConnectionString
		{
			get
			{
				return (string)this["ConnectionString"];
			}
			set
			{
				this["ConnectionString"] = value;
			}
		}

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("metadata=res://*/Models.DataModel.csdl|res://*/Models.DataModel.ssdl|res://*/Models.DataModel.msl;provider=System.Data.SqlClient;provider connection string=\"Server={0}; Database={1}; Integrated Security=SSPI; MultipleActiveResultSets=True;\"")]
		[UserScopedSetting]
		public string EntityConnectionString
		{
			get
			{
				return (string)this["EntityConnectionString"];
			}
			set
			{
				this["EntityConnectionString"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string Folder
		{
			get
			{
				return (string)this["Folder"];
			}
			set
			{
				this["Folder"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string InstructionCollection
		{
			get
			{
				return (string)this["InstructionCollection"];
			}
			set
			{
				this["InstructionCollection"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("settings.xml")]
		[UserScopedSetting]
		public string SettingsFile
		{
			get
			{
				return (string)this["SettingsFile"];
			}
			set
			{
				this["SettingsFile"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("SQLInstance")]
		[UserScopedSetting]
		public string SQLInstance
		{
			get
			{
				return (string)this["SQLInstance"];
			}
			set
			{
				this["SQLInstance"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("SOFTWARE\\\\Intermec\\\\SmartSystem")]
		[UserScopedSetting]
		public string SQLInstanceKey
		{
			get
			{
				return (string)this["SQLInstanceKey"];
			}
			set
			{
				this["SQLInstanceKey"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("SOFTWARE\\\\Intermec\\\\SmartSystem\\\\Server\\\\Software Store")]
		[UserScopedSetting]
		public string SSLIBPath
		{
			get
			{
				return (string)this["SSLIBPath"];
			}
			set
			{
				this["SSLIBPath"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("TranslateSettingsForSTC.xsl")]
		[UserScopedSetting]
		public string TransformFile
		{
			get
			{
				return (string)this["TransformFile"];
			}
			set
			{
				this["TransformFile"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string URLCollection
		{
			get
			{
				return (string)this["URLCollection"];
			}
			set
			{
				this["URLCollection"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("/*/Subsystem[@Name='WWAN Radio']")]
		[UserScopedSetting]
		public string WWANPath
		{
			get
			{
				return (string)this["WWANPath"];
			}
			set
			{
				this["WWANPath"] = value;
			}
		}

		static Settings()
		{
			Settings.defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		}

		public Settings()
		{
		}

		private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
		{
		}

		private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
		{
		}
	}
}