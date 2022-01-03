using Steamworks;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	public class OverlaySocialModule : Terraria.Social.Base.OverlaySocialModule
	{
		// Token: 0x060012A4 RID: 4772 RVA: 0x00463B6C File Offset: 0x00461D6C
		public override void Initialize()
		{
			this._gamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(new Callback<GamepadTextInputDismissed_t>.DispatchDelegate(this.OnGamepadTextInputDismissed));
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x000392B0 File Offset: 0x000374B0
		public override void Shutdown()
		{
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00463B85 File Offset: 0x00461D85
		public override bool IsGamepadTextInputActive()
		{
			return this._gamepadTextInputActive;
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00463B8D File Offset: 0x00461D8D
		public override bool ShowGamepadTextInput(string description, uint maxLength, bool multiLine = false, string existingText = "", bool password = false)
		{
			if (this._gamepadTextInputActive)
			{
				return false;
			}
			bool flag = SteamUtils.ShowGamepadTextInput(password ? EGamepadTextInputMode.k_EGamepadTextInputModePassword : EGamepadTextInputMode.k_EGamepadTextInputModeNormal, multiLine ? EGamepadTextInputLineMode.k_EGamepadTextInputLineModeMultipleLines : EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, description, maxLength, existingText);
			if (flag)
			{
				this._gamepadTextInputActive = true;
			}
			return flag;
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x00463BBC File Offset: 0x00461DBC
		public override string GetGamepadText()
		{
			uint enteredGamepadTextLength = SteamUtils.GetEnteredGamepadTextLength();
			string result;
			SteamUtils.GetEnteredGamepadTextInput(out result, enteredGamepadTextLength);
			return result;
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x00463BD9 File Offset: 0x00461DD9
		private void OnGamepadTextInputDismissed(GamepadTextInputDismissed_t result)
		{
			this._gamepadTextInputActive = false;
		}

		// Token: 0x04001106 RID: 4358
		private Callback<GamepadTextInputDismissed_t> _gamepadTextInputDismissed;

		// Token: 0x04001107 RID: 4359
		private bool _gamepadTextInputActive;
	}
}
