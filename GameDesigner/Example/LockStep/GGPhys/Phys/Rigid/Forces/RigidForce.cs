using TrueSync;

namespace GGPhys.Rigid.Forces
{

    /// <summary>
    /// ������������
    /// </summary>
    public abstract class RigidForce
    {
        /// <summary>
        /// Ϊ����������
        /// </summary>
        public abstract void UpdateForce(FP dt);

    }
}
