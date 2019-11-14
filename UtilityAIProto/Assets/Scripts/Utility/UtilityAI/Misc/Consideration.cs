using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [Serializable]
    public class Consideration
    {
        /// <summary>
        /// Curve to adjust behaviour
        /// The Property that is considered and the curve is being used for
        /// The Consideration weight
        /// If the Consideration is taken into account
        /// </summary>
        public AnimationCurve utilityCurve;
        public Property property;
        public float weight = 1.0f;
        public bool enabled = true;
        

        /// <summary>
        /// The base value mVal of the Property class
        /// Returns the Property value to use in the displaying of the curve
        /// 
        /// The Utility Value of the Property
        /// Uses the animation curve to return the Utility Score of the Property
        /// </summary>
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
