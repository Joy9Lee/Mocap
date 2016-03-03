using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
    public enum HumanBodyBones
    {
        Hips,
        LeftUpperLeg,
        RightUpperLeg,
        LeftLowerLeg,
        RightLowerLeg,
        LeftFoot,
        RightFoot,
        Spine,
        Chest,
        Neck,
        Head,
        LeftShoulder,
        RightShoulder,
        LeftUpperArm,
        RightUpperArm,
        LeftLowerArm,
        RightLowerArm,
        LeftHand,
        RightHand,
        LeftToes,
        RightToes,
        LeftEye,
        RightEye,
        Jaw,
        LeftThumbProximal,
        LeftThumbIntermediate,
        LeftThumbDistal,
        LeftIndexProximal,
        LeftIndexIntermediate,
        LeftIndexDistal,
        LeftMiddleProximal,
        LeftMiddleIntermediate,
        LeftMiddleDistal,
        LeftRingProximal,
        LeftRingIntermediate,
        LeftRingDistal,
        LeftLittleProximal,
        LeftLittleIntermediate,
        LeftLittleDistal,
        RightThumbProximal,
        RightThumbIntermediate,
        RightThumbDistal,
        RightIndexProximal,
        RightIndexIntermediate,
        RightIndexDistal,
        RightMiddleProximal,
        RightMiddleIntermediate,
        RightMiddleDistal,
        RightRingProximal,
        RightRingIntermediate,
        RightRingDistal,
        RightLittleProximal,
        RightLittleIntermediate,
        RightLittleDistal,
        LastBone
    }

    public struct Quaternion
    {
        //
        // Static Fields
        //
        public const float kEpsilon = 1E-06f;

        //
        // Fields
        //
        public float z;

        public float w;

        public float x;

        public float y;

        //
        // Static Properties
        //
        public static Quaternion identity
        {
            get
            {
                return new Quaternion(0f, 0f, 0f, 1f);
            }
        }

        // Constructors
        //
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct MsgQuaternion
    {
        public HumanBodyBones bone;
        //public Quaternion quat;
    }
}
