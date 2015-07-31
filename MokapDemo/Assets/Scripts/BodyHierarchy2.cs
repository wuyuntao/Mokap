using Mokap.Demo;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BodyHierarchy2 : MonoBehaviour
{
    private const float FrameTime = 0.033333f;

    private float time = 0;

    IEnumerator<Frame> frames;

    Dictionary<string, Transform> bones = new Dictionary<string, Transform>();

    // Use this for initialization
    void Start()
    {
        Log("up {0}", Vector3.up.ToString("f3"));
        Log("down {0}", Vector3.down.ToString("f3"));
        Log("left {0}", Vector3.left.ToString("f3"));
        Log("right {0}", Vector3.right.ToString("f3"));
        Log("forward {0}", Vector3.forward.ToString("f3"));
        Log("back {0}", Vector3.back.ToString("f3"));

        this.frames = Frame.ParseFromCsvFile("BodyFrameData").GetEnumerator();
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

        Transform fromBone;
        if (this.bones.TryGetValue(fromName, out fromBone))
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
        this.bones.Add(toName, toBone.transform);

        var rotation = Quaternion.LookRotation(toJoint.Position - fromJoint.Position);
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

                foreach (var bone in this.bones.Values)
                {
                    var toName = bone.name.Replace("Bone", "");
                    var fromName = GetFromBoneName(bone);

                    var fromJoint = this.frames.Current.FindJoint(fromName);
                    var toJoint = this.frames.Current.FindJoint(toName);

                    var rotation = Quaternion.LookRotation(toJoint.Position - fromJoint.Position);

                    bone.rotation = rotation;
                    var localRotation = bone.localRotation;

                    Quaternion calculatedRotation;
                    if (fromName == "SpineBase")
                    {
                        calculatedRotation = rotation;
                    }
                    else
                    {
                        var fromFromName = GetFromBoneName(bone.parent.parent);
                        var fromFromJoint = this.frames.Current.FindJoint(fromFromName);

                        calculatedRotation = AngleBetween(fromJoint.Position - fromFromJoint.Position, toJoint.Position - fromJoint.Position);
                    }
                    //bone.localRotation = calculatedRotation;

                    builder.AppendLine(string.Format("Rot {0} gr {1} lr {2} cr {3} jr {4}"
                                , toName
                                , rotation.eulerAngles.ToString("f3")
                                , localRotation.eulerAngles.ToString("f3")
                                , calculatedRotation.eulerAngles.ToString("f3")
                                , fromJoint.Rotation.eulerAngles.ToString("f3")
                            ));
                }

                Log(builder.ToString());

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

    Quaternion AngleBetween(Vector3 v1, Vector3 v2)
    {
        v1 = v1.normalized;
        v2 = v2.normalized;

        var dot = Vector3.Dot(v1, v2);
        var axis = Vector3.Cross(v1, v2);
        var w = Mathf.Sqrt(v1.sqrMagnitude * v2.sqrMagnitude) + dot;

        if (w < 0.0001f)    // vectors are 180 degrees apart
            return (new Quaternion(0, -v1.z, v1.y, v1.x));

        return new Quaternion(w, axis.x, axis.y, axis.z);
    }

    void Log(string msg, params object[] args)
    {
        Debug.Log(string.Format(msg, args));
    }
}
