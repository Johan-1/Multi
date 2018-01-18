using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraMovement))]
public class CustumCameraEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraMovement cam = (CameraMovement)target;

        if (GUILayout.Button("Set Player Bounding box In All Cameras"))
        {
            cam.SetPlayerBoundingboxAllCameras();
        }

        if (GUILayout.Button("Get Player Bounding Box And Set In This Camera"))
        {
            cam.GetPlayerBoundingbox();
        }


    }

}
