using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimberTargetPoint : MonoBehaviour
{




    void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 1.0f);
        
    }

}

