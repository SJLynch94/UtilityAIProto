using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUIActionElement : MonoBehaviour
    {
        private OverlayUI UI;
        private UAI_Action action;
        public Text text;
        public Text actionScoreText;
        public Slider slider;
        private ColorBlock normalColourBlock, selectedColourBlock;
        private bool bSelected = false;

        public UAI_Action Action
        {
            get { return action; }
            set { action = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            UI = GetComponentInParent<OverlayUI>();
            normalColourBlock = GetComponent<Button>().colors;
            selectedColourBlock = GetComponent<Button>().colors;
            selectedColourBlock.normalColor = new Color(0.4f, 0.4f, 0.3f, 1.0f);
        }

        public void SetAction(UAI_Action a)
        {
            Action = a;
            text.text = Action.name;
        }

        public void SetActionUI()
        {
            actionScoreText.text = "ActionScore: " + Action.ActionScore.ToString("0.00");
            slider.value = Action.ActionScore / 1.0f;
        }

        public void Select()
        {
            if (!bSelected)
            {
                UI.DisplayActionConsiderations(Action, false);
                bSelected = true;
                GetComponent<Button>().colors = selectedColourBlock;
            }
            else
            {
                UI.DisplayActionConsiderations(Action, true);
                bSelected = false;
                GetComponent<Button>().colors = normalColourBlock;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
