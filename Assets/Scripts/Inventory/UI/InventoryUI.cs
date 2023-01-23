using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public enum InventoryUIState {ItemSelection, PartySelection, Busy, MoveForget}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUi itemSlotsUI;

    [SerializeField] TextMeshProUGUI itemCategoryText;
    [SerializeField] Image itemCategoryIcon;
    [SerializeField] Sprite[] categorySprites;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] const int itemsInViewPort = 6;
    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] LearnMoveUi learnMoveUi;


    [SerializeField] AudioClip error;
    [SerializeField] AudioClip curser;
    [SerializeField] AudioClip back;
    [SerializeField] AudioClip selected;
    [SerializeField] AudioSource audioSource;

    Action<ItemBase> onItemUsed;

    [SerializeField] Color highlightColor = new Color(255, 0, 0);

    Inventory inventory;
    private int selectedItem = 0;

    MoveBase moveToLearn;

    private int selectedCategory;

    InventoryUIState invoState;

    RectTransform itemListRect;

    List<ItemSlotUi> slotUiList;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
        Debug.Log(inventory);
        
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateItemList();
        inventory.OnUpdated += UpdateItemList;
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;
        if (invoState == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (selectedItem + 1 >= inventory.GetSlotsByCategory(selectedCategory).Count)
                {
                    selectedItem = 0;
                }
                else
                {
                    ++selectedItem;
                }
                audioSource.PlayOneShot(curser);

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (selectedItem - 1 < 0)
                {
                    selectedItem = inventory.GetSlotsByCategory(selectedCategory).Count - 1;
                }
                else
                {
                    --selectedItem;
                }
                audioSource.PlayOneShot(curser);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (selectedCategory + 1 >= Inventory.ItemCategories.Count)
                {
                    selectedCategory = 0;
                }
                else
                {
                    ++selectedCategory;
                }
                audioSource.PlayOneShot(curser);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (selectedCategory - 1 < 0)
                {
                    selectedCategory = Inventory.ItemCategories.Count - 1;
                }
                else
                {
                    --selectedCategory;
                }
                audioSource.PlayOneShot(curser);
            }

            if (selectedCategory != prevCategory)
            {
                Debug.Log(selectedCategory);
                ResetSelection();

                itemCategoryText.text = Inventory.ItemCategories[selectedCategory];
                itemCategoryIcon.sprite = categorySprites[selectedCategory];
                UpdateItemList();
            }
            else if (selectedItem != prevSelection)
            {
                UpdateItemSelection();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                audioSource.PlayOneShot(selected);
                StartCoroutine(itemSelection());

            }

            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.X))
            {
                audioSource.PlayOneShot(back);

                onBack?.Invoke();

            }
        }
        else if (invoState == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                audioSource.PlayOneShot(selected);
                StartCoroutine(UsedItem());
            };
            Action onBackPartyScreen = () =>
            {
                audioSource.PlayOneShot(back);
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (invoState == InventoryUIState.MoveForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                audioSource.PlayOneShot(selected);
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };
            learnMoveUi.HandleSelection(onMoveSelected);
        }
    }

    IEnumerator itemSelection()
    {
        invoState = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedCategory, selectedItem);
        if (GameController.Instance.State == GameState.Battle)
        {
            if (!item.canUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"Can not use this item in battle");
                invoState = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            if (!item.canUseOutOfBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"Can not use this item out of battle");
                invoState = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UsedItem());
        }
        else
        {
            OpenPartyScreen();
            if (item is TMsHMsItem)
            {
                partyScreen.ShowIfUseable(item as TMsHMsItem);
            }
        }
    }

    

    void ResetSelection()
    {
        

        selectedItem = 0;
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        itemName.text = ""; 
        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    IEnumerator UsedItem()
    {
        invoState = InventoryUIState.Busy;

       yield return HandleTMItems();

       var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if(usedItem != null)
        {
            if(usedItem is RecoveryItem)
            {
                yield return DialogManager.Instance.ShowDialogText($"Red used a {usedItem.Name}");
            }
            
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if(selectedCategory == (int)ItemCategory.Healing)
            {
                yield return DialogManager.Instance.ShowDialogText("it wont have any effect");
            }
           
        }
        ClosePartyScreen();
    }
    
    IEnumerator HandleTMItems()
    {
        var tmItem = inventory.GetItem(selectedCategory, selectedItem) as TMsHMsItem;
        if(tmItem == null)
        {
            yield break;
        }
        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}");
            yield break;
        }
        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }

        if(pokemon.Moves.Count < PokemonBase.MaxMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learnt  {tmItem.Move.Name}");
        }
        else
        {
            
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn the move, {tmItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialogText($"but {pokemon.Base.Name} Can't learn move than {PokemonBase.MaxMoves} moves");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => invoState != InventoryUIState.MoveForget);
            yield return new WaitForSeconds(2f);
        }
        
    }
    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        invoState = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Choose a move you want to forget", true, false);
        learnMoveUi.gameObject.SetActive(true);
        learnMoveUi.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        invoState = InventoryUIState.MoveForget;
    }

    void UpdateItemList()
    {
        slotUiList = new List<ItemSlotUi>();
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotsUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUiList.Add(slotUIObj);
        }
        UpdateItemSelection();
    }
    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i <= slotUiList.Count - 1; i++)
        {
            if (i == selectedItem)
            {
                slotUiList[i].ItemText.color = highlightColor;

            }
            else
            {
                slotUiList[i].ItemText.color = Color.black;

            }
            
            if(slots.Count > 0)
            {
                var slot = slots[selectedItem];
                itemName.text = slot.Item.Name;
                itemIcon.sprite = slot.Item.Icon;
                itemDescription.text = slot.Item.Description;
            }
            
            
            HandleScrolling();

        }
    }
    void HandleScrolling() 
    {
        if (slotUiList.Count <= itemsInViewPort) return;

        float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUiList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        bool showUpArrow = selectedItem > itemsInViewPort / 2;
        upArrow.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewPort / 2 < slotUiList.Count;
        downArrow.SetActive(showDownArrow);
        
        

    }
    public void OpenPartyScreen()
    {
        invoState = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);

    }
    public void ClosePartyScreen()
    {
        invoState = InventoryUIState.ItemSelection;
        partyScreen.ClearPartySlotMessage();
        partyScreen.gameObject.SetActive(false);

    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        DialogManager.Instance.CloseDialog();
        var pokemon = partyScreen.SelectedMember;
        learnMoveUi.gameObject.SetActive(false);
        if (moveIndex == 0)
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}");
        }
        else
        {
            var selectedMove = pokemon.Moves[moveIndex - 1].Base;
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot the {selectedMove.Name} and learn the move {moveToLearn.Name}");
            
            pokemon.Moves[moveIndex - 1] = new Move(moveToLearn);
        }
        moveToLearn = null;
        invoState = InventoryUIState.ItemSelection;
    }

}
