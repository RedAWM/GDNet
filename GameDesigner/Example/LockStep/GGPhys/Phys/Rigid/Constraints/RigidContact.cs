using GGPhys.Core;
using GGPhys.Rigid.Collisions;
using System;
using TrueSync;

namespace GGPhys.Rigid.Constraints
{
    /// <summary>
    /// ��ײ������
    /// </summary>
    public class RigidContact
    {

        ///<summary>
        /// �໥��ײ����������
        ///</summary>
        public RigidBody[] Body = new RigidBody[2];

        /// <summary>
        /// �Ƿ������ڲ���������������
        /// </summary>
        public bool MatchedAwake = false;

        ///<summary>
        /// Ħ��ϵ��
        ///</summary>
        public FP Friction;

        ///<summary>
        /// �ص�ϵ��
        ///</summary>
        public FP Restitution;

        ///<summary>
        /// ��ײ��
        ///</summary>
        public TSVector3 ContactPoint;

        ///<summary>
        /// ��ײ����
        ///</summary>
        public TSVector3 ContactNormal;

        /// <summary>
        /// ��ײ����
        /// </summary>
        public TSVector3 ContactPerpendicular;

        public FP ContactVR; //�������������Լ����Ԥ�ȼ���õĲ���

        public TSVector3 CrossOne; //�������������Լ����Ԥ�ȼ���õĲ���

        public TSVector3 CrossTwo; //�������������Լ����Ԥ�ȼ���õĲ���

        public TSVector3 FCrossOne; //�������������Լ����Ԥ�ȼ���õĲ���

        public TSVector3 FCrossTwo; //�������������Լ����Ԥ�ȼ���õĲ���

        public FP JMJ; //�������������Լ����Ԥ�ȼ���õĲ���

        public FP FJMJ; //�������������Լ����Ԥ�ȼ���õĲ���

        public FP Lambda; //��ײ���ַ����������ճ���

        public FP FLambda; //Ħ���������������ճ���

        public int IntegrateTimes = 0; //����ײ����������Ĵ���

        ///<summary>
        /// �ཻ���
        ///</summary>
        public FP Penetration;

        ///<summary>
        /// �պ��ٶ�
        ///</summary>
        public TSVector3 ContactVelocity;

        ///<summary>
        /// ��������Ե����ĵ���ײ�������
        ///</summary>
        public TSVector3[] RelativeContactPosition = new TSVector3[2];


        ///<summary>
        /// ��ֵ
        ///</summary>
        public void SetData(RigidContactPotential pContact)
        {
            RigidBody one = pContact.Primitive1.Body;
            RigidBody two = pContact.Primitive2.Body;
            one.AddContactBody(two, pContact.ContactPoint);
            two.AddContactBody(one, pContact.ContactPoint);
            Body[0] = one;
            Body[1] = two;
            Friction = pContact.Friction;
            Restitution = pContact.Restitution;
            ContactNormal = pContact.ContactNormal;
            ContactPoint = pContact.ContactPoint;
            Penetration = pContact.Penetration;
            ContactVelocity = pContact.ContactVelocity;
            ContactPerpendicular = pContact.ContactPerpendicular;
            ContactVR = pContact.ContactVR;
            CrossOne = pContact.CrossOne;
            CrossTwo = pContact.CrossTwo;
            FCrossOne = pContact.FCrossOne;
            FCrossTwo = pContact.FCrossTwo;
            JMJ = pContact.JMJ;
            FJMJ = pContact.FJMJ;
            RelativeContactPosition[0] = pContact.RelativeContactPosition[0];
            RelativeContactPosition[1] = pContact.RelativeContactPosition[1];

            MatchAwakeState();
        }

        /// <summary>
        /// �����λ��׼������
        /// </summary>
        public void Clear()
        {
            Body[0] = null;
            Body[1] = null;
            Lambda = 0;
            FLambda = 0;
            IntegrateTimes = 0;
            MatchedAwake = false;
        }

        ///<summary>
        /// �����ڲ�����
        ///</summary>
        public void CalculateInternals()
        {
            if (Body[0] == null || Body[1] == null)
                throw new NullReferenceException("Body null");

            RigidBody body1 = Body[0];
            RigidBody body2 = Body[1];

            RelativeContactPosition[0] = ContactPoint - body1.Position;
            RelativeContactPosition[1] = ContactPoint - body2.Position;

            ContactVelocity = CalculateLocalVelocity(0);
            if (!Body[1].IsStatic)
                ContactVelocity -= CalculateLocalVelocity(1);
            FP normalContactVelocity = TSVector3.Dot(ContactVelocity, -ContactNormal);
            ContactPerpendicular = -(ContactVelocity + normalContactVelocity * ContactNormal).Normalized;

            ContactVR = normalContactVelocity * Restitution;

            CrossOne = TSVector3.Cross(-RelativeContactPosition[0], -ContactNormal);
            CrossTwo = TSVector3.Cross(RelativeContactPosition[1], -ContactNormal);

            FCrossOne = TSVector3.Cross(RelativeContactPosition[0], ContactPerpendicular);
            FCrossTwo = TSVector3.Cross(RelativeContactPosition[1], -ContactPerpendicular);

            FP oneMass = body1.IsStatic ? 0 : body1.InverseMass;
            FP twoMass = body2.IsStatic ? 0 : body2.InverseMass;
            Matrix3 oneTensor = body1.IsStatic ? Matrix3.Zero : body1.InverseInertiaTensorWorld;
            Matrix3 twoTensor = body2.IsStatic ? Matrix3.Zero : body2.InverseInertiaTensorWorld;

            FP linearPart = TSVector3.Dot(ContactNormal, ContactNormal) * (oneMass + twoMass);
            FP angularPart = TSVector3.Dot(CrossOne, oneTensor * CrossOne) + TSVector3.Dot(CrossTwo, twoTensor * CrossTwo);
            JMJ = linearPart + angularPart;

            FP flinearPart = TSVector3.Dot(ContactPerpendicular, ContactPerpendicular) * (oneMass + twoMass);
            FP fangularPart = TSVector3.Dot(FCrossOne, oneTensor * FCrossOne) + TSVector3.Dot(FCrossTwo, twoTensor * FCrossTwo);
            FJMJ = flinearPart + fangularPart;
        }

        ///<summary>
        /// ������ײ��������ٶȣ��������������ƶ������Ĳ��ֺ͸�����ת�����Ĳ���
        ///</summary>
        public TSVector3 CalculateLocalVelocity(int bodyIndex)
        {
            RigidBody thisBody = Body[bodyIndex];

            TSVector3 velocity = TSVector3.Cross(thisBody.Rotation, RelativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            return velocity;
        }

        ///<summary>
        /// �жϸ����Ƿ������������
        ///</summary>
        public void MatchAwakeState()
        {
            RigidBody body0 = Body[0];
            RigidBody body1 = Body[1];

            bool body0awake = body0.GetAwake();
            bool body1awake = body1.GetAwake();
            bool body0static = body0.IsStatic;
            bool body1static = body1.IsStatic;

            if (body0awake ^ body1awake)
            {
                if (body0static || body1static) return;
                if (body0awake && ContactVelocity.SqrMagnitude > body1.AwakeVelocityLimit)
                    body1.SetAwake();
                if (body1awake && ContactVelocity.SqrMagnitude > body0.AwakeVelocityLimit)
                    body0.SetAwake();
            }

            MatchedAwake = true;
        }
    }
}