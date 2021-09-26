using GGPhys.Core;
using GGPhysUnity;
using System.Collections.Generic;
using TrueSync;

namespace GGPhys.Rigid
{

    public delegate void OnCollisionEnterCallBack(BRigidBody otherBody, TSVector3 contactPoint);
    public delegate void OnCollisionStayCallBack(BRigidBody otherBody);
    public delegate void OnCollisionExitCallBack(BRigidBody otherBody);

    public delegate void OnTriggerEnterCallBack(BRigidBody otherBody);
    public delegate void OnTriggerStayCallBack(BRigidBody otherBody);
    public delegate void OnTriggerExitCallBack(BRigidBody otherBody);

    ///<summary>
    /// ������
    ///</summary>
    public class RigidBody
    {
        public event OnCollisionEnterCallBack OnCollisionEnterEvent; // ��ײ�����ص�
        public event OnCollisionStayCallBack OnCollisionStayEvent; // ��ײͣ���ص�
        public event OnCollisionExitCallBack OnCollisionExitEvent; // ��ײ�����ص�

        public event OnTriggerEnterCallBack OnTriggerEnterEvent; // ���������ص�
        public event OnTriggerStayCallBack OnTriggerStayEvent; // ����ͣ���ص�
        public event OnTriggerExitCallBack OnTriggerExitEvent; // ���������ص�

        public string name;

        /// <summary>
        /// Unity�е�RigidBody
        /// </summary>
        public BRigidBody UnityBody;

        /// <summary>
        /// �Ƿ��Ǿ�̬����
        /// </summary>
        public bool IsStatic = false;

        /// <summary>
        /// �Ƿ��ܳ���(������)Ӱ��
        /// </summary>
        public bool UseAreaForce = true;

        /// <summary>
        /// ����λ�ã�����X��Y��Z
        /// </summary>
        public byte FreezePosition = 0x00;
        /// <summary>
        /// ������ת��������X��Y��Z
        /// </summary>
        public byte FreezeRotation = 0x00;

        /// <summary>
        /// Ħ����
        /// </summary>
        public FP Friction = 0.1;

        /// <summary>
        /// �ص�ϵ��
        /// </summary>
        public FP Restitution = 0;

        /// <summary>
        /// ����ϵ��
        /// </summary>
        public FP SleepEpsilon = 0.12;

        /// <summary>
        /// �����ٶ�����
        /// </summary>
        public FP AwakeVelocityLimit = 0.1;

        ///<summary>
        /// �����ĵ���
        ///</summary>
        public FP InverseMass;

        ///<summary>
        /// �Ƿ�Ϊ��������
        ///</summary>
        public bool HasFiniteMass => InverseMass != 0;

        ///<summary>
        /// �Ƿ�Ϊ��������
        ///</summary>
        public bool HasInfiniteMass => InverseMass == 0;

        ///<summary>
        /// �����������������
        ///</summary>
        public Matrix3 InverseInertiaTensor;


        ///<summary>
        /// ��������ϵ��
        ///</summary>
        public FP LinearDamping = 0.99;

        ///<summary>
        /// ���ٶ�����ϵ��
        ///</summary>
        public FP AngularDamping = 0.99;

        ///<summary>
        /// λ��
        ///</summary>
        public TSVector3 Position;

        ///<summary>
        /// ��ת
        ///</summary>
        public TSQuaternion Orientation;

        ///<summary>
        /// �����ٶ�
        ///</summary>
        public TSVector3 Velocity;

        ///<summary>
        /// ���ٶ�
        ///</summary>
        public TSVector3 Rotation;

        ///<summary>
        /// �����������������
        ///</summary>
        public Matrix3 InverseInertiaTensorWorld;

        ///<summary>
        /// �˶��̶ȣ���������
        ///</summary>
        private FP m_motion;

        ///<summary>
        /// �Ƿ�����״̬
        ///</summary>
        private bool m_isAwake;

        ///<summary>
        /// �ܷ�����
        ///</summary>
        private bool m_canSleep;

        ///<summary>
        /// �任����
        ///</summary>
        public Matrix4 Transform;

        ///<summary>
        /// ��������
        ///</summary>
        private TSVector3 m_forceAccum;

        ///<summary>
        /// ��ת��
        ///</summary>
        private TSVector3 m_torqueAccum;

        ///<summary>
        /// �̶����ٶ�
        ///</summary>
        private TSVector3 m_acceleration;

        ///<summary>
        /// ��һ֡���ٶ�
        ///</summary>
        public TSVector3 LastFrameAcceleration;

