using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    //Variables

    public bool inventoryEnabled;
    public GameObject inventory;
    //public GameObject itemDB; //not sure if in use? - SJ
    private Transform[] slot;
    public int slotAmount;
    public GameObject slotHolder;

    // Use this for initialization
    void Start()
    {
        GetAllSlots();
        SetEnableInventory(false);
    }

    public void AddItem(GameObject item)
    {
        print("adding item" + ": " + item.gameObject.name);

        for(int i = 0; i < slotAmount; i++)
        {
            if(slot[i].GetComponent<Slot>().empty && !item.GetComponent<Item>().bIsPickedUp) // If the slot is free and the item has not been picked up
            {
                slot[i].GetComponent<Slot>().item = item; // Set that slots item to be the item
                slot[i].GetComponent<Slot>().ChangeTexture(); // Change the slots texture to reflect that item
                StoreItemOnPlayer(item); // Store on the player by making changes to game object
            }
        }
    }

    void StoreItemOnPlayer(GameObject item)
    {
        item.GetComponent<Item>().bIsPickedUp = true; // Set item to be picked up by the player
        item.transform.parent = GetComponent<AILogic>().mWeaponHolder.transform; // Attach the item to player -- NEEDS TO BE MADE DYNAMIC
        item.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f); // Set local position from the transform to be 0. Directly on transform
        item.transform.localRotation = Quaternion.Euler(item.GetComponent<Item>().mItemRotation); // Set local rotation to be the rotation stored by the item
        Destroy(item.GetComponent<Rigidbody>()); // Destroy rigid body on item (not used when equipped by the player)
        item.SetActive(false); // Disable the item so it is not rendered or logically processed
    }

    // Get all the slots in the UI canvas
    public void GetAllSlots()
    {
        slot = new Transform[slotAmount];
        for (int i = 0; i < slotAmount; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i);
        }
    }

    // Set whether to enable the inventory or not and activate the game object respectively
    public void SetEnableInventory(bool enableInventory)
    {
        inventoryEnabled = enableInventory;

        if (inventoryEnabled)
        {
            inventory.SetActive(true);
        }
        else
        {
            inventory.SetActive(false);
        }
    }
}
