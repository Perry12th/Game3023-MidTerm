using System;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [Tooltip("Path under a Resource folder to collect all of the recipes")]
    [SerializeField]
    private const string Path = "Recipes";

    [Tooltip("List size determines how many slots there will be. Contents will replaced by copies of the first element")]
    [SerializeField]
    private List<ItemSlot> craftingSlots;

    [Tooltip("The object which will hold Item Slots as its direct childern")]
    [SerializeField]
    private GameObject craftingPanel;

    [Tooltip("Output slot where the crafting system will place any valid recipe items in")]
    [SerializeField]
    private OutputSlot outputSlot;

    [Tooltip("List of recipes that the system will check against to create an item in the output slot")]
    [SerializeField]
    private List<Recipe> recipeList;

    private void Start()
    {
        InitItemSlots();
        recipeList = new List<Recipe>(Resources.LoadAll<Recipe>(Path));
        
    }

    private void InitItemSlots()
    {
        Assert.IsTrue(craftingSlots.Count > 0, "itemSlots was empty");
        Assert.IsNotNull(craftingSlots[0], "Inventory is missing a prefab for itemSlots. Add it as the first element of its itemSlot list");

        // init item slots
        for (int i = 1; i < craftingSlots.Count; i++)
        {
            GameObject newObject = Instantiate(craftingSlots[0].gameObject, craftingPanel.transform);
            ItemSlot newSlot = newObject.GetComponent<ItemSlot>();
            craftingSlots[i] = newSlot;
        }

        foreach (ItemSlot slot in craftingSlots)
        {
            slot.OnItemMoved += CraftingSystem_OnItemMoved;
        }

        outputSlot.OnItemGrabbed += CraftingSystem_OnOutputItemGrabbed;
    }

    // Event callbacks // 

    // Anytime the output slot had its item grabbed the recipe items will be consumed
    private void CraftingSystem_OnOutputItemGrabbed(object sender, OutputSlot.OnItemGrabbedEventArgs e)
    {
        ConsumeRecipeItems();
        GetRecipeOutput();
    }
    // Anytime the crafting slots had an item moved (grabbed or dropped) the crafting will check for a vaild recipe
    private void CraftingSystem_OnItemMoved(object sender, ItemSlot.OnItemMovedEventArgs e)
    {
        GetRecipeOutput();
    }

    // Helper Functions
    private bool HasItem(int index)
    {
        return craftingSlots[index].HasItem();
    }

    private Item GetItem(int index)
    {
        return craftingSlots[index].ItemInSlot;
    }

    private void SetItem(Item item, int amount, int index)
    {
        craftingSlots[index].SetContents(item, amount);
    }

    private void IncreaseItemAmount(int count, int index)
    {
        craftingSlots[index].TryAddItems(count);
    }

    private void DecreaseItemAmount(int count, int index)
    {
        craftingSlots[index].TryRemoveItems(count);
    }

    private void RemoveItem(int index)
    {
        craftingSlots[index].ClearSlot();
    }

    private bool TryAddItem(Item item, int amount, int index)
    {
        if (HasItem(index))
        {
            SetItem(item, amount, index);
            return true;
        }
        else
        {
            if (item == GetItem(index))
            {
                IncreaseItemAmount(amount, index);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void GetRecipeOutput()
    {
        outputSlot.ClearSlot();

        foreach (Recipe recipe in recipeList)
        {
            // If there's too many slots in the recipe then skip it
            if(recipe.RequiredItems.Count > craftingSlots.Count)
            {
                continue;
            }

            // Once there's a recipe the system can handle grab its item list
            bool compeletedRecipe = true;
            List<Item> requireitems = recipe.RequiredItems;
            // For every requireditem
            for(int i = 0; i < requireitems.Count; i++)
            {
                // If there's a required item in this slot
                if (requireitems[i] != null)
                {
                    // Check if the matching craftingslot has an item that matches the required one
                    if (!craftingSlots[i].HasItem() || craftingSlots[i].ItemInSlot != requireitems[i])
                    {
                        // If not vaild then the recipe is uncraftable so break out of this loop and try the next one
                        compeletedRecipe = false;
                        break;
                    }
                }
            }

            // If a recipe has been properly created then place its output in the output slot
            if(compeletedRecipe)
            {
                outputSlot.SetContents(recipe.output, recipe.outputAmount);
                return;
            }
        }
    }

    // Once the output item has been removed, the recipe items will all lose one item count
    private void ConsumeRecipeItems()
    {
        foreach(ItemSlot slot in craftingSlots)
        {
            slot.TryRemoveItems(1);
        }
    }
}
