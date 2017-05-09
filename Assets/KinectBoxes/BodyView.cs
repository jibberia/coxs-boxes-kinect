using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodyView : MonoBehaviour 
{
    public Material bodyMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> jointMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
         { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
         { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
         { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
         { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
         { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
         { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
         { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
         { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
         { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
         { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },

        { Kinect.JointType.Head, Kinect.JointType.Head },
    };

    // private List<GameObject> bones = new List<GameObject>();
    // private Dictionary<Kinect.JointType, Transform> jointTransforms = new Dictionary<Kinect.JointType, Transform>();
    private Dictionary<ulong, Dictionary<Kinect.JointType, Transform>> bodyJointTransforms =
        new Dictionary<ulong, Dictionary<Kinect.JointType, Transform>>();
    // private Dictionary<Kinect.JointType, GameObject> bones = new Dictionary<Kinect.JointType, GameObject>();
    private Dictionary<ulong, Dictionary<Kinect.JointType, GameObject>> bodyBones =
        new Dictionary<ulong, Dictionary<Kinect.JointType, GameObject>>();

    void Update () 
    {
        if (BodySourceManager == null) return;
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null) return;
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null) return;
        
        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data) {
            if (body == null) continue;
            if (body.IsTracked) {
                trackedIds.Add(body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach (ulong trackingId in knownIds) {
            if (!trackedIds.Contains(trackingId)) {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
                bodyBones.Remove(trackingId);
                bodyJointTransforms.Remove(trackingId);
            }
        }

        foreach (var body in data) {
            if (body == null) continue;
            
            if (body.IsTracked) {
                if (!_Bodies.ContainsKey(body.TrackingId)) {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
                RefreshBones(bodyBones[body.TrackingId], bodyJointTransforms[body.TrackingId]);
            }
        }
    }

    void RefreshBones(Dictionary<Kinect.JointType, GameObject> bones, Dictionary<Kinect.JointType, Transform> jointTransforms)
    {
        foreach (Kinect.JointType jt in jointMap.Keys) {
            if (jt == jointMap[jt]) continue;
            if (!bones.ContainsKey(jt)) continue;
            GameObject bone = bones[jt];
            if (bone == null || bone.transform.parent == null) continue;
            Transform tempParent = bone.transform.parent;
            bone.transform.parent = null;
            SetPositionBetween(bone.transform, jointTransforms[jt].localPosition, jointTransforms[jointMap[jt]].localPosition);
            bone.transform.SetParent(tempParent, false);
        }
    }
    
    public float jointScale = 0.3f;
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        MeshRenderer mr;
        Rigidbody rb;
//        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
        foreach (Kinect.JointType jt in jointMap.Keys) {
            if (!bodyJointTransforms.ContainsKey(id)) bodyJointTransforms[id] = new Dictionary<Kinect.JointType, Transform>();
            if (!bodyBones.ContainsKey(id)) bodyBones[id] = new Dictionary<Kinect.JointType, GameObject>();
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            rb = jointObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = 5;

            mr = jointObj.GetComponent<MeshRenderer>();
            mr.material = bodyMaterial;
            mr.enabled = true;

            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            jointObj.transform.localScale = new Vector3(jointScale, jointScale, jointScale);// * 60f);
            bodyJointTransforms[id][jt] = jointObj.transform;

            if (jointMap[jt] != jt) { // skip head
                GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                rb = bone.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.mass = 15;
                
                mr = bone.GetComponent<MeshRenderer>();
                mr.material = bodyMaterial;
                mr.enabled = true;

                bone.name = "Bone<" + jt.ToString() + "," + jointMap[jt].ToString() + ">";
                bone.transform.parent = body.transform;
                bodyBones[id][jt] = bone;
            }
        }

        body.transform.SetParent(this.gameObject.transform, false); // worldPositionStays
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        // for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        foreach (Kinect.JointType jt in jointMap.Keys) {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint targetJoint = body.Joints[jointMap[jt]];
            // Kinect.Joint? targetJoint = null;
            
            // if (jointMap.ContainsKey(jt)) {
            //     targetJoint = body.Joints[jointMap[jt]];
            // }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            // if (jt != jointMap[jt]) { // skip head
            //     GameObject bone = bones[jt];
            //     Vector3 source = jointTransforms[jt].position;//GetVector3FromJoint(sourceJoint);
            //     Vector3 target = jointTransforms[jointMap[jt]].position;//GetVector3FromJoint(targetJoint);
            //     SetPositionBetween(bone.transform, source, target);
            // }
        }
    }
    
    public float vectorScale = 10.0f;
    private Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * vectorScale, joint.Position.Y * vectorScale, joint.Position.Z * vectorScale);
    }

    public float width = 0.2f;
    void SetPositionBetween(Transform bone, Vector3 a, Vector3 b) {
        // Vector3 a = this.transform.position + from;
        // Vector3 b = this.transform.position + to;
    
		Vector3 offset = b - a;
		Vector3 midpoint = a + (offset / 2);

		bone.localPosition = midpoint;
		bone.up = offset;
		bone.localScale = new Vector3(width, offset.magnitude/2f, width);
	}
}
