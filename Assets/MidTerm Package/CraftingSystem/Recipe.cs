using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
[Serializable]
public class Recipe : ScriptableObject
{
    public Item output;
    public int outputAmount = 1;

    public List<Item> RequiredItems;
}
