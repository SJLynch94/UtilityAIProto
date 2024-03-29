﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAIProto
{
    public class OverlayUI : MonoBehaviour
    {
        public Text currentActionText, actionTimerText;

        private Agent[] agents;
        private Agent displayedAgent;
        private Consideration displayedConsideration, displayedActionConsideration;
        private bool bDisplayCurve = false;
        private bool bDisplayingAgent = false;
        private bool bDisplayActionCurve = false;

        public Agent DisplayedAgent
        {
            get { return displayedAgent; }
        }

        public GameObject ModifiablePropertyElement, PropertyElement, considerationElement, actionElement, agentElement, historyElement,
                      propertyContent, considerationContent, actionContent, agentContent, actionHistoryContent, actionConsiderationContent,
                      actionsPanel, propertiesPanel, eventsPanel, propertyConsiderationsPanel, agentsPanel, utilityCurve, actionConsiderationCurve,
                      showActionsPanel, showPropertiesPanel, showEventPanel, showAgentsPanel, actionConsiderationsPanel,
                      utilityIndicator, propertyIndicator, actionPropertyIndicator, actionUtilityIndicator;

        public Text utilitySpeedText;
	    public Image utilityCurveRenderer, actionUtilityCurveRenderer;
        public Button pauseButton;

        private List<Property> agentProperties = new List<Property>();
        private List<GameObject> actionElements = new List<GameObject>();
        private List<GameObject> agentElements = new List<GameObject>();
        private List<GameObject> considerationElements = new List<GameObject>();
        private List<GameObject> actionConsiderationElements = new List<GameObject>();
        private List<GameObject> propertyElements = new List<GameObject>();
        private List<GameObject> historyElements = new List<GameObject>();

        private Action selectedAction;
        private Property selectedProperty;
        private Consideration selectedPropertyConsideration, selectedActionConsideration;
        private ColorBlock normalColorBlock, selectedColorBlock;

        public Action SelectedAction
        {
            get { return selectedAction; }
            set { selectedAction = value; }
        }

        public Property SelectedProperty
        {
            get { return selectedProperty; }
            set { selectedProperty = value; }
        }

        public Consideration SelectedPropertyConsideration
        {
            get { return selectedPropertyConsideration; }
            set { selectedPropertyConsideration = value; }
        }

        public Consideration SelectedActionConsideration
        {
            get { return selectedActionConsideration; }
            set { selectedActionConsideration = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            normalColorBlock = pauseButton.colors;
            selectedColorBlock = pauseButton.colors;
            selectedColorBlock.normalColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

            agents = FindObjectsOfType(typeof(Agent)) as Agent[];

            //// Add Agents to the UI Agents
            for(var i = 0; i < agents.Length; ++i)
            {
                if(agents[i].gameObject != null)
                {
                    GameObject tmpAgent = Instantiate(agentElement, new Vector3(agentContent.transform.position.x + 100,
                    agentContent.transform.position.y + agentElements.Count * -22,
                    agentContent.transform.position.z), Quaternion.identity) as GameObject;
                    tmpAgent.transform.SetParent(agentContent.transform);
                    tmpAgent.GetComponent<OverlayUIAgentElement>().SetAgent(agents[i]);
                    agentElements.Add(tmpAgent);
                }
            }
            agentContent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, agentElements.Count * 22);
            utilitySpeedText.text = "Speed: " + UAI_Time.speed.ToString("0.00") + "x";
        }

        public void DisplayAgent(Agent agent, bool selected)
        {
            /// <summary>
            /// Destroy prefabs of the Agent that was selected
            /// Clear the lists of the prefabs so we can re-use the list for the next Agent selected
            /// 
            /// Set The Considerations panel of the Property and Action panel in the UI to false so they are not seen any longer
            /// Set the Selected Agents data to null
            /// </summary>
            for (var i = 0; i < actionElements.Count; ++i)
            {
                Destroy(actionElements[i]);
            }
            for (var i = 0; i < propertyElements.Count; ++i)
            {
                Destroy(propertyElements[i]);
            }
            for (var i = 0; i < considerationElements.Count; ++i)
            {
                Destroy(considerationElements[i]);
            }

            actionElements.Clear();
            considerationElements.Clear();
            agentProperties.Clear();
            propertyElements.Clear();

            propertyConsiderationsPanel.SetActive(false);
            actionConsiderationsPanel.SetActive(false);
            SelectedAction = null;
            SelectedProperty = null;
            SelectedPropertyConsideration = null;
            SelectedActionConsideration = null;

            //// If true deselect other agents
            if (!selected)
            {
                if (displayedAgent != null)
                {
                    for (var i = 0; i < agentElements.Count; ++i)
                    {
                        if (agentElements[i].GetComponent<OverlayUIAgentElement>().Agent == displayedAgent)
                        {
                            agentElements[i].GetComponent<OverlayUIAgentElement>().Select();
                            break;
                        }
                    }
                }

                /// <summary>
                /// Above we find the Agent selected when the displayedAgent is not null
                /// Using the agents list of the prefabs we check which agent has been selected against the displayedAgent and call Select to display the data
                /// Set the displayedAgent to the agent passed over and set the bDisplayingAgent boolean to true
                /// Then set and instantiate the prefabs for the Properties of the Agent selected
                /// Then set and instantiate the prefabs for the Actions of the Agent selected
                /// </summary>

                displayedAgent = agent;
                bDisplayingAgent = true;

                for (var i = 0; i < agent.GetComponentsInChildren<Property>().Length; ++i)
                {
                    agentProperties.Add(agent.GetComponentsInChildren<Property>()[i]);
                }

                for (var i = 0; i < agentProperties.Count; ++i)
                {
                    GameObject tmpProp;
                    if (agentProperties[i].modifiable)
                    {
                        tmpProp = Instantiate(ModifiablePropertyElement, new Vector3(propertyContent.transform.position.x, 
                            considerationContent.transform.position.y + propertyElements.Count * -27,
                            considerationContent.transform.position.z), Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        tmpProp = Instantiate(PropertyElement, new Vector3(propertyContent.transform.position.x, 
                            considerationContent.transform.position.y + propertyElements.Count * -27,
                            considerationContent.transform.position.z), Quaternion.identity) as GameObject;
                    }
                    tmpProp.transform.SetParent(propertyContent.transform);
                    tmpProp.GetComponent<OverlayUIPropertyElement>().SetProperty(agentProperties[i]);
                    propertyElements.Add(tmpProp);
                }
                propertyContent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, propertyElements.Count * 27);


                //// Add Agents Actions to the Action UI
                for (var i = 0; i < agent.linkedActions.Count; ++i)
                {
                    GameObject tmpAct = Instantiate(actionElement, new Vector3(actionContent.transform.position.x + 100, 
                        actionContent.transform.position.y + actionElements.Count * -27,
                        actionContent.transform.position.z), Quaternion.identity) as GameObject;
                    tmpAct.transform.SetParent(actionContent.transform);
                    tmpAct.GetComponent<OverlayUIActionElement>().SetAction(agent.linkedActions[i].action);
                    actionElements.Add(tmpAct);
                }
                actionContent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, actionElements.Count * 27);
            }
            else
            {
                bDisplayingAgent = false;
                displayedAgent = null;
            }

        }

        public void DisplayConsiderations(Property property, bool selected)
        {
            for (var i = 0; i < considerationElements.Count; ++i)
            {
                Destroy(considerationElements[i]);
            }
            considerationElements.Clear();

            if (!selected)
            {
                if (selectedProperty != null)
                {
                    for (var i = 0; i < propertyElements.Count; ++i)
                    {
                        Property tmpProp = propertyElements[i].GetComponent<OverlayUIPropertyElement>().Property;
                        if (selectedProperty == tmpProp)
                        {
                            propertyElements[i].GetComponent<OverlayUIPropertyElement>().Select();
                            break;
                        }
                    }
                }

                for (var i = 0; i < displayedAgent.linkedActions.Count; ++i)
                {
                    for (var j = 0; j < displayedAgent.linkedActions[i].action.considerations.Count; ++j)
                    {
                        if (displayedAgent.linkedActions[i].action.considerations[j].property == property)
                        {
                            GameObject tmpCon = Instantiate(considerationElement, new Vector3(considerationContent.transform.position.x,
                                     considerationContent.transform.position.y + considerationElements.Count * -27,
                                     considerationContent.transform.position.z), Quaternion.identity) as GameObject;
                            tmpCon.transform.SetParent(considerationContent.transform);
                            tmpCon.GetComponent<OverlayUIConsiderationElement>().SetConsideration(
                                displayedAgent.linkedActions[i].action.considerations[j], false, displayedAgent.linkedActions[i].action.name);
                            considerationElements.Add(tmpCon);
                        }
                    }
                }
                considerationContent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, considerationElements.Count * 27);

                utilityCurve.SetActive(false);
                bDisplayCurve = false;
                propertyConsiderationsPanel.SetActive(true);
                selectedProperty = property;
            }
            else
            {
                utilityCurve.SetActive(false);
                bDisplayCurve = false;
                propertyConsiderationsPanel.SetActive(false);
                selectedProperty = null;
                selectedPropertyConsideration = null;
            }
        }

        public void DisplayActionConsiderations(Action action, bool selected)
        {
            for (var i = 0; i < actionConsiderationElements.Count; ++i)
            {
                Destroy(actionConsiderationElements[i]);
            }
            actionConsiderationElements.Clear();

            if (!selected)
            {
                if (selectedAction != null)
                {
                    for (var i = 0; i < actionElements.Count; ++i)
                    {
                        Action tmpAction = actionElements[i].GetComponent<OverlayUIActionElement>().Action;
                        if (selectedAction == tmpAction)
                        {
                            actionElements[i].GetComponent<OverlayUIActionElement>().Select();
                            break;
                        }
                    }
                }

                for (var i = 0; i < action.considerations.Count; ++i)
                {
                    GameObject tmpCon = Instantiate(considerationElement, new Vector3(actionConsiderationContent.transform.position.x,
                                 actionConsiderationContent.transform.position.y + actionConsiderationElements.Count * -27,
                                 actionConsiderationContent.transform.position.z), Quaternion.identity) as GameObject;
                    tmpCon.transform.SetParent(actionConsiderationContent.transform);
                    tmpCon.GetComponent<OverlayUIConsiderationElement>().SetConsideration(action.considerations[i], true, action.name);
                    actionConsiderationElements.Add(tmpCon);
                }
                actionConsiderationContent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, actionConsiderationElements.Count * 27);

                actionConsiderationCurve.SetActive(false);
                bDisplayActionCurve = false;
                actionConsiderationsPanel.SetActive(true);
                selectedAction = action;
            }
            else
            {
                actionConsiderationCurve.SetActive(false);
                bDisplayActionCurve = false;
                actionConsiderationsPanel.SetActive(false);
                selectedAction = null;
                selectedActionConsideration = null;
            }
        }

        public void DisplayCurve(Consideration consideration, bool isActionConsideration, bool selected)
        {
            if (!selected)
            {
                if (isActionConsideration)
                {
                    if (selectedActionConsideration != null)
                    {
                        for (var i = 0; i < actionConsiderationElements.Count; ++i)
                        {
                            Consideration tmpCon = actionConsiderationElements[i].GetComponent<OverlayUIConsiderationElement>().Consideration;
                            if (selectedActionConsideration == tmpCon)
                            {
                                actionConsiderationElements[i].GetComponent<OverlayUIConsiderationElement>().Select();
                                break;
                            }
                        }
                    }
                    BuildUtilityCurve(consideration, true);
                    bDisplayActionCurve = true;
                    displayedActionConsideration = consideration;
                    actionConsiderationCurve.SetActive(true);
                    selectedActionConsideration = consideration;
                }
                else
                {
                    if (selectedPropertyConsideration != null)
                    {
                        for (var i = 0; i < considerationElements.Count; ++i)
                        {
                            Consideration tmpCon = considerationElements[i].GetComponent<OverlayUIConsiderationElement>().Consideration;
                            if (selectedPropertyConsideration == tmpCon)
                            {
                                considerationElements[i].GetComponent<OverlayUIConsiderationElement>().Select();
                                break;
                            }
                        }
                    }
                    BuildUtilityCurve(consideration, false);
                    bDisplayCurve = true;
                    displayedConsideration = consideration;
                    utilityCurve.SetActive(true);
                    selectedPropertyConsideration = consideration;
                }
            }
            else
            {
                if (isActionConsideration)
                {
                    actionConsiderationCurve.SetActive(false);
                    bDisplayActionCurve = false;
                    selectedActionConsideration = null;
                }
                else
                {
                    utilityCurve.SetActive(false);
                    bDisplayCurve = false;
                    selectedPropertyConsideration = null;
                }
            }
        }

        /// <summary>
        /// Create a Texture to display to before applying to the image
        /// Loop through 128 times as thats the size of the image
        /// Calculate the y of the Curve using the Evaluation of the Curve used for the Consideration being used
        /// Set the pixels of the texture to i for the x and the y value calculated to the y of the texture and apply the texture
        /// 
        /// If isActionCon is true then set the Utility Curve image in the Action Consideration panel to the texture and the size of the image rect
        /// Otherwise apply the texture to the Utility Curve in the Property Consideration panel
        /// </summary>
        private void BuildUtilityCurve(Consideration consideration, bool isActionCon)
        {
            Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            for (var i = 0; i < 128; ++i)
            {
                int y = Mathf.FloorToInt(consideration.utilityCurve.Evaluate(i / 128.0f) * 128.0f);
                texture.SetPixel(i, y, Color.black);
            }
            texture.Apply();

            if (isActionCon)
            {
                Rect rect = actionUtilityCurveRenderer.sprite.rect;
                actionUtilityCurveRenderer.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }
            else
            {
                Rect rect = utilityCurveRenderer.sprite.rect;
                utilityCurveRenderer.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            }
        }

        /// <summary>
        /// Based on the panel passed over through the Editor
        /// we set the panels to true and false for the minimized and maximized panels of the UI
        /// </summary>
        public void OnClickHide(string button)
        {
            if (button == "HideActionsPanel")
            {
                actionsPanel.SetActive(false);
                showActionsPanel.SetActive(true);
            }
            else if (button == "HidePropertiesPanel")
            {
                propertiesPanel.SetActive(false);
                showPropertiesPanel.SetActive(true);
            }
            else if (button == "HideEventsPanel")
            {
                eventsPanel.SetActive(false);
                showEventPanel.SetActive(true);
            }
            else if (button == "HideAgentsPanel")
            {
                agentsPanel.SetActive(false);
                showAgentsPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Based on the panel passed over through the Editor
        /// we set the panels to true and false for the minimized and maximized panels of the UI
        /// </summary>
        public void OnClickShow(string button)
        {
            if (button == "ShowActionsPanel")
            {
                actionsPanel.SetActive(true);
                showActionsPanel.SetActive(false);
            }
            else if (button == "ShowPropertiesPanel")
            {
                propertiesPanel.SetActive(true);
                showPropertiesPanel.SetActive(false);
            }
            else if (button == "ShowEventsPanel")
            {
                eventsPanel.SetActive(true);
                showEventPanel.SetActive(false);
            }
            else if (button == "ShowAgentsPanel")
            {
                showAgentsPanel.SetActive(false);
                agentsPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Based on the int value passed over through the Editor
        /// Set Paused of the overall Utility System and Agents to true or false or the time increase or decrease and set the UI speed text
        /// </summary>
        public void ChangeTime(int function)
        {
            if (function == 0 && !UAI_Time.paused)
            {
                UAI_Time.paused = true;
                pauseButton.colors = selectedColorBlock;
            }
            else if (function == 0 && UAI_Time.paused)
            {
                UAI_Time.paused = false;
                pauseButton.colors = normalColorBlock;
            }
            else if (function == 1 && UAI_Time.speed > 0.25f)
            {
                UAI_Time.speed -= 0.25f;
            }
            else if (function == 2)
            {
                UAI_Time.speed += 0.25f;
            }
            utilitySpeedText.text = "Speed: " + UAI_Time.speed.ToString("0.00") + "x";
        }

        /// <summary>
        /// Remove the Agent from the list based on the Agent name passed over
        /// </summary>
        public void RemoveAgent(string agentName)
        {
            //for (var i = 0; i < agents.Length; ++i)
            //{
            //    if (agents[i].agentName == agentName)
            //    {
            //        agents[i] = null;
            //        agents[i] = agents[agents.Length - 1];
            //        break;
            //    }
            //}
            //agents.Resize(ref agents, agents.Length - 1);
            
            for (var i = 0; i < agentElements.Count; ++i)
            {
                if(agentElements[i].name == agentName)
                {
                    agentElements.RemoveAt(i);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            /// <summary>
            /// Update the Text and Data of the prefabs for the Property, Action, Property Consideration and the Action Consideration
            /// Destroy prefabs of the history list
            /// Clear the list to re-use fo the next history list
            /// </summary>
            for (var i = 0; i < propertyElements.Count; ++i)
            {
                propertyElements[i].GetComponent<OverlayUIPropertyElement>().SetPropertyUI();
            }

            for (var i = 0; i < considerationElements.Count; ++i)
            {
                considerationElements[i].GetComponent<OverlayUIConsiderationElement>().SetConsiderationUI();
            }

            for (var i = 0; i < actionConsiderationElements.Count; ++i)
            {
                actionConsiderationElements[i].GetComponent<OverlayUIConsiderationElement>().SetConsiderationUI();
            }

            for (var i = 0; i < actionElements.Count; ++i)
            {
                actionElements[i].GetComponent<OverlayUIActionElement>().SetActionUI();
            }

            for (var i = 0; i < historyElements.Count; ++i)
            {
                Destroy(historyElements[i]);
            }

            historyElements.Clear();


            /// <summary>
            /// If the boolean bDisplayingAgent is true we set the Action and the Action timer text to the respective data from the displayAgent variable
            /// Then loop through the Agents history and instantiate the history prefabs list and populate the list of elements
            /// 
            /// If bDisplayCurve is true then set the Property panel Consideration panel to update the Utility and Property Indicator of the Utility Curve
            /// Using the values of the Property Score and Utility Score to set the local position to show the Utility Curve
            /// If bDisplayActionCurve is true then set the Action panel Consideration panel to update the Utility and Property Indicators of the Utility Curve within the Action panel
            /// Use the values of the Property Score and Utility Score to set the local position to show the Utility Curve again
            /// </summary>
            if (bDisplayingAgent)
            {
                currentActionText.text = "Current Action: \n" + displayedAgent.TopAction.name;
                actionTimerText.text = "Action Time: " + displayedAgent.ActionTimer.ToString("0.00");
                for (var i = 0; i < displayedAgent.actionHistory.Count; ++i)
                {
                    actionHistoryContent.GetComponent<RectTransform>().sizeDelta = new Vector2(155, historyElements.Count * 15 + 15);
                    GameObject tmpHistory = Instantiate(historyElement, new Vector3(actionHistoryContent.transform.position.x, 
                        actionHistoryContent.transform.position.y, 
                        actionHistoryContent.transform.position.z), Quaternion.identity) as GameObject;
                    tmpHistory.GetComponent<Text>().text = displayedAgent.actionHistory[i];
                    tmpHistory.transform.SetParent(actionHistoryContent.transform);
                    historyElements.Add(tmpHistory);
                }

                if (bDisplayCurve)
                {
                    utilityIndicator.transform.localPosition = new Vector3(0, (displayedConsideration.UtilityScore) * 128 - 64, 0);
                    propertyIndicator.transform.localPosition = new Vector3(displayedConsideration.PropertyScore * 128 - 64, 0, 0);
                }

                if (bDisplayActionCurve)
                {
                    actionUtilityIndicator.transform.localPosition = new Vector3(0, (displayedActionConsideration.UtilityScore) * 128 - 64, 0);
                    actionPropertyIndicator.transform.localPosition = new Vector3(displayedActionConsideration.PropertyScore * 128 - 64, 0, 0);
                }
            }
        }
    }
}
