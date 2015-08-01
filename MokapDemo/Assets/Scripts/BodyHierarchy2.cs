using Mokap.Demo;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BodyHierarchy2 : MonoBehaviour
{
    private const float FrameTime = 0.033333f;

    private float time = 0;

    IEnumerator<Frame> frames;

    List<Transform> bones = new List<Transform>();

    // Use this for initialization
    void Start()
    {
        Log("up {0}", Vector3.up.ToString("f3"));
        Log("down {0}", Vector3.down.ToString("f3"));
        Log("left {0}", Vector3.left.ToString("f3"));
        Log("right {0}", Vector3.right.ToString("f3"));
        Log("forward {0}", Vector3.forward.ToString("f3"));
        Log("back {0}", Vector3.back.ToString("f3"));

        this.frames = Frame.ParseFromCsvFile("BodyFrameData2").GetEnumerator();
        if (this.frames.MoveNext())
        {
            CreateBone("SpineBase", "SpineMid");
            CreateBone("SpineMid", "SpineShoulder");
            CreateBone("SpineShoulder", "Neck");
            CreateBone("Neck", "Head");

            CreateBone("SpineShoulder", "ShoulderRight");
            CreateBone("ShoulderRight", "ElbowRight");
            CreateBone("ElbowRight", "WristRight");
            CreateBone("WristRight", "HandRight");

            CreateBone("SpineShoulder", "ShoulderLeft");
            CreateBone("ShoulderLeft", "ElbowLeft");
            CreateBone("ElbowLeft", "WristLeft");
            CreateBone("WristLeft", "HandLeft");

            CreateBone("SpineBase", "HipLeft");
            CreateBone("HipLeft", "KneeLeft");
            CreateBone("KneeLeft", "AnkleLeft");
            CreateBone("AnkleLeft", "FootLeft");

            CreateBone("SpineBase", "HipRight");
            CreateBone("HipRight", "KneeRight");
            CreateBone("KneeRight", "AnkleRight");
            CreateBone("AnkleRight", "FootRight");
        }
    }

    private void CreateBone(string fromName, string toName)
    {
        var fromJoint = this.frames.Current.FindJoint(toName);
        var toJoint = this.frames.Current.FindJoint(fromName);

        Log("From {0} pos {1}, rot {2} / {3}"
                , fromName
                , fromJoint.Position.ToString("f3")
                , fromJoint.Rotation.ToString("f3")
                , fromJoint.Rotation.eulerAngles.ToString("f3"));

        Log("To {0} pos {1}, rot {2} / {3}"
            , toName
            , toJoint.Position.ToString("f3")
            , toJoint.Rotation.ToString("f3")
            , toJoint.Rotation.eulerAngles.ToString("f3"));

        var bone = Resources.Load("Bone");
        var toBone = (GameObject)GameObject.Instantiate(bone);

        var fromBone = this.bones.Find(b => b.name == fromName + "Bone");
        if (fromBone != null)
        {
            toBone.transform.parent = fromBone.Find("End");
        }
        else
        {
            toBone.transform.parent = transform;
        }

        var length = (fromJoint.Position - toJoint.Position).magnitude;
        var inner = toBone.transform.Find("Inner");
        var end = toBone.transform.Find("End");

        var scale = inner.localScale;
        scale.z = length;
        inner.localScale = scale;
        inner.localPosition = new Vector3(0, 0, length / 2);
        end.localPosition = new Vector3(0, 0, length);

        toBone.name = string.Format("{0}Bone", toName);
        this.bones.Add(toBone.transform);

        var rotation = Quaternion.LookRotation(fromJoint.Position - toJoint.Position);
        toBone.transform.rotation = rotation;
        toBone.transform.localPosition = Vector3.zero;

        Log("Rot {0} r0 {1}, r1 {1}", toName, toJoint.Rotation, rotation);
    }

    // Update is called once per frame
    void Update()
    {
        this.time += Time.deltaTime;

        while (this.time > FrameTime)
        {
            if (this.frames.MoveNext())
            {
                var builder = new StringBuilder();

                foreach (var bone in this.bones)
                {
                    var toName = bone.name.Replace("Bone", "");
                    var fromName = GetFromBoneName(bone);

                    var fromJoint = this.frames.Current.FindJoint(fromName);
                    var toJoint = this.frames.Current.FindJoint(toName);

                    var rotation = Quaternion.FromToRotation(Vector3.forward, toJoint.Position - fromJoint.Position);

                    bone.rotation = rotation;
                    var localRotation = bone.localRotation;     // Correct local rotation

                    Quaternion calculatedRotation;
                    if (fromName == "SpineBase")
                    {
                        calculatedRotation = rotation;
                    }
                    else
                    {
                        var fromFromName = GetFromBoneName(bone.parent.parent);
                        var fromFromJoint = this.frames.Current.FindJoint(fromFromName);
                        var fromDir = fromJoint.Position - fromFromJoint.Position;
                        var toDir = toJoint.Position - fromJoint.Position;

                        calculatedRotation = Quaternion.Inverse(FromToRotation(Vector3.forward, fromDir)) * FromToRotation(Vector3.forward, toDir);

                        Log("{0} ({1}) -> {2} ({3}) -> {4} ({5}) : {6} -> {7} : {8} / {9}",
                            fromFromName, fromFromJoint.Position.ToString("f3"),
                            fromName, fromJoint.Position.ToString("f3"),
                            toName, toJoint.Position.ToString("f3"),
                            fromDir.ToString("f3"), toDir.ToString("f3"),
                            calculatedRotation.ToString("f3"),
                            calculatedRotation.eulerAngles.ToString("f3"));
                    }
                    bone.localRotation = calculatedRotation;

                    //if (toName == "SpineMid")
                    //{
                    //    builder.AppendLine(string.Format("Rot {0} gr {1} lr {2} cr {3} jr {4}"
                    //                , toName
                    //                , rotation.eulerAngles.ToString("f1")
                    //                , localRotation.eulerAngles.ToString("f1")
                    //                , calculatedRotation.eulerAngles.ToString("f1")
                    //                , fromJoint.Rotation.eulerAngles.ToString("f1")
                    //            ));
                    //}
                }

                //Log(builder.ToString());

                time -= FrameTime;
            }
        }
    }

    private static string GetFromBoneName(Transform bone)
    {
        var fromName = "SpineBase";

        if (bone.parent.name == "End")
            fromName = bone.parent.parent.name.Replace("Bone", "");

        return fromName;
    }

    Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
    {
        var unitFrom = fromDirection.normalized;
        var unitTo = toDirection.normalized;
        var d = Vector3.Dot(unitFrom, unitTo);

        if (d >= 1.0f)
        {
            // In the case where the two vectors are pointing in the same
            // direction, we simply return the identity rotation.
            return Quaternion.identity;
        }
        else if (d <= -1.0f)
        {
            // If the two vectors are pointing in opposite directions then we
            // need to supply a quaternion corresponding to a rotation of
            // PI-radians about an axis orthogonal to the fromDirection.
            var axis = Vector3.Cross(unitFrom, Vector3.right);
            if (axis.sqrMagnitude < 1e-6)
            {
                // Bad luck. The x-axis and fromDirection are linearly
                // dependent (colinear). We'll take the axis as the vector
                // orthogonal to both the y-axis and fromDirection instead.
                // The y-axis and fromDirection will clearly not be linearly
                // dependent.
                axis = Vector3.Cross(unitFrom, Vector3.up);
            }

            // Note that we need to normalize the axis as the cross product of
            // two unit vectors is not nececessarily a unit vector.
            return Quaternion.AngleAxis(Mathf.PI, axis.normalized);
        }
        else
        {
            // Scalar component.
            var s = Mathf.Sqrt(unitFrom.sqrMagnitude * unitTo.sqrMagnitude)
                + Vector3.Dot(unitFrom, unitTo);

            // Vector component.
            var v = Vector3.Cross(unitFrom, unitTo);

            // Return the normalized quaternion rotation.
            return Normalize(new Quaternion(v.x, v.y, v.z, s));
        }
    }

    Quaternion AngleAxis(float angle, Vector3 axis)
    {
        // The axis supplied should be a unit vector. We don't automatically
        // normalize the axis for efficiency.
        //assert(Mathf.Absabs(axis.magnitude - 1.0f) < 1e-6);

        float halfAngle = 0.5f * angle;
        var s = Mathf.Cos(halfAngle);
        var v = axis * Mathf.Sign(halfAngle);
        return new Quaternion(v.x, v.y, v.z, s);
    }

    Quaternion LookRotation(Vector3 forward)
    {
        //assert(forward.sqrMagnitude() > 0.0f);
        return FromToRotation(Vector3.forward, forward);
    }

    Quaternion Normalize(Quaternion q)
    {
        var magnitude = Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);

        return new Quaternion(q.x / magnitude, q.y / magnitude, q.z / magnitude, q.w / magnitude);
    }

    void Log(string msg, params object[] args)
    {
        Debug.Log(string.Format(msg, args));
    }
}
