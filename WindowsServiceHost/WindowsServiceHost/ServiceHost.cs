using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DKK.WindowsServiceHost
{
	public interface IReloader
	{
		void Reload();
	}

	public partial class ServiceHost : ServiceBase, IReloader
	{
		private SERVICE_STATUS myServiceStatus;
		private AppDomain WorkerDomain;
		private ServiceHostWorker ServiceHostWorker;

		#region Service Management stuff
		[StructLayout(LayoutKind.Sequential)]
		internal struct SERVICE_STATUS
		{
			public int serviceType;
			public int currentState;
			public int controlsAccepted;
			public int win32ExitCode;
			public int serviceSpecificExitCode;
			public int checkPoint;
			public int waitHint;
		}

		internal enum State
		{
			SERVICE_STOPPED = 0x00000001,
			SERVICE_START_PENDING = 0x00000002,
			SERVICE_STOP_PENDING = 0x00000003,
			SERVICE_RUNNING = 0x00000004,
			SERVICE_CONTINUE_PENDING = 0x00000005,
			SERVICE_PAUSE_PENDING = 0x00000006,
			SERVICE_PAUSED = 0x00000007,
		}
		#endregion

		public ServiceHost()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			if (!System.Diagnostics.Debugger.IsAttached)
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

			this.SetServiceState(State.SERVICE_START_PENDING);

			var fi = new FileInfo(Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().CodeBase));
			DebugTrace($"         EXE Info: {fi.Name}");
			DebugTrace($"      EXE Created: {fi.CreationTime}");
			DebugTrace($"     EXE Modified: {fi.LastWriteTime}");
			DebugTrace($"        64bit EXE: {Environment.Is64BitProcess}");
			DebugTrace($"         64bit OS: {Environment.Is64BitOperatingSystem}");
			DebugTrace($"Current Directory: {Environment.CurrentDirectory}");
			DebugTrace($"   Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
			DebugTrace(string.Empty);
			if (Environment.CurrentDirectory != AppDomain.CurrentDomain.BaseDirectory)
				Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

			// This service is supposed to be the thinnest wrapper,
			// something that never needs updating.

			// It should reference almost nothing. It has a FileLoader and a Worker.
			// It'll load the worker in an AppDomain, specifically so that AppDomain
			// can be unloaded, new files can be copied into place by the FileLoader,
			// and the Worker reloaded in a new AppDomain.

			Task.Run(() =>
			{
				this.RefreshFiles(true);
				this.StartWorker();
			});

			this.SetServiceState(State.SERVICE_RUNNING);
		}

		protected override void OnStop()
		{
			this.SetServiceState(State.SERVICE_STOP_PENDING);

			this.StopWorker();

			this.SetServiceState(State.SERVICE_STOPPED);
		}

		protected override void OnPause()
		{
			this.SetServiceState(State.SERVICE_PAUSE_PENDING);

			Task.Run(() => this.PauseWorker());

			this.SetServiceState(State.SERVICE_PAUSED);
		}

		protected override void OnContinue()
		{
			this.SetServiceState(State.SERVICE_CONTINUE_PENDING);

			Task.Run(() => this.ContinueWorker());

			this.SetServiceState(State.SERVICE_RUNNING);
		}

		public void Reload()
		{
			Task.Run(() =>
			{
				DKKWindowsServiceHostEventSource.Log.Activity("Reload Start");
				this.StopWorker();
				this.RefreshFiles(false);
				this.StartWorker();
				DKKWindowsServiceHostEventSource.Log.Activity("Reload Complete");
			});
		}

		internal void DebugStart()
		{
			OnStart(null);
		}

		internal void DebugStop()
		{
			OnStop();
		}

		private void SetServiceState(State state)
		{
			var handle = this.ServiceHandle;
			myServiceStatus.currentState = (int)state;
			if (handle != IntPtr.Zero)
				NativeMethods.SetServiceStatus(handle, ref myServiceStatus);
			DKKWindowsServiceHostEventSource.Log.ServiceStateChanging(Enum.GetName(typeof(State), state));
		}

		private void RefreshFiles(bool serviceStarting)
		{
			DKKWindowsServiceHostEventSource.Log.DebugTrace("Retrieving configuration");

			// Retrieve the "settings" from the Configuration Service
			var configClient = new SvcHostConfig.SvcHostConfigClient("BasicHttpBinding_ISvcHostConfig", Properties.Settings.Default.SvcHostConfigURL);
			var settings = configClient.GetConfig(new SvcHostConfig.GetConfigRequest()).GetConfigResult;

			// On Service Startup, only Update if the Settings say to.
			// All other times, Update.
			if (serviceStarting && !(settings.SingleOrDefault(s => s.Key == "FileUpdateOnStart").Value == "True"))
			{
				DKKWindowsServiceHostEventSource.Log.DebugTrace("Service starting, and not set to update files on start");
				return;
			}

			var updateLocation = settings.SingleOrDefault(s => s.Key == "FileUpdateLocation").Value;
			if (string.IsNullOrWhiteSpace(updateLocation))
			{
				DKKWindowsServiceHostEventSource.Log.DebugTrace("No update location specified");
				return;
			}

			var ignoredSpecs = new List<string>();

			var tmp = settings.SingleOrDefault(s => s.Key == "FileUpdateIgnoreFileSpecs").Value;
			if (tmp != null)
			{
				ignoredSpecs.AddRange(tmp.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
				DKKWindowsServiceHostEventSource.Log.DebugTrace($"Ignoring {(string.Join(", ", ignoredSpecs))}");
			}

			var fl = new FileLoader(
									updateLocation,
									Environment.CurrentDirectory,
									ignoredSpecs);

			try
			{
				fl.Execute();
			}
			catch (Exception ex)
			{
				DKKWindowsServiceHostEventSource.Log.Exception(ex);
				throw;
			}
		}

		private void StartWorker()
		{
			if (this.WorkerDomain != null)
				throw new NotSupportedException("Worker domain already exists");

			if (this.ServiceHostWorker != null)
				throw new NotSupportedException("ServiceHostWorker already exists");

			string configURL = Properties.Settings.Default.SvcHostConfigURL;

			var adSetup = new AppDomainSetup();
			adSetup.ApplicationName = "ServiceHostWorker";
			this.WorkerDomain = AppDomain.CreateDomain("ServiceHostWorker", null, adSetup);
			this.ServiceHostWorker = this.WorkerDomain.CreateInstanceAndUnwrap(this.GetType().Assembly.FullName, "DKK.WindowsServiceHost.ServiceHostWorker") as ServiceHostWorker;
			this.ServiceHostWorker.ConfigURL = configURL;
			this.ServiceHostWorker.Reloader = this as IReloader;
			this.WorkerDomain.DoCallBack(() => this.ServiceHostWorker.Start());
		}

		private void StopWorker()
		{
			if (this.ServiceHostWorker != null)
			{
				this.ServiceHostWorker.Stop();
				this.ServiceHostWorker.Dispose();
				this.ServiceHostWorker = null;
			}

			if (this.WorkerDomain != null)
			{
				AppDomain.Unload(this.WorkerDomain);
				this.WorkerDomain = null;
			}
		}

		private void PauseWorker()
		{
			if (this.ServiceHostWorker != null)
				this.ServiceHostWorker.Pause();
		}

		private void ContinueWorker()
		{
			if (this.ServiceHostWorker != null)
				this.ServiceHostWorker.Continue();
		}

		private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			DKKWindowsServiceHostEventSource.Log.Exception(e.ExceptionObject as Exception);
		}

		private void DebugTrace(string message)
		{
			DKKWindowsServiceHostEventSource.Log.DebugTrace(message);
			Trace.WriteLine(message);
		}
	}
}
