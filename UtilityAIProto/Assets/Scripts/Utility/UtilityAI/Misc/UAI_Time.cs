using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    public class UAI_Time : MonoBehaviour
    {
        public static float speed = 1.0f;
        public static bool paused = false;

        static public float MyTime
        {
            get
            {
                if (paused)
                {
                    return 0.0f;
                }
                else
                {
                    return Time.fixedDeltaTime * speed;
                }
            }
        }
    }
}
