using System;
using System.Collections.Generic;
using Steamworks;
using Terraria;
using Terraria.Social.Steam;

namespace Terraria.Social.Steam
{
	public class Lobby 
	{
		private HashSet<CSteamID> _usersSeen = new HashSet<CSteamID>();

		// Token: 0x0400111E RID: 4382
		private byte[] _messageBuffer = new byte[1024];

		// Token: 0x0400111F RID: 4383
		public CSteamID Id = CSteamID.Nil;

		// Token: 0x04001120 RID: 4384
		public CSteamID Owner = CSteamID.Nil;

		// Token: 0x04001121 RID: 4385
		public LobbyState State;

		// Token: 0x04001122 RID: 4386
		private CallResult<LobbyEnter_t> _lobbyEnter;

		// Token: 0x04001123 RID: 4387
		private CallResult<LobbyEnter_t>.APIDispatchDelegate _lobbyEnterExternalCallback;

		// Token: 0x04001124 RID: 4388
		private CallResult<LobbyCreated_t> _lobbyCreated;

		// Token: 0x04001125 RID: 4389
		private CallResult<LobbyCreated_t>.APIDispatchDelegate _lobbyCreatedExternalCallback;

		public Lobby()
		{
			this._lobbyEnter = CallResult<LobbyEnter_t>.Create(new CallResult<LobbyEnter_t>.APIDispatchDelegate(this.OnLobbyEntered));
			this._lobbyCreated = CallResult<LobbyCreated_t>.Create(new CallResult<LobbyCreated_t>.APIDispatchDelegate(this.OnLobbyCreated));
		}

		public void Create(bool inviteOnly, CallResult<LobbyCreated_t>.APIDispatchDelegate callResult)
		{
			SteamAPICall_t hAPICall = SteamMatchmaking.CreateLobby(inviteOnly ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypeFriendsOnly, 256);
			this._lobbyCreatedExternalCallback = callResult;
			this._lobbyCreated.Set(hAPICall, null);
			this.State = LobbyState.Creating;
		}

		public void OpenInviteOverlay()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (State == LobbyState.Inactive)
			{
				SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(Main.LobbyId));
			}
			else
			{
				SteamFriends.ActivateGameOverlayInviteDialog(Id);
			}
		}

		public void Join(CSteamID lobbyId, CallResult<LobbyEnter_t>.APIDispatchDelegate callResult)
		{
			if (this.State != LobbyState.Inactive)
			{
				return;
			}
			this.State = LobbyState.Connecting;
			this._lobbyEnterExternalCallback = callResult;
			SteamAPICall_t hAPICall = SteamMatchmaking.JoinLobby(lobbyId);
			this._lobbyEnter.Set(hAPICall, null);
		}

		public byte[] GetMessage(int index)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			CSteamID val = default(CSteamID);
			EChatEntryType val2 = default(EChatEntryType);
			int lobbyChatEntry = SteamMatchmaking.GetLobbyChatEntry(Id, index, out val, _messageBuffer, _messageBuffer.Length, out val2);
			byte[] array = new byte[lobbyChatEntry];
			Array.Copy(_messageBuffer, array, lobbyChatEntry);
			return array;
		}

		public int GetUserCount()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return SteamMatchmaking.GetNumLobbyMembers(Id);
		}

		public CSteamID GetUserByIndex(int index)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return SteamMatchmaking.GetLobbyMemberByIndex(Id, index);
		}

		public bool SendMessage(byte[] data)
		{
			return SendMessage(data, data.Length);
		}

		public bool SendMessage(byte[] data, int length)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (State != LobbyState.Active)
			{
				return false;
			}
			return SteamMatchmaking.SendLobbyChatMsg(Id, data, length);
		}

		public void Set(CSteamID lobbyId)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			Id = lobbyId;
			State = LobbyState.Active;
			Owner = SteamMatchmaking.GetLobbyOwner(lobbyId);
		}

		public void SetPlayedWith(CSteamID userId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!_usersSeen.Contains(userId))
			{
				SteamFriends.SetPlayedWith(userId);
				_usersSeen.Add(userId);
			}
		}

		public void Leave()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (State == LobbyState.Active)
			{
				SteamMatchmaking.LeaveLobby(Id);
			}
			State = LobbyState.Inactive;
			_usersSeen.Clear();
		}

		private void OnLobbyEntered(LobbyEnter_t result, bool failure)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (State == LobbyState.Connecting)
			{
				if (failure)
				{
					State = LobbyState.Inactive;
				}
				else
				{
					State = LobbyState.Active;
				}
				Id = new CSteamID(result.m_ulSteamIDLobby);
				Owner = SteamMatchmaking.GetLobbyOwner(Id);
				_lobbyEnterExternalCallback.Invoke(result, failure);
			}
		}

		private void OnLobbyCreated(LobbyCreated_t result, bool failure)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if (State == LobbyState.Creating)
			{
				if (failure)
				{
					State = LobbyState.Inactive;
				}
				else
				{
					State = LobbyState.Active;
				}
				Id = new CSteamID(result.m_ulSteamIDLobby);
				Owner = SteamMatchmaking.GetLobbyOwner(Id);
				_lobbyCreatedExternalCallback.Invoke(result, failure);
			}
		}
	}
}
