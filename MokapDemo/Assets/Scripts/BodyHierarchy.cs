﻿using Mokap.Demo;
using System.Collections.Generic;
using UnityEngine;

public class BodyHierarchy : MonoBehaviour
{
    private const float FrameTime = 0.033333f;

    private float time = 0;

    private IEnumerator<Frame> frames;

    private Dictionary<string, BodyJoint> joints = new Dictionary<string, BodyJoint>();

    // Use this for initialization
    private void Start()
    {
        this.frames = Frame.ParseFromCsvFile("BodyFrameData").GetEnumerator();

        InitializeJoint(transform.Find("SpineBase"));
    }

    private void InitializeJoint(Transform transform)
    {
        var joint = transform.gameObject.AddComponent<BodyJoint>();

        joints.Add(transform.name, joint);

        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);

            InitializeJoint(child);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this.time += Time.deltaTime;

        while (this.time > FrameTime)
        {
            if (this.frames.MoveNext())
            {
                Debug.Log(string.Format("Update frame: {0}", this.time));

                UpdateJoints(this.frames.Current);
            }
            else
            {
                Debug.Log("End.");

                Destroy(this);
            }

            time -= FrameTime;
        }
    }

    private void UpdateJoints(Frame frame)
    {
        foreach (var j in frame.Joints)
        {
            var joint = this.joints[j.Type];

            if (!joint)
            {
                Debug.LogError(string.Format("Missing joint {0}", j.Type));
            }
            else
            {
                joint.UpdateTransformData(j.Position, j.Rotation);
            }
        }
    }
}