using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUIPropertyElement : MonoBehaviour
    {
        private OverlayUI UI;
        private UAI_Property property;
        private bool bSelected = false;
        public Text nameText, propertyText;
        public Slider propertySlider;
        private ColorBlock normalColourBlock, selectedColourBlock;

        public UAI_Property Property
        {
            get { return property; }
            set { property = value; }
        }


        // Start is called before the first frame update
        void Start()
        {
            UI = GetComponentInParent<OverlayUI>();
            normalColourBlock = GetComponent<Button>().colors;
            selectedColourBlock = GetComponent<Button>().colors;
            selectedColourBlock.normalColor = new Color(0.4f, 0.4f, 0.3f, 1.0f);
        }

        public void SetProperty(UAI_Property p)
        {
            Property = p;
            nameText.text = Property.transform.name;
        }

        public void SetPropertyUI()
        {
            float propertyVal = Property.NormalizedVal;
            propertyText.text = "Property: " + propertyVal.ToString("0.00");
            propertySlider.value = propertyVal;
        }

        public void Select()
        {
            if(!bSelected)
            {
                UI.DisplayConsiderations(Property, false);
                bSelected = true;
                GetComponent<Button>().colors = selectedColourBlock;
            }
            else
            {
                UI.DisplayConsiderations(Property, true);
                bSelected = false;
                GetComponent<Button>().colors = normalColourBlock;
            }
        }

        public void SliderValChange()
        {
            if(Property is UAI_PropertyFloat)
            {
                UAI_PropertyFloat tmp = (UAI_PropertyFloat)Property;
                tmp.Value = propertySlider.value * tmp.maxVal + tmp.minVal;
            }
            else if(Property is UAI_PropertyDouble)
            {
                UAI_PropertyDouble tmp = (UAI_PropertyDouble)Property;
                tmp.Value = propertySlider.value * tmp.maxVal + tmp.minVal;
            }
            else if (Property is UAI_PropertyInt)
            {
                UAI_PropertyInt tmp = (UAI_PropertyInt)Property;
                tmp.Value = Mathf.FloorToInt(propertySlider.value * tmp.maxVal + tmp.minVal);
            }
            else if (Property is UAI_PropertyBool)
            {
                UAI_PropertyBool tmp = (UAI_PropertyBool)Property;
                if(propertySlider.value < 0.5)
                {
                    tmp.Value = false;
                }
                else
                {
                    tmp.Value = true;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
