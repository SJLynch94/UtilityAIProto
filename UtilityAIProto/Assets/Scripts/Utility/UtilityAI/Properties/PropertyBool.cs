﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [AddComponentMenu("UtilityAIProto/Bool Property")]
    public class PropertyBool : Property
    {
        public bool startVal;
        private bool currentVal;

        // Start is called before the first frame update
        void Start()
        {
            if(randomStartVal)
            {
                float tmp = Random.value;
                if(tmp < 0.5f)
                {
                    currentVal = false;
                }
                else
                {
                    currentVal = true;
                }
            }
            else
            {
                currentVal = startVal;
            }
        }

        public bool Value
        {
            get { return currentVal; }
            set
            {
                currentVal = value;
                if(currentVal)
                {
                    mVal = 1.0f;
                }
                else
                {
                    mVal = 0.0f;
                }
            }
        }
    }
}

