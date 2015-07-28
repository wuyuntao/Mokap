using UnityEngine;
using System.Collections;

public class BodyJoint : MonoBehaviour
{
    private bool changed;
    private Vector3 newPosition;
    private Quaternion newRotation;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (this.changed)
        {
            if (transform.name == "SpineBase")
            {
                transform.localPosition = newPosition;

                var cameraPosition = transform.localPosition;
                cameraPosition.z -= 5;
                Camera.main.transform.localPosition = cameraPosition;
            }
            else
            {
                var parentPosition = transform.parent.GetComponent<BodyJoint>().newPosition;

                transform.localPosition = newPosition - parentPosition;
            }

            this.changed = false;
        }
    }

    public void UpdateTransformData(Vector3 position, Quaternion rotation)
    {
        this.newPosition = position;
        this.newRotation = rotation;
        this.changed = true;

        //Debug.Log(string.Format("Update {0} {1}", joint.name, joint.localPosition));
    }
}
