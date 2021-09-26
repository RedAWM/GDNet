using TrueSync;

namespace GGPhys.Rigid.Forces
{

    ///<summary>
    /// ����������������
    ///</summary>
    public class RigidSpringForce : RigidForce
    {

        ///<summary>
        /// �������������õ���������
        ///</summary>
        private RigidBody m_bodyA, m_bodyB;

        ///<summary>
        /// ��������ϵ�е��������ӵ�
        ///</summary>
        private TSVector3 m_connectionA, m_connectionB;

        ///<summary>
        /// ����ϵ��
        ///</summary>
        private FP m_springConstant;

        ///<summary>
        /// ������������
        ///</summary>
        private FP m_restLength;

        public RigidSpringForce(RigidBody a, RigidBody b, TSVector3 connectionA, TSVector3 connectionB, FP springConstant, FP restLength)
        {
            m_bodyA = a;
            m_bodyB = b;
            m_connectionA = connectionA;
            m_connectionB = connectionB;
            m_springConstant = springConstant;
            m_restLength = restLength;
        }

        public override void UpdateForce(FP dt)
        {
            UpdateForce(m_bodyA, m_bodyB, m_connectionA, m_connectionB, dt);
            UpdateForce(m_bodyB, m_bodyA, m_connectionB, m_connectionA, dt);
        }

        private void UpdateForce(RigidBody body, RigidBody other, TSVector3 connection, TSVector3 otherConnection, FP dt)
        {
            if (body.HasInfiniteMass) return;

            TSVector3 lws = body.GetPointInWorldSpace(connection);
            TSVector3 ows = other.GetPointInWorldSpace(otherConnection);

            TSVector3 force = lws - ows;

            FP magnitude = force.Magnitude;

            magnitude = (m_restLength - magnitude) * m_springConstant;

            force.Normalize();
            force *= magnitude;
            body.AddForceAtPoint(force, lws);

        }
    }

}