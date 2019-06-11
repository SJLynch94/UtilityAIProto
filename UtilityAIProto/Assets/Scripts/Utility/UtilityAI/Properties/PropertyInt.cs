using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [AddComponentMenu("UtilityAIProto/Int Property")]
    public class PropertyInt : Property
    {
        public int minVal;
        public int maxVal;
        public int startVal;
        private int currentVal;

        // Start is called before the first frame update
        void Start()
        {
            if (randomStartVal)
            {
                currentVal = Mathf.FloorToInt(Random.Range(minVal + 1, maxVal + 1)) - 1 + minVal;
            }
            else
            {
                currentVal = startVal;
            }
        }

        public int Value
        {
            get { return currentVal; }
            set
            {
                currentVal = value;
                if(currentVal < minVal)
                {
                    currentVal = minVal;
                }
                if(currentVal > maxVal)
                {
                    currentVal = maxVal;
                }
                mVal = ((currentVal - minVal) / (maxVal - minVal));
            }
        }
    }
}
