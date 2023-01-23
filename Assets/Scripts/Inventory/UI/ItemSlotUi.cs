using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlotUi : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI countText;

    RectTransform rectTransform;

    

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float Height => rectTransform.rect.height;

    public TextMeshProUGUI ItemText => itemText;

    public TextMeshProUGUI CountText => countText;

    public void SetData(ItemSlot itemSlot)
    {
        itemText.text = itemSlot.Item.Name;
        countText.text = $" X  { itemSlot.Count}";
    }

}

