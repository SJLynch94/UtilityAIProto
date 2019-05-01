using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUIAgentElement : MonoBehaviour
    {
        private UAI_Agent agent;
        public Text text;
        private OverlayUI UI;
        private bool bSelected = false;
        private ColorBlock normalColourBlock, selectedColourBlock, pauseNormalColourBlock, pauseSeletedColourBlock;
        public Button button, pauseButton;

        public UAI_Agent Agent
        {
            get { return agent; }
            set { agent = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            text = GetComponentInChildren<Text>();
            UI = GetComponentInParent<OverlayUI>();
            normalColourBlock = button.colors;
            selectedColourBlock = button.colors;
            selectedColourBlock.normalColor = new Color(0.4f, 0.4f, 0.3f, 1.0f);
            pauseNormalColourBlock = pauseButton.colors;
            pauseSeletedColourBlock = pauseButton.colors;
            pauseSeletedColourBlock.normalColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }

        public void SetAgent(UAI_Agent a)
        {
            Agent = a;
            SetAgentUI();
        }

        public void Select()
        {
            if(!bSelected)
            {
                UI.DisplayAgent(Agent, false);
                bSelected = true;
                Agent.AIIndicator.SetActive(true);
                GetComponent<Button>().colors = selectedColourBlock;
            }
            else
            {
                UI.DisplayAgent(Agent, true);
                bSelected = false;
                Agent.AIIndicator.SetActive(false);
                GetComponent<Button>().colors = normalColourBlock;
            }
        }

        public void PauseAgent()
        {
            Agent.Pause();
            if(Agent.IsPaused)
            {
                pauseButton.colors = pauseSeletedColourBlock;
            }
            else
            {
                pauseButton.colors = pauseNormalColourBlock;
            }
        }

        public void SetAgentUI()
        {
            text.text = Agent.agentName;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
