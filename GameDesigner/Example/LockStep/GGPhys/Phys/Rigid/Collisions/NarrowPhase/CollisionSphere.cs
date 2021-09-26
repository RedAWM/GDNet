using TrueSync;

namespace GGPhys.Rigid.Collisions
{
    /// <summary>
    /// ������ײ������
    /// </summary>
    public class CollisionSphere : CollisionPrimitive
    {
        public FP Radius;

        public CollisionSphere(FP radius)
        {
            Radius = radius;
            BoundingVolum = new BoundingSphere();
            BoundingVolum.Init(this);
        }

        public override void CalculateInternals()
        {
            base.CalculateInternals();
            BoundingVolum.Update(this);
        }
    }
}
