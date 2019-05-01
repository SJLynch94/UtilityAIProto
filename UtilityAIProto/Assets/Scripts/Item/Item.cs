using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Item : MonoBehaviour
{
    public enum EItemType { Food, Drink, Weapon, Ammo};

    public enum EItemRarity { Common, Uncommon, Rare, VeryRare};

    [XmlElement("Item Name")]
    public string mItemName;
    [XmlElement("Item Description")]
    public string mItemDescription;
    [XmlElement("Item is Picked Up")]
    public bool bIsPickedUp = false;

    [XmlElement("Item Texture")]
    public Texture mItemTexture;
    [XmlElement("Item Model")]
    public GameObject mItemModel;
    [XmlElement("Item Type")]
    public EItemType mItemType;
    [XmlElement("Item Rarity")]
    public EItemRarity mItemRarity;
    [XmlElement("Item Rotation")]
    public Vector3 mItemRotation;
}