        /// <summary>
        /// ��ǰ��ײ���ĸ���
        /// </summary>
        public List<RigidBody> ContactRigidBodies;

        /// <summary>
        /// ��ǰ��ײ���ĸ������
        /// </summary>
        public Dictionary<RigidBody, int> ContactRigidBodiesMap;

        /// <summary>
        /// ��ǰ�����еĸ���
        /// </summary>
        public List<RigidBody> TriggerRigidBodies;

        /// <summary>
        /// ��ǰ�����еĸ��崥������
        /// </summary>
        public Dictionary<RigidBody, int> TriggerRigidBodiesMap;

        public TSVector3 ForceAccum { get => m_forceAccum; }

        public TSVector3 TorqueAccum { get => m_torqueAccum; }


        public RigidBody()
        {
            Orientation = TSQuaternion.identity;
            InverseInertiaTensor = Matrix3.Identity;
            Transform = Matrix4.Identity;
            ContactRigidBodies = new List<RigidBody>();
            ContactRigidBodiesMap = new Dictionary<RigidBody, int>();
            TriggerRigidBodies = new List<RigidBody>();
            TriggerRigidBodiesMap = new Dictionary<RigidBody, int>();
        }

        /// <summary>
        /// λ�ö���
        /// </summary>
        public void ApplyFreezePosConstraints()
        {
            if (FreezePosition != 0)
            {
                if ((FreezePosition & 0x01) != 0)
                {
                    Velocity.x = 0;
                }
                if ((FreezePosition & 0x02) != 0)
                {
                    Velocity.y = 0;
                }
                if ((FreezePosition & 0x04) != 0)
                {
                    Velocity.z = 0;
                }
            }
        }

        /// <summary>
        /// ��ת�Ƕȶ���
        /// </summary>
        public void ApplyFreezeRotConstraints()
        {
            if (FreezeRotation != 0)
            {
                FP Ixx = (FreezeRotation & 0x01) != 0 ? 0 : InverseInertiaTensor.data0;
                FP Iyy = (FreezeRotation & 0x02) != 0 ? 0 : InverseInertiaTensor.data4;
                FP Izz = (FreezeRotation & 0x04) != 0 ? 0 : InverseInertiaTensor.data8;
                InverseInertiaTensor.data0 = Ixx;
                InverseInertiaTensor.data4 = Iyy;
                InverseInertiaTensor.data8 = Izz;
            }
        }

        /// <summary>
        /// ����һ����ײ���ĸ���
        /// </summary>
        /// <param name="body"></param>
        public void AddContactBody(RigidBody body, TSVector3 contactPoint)
        {
            if (!ContactRigidBodies.Contains(body))
            {
                ContactRigidBodies.Add(body);
                ContactRigidBodiesMap.Add(body, 2);
                OnCollisionEnter(body, contactPoint);
            }
            else
            {
                ContactRigidBodiesMap[body] += 1;
            }
        }

        /// <summary>
        /// �Ƴ�һ��������ײ�ĸ���
        /// </summary>
        /// <param name="body"></param>
        private void RemoveContactBody(RigidBody body)
        {
            if (ContactRigidBodies.Remove(body))
            {
                ContactRigidBodiesMap.Remove(body);
                OnCollisionExit(body);
            }
        }

        /// <summary>
        /// ����һ�������еĸ���
        /// </summary>
        /// <param name="body"></param>
        public void AddTriggerBody(RigidBody body)
        {
            if (!TriggerRigidBodies.Contains(body))
            {
                TriggerRigidBodies.Add(body);
                TriggerRigidBodiesMap.Add(body, 2);
                OnTriggerEnter(body);
            }
            else
            {
                TriggerRigidBodiesMap[body] += 1;
            }
        }

        /// <summary>
        /// �Ƴ�һ�����������ĸ���
        /// </summary>
        /// <param name="body"></param>
        private void RemoveTriggerBody(RigidBody body)
        {
            if (TriggerRigidBodies.Remove(body))
            {
                TriggerRigidBodiesMap.Remove(body);
                OnTriggerExit(body);
            }
        }

        public void RemoveContactAndTriggerBodys()
        {
            for (int i = ContactRigidBodies.Count - 1; i >= 0; i--)
            {
                RigidBody body = ContactRigidBodies[i];
                ContactRigidBodiesMap[body] -= 1;
                if (ContactRigidBodiesMap[body] <= 0)
                {
                    RemoveContactBody(body);
                }
            }
            for (int i = TriggerRigidBodies.Count - 1; i >= 0; i--)
            {
                RigidBody body = TriggerRigidBodies[i];
                TriggerRigidBodiesMap[body] -= 1;
                if (TriggerRigidBodiesMap[body] <= 0)
                {
                    RemoveTriggerBody(body);
                }
            }
        }

