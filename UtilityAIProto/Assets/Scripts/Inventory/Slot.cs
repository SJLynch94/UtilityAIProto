using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {

    //Variables
    public bool empty = true;
    public Texture slotTexture;
    public Texture itemTexture;
    public GameObject item;
    public int slotCapacity;
    public GameObject parent;

    //Functions

     

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Used to select the object from the slot in the inventory
        if (item)
        {
            Debug.Log(item.GetComponent<Item>().mItemName);
            Debug.Log(item.GetComponent<Item>().mItemDescription);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(item)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                item.SetActive(true);
                //parent.GetComponent<MeeleeManager>().GetWeaponAnimator();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                item.transform.parent = null;
                item.GetComponent<Item>().bIsPickedUp = false;
                item.AddComponent<Rigidbody>();
                empty = true;
                item.SetActive(true);
                item = null;
                ChangeTexture();

            }
        }
    }   

    // Update is called once per frame
    public void ChangeTexture()
    {
        //Changing a Texture
        
        if (item)
        {
            if (item.GetComponent<Item>().mItemTexture) itemTexture = item.GetComponent<Item>().mItemTexture;
            this.GetComponent<RawImage>().texture = itemTexture;
            empty = false;
        }
        else
        {
            this.GetComponent<RawImage>().texture = slotTexture;
            empty = true;
        }
    }
}
