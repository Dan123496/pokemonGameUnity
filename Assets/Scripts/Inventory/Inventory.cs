using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum ItemCategory {Healing, Pokeballs, TMsHMs, Special, Battle, Other, Key}
public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] List<ItemSlot> recoverySlots;
    [SerializeField] List<ItemSlot> pokeballSlots;
    [SerializeField] List<ItemSlot> tmHmSlots;
    [SerializeField] List<ItemSlot> specialSlots;
    [SerializeField] List<ItemSlot> battleSlots;
    [SerializeField] List<ItemSlot> otherSlots;
    [SerializeField] List<ItemSlot> keySlots;

    [SerializeField] List<List<ItemSlot>> allSlots;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "HEALING ITEMS", "POKEBALLS", "TM's & HM's ", "SPECIAL ITEMS", "BATTLE ITEMS", "OTHER ITEMS",  "KEY ITEMS"
    };

    public List<ItemSlot> RecoverySlots => recoverySlots;
    public List<ItemSlot> PokeballSlots => pokeballSlots;
    public List<ItemSlot> TmHmSlots => tmHmSlots;
    public List<ItemSlot> SpecialSlots => specialSlots;
    public List<ItemSlot> BattleSlots => battleSlots;
    public List<ItemSlot> OtherSlots => otherSlots;
    public List<ItemSlot> KeySlots => keySlots;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { recoverySlots, pokeballSlots, tmHmSlots, specialSlots, battleSlots, otherSlots, keySlots };
    }

    public event Action OnUpdated;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int categoryIndex, int itemIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(selectedCategory, itemIndex);
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            if (!item.IsReuseable)
            {
                RemoveItem(item, selectedCategory);
            }
            
            return item;
        }
        return null;
    }
    public void RemoveItem(ItemBase item, int selectedCategory)

    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if(itemSlot.Count <= 0)
        {
            currentSlots.Remove(itemSlot);
            
        }
        OnUpdated?.Invoke();
    }
    
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item => item;
    public int Count
    {
        get => count;
        set => count = value;
    }



}