        internal TSVector3 Offset;

        ///<summary>
        /// ������������
        ///</summary>
        public void CalculateDerivedData()
        {
            Orientation.Normalize();
            // ����任����
            CalculateTransformMatrix(ref Transform, Position + Offset, Orientation);
            // ���������������������
            TransformInertiaTensor(ref InverseInertiaTensorWorld, InverseInertiaTensor, Transform);
        }

        /// <summary>
        /// �������������ٶȽ��ٶ�
        /// </summary>
        /// <param name="dt"></param>
        public void ApplyForceToVelocity(FP dt)
        {
            if (!m_isAwake || IsStatic) return;

            LastFrameAcceleration = m_acceleration;
            LastFrameAcceleration += m_forceAccum * InverseMass;

            TSVector3 angularAcceleration = InverseInertiaTensorWorld * m_torqueAccum;

            Velocity += LastFrameAcceleration * dt;

            Rotation += angularAcceleration * dt;

            Velocity *= TSMathf.Pow(LinearDamping, dt);
            Rotation *= TSMathf.Pow(AngularDamping, dt);
        }

        ///<summary>
        /// ����λ����ת
        ///</summary>
        public void Integrate(FP dt)
        {
            RemoveContactAndTriggerBodys();
            ClearAccumulators();

            if (!m_isAwake || IsStatic) return;

            ApplyFreezePosConstraints();

            Position += Velocity * dt;

            Orientation.AddScaledVector(Rotation, dt);

            if (m_canSleep)
            {
                FP currentMotion = TSVector3.Dot(Velocity, Velocity) + TSVector3.Dot(Rotation, Rotation);

                FP bias = 0.92;
                m_motion = bias * m_motion + (1 - bias) * currentMotion;

                if (m_motion < SleepEpsilon)
                    SetAwake(false);
                else if (m_motion > 10 * SleepEpsilon)
                    m_motion = 10 * SleepEpsilon;

            }
        }

        ///<summary>
        /// ��ֵ����
        ///</summary>
        public void SetMass(FP mass)
        {
            if (mass <= 0)
                InverseMass = 0;
            else
                InverseMass = 1.0 / mass;
        }

        ///<summary>
        /// ��ȡ����
        ///</summary>
        public FP GetMass()
        {
            if (InverseMass == 0)
                return FP.MaxValue;
            else
                return 1.0 / InverseMass;
        }

        ///<summary>
        /// ��ֵ��������
        ///</summary>
        public void SetInertiaTensor(Matrix3 inertiaTensor)
        {
            InverseInertiaTensor = inertiaTensor.Inverse();
        }

        ///<summary>
        /// ��ȡ��������
        ///</summary>
        public Matrix3 GetInertiaTensor()
        {
            return InverseInertiaTensor.Inverse();
        }

        ///<summary>
        /// ��ȡ���������������
        ///</summary>
        public Matrix3 GetInertiaTensorWorld()
        {
            return InverseInertiaTensorWorld.Inverse();
        }

        ///<summary>
        /// ת��һ������������굽��������
        ///</summary>
        public TSVector3 GetPointInLocalSpace(TSVector3 point)
        {
            return Transform.TransformInverse(point);
        }

        ///<summary>
        /// ת��һ����ı������굽��������
        ///</summary>
        public TSVector3 GetPointInWorldSpace(TSVector3 point)
        {
            return Transform.Transform(point);
        }

        ///<summary>
        /// ת��һ���������������굽��������
        ///</summary>
        public TSVector3 GetDirectionInLocalSpace(TSVector3 direction)
        {
            return Transform.TransformInverseDirection(direction);
        }

        ///<summary>
        /// ת��һ�������ӱ������굽��������
        ///</summary>
        public TSVector3 GetDirectionInWorldSpace(TSVector3 direction)
        {
            return Transform.TransformDirection(direction);
        }

        ///<summary>
        /// ��ȡ����״̬
        ///</summary>
        public bool GetAwake()
        {
            return m_isAwake;
        }

