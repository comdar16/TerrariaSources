using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using Terraria.IO;
using Terraria.Social.Base;
using Terraria.Utilities;

namespace Terraria.Social.Steam
{
	public class WorkshopHelper
	{
		public class UGCBased
		{
			public struct SteamWorkshopItem
			{
				public string ContentFolderPath;

				public string Description;

				public string PreviewImagePath;

				public string[] Tags;

				public string Title;

				public ERemoteStoragePublishedFileVisibility? Visibility;
			}

			public class Downloader
			{
				public List<string> ResourcePackPaths { get; private set; }

				public List<string> WorldPaths { get; private set; }

				public Downloader()
				{
					ResourcePackPaths = new List<string>();
					WorldPaths = new List<string>();
				}

				public static Downloader Create()
				{
					return new Downloader();
				}

				public List<string> GetListOfSubscribedItemsPaths()
				{
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					PublishedFileId_t[] array = new PublishedFileId_t[SteamUGC.GetNumSubscribedItems()];
					SteamUGC.GetSubscribedItems((PublishedFileId_t[])(object)array, (uint)array.Length);
					ulong num = 0uL;
					string empty = string.Empty;
					uint num2 = 0u;
					List<string> list = new List<string>();
					PublishedFileId_t[] array2 = (PublishedFileId_t[])(object)array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (SteamUGC.GetItemInstallInfo(array2[i], out num, out empty, 1024u, out num2))
						{
							list.Add(empty);
						}
					}
					return list;
				}

				public bool Prepare(WorkshopIssueReporter issueReporter)
				{
					return Refresh(issueReporter);
				}

				public bool Refresh(WorkshopIssueReporter issueReporter)
				{
					ResourcePackPaths.Clear();
					WorldPaths.Clear();
					foreach (string listOfSubscribedItemsPath in GetListOfSubscribedItemsPaths())
					{
						if (listOfSubscribedItemsPath == null)
						{
							continue;
						}
						try
						{
							string path = listOfSubscribedItemsPath + Path.DirectorySeparatorChar + "workshop.json";
							if (!File.Exists(path))
							{
								continue;
							}
							string text = AWorkshopEntry.ReadHeader(File.ReadAllText(path));
							if (!(text == "World"))
							{
								if (text == "ResourcePack")
								{
									ResourcePackPaths.Add(listOfSubscribedItemsPath);
								}
							}
							else
							{
								WorldPaths.Add(listOfSubscribedItemsPath);
							}
						}
						catch (Exception exception)
						{
							issueReporter.ReportDownloadProblem("Workshop.ReportIssue_FailedToLoadSubscribedFile", listOfSubscribedItemsPath, exception);
							return false;
						}
					}
					return true;
				}
			}

			public class PublishedItemsFinder
			{
				private Dictionary<ulong, SteamWorkshopItem> _items = new Dictionary<ulong, SteamWorkshopItem>();

				private UGCQueryHandle_t m_UGCQueryHandle;

				private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

				private CallResult<SteamUGCRequestUGCDetailsResult_t> OnSteamUGCRequestUGCDetailsResultCallResult;

				public bool HasItemOfId(ulong id)
				{
					return _items.ContainsKey(id);
				}

				public static PublishedItemsFinder Create()
				{
					PublishedItemsFinder publishedItemsFinder = new PublishedItemsFinder();
					publishedItemsFinder.LoadHooks();
					return publishedItemsFinder;
				}

				private void LoadHooks()
				{
					this.OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryCompleted));
					this.OnSteamUGCRequestUGCDetailsResultCallResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(new CallResult<SteamUGCRequestUGCDetailsResult_t>.APIDispatchDelegate(this.OnSteamUGCRequestUGCDetailsResult));
				}

				public void Prepare()
				{
					Refresh();
				}

