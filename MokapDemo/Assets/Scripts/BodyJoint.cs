using UnityEngine;
using System.Collections;
using System;

public class BodyJoint : MonoBehaviour
{
    private const float MainCameraDistance = 3;

    private bool firstChanged;
    private bool changed;
    private Vector3 newPosition;
    private Quaternion newRotation;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.changed)
        {
            if (!this.firstChanged)
            {
                UpdateByPosition();

                // Adjust camera position
                if (name == "SpineBase")
                {
                    var cameraPosition = transform.localPosition;
                    cameraPosition.z -= MainCameraDistance;

                    Camera.main.transform.localPosition = cameraPosition;
                }

                this.firstChanged = true;
            }
            else
            {
                UpdateByPosition();
            }

            this.changed = false;
        }
    }

    private void UpdateByPosition()
    {
        if (name == "SpineBase")
        {
            transform.localPosition = newPosition;
        }
        else
        {
            transform.localPosition = newPosition - ParentJoint.newPosition;
        }
    }

    public void UpdateTransformData(Vector3 position, Quaternion rotation)
    {
        this.newPosition = position;
        this.newRotation = rotation;
        this.changed = true;

        //Debug.Log(string.Format("Update {0} {1}", joint.name, joint.localPosition));
    }

    private BodyJoint ParentJoint
    {
        get { return transform.parent.GetComponent<BodyJoint>(); }
    }
}
