using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [Serializable]
    [AddComponentMenu("UtilityAIProto/Action")]
    public class Action : MonoBehaviour
    {
        public float time;

        public delegate void Del();
        public Del handle;
        public int priorityLvl;
        public bool interruptible;

        public List<Consideration> considerations = new List<Consideration>();

        private float actionScore;

        public float ActionScore
        {
            get { return actionScore; }
            set { actionScore = value; }
        }

        public int PriorityLevel
        {
            get { return priorityLvl; }
        }

        public void EnableConsideration(string propertyName)
        {
            for(var i = 0; i < considerations.Count; ++i)
            {
                if(considerations[i].property.name == propertyName)
                {
                    considerations[i].enabled = true;
                }
            }
        }

        public void DisableConsideration(string propertyName)
        {
            for (var i = 0; i < considerations.Count; ++i)
            {
                if (considerations[i].property.name == propertyName)
                {
                    considerations[i].enabled = false;
                }
            }
        }

        public void EvaluateAction()
        {
            ActionScore = 0.0f;
            int enabledConsiderationsCount = 0;
            for(var i = 0; i < considerations.Count; ++i)
            {
                if(considerations[i].enabled)
                {
                    ActionScore += considerations[i].UtilityScore * considerations[i].weight;
                    ++enabledConsiderationsCount;
                }
            }
            ActionScore = ActionScore / enabledConsiderationsCount;
        }
    }
}
