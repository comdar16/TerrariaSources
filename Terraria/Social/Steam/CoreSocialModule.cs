using System;
using System.Threading;
using System.Windows.Forms;
using ReLogic.OS;
using Steamworks;
using Terraria.Localization;

namespace Terraria.Social.Steam
{
	public class CoreSocialModule : ISocialModule
	{
		private static CoreSocialModule _instance;

		public const int SteamAppId = 105600;

		private bool IsSteamValid;

		private object _steamTickLock = new object();

		private object _steamCallbackLock = new object();

		private Callback<GameOverlayActivated_t> _onOverlayActivated;

		private bool _skipPulsing;

		public static event Action OnTick;

		public static void SetSkipPulsing(bool shouldSkipPausing)
		{
		}

		public void Initialize()
		{
			CoreSocialModule._instance = this;
			if (SteamAPI.RestartAppIfNecessary(new AppId_t(105600U)))
			{
				Environment.Exit(1);
				return;
			}
			if (!SteamAPI.Init())
			{
				MessageBox.Show(Language.GetTextValue("Error.LaunchFromSteam"), Language.GetTextValue("Error.Error"));
				Environment.Exit(1);
			}
			this.IsSteamValid = true;
			new Thread(new ParameterizedThreadStart(this.SteamCallbackLoop))
			{
				IsBackground = true
			}.Start();
			new Thread(new ParameterizedThreadStart(this.SteamTickLoop))
			{
				IsBackground = true
			}.Start();
			Main.OnTickForThirdPartySoftwareOnly += this.PulseSteamTick;
			Main.OnTickForThirdPartySoftwareOnly += this.PulseSteamCallback;
			if (Platform.IsOSX)
			{
				this._onOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnOverlayActivated));
			}
		}

		public void PulseSteamTick()
		{
			if (Monitor.TryEnter(_steamTickLock))
			{
				Monitor.Pulse(_steamTickLock);
				Monitor.Exit(_steamTickLock);
			}
		}

		public void PulseSteamCallback()
		{
			if (Monitor.TryEnter(_steamCallbackLock))
			{
				Monitor.Pulse(_steamCallbackLock);
				Monitor.Exit(_steamCallbackLock);
			}
		}

		public static void Pulse()
		{
			_instance.PulseSteamTick();
			_instance.PulseSteamCallback();
		}

		private void SteamTickLoop(object context)
		{
			Monitor.Enter(_steamTickLock);
			while (IsSteamValid)
			{
				if (_skipPulsing)
				{
					Monitor.Wait(_steamCallbackLock);
					continue;
				}
				if (CoreSocialModule.OnTick != null)
				{
					CoreSocialModule.OnTick();
				}
				Monitor.Wait(_steamTickLock);
			}
			Monitor.Exit(_steamTickLock);
		}

		private void SteamCallbackLoop(object context)
		{
			Monitor.Enter(_steamCallbackLock);
			while (IsSteamValid)
			{
				if (_skipPulsing)
				{
					Monitor.Wait(_steamCallbackLock);
					continue;
				}
				SteamAPI.RunCallbacks();
				Monitor.Wait(_steamCallbackLock);
			}
			Monitor.Exit(_steamCallbackLock);
			SteamAPI.Shutdown();
		}

		public void Shutdown()
		{
			Application.ApplicationExit += delegate
			{
				IsSteamValid = false;
			};
		}

		public void OnOverlayActivated(GameOverlayActivated_t result)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Main.instance.IsMouseVisible = result.m_bActive == 1;
		}
	}
}
