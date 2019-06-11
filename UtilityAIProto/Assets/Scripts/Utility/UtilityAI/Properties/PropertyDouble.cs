using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [AddComponentMenu("UtilityAIProto/Double Property")]
    public class PropertyDouble : Property
    {
        public double minVal = 0.0d;
        public double maxVal = 100.0d;
        public double startVal = 50.0d;
        public double changePerTick = 0.0d;
        private double currentVal;

        // Start is called before the first frame update
        void Start()
        {
            if(randomStartVal)
            {
                currentVal = Random.value * (maxVal - minVal) + minVal;
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

        public double Value
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
                mVal = (float)((currentVal - minVal) / (maxVal - minVal));
            }
        }
    }
}
