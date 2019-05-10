using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUIConsiderationElement : MonoBehaviour
    {
        private OverlayUI UI;
        private UAI_Consideration consideration;
        public Text nameText, utilityText;
        public Slider utilitySlider;
        private ColorBlock normalColourBlock, selectedColourBlock;
        private bool bIsActionConsideration = false;
        private bool bSelected = false;

        public UAI_Consideration Consideration
        {
            get { return consideration; }
            set { consideration = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            UI = GetComponentInParent<OverlayUI>();
            normalColourBlock = GetComponent<Button>().colors;
            selectedColourBlock = GetComponent<Button>().colors;
            selectedColourBlock.normalColor = new Color(0.4f, 0.4f, 0.3f, 1.0f);
        }

        public void SetConsideration(UAI_Consideration c, bool isAction, string actionName)
        {
            Consideration = c;
            bIsActionConsideration = isAction;
            if(isAction)
            {
                nameText.text = Consideration.property.name;
            }
            else
            {
                nameText.text = actionName;
            }
        }

        public void SetConsiderationUI()
        {
            float utilityVal = Consideration.UtilityScore;
            utilityText.text = "U Val: " + utilityVal.ToString("0.00");
            utilitySlider.value = utilityVal;
        }

        public void Select()
        {
            if(bIsActionConsideration)
            {
                if(!bSelected)
                {
                    UI.DisplayCurve(Consideration, true, false);
                    bSelected = true;
                    GetComponent<Button>().colors = selectedColourBlock;
                }
                else
                {
                    UI.DisplayCurve(Consideration, true, true);
                    bSelected = false;
                    GetComponent<Button>().colors = normalColourBlock;
                }
            }
            else
            {
                if (!bSelected)
                {
                    UI.DisplayCurve(Consideration, false, false);
                    bSelected = true;
                    GetComponent<Button>().colors = selectedColourBlock;
                }
                else
                {
                    UI.DisplayCurve(Consideration, false, true);
                    bSelected = false;
                    GetComponent<Button>().colors = normalColourBlock;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
