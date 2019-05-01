using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    public class UAI_AIIndicator : MonoBehaviour
    {

        private Camera cam;
        public GameObject indicator;

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<RectTransform>().rotation = cam.transform.rotation;
        }
    }
}
