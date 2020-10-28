using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPasser : MonoBehaviour
{
    
    // Info to pass between ItemSlots
    public Item Item2Pass;
    public int ItemCount;

    // flag to tell ItemSlot it needs to update itself after being changed
    private bool b_needsUpdate = true;

    // Scene References
    [SerializeField]
    private TMPro.TextMeshProUGUI itemCountText;
    [SerializeField]
    private Image itemIcon;
    private RectTransform rectTransform;

    // Grab the scene references and update the visual
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon = GetComponent<Image>();
        itemCountText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        b_needsUpdate = true;
    }
    
    // Set the passer under the mouse's position and check if the visual needs an update
    void Update()
    {
        rectTransform.position = Input.mousePosition;

        if (b_needsUpdate)
        {
            UpdatePasser();
        }
    }

    // Check if there's an item in the passer and if so display it and its count
    private void UpdatePasser()
    {
        if (ItemCount == 0)
        {
            Item2Pass = null;
        }

        if (Item2Pass != null)
        {
            itemCountText.text = ItemCount.ToString();
            itemIcon.sprite = Item2Pass.Icon;
            itemIcon.enabled = true;
            itemCountText.enabled = true;
        }
        else
        {
            itemIcon.enabled = false;
            itemCountText.enabled = false;
        }

        b_needsUpdate = false;
    }

    // Set the item to pass and its count and set the visual to update
    public void AddItem(Item item2pass, int count)
    {
        Item2Pass = item2pass;
        ItemCount = count;
        itemIcon.sprite = Item2Pass.Icon;
        itemCountText.text = ItemCount.ToString();
        b_needsUpdate = true;
    }

    // Clear the passer and set the visual to update
    public void clearPasser()
    {
        Item2Pass = null;
        ItemCount = 0;
        b_needsUpdate = true;
    }
}
