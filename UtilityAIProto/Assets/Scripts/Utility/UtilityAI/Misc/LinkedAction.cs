﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    [Serializable]
    public class LinkedAction
    {
        public Action action;
        public bool actionEnabled = true;
        public float cooldown = 0.0f;
        public float cooldownTimer = 0.0f;
    }
}
