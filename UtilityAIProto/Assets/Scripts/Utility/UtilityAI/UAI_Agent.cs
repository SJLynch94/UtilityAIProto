using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    public class UAI_Agent : MonoBehaviour
    {
        public string agentName;
        public bool bConsoleLogging = false;
        public int historyStates = 10;
        public float secondsBetweenEvaluations = 0.0f;
        public GameObject AIIndicator;

        public List<UAI_LinkedAction> linkedActions = new List<UAI_LinkedAction>();

        public List<string> actionHistory = new List<string>();
        public float actionTimer;

        public float ActionTimer
        {
            get { return actionTimer; }
            set { actionTimer = value; }
        }
        public bool newAction;

        public bool NewAction
        {
            get { return newAction; }
            set { newAction = value; }
        }

        private float secondsSinceLastEval = 0.0f;
        private UAI_Action previousAction, topAction;
        private float currentActionScore;
        private bool bTiming = true;
        private bool bPaused = false;
        private int topLinkedActionIndex;

        private static int agentEvalCounter = 0;
        private static int maxAgentEvals = 0;

        public UAI_Action TopAction
        {
            get { return topAction; }
            set { topAction = value; }
        }

        public UAI_Action PreviousAction
        {
            get { return previousAction; }
            set { previousAction = value; }
        }

        private IEnumerator ResetEval()
        {
            yield return new WaitForEndOfFrame();
            agentEvalCounter = 0;
        }

        private bool CanEvaluate()
        {
            if(agentEvalCounter == 0)
            {
                StartCoroutine(ResetEval());
            }
            ++agentEvalCounter;
            return (maxAgentEvals <= 0 || agentEvalCounter <= maxAgentEvals);
        }

        public void EnableAction(string actionName)
        {
            for(var i = 0; i < linkedActions.Count; ++i)
            {
                if(linkedActions[i].action.name == actionName)
                {
                    linkedActions[i].actionEnabled = true;
                    if(bConsoleLogging)
                    {
                        Debug.Log(agentName + ". Action Enabled: " + actionName);
                    }
                }
            }
        }

        public void DisableAction(string actionName)
        {
            for (var i = 0; i < linkedActions.Count; ++i)
            {
                if (linkedActions[i].action.name == actionName)
                {
                    linkedActions[i].actionEnabled = false;
                    linkedActions[i].action.ActionScore = 0.0f;
                    if (bConsoleLogging)
                    {
                        Debug.Log(agentName + ". Action Disabled: " + actionName);
                    }
                }
            }
        }

        public void UpdateUAI()
        {
            if(bPaused)
            {
                return;
            }

            if(bTiming)
            {
                ActionTimer -= UAI_Time.MyTime;
            }

            if(topAction == null)
            {
                Evaluate();
                ActionTimer = TopAction.time;
                secondsSinceLastEval = secondsBetweenEvaluations;
            }

            TopAction.handle();

            if(TopAction.interruptible)
            {
                secondsSinceLastEval -= UAI_Time.MyTime;
                if(secondsSinceLastEval <= 0.0f)
                {
                    if(EvaluateInterruption())
                    {
                        ActionTimer = TopAction.time;
                    }
                }
            }

            if(ActionTimer <= 0.0f)
            {
                if(!CanEvaluate())
                {
                    Debug.Log("Cannot evaluate, UAI_Agent: " + agentName + " line 116");
                    return;
                }
                StopTimer();
                Evaluate();
                ActionTimer = TopAction.time;
                secondsSinceLastEval = secondsBetweenEvaluations;
            }
        }

        public void SetActionDelegate(string name, UAI_Action.Del del)
        {
            for(var i = 0; i < linkedActions.Count; ++i)
            {
                if(linkedActions[i].action.name == name)
                {
                    linkedActions[i].action.handle = del;
                    return;
                }
            }
        }

        public void StartTimer()
        {
            bTiming = true;
        }

        public void StopTimer()
        {
            bTiming = false;
        }

        public void Pause()
        {
            if(!bPaused)
            {
                bPaused = true;
            }
            else
            {
                bPaused = false;
            }
        }

        public bool IsPaused
        {
            get { return bPaused; }
        }

        public float Evaluate()
        {
            if(TopAction != null)
            {
                PreviousAction = TopAction;
            }

            float topActionScore = 0.0f;

            for(var i = 0; i < linkedActions.Count; ++i)
            {
                if(linkedActions[i].actionEnabled == true)
                {
                    linkedActions[i].action.EvaluateAction();
                    if(linkedActions[i].action.ActionScore > topActionScore)
                    {
                        TopAction = linkedActions[i].action;
                        topActionScore = linkedActions[i].action.ActionScore;
                        topLinkedActionIndex = i;
                    }
                }
            }

            if(TopAction != PreviousAction)
            {
                NewAction = true;
            }
            else
            {
                StartTimer();
            }

            if(TopAction.interruptible)
            {
                secondsSinceLastEval = 0.0f;
            }

            actionHistory.Add(TopAction.name);
            if(actionHistory.Count > historyStates)
            {
                actionHistory.RemoveAt(0);
            }

            if (linkedActions[topLinkedActionIndex].cooldown > 0.0f)
            {
                DisableAction(linkedActions[topLinkedActionIndex].action.name);
                StartCoroutine(CooldownAction(topLinkedActionIndex));
            }

            if(bConsoleLogging)
            {
                Debug.Log(agentName + ". New TopAction: " + TopAction.name + ". With ActionScore: " + topActionScore);
            }

            currentActionScore = topActionScore;
            return topActionScore;
        }

        public bool EvaluateInterruption()
        {
            int topActionPriority = TopAction.priorityLvl;
            float topActionScore = 0.0f;
            UAI_Action topInterruption = TopAction;
            bool validInterruption = false;

            for(var i = 0; i < linkedActions.Count; ++i)
            {
                if(linkedActions[i].actionEnabled == true)
                {
                    if(linkedActions[i].action.priorityLvl < topActionPriority)
                    {
                        linkedActions[i].action.EvaluateAction();
                        if(linkedActions[i].action.ActionScore > currentActionScore &&
                            linkedActions[i].action.ActionScore > topActionScore)
                        {
                            topInterruption = linkedActions[i].action;
                            topActionScore = linkedActions[i].action.ActionScore;
                            validInterruption = true;
                        }
                    }
                }
            }

            if(validInterruption)
            {
                NewAction = true;
                TopAction = topInterruption;
                actionHistory.Add("Interruption: " + TopAction.name);
                if(actionHistory.Count > historyStates)
                {
                    actionHistory.RemoveAt(0);
                }
                currentActionScore = topActionScore;

                if(TopAction.interruptible)
                {
                    secondsSinceLastEval = 0.0f;
                }

                if(bConsoleLogging)
                {
                    Debug.Log(agentName + ". Interruption: " + TopAction.name + ". With ActionScore: " + topActionScore);
                }

                return true;
            }
            return false;
        }

        IEnumerator CooldownAction(int i)
        {
            while(linkedActions[i].cooldown >= linkedActions[i].cooldownTimer)
            {
                linkedActions[i].cooldownTimer += UAI_Time.MyTime;
                yield return new WaitForEndOfFrame();
            }
            linkedActions[i].actionEnabled = true;
            linkedActions[i].cooldownTimer = 0.0f;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateUAI();
        }
    }
}