        ///<summary>
        /// ��ֵ����״̬
        ///</summary>
        public void SetAwake(bool awake = true)
        {
            if (awake)
            {
                if (m_isAwake) return;

                m_isAwake = true;
                m_motion = SleepEpsilon * 10; //�����ѣ���Ҫһ����ʼ�ƶ����������������������
            }
            else
            {
                m_isAwake = false;
                Velocity = TSVector3.Zero;
                Rotation = TSVector3.Zero;
                LastFrameAcceleration = TSVector3.Zero;
            }
        }

        public bool GetCanSleep()
        {
            return m_canSleep;
        }

        public void SetCanSleep(bool canSleep = true)
        {
            m_canSleep = canSleep;
            if (!m_canSleep && !m_isAwake) SetAwake();
        }


        ///<summary>
        /// ������������ͺ�ת��
        ///</summary>
        public void ClearAccumulators()
        {
            m_forceAccum = TSVector3.Zero;
            m_torqueAccum = TSVector3.Zero;
        }

        ///<summary>
        /// �����
        ///</summary>
        public void AddForce(TSVector3 force, bool awakeBody = false)
        {
            m_forceAccum += force;
            if (awakeBody)
                SetAwake(true);
        }

        ///<summary>
        /// ��ĳ�����������
        ///</summary>
        public void AddForceAtPoint(TSVector3 force, TSVector3 point)
        {
            TSVector3 pt = point - Position;

            m_forceAccum += force;
            m_torqueAccum += TSVector3.Cross(pt, force);
            SetAwake(true);
        }

        ///<summary>
        /// ��ĳ������������������
        ///<summary>
        public void AddForceAtBodyPoint(TSVector3 force, TSVector3 point)
        {
            TSVector3 pt = GetPointInWorldSpace(point);
            AddForceAtPoint(force, pt);
        }

        ///<summary>
        /// ���ת��
        ///<summary>
        public void AddTorque(TSVector3 torque, bool awakeBody = false)
        {
            m_torqueAccum += torque;
            if (awakeBody)
                m_isAwake = true;
        }

        /// <summary>
        /// �������Գ���
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyLinearImpulse(TSVector3 impulse)
        {
            TSVector3 linearChange = impulse * InverseMass;
            Velocity += linearChange;
            SetAwake(true);
        }

        /// <summary>
        /// ���ýǳ���
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyAngularImpulse(TSVector3 impulse)
        {
            TSVector3 angularChange = InverseInertiaTensorWorld * impulse;
            Rotation += angularChange;
            SetAwake(true);
        }

        /// <summary>
        /// �ƶ�
        /// </summary>
        /// <param name="delta"></param>
        public void Move(TSVector3 delta)
        {
            Position += delta;
            SetAwake(true);
        }

        /// <summary>
        /// ��ת
        /// </summary>
        /// <param name="delta"></param>
        public void Rotate(TSVector3 delta)
        {
            Orientation.AddScaledVector(delta, 1);
            SetAwake(true);
        }

        public void OnCollisionEnter(RigidBody otherBody, TSVector3 contactPoint)
        {
            OnCollisionEnterEvent?.Invoke(otherBody.UnityBody, contactPoint);
        }

        public void OnCollisionStay(RigidBody otherBody)
        {
            OnCollisionStayEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnCollisionExit(RigidBody otherBody)
        {
            OnCollisionExitEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerEnter(RigidBody otherBody)
        {
            OnTriggerEnterEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerStay(RigidBody otherBody)
        {
            OnTriggerStayEvent?.Invoke(otherBody.UnityBody);
        }

        public void OnTriggerExit(RigidBody otherBody)
        {
            OnTriggerExitEvent?.Invoke(otherBody.UnityBody);
        }

        ///<summary>
        /// ���������������������
        ///</summary>
        private static void TransformInertiaTensor(ref Matrix3 iitWorld, Matrix3 iitBody, Matrix4 rotmat)
        {
            Matrix3 rotM3 = Matrix3.FromLong(rotmat.raw0, rotmat.raw1, rotmat.raw2,
                                             rotmat.raw4, rotmat.raw5, rotmat.raw6,
                                             rotmat.raw8, rotmat.raw9, rotmat.raw10);
            iitWorld = rotM3 * iitBody * rotM3.Transpose();
        }

        ///<summary>
        /// ����任����
        ///</summary>
        private static void CalculateTransformMatrix(ref Matrix4 transformMatrix, TSVector3 position, TSQuaternion orientation)
        {
            transformMatrix.SetOrientationAndPos(orientation, position);
        }

        public override string ToString()
        {
            return $"{UnityBody.name}";
        }
    }
}