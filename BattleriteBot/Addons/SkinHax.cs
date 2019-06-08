using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gameplay;
using Gameplay.GameObjects;
using Gameplay.View;
using StunShared;
using BloodGUI;
using BloodGUI_Binding;
using BloodGUI_Binding.Base;

namespace BattleriteBot.Addons
{
    public class SkinHax : MonoBehaviour
    {
        static Int32 Margin = 5;
        static Int32 WindowWidth = 400;
        Int32 SkinSelectorId;
        Rect SkinSelectorWindow = new Rect(Screen.width - WindowWidth - Margin, Margin, WindowWidth, Screen.height - Margin * 2);
        Vector2 SkinScrollPos;
        void Awake()
        {
            SkinSelectorId = GetHashCode();
        }
        void OnEnable()
        {
            var xl = typeof(GameClient).Assembly.GetTypes().ToList().Find(t =>
                t.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                //&& t.GetMethods(Reflection.flags).Count(m => m.GetParameters().Count(p => p.ParameterType == typeof(StunNetwork.UserId)) > 0) > 0); 
                && t.GetMethods(Reflection.flags).Count(m => m.GetParameters().Count() == 3 && m.GetParameters()[0].ParameterType == typeof(GameClient).BaseType && m.GetParameters()[1].ParameterType == typeof(StunNetwork.UserId)) > 0);
            var tl = xl.GetMethods(Reflection.flags).First(m => m.GetParameters().Count() == 3 && m.GetParameters()[0].ParameterType == typeof(GameClient).BaseType && m.GetParameters()[1].ParameterType == typeof(StunNetwork.UserId));
            var o = Activator.CreateInstance(tl.GetParameters()[2].ParameterType.GetElementType(), null, null, null);
            var pars = new object[3] { API.Instance.GameClientObject, API.Instance.GameClientObject.LocalUserId, o };
            Debug.Log(o + " : ");
            var outfit = o.GetField("Outfit");
            Debug.Log(outfit + " : ");
            tl.Invoke(null, pars);

            return;
            var gc = API.Instance.GameClientObject.GetType();
            Debug.Log(gc.BaseType.FullName);
            var ms = gc.BaseType.GetMethods(Reflection.flags);
            foreach(var m in ms)
            {
                if (m.Name.Contains("tl"))
                    Debug.Log(m.Name + " : " + m.ReturnType.Name);
            }
            /*
             * #b.#tl(userid user.#a , out #Op)
             * 
					#Le = #Op.Mount;
					#Qe = #Op.Attachment;
					#Pe = #Op.Outfit;

                    #b.#KLb(userid, out #ff stateTable)

            var q = API.Instance.GameClientObject.GetUser(API.Instance.LocalPlayer.ID.ToGame());*/
        }
        void OnGUI()
        {
            SkinSelectorWindow = GUILayout.Window(SkinSelectorId, SkinSelectorWindow, SkinSelectorMethod, "Skin Select");
        }
        void SkinSelectorMethod(Int32 id)
        {
            SkinScrollPos = GUILayout.BeginScrollView(SkinScrollPos);
            {
                List<GameObjectTypeId> characters = new List<GameObjectTypeId>();
                var viewSystemsData = BloodgateEffects.Instance.GetField<UnityMain.ViewSystemsData>("_ViewSystemsData");
                //API.GameData.GetGameObjectsByTag("Character", ref characters);
                //API.GameData.GetGameObjectsByTag("Mount", ref characters);
                API.GameData.GetGameObjectsByTag("Outfits", ref characters);
                API.GameData.GetGameObjectsByTag("Pets", ref characters);
                API.GameData.GetGameObjectsByTag("VictoryPoses", ref characters);
                API.GameData.GetGameObjectsByTag("Emotes", ref characters);
                API.GameData.GetGameObjectsByTag("Attachment", ref characters);
                var gameData = MergedUnity.Glue.GUI.GUIGlobals.Glue.Get<IGameplayData>(MergedUnity.Glue.LoadingState.Ready);
                int i = 0;
                foreach(var character in characters)
                {
                    CharacterGameplayData characterGameplayData;
                    GameplayBinder.TryGetCharacter(character, out characterGameplayData);
                    var nameGuid = gameData.GetData<LocalizationAsset>(character, "Name").GUID;
                    var name = Localization.HasKey(nameGuid) ? Localization.Get(nameGuid, true) : "unk";
                    if (GUILayout.Button(name))
                    {
                        var modelAssetContainer = API.Instance.GameClientObject.GetField("#a", DeObfuscator.GameClientModelAssetsType.Name, "");
                        var modelAssets = modelAssetContainer.GetField<ModelAsset[]>("#a");
                        var modelStatesContainer = API.Instance.GameClientObject.GetField("#a", DeObfuscator.GameClientModelStatesType.Name, "");
                        var modelStates = modelStatesContainer.GetField<ModelState[]>("#a");
                        for (int j = 0; j < modelStates.Length; j++)
                        {
                            ModelState s = modelStates[j];
                            if (s.Id == API.Instance.ViewState.GetLookAtObject(false))
                            {
                                Debug.Log(s.Id + " : " + s.ModelAssetIndex.Index + " vs " + i + " @ " + s.Position);
                                Debug.Log(s.Id + " : " + modelAssets[s.ModelAssetIndex.Index].GUID);
                                var o = modelAssets[s.ModelAssetIndex.Index].GUID;
                                var n = modelAssets[i].GUID;
                                modelAssets[s.ModelAssetIndex.Index].GUID = n;
                                modelAssets[i].GUID = o;
                                //s.ModelAssetIndex = new ModelAssetIndex(i);
                                //modelStates[j] = s;
                            }
                        }
                        //modelStatesContainer.SetField("#a", modelStates);
                        //modelAssets = modelAssets.Reverse().ToArray();
                        modelAssetContainer.SetField("#a", modelAssets);
                        //API.Instance.GameClientObject.SetField("#a", DeObfuscator.GameClientModelAssetsType.Name, "", modelContainer);

                        /*for (int j = 0; j < API.Instance.ViewState.Models.Values.Length; j++)
                        {
                            ModelState s = API.Instance.ViewState.Models.Values[j];
                            if (s.Id == API.Instance.ViewState.GetLookAtObject(false))
                            {
                                Debug.Log(s.Id + " : " + s.ModelAssetIndex.Index + " vs " + i + " @ " + s.Position);
                                s.ModelAssetIndex = new ModelAssetIndex(i);
                                API.Instance.ViewState.Models.Values[j] = s;
                                Debug.Log("new2 : " + API.Instance.ViewState.Models.Values[j].ModelAssetIndex.Index);
                                //break;
                            }
                        }*/
            /*
            for (int j = 0; j < viewSystemsData.Models.Count; j++)
            {
                ModelState s = viewSystemsData.Models.Values[j];
                if (s.Id == API.Instance.ViewState.GetLookAtObject(false))
                {
                    Debug.Log(s.Id + " : " + s.ModelAssetIndex.Index + " vs " + i);
                    s.ModelAssetIndex = new ModelAssetIndex(i);
                    viewSystemsData.Models.Values[j] = s;
                    Debug.Log("new3 : " + viewSystemsData.Models.Values[j].ModelAssetIndex.Index);
                    //break;
                }
            }*/
        }
        i++;
                }
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
        /*
        UI_CharacterMenu CharacterMenu;
        //CustomChampionDetailsBinding Binding;
        //CustomShop Shop;
        //UI_ChampionDetailsHolder ChampionDetails;
        CharacterKey currentCharacter = default(CharacterKey);// GameplayBinder.GetCharacterKeys(CharacterKeyFilter.None).First();
        void Update()
        {
            if (CharacterMenu == null)
            {
                GameObject hudBaseObj = Resources.Load<GameObject>("UI_HUDBase");
                UI_HUDBase HudBase = hudBaseObj.GetComponent<UI_HUDBase>();
                CharacterMenu = UIHelper.InstantiatePrefabUnderAnchor(HudBase.CharacterMenu, API.Instance.HudBase.GetComponent<RectTransform>());
                CharacterMenu.GetDataFunc = delegate () { return new Data_CharacterMenu() { AllowCharacterMenu = true }; };
                CharacterMenu.Initialize();
                CharacterMenu.name = "SkinhaxChampionSelect";
                CharacterMenu.OpenMenuText.TextType = LocalizedTextType.ForcedText;
                CharacterMenu.OpenMenuText.ForceSet("Click to Close!");
                CharacterMenu.ChampionList.GetData = GetAdvancedChampionListData;
                CharacterMenu.ChampionList.FilterClicked = ToggleFilter;
                CharacterMenu.ChampionList.FilterHovered = ShowFilterTooltip;
                CharacterMenu.ChampionList.OnChampionIconClicked = ChangeChamp;
                CharacterMenu.ChampionList.Initialize();
                CharacterMenu.ChampionList.ChampionList.GetData = GetData;
                CharacterMenu.ChampionList.Update();
                CharacterMenu.ToggleMenu();
                CharacterMenu.Button_ToggleMenu.OnClick = ToggleMenu;
            }
            /*if (ChampionDetails == null)
            {
                UI_ChampionDetailsHolder orig = (UI_ChampionDetailsHolder)((UI_SpawnElement)API.Instance.UiBloodgateBase.Prefabs.CharacterSelect.GetField("_ChampionDetailsSpawner")).Get();
                ChampionDetails = UIHelper.InstantiatePrefabUnderAnchor(orig, Loader.Controller.HudBase.GetComponent<RectTransform>());
                Binding = new CustomChampionDetailsBinding()
                {
                    currentCharacter = currentCharacter,
                    _CurrentTab = ChampionDetailsTab.Outfits
                };
                Shop = new CustomShop();
                ChampionDetails.Initialize(Binding, Shop);
                ChampionDetails.name = "SkinhaxEquipmentSelect";
                ChampionDetails.transform.position = new Vector3(ChampionDetails.transform.position.x - 700, ChampionDetails.transform.position.y + 300, 0);
                ChampionDetails.PressTabButton(ChampionDetailsTab.Outfits);
            }
            /*var viewSystemsData = (UnityMain.ViewSystemsData)typeof(BloodgateEffects).GetField("_Instance").GetField("_ViewSystemsData");
            var modelContainer = API.Instance.GameClientObject.GetField("#a", DeObfuscator.GameClientModelsType.Name, "");
            var modelAssets = modelContainer.GetField<ModelAsset[]>("#a");
            for (int i = 0; i < API.Instance.ViewState.Models.Values.Length; i++)
            {
                ModelState s = API.Instance.ViewState.Models.Values[i];
                if (s.Id == API.Instance.ViewState.GetLookAtObject(false))
                {/*
                    if (Binding.selectedOutfit != GameObjectTypeId.Empty)
                    {
                        AssetGUID guid = GameplayBinder.GetData<ModelAsset>(Binding.selectedOutfit, "Model").GUID;
                        AssetService.AssetBundleLoader.AddAssetRef(guid);
                        s.ModelAssetIndex = new ModelAssetIndex(2);// new ModelAsset(guid);
                    }
                    if (Binding.selectedWeapon != GameObjectTypeId.Empty)
                    {
                        AssetGUID guid = GameplayBinder.GetData<ModelAsset>(Binding.selectedWeapon, "Model").GUID;
                        AssetService.AssetBundleLoader.AddAssetRef(guid);
                        //s.AttachmentAssetIndex = new ModelAsset(guid);
                    }
                    if (Binding.selectedMount != GameObjectTypeId.Empty)
                    {
                        AssetGUID guid = GameplayBinder.GetData<ModelAsset>(Binding.selectedMount, "Model").GUID;
                        AssetService.AssetBundleLoader.AddAssetRef(guid);
                        s.MountAssetIndex = new ModelAsset(guid);
                    }
                    API.Instance.ViewState.Models.Values[i] = s;
                    break;
                }
            }
        }
        public void OnDisable()
        {
            GameObject oldBaseObject;
            while (oldBaseObject = GameObject.Find("SkinhaxChampionSelect"))
                GameObject.DestroyImmediate(oldBaseObject);
            while (oldBaseObject = GameObject.Find("SkinhaxEquipmentSelect"))
                GameObject.DestroyImmediate(oldBaseObject);
        }

        ChampionProperty _ActiveProperties;
        ChampionProperty _HoveredProperty;
        List<CharacterFilterData> _NormalFilters = new List<CharacterFilterData>();
        List<CharacterFilterData> _SpecialFilters = new List<CharacterFilterData>();
        AdvancedChampionListData GetAdvancedChampionListData()
        {
            AdvancedChampionListData result;
            result.Filters = UI_CharacterSelectBindings.GetFilters(_NormalFilters, _SpecialFilters, _ActiveProperties);
            return result;
        }
        void ChangeChamp(CharacterKey character)
        {
            if (currentCharacter == character)
            {
                //UIHelper.SetActive(ChampionDetails.gameObject, false);
                currentCharacter = default(CharacterKey);
            }
            else
            {
                //UIHelper.SetActive(ChampionDetails.gameObject, true);
                currentCharacter = character;
                //Binding.currentCharacter = character;
               // ChampionDetails.SetField("_Shown", true);
               // ChampionDetails.Update();
            }
        }
        public ChampionListData GetData()
        {
            ChampionListData champList = new ChampionListData()
            {
                MaxAmountPerPage = 16,
                Icons = new List<ChampionListIconData>()
            };
            CharacterKey[] characterKeys = GameplayBinder.GetCharacterKeys(ChampionKeyFilter.None);
            for (int i = 0; i < characterKeys.Length; i++)
            {
                CharacterGameplayData characterGameplayData;
                GameplayBinder.TryGetCharacter(characterKeys[i], out characterGameplayData);
                ChampionListIconData item = default(ChampionListIconData);
                item.IsOwnedByPlayer = true;
                item.IsPickedEnemy = item.ViewedByLocalPlayer = false;
                item.IsRecommendedForNew = item.IsOnFreeRotation = item.IsPickedLocal = (characterGameplayData.Key == currentCharacter);
                item.Character = characterKeys[i];
                item.ChampionType = characterGameplayData.ChampionType;
                item.IconSprite = AssetHelper.GetSprite(characterGameplayData.IconAsset);
                item.CharacterGameId = characterGameplayData.ID.Id;
                int colorsUnlocked = 1;
                ProfileInfo profileInfo;
                //if (!API.Instance.Server.Account.TryGetProfile(out profileInfo))
                //    colorsUnlocked = 0;
                //else
                //    colorsUnlocked = profileInfo.Inventory.GetCharacteFrame(characterGameplayData.ID);
                int num;
                bool championHovered = CharacterMenu.ChampionList.ChampionList.GetChampionHovered(out num);
                item.CanBePicked = true;
                item.IsFilteredOut = false;
                bool viewedByLocalPlayer = item.ViewedByLocalPlayer;
                bool hovered = championHovered && num == characterGameplayData.ID.Id;
                item.BorderSprite = UI_GeneralSettings.Instance.ChampionListSettings.GetPortraitFrame(colorsUnlocked, item.IsPickedLocal, item.IsPickedEnemy, hovered, item.ViewedByLocalPlayer, false, false);
                champList.Icons.Add(item);
            }
            return champList;
        }
        private void ToggleFilter(ChampionProperty property)
        {
            UI_CharacterSelectBindings.ToggleFilter(ref _ActiveProperties, property);
        }
        public void ShowFilterTooltip(ChampionProperty property)
        {
            _HoveredProperty = property;
            //Optional<UI_TooltipRequestID> optional = default(Optional<UI_TooltipRequestID>);
            //_TooltipBinding.ShowTooltip(ref optional, _ChampionListBinding.ChampionList.GetCharacterTooltipPosition(true), UI_TooltipAnchorPoint.TopRight, new Func<UI_TooltipLayout>(GetProperyLayout), false, 0);
        }
        public void ToggleMenu()
        {
            Boolean toggleVisible = !(Boolean)CharacterMenu.GetField("_Visible");
            CharacterMenu.SetField("_Visible", toggleVisible);
            UIHelper.SetActive(CharacterMenu.Background.gameObject, toggleVisible);
            UIHelper.SetActive(CharacterMenu.Button_ToggleMenu.gameObject, toggleVisible);
            //UIHelper.SetActive(ChampionDetails.gameObject, toggleVisible);
        }

        /*
        public class CustomShop : IShop
        {
            public string CurrencyCode => throw new NotImplementedException();

            public string CountryCode => throw new NotImplementedException();

            public void Buy(CurrencyType currency, int stackableId, Action<BuyResult> callback)
            {
                throw new NotImplementedException();
            }

            public void Buy(CurrencyType currency, int stackableId, Action<BuyResult, object> callback)
            {
                throw new NotImplementedException();
            }

            public string FormatRealCurrency(int cost)
            {
                throw new NotImplementedException();
            }

            public string FormatRealCurrency(int cost, string currency)
            {
                throw new NotImplementedException();
            }

            public List<Bundle> GetBundles()
            {
                throw new NotImplementedException();
            }

            public void GetBundlesContaining(int containingStackableId, List<Bundle> outList)
            {
                throw new NotImplementedException();
            }

            public int GetRealCurrencyConversion(RealCurrencyId id)
            {
                throw new NotImplementedException();
            }

            public void GetRealCurrencyInfo(Action<bool> onDone)
            {
                throw new NotImplementedException();
            }

            public bool HasRealCurrency()
            {
                throw new NotImplementedException();
            }

            public void ReloadShopDocument(Action<int> onDone = null)
            {
                throw new NotImplementedException();
            }

            public void SetDebugCurrency(string currency)
            {
                throw new NotImplementedException();
            }

            public bool TryGetAppPrice(uint appId, out PriceOverview priceOverview)
            {
                throw new NotImplementedException();
            }

            public bool TryGetBundle(int stackableId, out Bundle bundle)
            {
                throw new NotImplementedException();
            }

            public bool TryGetShopItemData(int stackableId, out ShopDocumentItem shopItem, bool includeAllPremiumCurrencies = false)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                throw new NotImplementedException();
            }
        }*/
                 /*
                 public class CustomChampionDetailsBinding : IChampionDetailsBindings
                 {
                     public CharacterKey currentCharacter;
                     public ChampionDetailsTab _CurrentTab;
                     public GameObjectTypeId selectedOutfit = GameObjectTypeId.Empty;
                     public GameObjectTypeId selectedWeapon = GameObjectTypeId.Empty;
                     public GameObjectTypeId selectedMount = GameObjectTypeId.Empty;
                     public void AbiltySlotHovered(int abilitySlotId)
                     {
                     }

                     public Data_AbilityBarPlayer GetAbilityBarBloodlineData()
                     {
                         return default(Data_AbilityBarPlayer);
                     }

                     public List<Data_Buff> GetAbilityBarBuffData()
                     {
                         return new List<Data_Buff>();
                     }

                     public Data_AbilityBar GetAbilityBarData(bool lowerBar)
                     {
                         return default(Data_AbilityBar);
                     }

                     public ChampionDetailsData GetData()
                     {
                         ChampionDetailsData result;
                         result.Tab = _CurrentTab;
                         CharacterKey character = currentCharacter;
                         CharacterGameplayData characterGameplayData;
                         GameplayBinder.TryGetCharacter(character, out characterGameplayData);
                         result.CharacterName = characterGameplayData.Name;
                         result.CharacterTitle = characterGameplayData.Title;
                         return result;
                     }

                     public TabBarData GetEquipTabData(int depth)
                     {
                         List<ChampionDetailsTab> tabs = UI_GeneralSettings.Instance.ChampionDetailsSettings.TabNodesEquip;
                         List<ChampionDetailsTabNode> _Nodes = new List<ChampionDetailsTabNode>();
                         TabBarData _TabBarData = new TabBarData();
                         UI_GeneralSettings.Instance.ChampionDetailsSettings.GetTabs(tabs, _Nodes);
                         _TabBarData.Buttons = new List<TabBarButtonData>();
                         _TabBarData.Buttons.Clear();
                         foreach (ChampionDetailsTabNode node in _Nodes)
                         {
                             ChampionDetailsTab tab = node.Tab;
                             TabBarButtonData item;
                             item.Text = node.Text;
                             item.Id = (int)node.Tab;
                             item.Interactable = node.Enabled;
                             item.DisabledReason = node.DisabledReason;
                             item.IsCurrentTab = (item.Id == (int)_CurrentTab);
                             item.IsImportant = false;
                             item.IsActive = true;
                             _TabBarData.Buttons.Add(item);
                         }
                         _TabBarData.Buttons.RemoveAt(2);
                         _TabBarData.Active = true;
                         _TabBarData.GotSubBar = false;
                         return _TabBarData;
                     }

                     public ChampionDetailsInfoData GetInfoData()
                     {
                         return default(ChampionDetailsInfoData);
                     }

                     public TabBarData GetInfoTabData(int tab)
                     {
                         return new TabBarData();
                     }

                     public ChampionDetailsItemsData GetItemsData()
                     {
                         CharacterKey characterKey = currentCharacter;
                         CharacterGameplayData characterGameplayData;
                         GameplayBinder.TryGetCharacter(characterKey, out characterGameplayData);
                         ProfileInfo profileInfo;
                         Loader.Controller.Server.Account.TryGetProfile(out profileInfo);
                         ChampionDetailsTabNode tab = UI_GeneralSettings.Instance.ChampionDetailsSettings.GetTab(_CurrentTab);
                         ChampionDetailsItemsData result;
                         result.CurrentTab = _CurrentTab;
                         result.CurrentCharacter = characterKey;
                         GameObjectTypeId gameObjectTypeId = new GameObjectTypeId(Loader.Controller.Server.LocalState.GetState(tab.SerializedName, characterKey.Name));
                         List<GameObjectTypeId> tempTypes = new List<GameObjectTypeId>();
                         result.SelectedItem = GameObjectTypeId.Empty;
                         if (_CurrentTab == ChampionDetailsTab.Mounts)
                         {
                             MountKey[] mountKeys = (MountKey[])typeof(GameplayBinder).GetField("_MountKeyCache", Extensions.flags).GetValue(null);
                             for (int i = 0; i < mountKeys.Length; i++)
                             {
                                 MountKey mount = mountKeys[i];
                                 MountGameplayData mountGameplayData;
                                 if (GameplayBinder.TryGetMount(mount, out mountGameplayData))
                                 {
                                     tempTypes.Add(mountGameplayData.Id);
                                 }
                             }
                             result.SelectedItem = selectedMount;
                         }
                         else if (_CurrentTab == ChampionDetailsTab.Outfits)
                         {
                             tempTypes.AddRange(characterGameplayData.Outfits);
                             result.SelectedItem = selectedOutfit;
                         }
                         else if (_CurrentTab == ChampionDetailsTab.Attachments)
                         {
                             tempTypes.AddRange(characterGameplayData.Attachments);
                             result.SelectedItem = selectedWeapon;
                         }
                         if (result.SelectedItem == GameObjectTypeId.Empty)
                             result.SelectedItem = new GameObjectTypeId(tempTypes[0].Id);
                         result.SelectedItemData = default(UI_CustomizationListElement.Data);
                         result.ShowEquip = false;
                         result.CanEquip = false;
                         List<UI_CustomizationListElement.Data> itemIcons = new List<UI_CustomizationListElement.Data>();
                         for (int j = 0; j < tempTypes.Count; j++)
                         {
                             UI_CustomizationListElement.Data championDetailsItemData = default(UI_CustomizationListElement.Data);
                             championDetailsItemData.ItemId = tempTypes[j];
                             ShopItem shopItem = Loader.Controller.Server.Shop.GetShopItem(championDetailsItemData.ItemId);
                             championDetailsItemData.ItemRarity = (DropRarity)GameplayBinder.GetData<EnumValue>(championDetailsItemData.ItemId, "Rarity").Value;
                             championDetailsItemData.ItemType = (DropItemType)GameplayBinder.GetData<EnumValue>(championDetailsItemData.ItemId, "ItemType").Value;
                             championDetailsItemData.NameGUID = GameplayBinder.GetData<LocalizationAsset>(championDetailsItemData.ItemId, "Name").GUID;
                             //championDetailsItemData. = (result.SelectedItem == championDetailsItemData.ItemId);
                             //championDetailsItemData.OwnItem = true;
                             championDetailsItemData.ShowBackground = true;
                             championDetailsItemData.HasData = true;

                             championDetailsItemData.ShowDescriptionIcon = false;
                             ItemRarityData itemRarityData = UI_GeneralSettings.Instance.ChampionDetailsSettings.GetItemRarityData(championDetailsItemData.ItemRarity);
                             championDetailsItemData.RarityIcon = itemRarityData.Icon;
                             championDetailsItemData.TextColor = itemRarityData.RarityColor;
                             championDetailsItemData.ShowDescriptionText = false;
                             championDetailsItemData.ShowDescriptionTextOnHover = false;
                             championDetailsItemData.DescriptionColor = null;
                             championDetailsItemData.DescriptionHoverColor = null;
                             championDetailsItemData.DescriptionText = null;
                             championDetailsItemData.DescriptionHoverText = null;
                             championDetailsItemData.DescriptionIconSprite = null;
                             championDetailsItemData.BackgroundSprite = null;
                             championDetailsItemData.BackgroundHoverSprite = UI_GeneralSettings.Instance.ChampionDetailsSettings.DefaultItemBackground;
                             DropItemCategorySetting dropItemCategorySetting;
                             if (GameplayBinder.TryGetDropCategorySetting(shopItem.Category, out dropItemCategorySetting))
                             {
                                 championDetailsItemData.CategoryHoverText = dropItemCategorySetting.HoverText;
                                 championDetailsItemData.CategoryIcon = GameplayBinder.GetSprite(dropItemCategorySetting.Sprite);
                             }
                             else
                             {
                                 championDetailsItemData.CategoryHoverText = default(LocalizationKey);
                                 championDetailsItemData.CategoryIcon = null;
                             }
                             bool flag = false;
                             for (int k = 0; k < itemIcons.Count; k++)
                             {
                                 if (itemIcons[k].ItemRarity > championDetailsItemData.ItemRarity)
                                 {
                                     itemIcons.Insert(k, championDetailsItemData);
                                     flag = true;
                                     break;
                                 }
                             }
                             if (!flag)
                             {
                                 itemIcons.Add(championDetailsItemData);
                             }
                         }
                         result.Cost = default(PriceData);
                         result.CanBuy = false;
                         result.ShowBuy = false;
                         result.ItemIcons = itemIcons;
                         return result;
                     }

                     public RenderTexture GetRenderedItem(Vector2 textureSize, AssetGUID model, AssetGUID animation, PortraitType portraitType)
                     {
                         RenderTexture renderTexture = ChampionRenderManager.GetRenderTexture(textureSize);
                         ChampionRenderManager.Render(model, renderTexture, animation, portraitType);
                         return renderTexture;
                     }

                     public TextStyle GetTabBarTextStyle(int depth, int tab)
                     {
                         return UI_GeneralSettings.Instance.ChampionDetailsSettings.TabStyle;
                     }

                     public Data_UpgradeTree GetUpgradeTreeData()
                     {
                         Data_UpgradeTree result = default(Data_UpgradeTree);
                         result.AllowUpgradeTree = false;
                         result.IsMenu = false;
                         return result;
                     }

                     public void OnItemHovered(GameObjectTypeId id)
                     {
                         Loader.Controller.Server.LocalState.SetStackableIsNew(GameplayBinder.GetStackableId(id), false);
                     }

                     public void OpenBuyPopup(CurrencyType type)
                     {
                     }

                     public void ResetViewStates()
                     {
                     }

                     public void SetSelectedItem(GameObjectTypeId id)
                     {
                         if (_CurrentTab == ChampionDetailsTab.Mounts)
                         {
                             selectedMount = id;
                         }
                         else if (_CurrentTab == ChampionDetailsTab.Outfits)
                         {
                             selectedOutfit = id;
                         }
                         else if (_CurrentTab == ChampionDetailsTab.Attachments)
                         {
                             selectedWeapon = id;
                         }
                     }

                     public void ShowChampionPropertyTooltip(CharacterProperty property)
                     {
                     }

                     public void ShowLevelbarTooltip()
                     {
                     }

                     public void TabBarButtonPressed(int depth, int tab)
                     {
                         _CurrentTab = (ChampionDetailsTab)tab;
                     }

                     public void UpgradeHovered(int upgradeId)
                     {
                     }
                 }*/
                }
}