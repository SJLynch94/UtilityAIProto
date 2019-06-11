using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUIPropertyElement : MonoBehaviour
    {
        private OverlayUI UI;
        private Property property;
        private bool bSelected = false;
        public Text nameText, propertyText;
        public Slider propertySlider;
        private ColorBlock normalColourBlock, selectedColourBlock;

        public Property Property
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

        public void SetProperty(Property p)
        {
            Property = p;
            nameText.text = Property.transform.name;
        }

        public void SetPropertyUI()
        {
            float propertyVal = Property.NormalizedVal;
            propertyText.text = "P: " + propertyVal.ToString("0.00");
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
            if(Property is PropertyFloat)
            {
                PropertyFloat tmp = (PropertyFloat)Property;
                tmp.Value = propertySlider.value * tmp.maxVal + tmp.minVal;
            }
            else if(Property is PropertyDouble)
            {
                PropertyDouble tmp = (PropertyDouble)Property;
                tmp.Value = propertySlider.value * tmp.maxVal + tmp.minVal;
            }
            else if (Property is PropertyInt)
            {
                PropertyInt tmp = (PropertyInt)Property;
                tmp.Value = Mathf.FloorToInt(propertySlider.value * tmp.maxVal + tmp.minVal);
            }
            else if (Property is PropertyBool)
            {
                PropertyBool tmp = (PropertyBool)Property;
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