				public void Refresh()
				{
					this.m_UGCQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_All, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, SteamUtils.GetAppID(), SteamUtils.GetAppID(), 1U);
					CoreSocialModule.SetSkipPulsing(true);
					SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(this.m_UGCQueryHandle);
					this.OnSteamUGCQueryCompletedCallResult.Set(hAPICall, new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryCompleted));
					CoreSocialModule.SetSkipPulsing(false);
				}

				private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
				{
					this._items.Clear();
					if (bIOFailure || pCallback.m_eResult != EResult.k_EResultOK)
					{
						SteamUGC.ReleaseQueryUGCRequest(this.m_UGCQueryHandle);
						return;
					}
					for (uint num = 0U; num < pCallback.m_unNumResultsReturned; num += 1U)
					{
						SteamUGCDetails_t steamUGCDetails_t;
						SteamUGC.GetQueryUGCResult(this.m_UGCQueryHandle, num, out steamUGCDetails_t);
						ulong publishedFileId = steamUGCDetails_t.m_nPublishedFileId.m_PublishedFileId;
						WorkshopHelper.UGCBased.SteamWorkshopItem value = new WorkshopHelper.UGCBased.SteamWorkshopItem
						{
							Title = steamUGCDetails_t.m_rgchTitle,
							Description = steamUGCDetails_t.m_rgchDescription
						};
						this._items.Add(publishedFileId, value);
					}
					SteamUGC.ReleaseQueryUGCRequest(this.m_UGCQueryHandle);
				}

				private void OnSteamUGCRequestUGCDetailsResult(SteamUGCRequestUGCDetailsResult_t pCallback, bool bIOFailure)
				{
				}
			}

			public abstract class APublisherInstance
			{
				public delegate void FinishedPublishingAction(APublisherInstance instance);

				protected WorkshopItemPublicSettingId _publicity;

				protected SteamWorkshopItem _entryData;

				protected PublishedFileId_t _publishedFileID;

				private UGCUpdateHandle_t _updateHandle;

				private CallResult<CreateItemResult_t> _createItemHook;

				private CallResult<SubmitItemUpdateResult_t> _updateItemHook;

				private FinishedPublishingAction _endAction;

				private WorkshopIssueReporter _issueReporter;

				public void PublishContent(WorkshopHelper.UGCBased.PublishedItemsFinder finder, WorkshopIssueReporter issueReporter, WorkshopHelper.UGCBased.APublisherInstance.FinishedPublishingAction endAction, string itemTitle, string itemDescription, string contentFolderPath, string previewImagePath, WorkshopItemPublicSettingId publicity, string[] tags)
				{
					this._issueReporter = issueReporter;
					this._endAction = endAction;
					this._createItemHook = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.CreateItemResult));
					this._updateItemHook = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(this.UpdateItemResult));
					ERemoteStoragePublishedFileVisibility visibility = this.GetVisibility(publicity);
					this._entryData = new WorkshopHelper.UGCBased.SteamWorkshopItem
					{
						Title = itemTitle,
						Description = itemDescription,
						ContentFolderPath = contentFolderPath,
						Tags = tags,
						PreviewImagePath = previewImagePath,
						Visibility = new ERemoteStoragePublishedFileVisibility?(visibility)
					};
					ulong? num = null;
					FoundWorkshopEntryInfo foundWorkshopEntryInfo;
					if (AWorkshopEntry.TryReadingManifest(contentFolderPath + Path.DirectorySeparatorChar.ToString() + "workshop.json", out foundWorkshopEntryInfo))
					{
						num = new ulong?(foundWorkshopEntryInfo.workshopEntryId);
					}
					if (num != null && finder.HasItemOfId(num.Value))
					{
						this._publishedFileID = new PublishedFileId_t(num.Value);
						this.PreventUpdatingCertainThings();
						this.UpdateItem();
						return;
					}
					this.CreateItem();
				}
				private void PreventUpdatingCertainThings()
				{
					_entryData.Title = null;
					_entryData.Description = null;
				}

				private ERemoteStoragePublishedFileVisibility GetVisibility(WorkshopItemPublicSettingId publicityId)
				{
					switch (publicityId)
					{
					default:
						return (ERemoteStoragePublishedFileVisibility)2;
					case WorkshopItemPublicSettingId.FriendsOnly:
						return (ERemoteStoragePublishedFileVisibility)1;
					case WorkshopItemPublicSettingId.Public:
						return (ERemoteStoragePublishedFileVisibility)0;
					}
				}

				private void CreateItem()
				{
					CoreSocialModule.SetSkipPulsing(true);
					SteamAPICall_t hAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
					this._createItemHook.Set(hAPICall, new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.CreateItemResult));
					CoreSocialModule.SetSkipPulsing(false);
				}

				private void CreateItemResult(CreateItemResult_t param, bool bIOFailure)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002c: Invalid comparison between Unknown and I4
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement)
					{
						_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_UserDidNotAcceptWorkshopTermsOfService");
						_endAction(this);
					}
					else if ((int)param.m_eResult == 1)
					{
						_publishedFileID = param.m_nPublishedFileId;
						UpdateItem();
					}
					else
					{
						_issueReporter.ReportDelayedUploadProblemWithoutKnownReason("Workshop.ReportIssue_FailedToPublish_WithoutKnownReason", ((object)(EResult)(param.m_eResult)).ToString());
						_endAction(this);
					}
				}

				protected abstract string GetHeaderText();

				protected abstract void PrepareContentForUpdate();

				private void UpdateItem()
				{
					string headerText = this.GetHeaderText();
					if (!this.TryWritingManifestToFolder(this._entryData.ContentFolderPath, headerText))
					{
						this._endAction(this);
						return;
					}
					this.PrepareContentForUpdate();
					UGCUpdateHandle_t ugcupdateHandle_t = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), this._publishedFileID);
					if (this._entryData.Title != null)
					{
						SteamUGC.SetItemTitle(ugcupdateHandle_t, this._entryData.Title);
					}
					if (this._entryData.Description != null)
					{
						SteamUGC.SetItemDescription(ugcupdateHandle_t, this._entryData.Description);
					}
					SteamUGC.SetItemContent(ugcupdateHandle_t, this._entryData.ContentFolderPath);
					SteamUGC.SetItemTags(ugcupdateHandle_t, this._entryData.Tags);
					if (this._entryData.PreviewImagePath != null)
					{
						SteamUGC.SetItemPreview(ugcupdateHandle_t, this._entryData.PreviewImagePath);
					}
					if (this._entryData.Visibility != null)
					{
						SteamUGC.SetItemVisibility(ugcupdateHandle_t, this._entryData.Visibility.Value);
					}
					CoreSocialModule.SetSkipPulsing(true);
					SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(ugcupdateHandle_t, "");
					this._updateHandle = ugcupdateHandle_t;
					this._updateItemHook.Set(hAPICall, new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(this.UpdateItemResult));
					CoreSocialModule.SetSkipPulsing(false);
				}

				private void UpdateItemResult(SubmitItemUpdateResult_t param, bool bIOFailure)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_002c: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Invalid comparison between Unknown and I4
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Invalid comparison between Unknown and I4
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Invalid comparison between Unknown and I4
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Invalid comparison between Unknown and I4
					//IL_0040: Unknown result type (might be due to invalid IL or missing references)
					//IL_0043: Invalid comparison between Unknown and I4
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Invalid comparison between Unknown and I4
					//IL_004d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Invalid comparison between Unknown and I4
					if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement)
					{
						_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_UserDidNotAcceptWorkshopTermsOfService");
						_endAction(this);
						return;
					}
					EResult eResult = param.m_eResult;
					if ((int)eResult <= 9)
					{
						if ((int)eResult != 1)
						{
							if ((int)eResult != 8)
							{
								if ((int)eResult != 9)
								{
									goto IL_0075;
								}
								_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_CouldNotFindFolderToUpload");
							}
							else
							{
								_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_InvalidParametersForPublishing");
							}
						}
						else
						{
							SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + _publishedFileID.m_PublishedFileId);
						}
					}
					else if ((int)eResult != 15)
					{
						if ((int)eResult != 25)
						{
							if ((int)eResult != 33)
							{
								goto IL_0075;
							}
							_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_SteamFileLockFailed");
						}
						else
						{
							_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_LimitExceeded");
						}
					}
					else
					{
						_issueReporter.ReportDelayedUploadProblem("Workshop.ReportIssue_FailedToPublish_AccessDeniedBecauseUserDoesntOwnLicenseForApp");
					}
					goto IL_00f1;
					IL_00f1:
					_endAction(this);
					return;
					IL_0075:
					_issueReporter.ReportDelayedUploadProblemWithoutKnownReason("Workshop.ReportIssue_FailedToPublish_WithoutKnownReason", ((object)(EResult)(param.m_eResult)).ToString());
					goto IL_00f1;
				}

				private bool TryWritingManifestToFolder(string folderPath, string manifestText)
				{
					string path = folderPath + Path.DirectorySeparatorChar + "workshop.json";
					bool result = true;
					try
					{
						File.WriteAllText(path, manifestText);
						return result;
					}
					catch (Exception exception)
					{
						_issueReporter.ReportManifestCreationProblem("Workshop.ReportIssue_CouldNotCreateResourcePackManifestFile", exception);
						return false;
					}
				}

				public bool TryGetProgress(out float progress)
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					progress = 0f;
					if (_updateHandle == default(UGCUpdateHandle_t))
					{
						return false;
					}
					ulong num = default(ulong);
					ulong num2 = default(ulong);
					SteamUGC.GetItemUpdateProgress(_updateHandle, out num, out num2);
					if (num2 == 0L)
					{
						return false;
					}
					progress = (float)((double)num / (double)num2);
					return true;
				}
			}

			public class ResourcePackPublisherInstance : APublisherInstance
			{
				private ResourcePack _resourcePack;

				public ResourcePackPublisherInstance(ResourcePack resourcePack)
				{
					_resourcePack = resourcePack;
				}

				protected override string GetHeaderText()
				{
					return TexturePackWorkshopEntry.GetHeaderTextFor(_resourcePack, _publishedFileID.m_PublishedFileId, _entryData.Tags, _publicity, _entryData.PreviewImagePath);
				}

				protected override void PrepareContentForUpdate()
				{
				}
			}

			public class WorldPublisherInstance : APublisherInstance
			{
				private WorldFileData _world;

				public WorldPublisherInstance(WorldFileData world)
				{
					_world = world;
				}

				protected override string GetHeaderText()
				{
					return WorldWorkshopEntry.GetHeaderTextFor(_world, _publishedFileID.m_PublishedFileId, _entryData.Tags, _publicity, _entryData.PreviewImagePath);
				}

				protected override void PrepareContentForUpdate()
				{
					if (_world.IsCloudSave)
					{
						FileUtilities.CopyToLocal(_world.Path, _entryData.ContentFolderPath + Path.DirectorySeparatorChar + "world.wld");
					}
					else
					{
						FileUtilities.Copy(_world.Path, _entryData.ContentFolderPath + Path.DirectorySeparatorChar + "world.wld", cloud: false);
					}
				}
			}

			public const string ManifestFileName = "workshop.json";
		}
	}
}
