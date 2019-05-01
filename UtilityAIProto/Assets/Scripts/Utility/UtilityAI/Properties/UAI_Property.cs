using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    public class UAI_Property : MonoBehaviour
    {
        protected float mVal;

        public float NormalizedVal
        {
            get { return mVal; }
        }

        public bool modifiable;
        public bool randomStartVal;
    }
}

