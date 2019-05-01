using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [Serializable]
    public class UAI_Consideration
    {
        public AnimationCurve utilityCurve;
        public UAI_Property property;
        public float weight = 1.0f;
        public bool enabled = true;
        
        public float PropertyScore
        {
            get { return property.NormalizedVal; }
        }

        public float UtilityScore
        {
            get { return utilityCurve.Evaluate(property.NormalizedVal); }
        }
    }
}
