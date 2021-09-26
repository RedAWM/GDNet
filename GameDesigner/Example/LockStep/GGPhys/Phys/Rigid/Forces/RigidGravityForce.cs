using TrueSync;

namespace GGPhys.Rigid.Forces
{

    ///<summary>
    /// ����������������
    ///</summary>
    public class RigidGravityForce : RigidForceArea
    {

        ///<summary>
        /// ��������
        ///<summary>
        TSVector3 m_gravity;

        public RigidGravityForce(FP gravity)
        {
            m_gravity = new TSVector3(0, gravity, 0);
        }

        public RigidGravityForce(TSVector3 gravity)
        {
            m_gravity = gravity;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(FP gravity)
        {
            m_gravity = new TSVector3(0, gravity, 0);
        }

        ///<summary>
        /// ����������
        ///</summary>
        public override void UpdateForce(RigidBody body, FP dt)
        {
            if (body.HasInfiniteMass) return;
            body.AddForce(m_gravity * body.GetMass());
        }
    }

}