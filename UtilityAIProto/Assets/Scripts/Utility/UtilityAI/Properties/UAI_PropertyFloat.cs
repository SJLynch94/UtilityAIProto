using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [AddComponentMenu("UtilityAIProto/Float Property")]
    public class UAI_PropertyFloat : UAI_Property
    {
        public float minVal = 0.0f;
        public float maxVal = 100.0f;
        public float startVal = 50.0f;
        public float changePerTick = 0.0f;
        private float currentVal;

        // Start is called before the first frame update
        void Start()
        {
            if (randomStartVal)
            {
                currentVal = Random.Range(minVal, maxVal) + minVal;
            }
            else
            {
                currentVal = startVal;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Value += UAI_Time.MyTime * changePerTick;
        }

        public float Value
        {
            get { return currentVal; }
            set
            {
                currentVal = value;
                if (currentVal < minVal)
                {
                    currentVal = minVal;
                }
                if (currentVal > maxVal)
                {
                    currentVal = maxVal;
                }
                mVal = ((currentVal - minVal) / (maxVal - minVal));
            }
        }
    }
}
