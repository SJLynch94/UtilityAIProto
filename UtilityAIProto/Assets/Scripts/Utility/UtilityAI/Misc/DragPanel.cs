using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIProto
{
    public class DragPanel : MonoBehaviour
    {
        public GameObject panel;

        public void OnMouseDrag()
        {
            panel.transform.position = new Vector3(Input.mousePosition.x - transform.position.x + 8, Input.mousePosition.y - transform.position.y + 8, 0.0f);
        }
    }
}
