using GGPhys.Rigid.Constraints;
using GGPhys.Rigid.Forces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrueSync;

namespace GGPhys.Rigid
{
    ///<summary>
    /// ����������
    ///</summary>
    [System.Serializable]
    public class RigidBodyEngine
    {
        ///<summary>
        /// ���и���
        ///</summary>
        public List<RigidBody> Bodies;

        /// <summary>
        /// ������������
        /// </summary>
        public List<RigidForceArea> ForceAreas;

        /// <summary>
        /// ������������
        /// </summary>
        public List<RigidForce> Forces;

        ///<summary>
        /// ����Լ���б�
        ///<summary>
        public List<RigidConstraint> Constraints;

        /// <summary>
        /// ��ײԼ��
        /// </summary>
        public CollisionConstraint Collisions;

        ///<summary>
        /// ��ײԼ�������
        ///<summary>
        public RigidContactSIResolver SIResolver;

        /// <summary>
        /// ����������
        /// </summary>
        public RigidGravityForce GravityForceArea;

        /// <summary>
        /// ���ʹ���߳���
        /// </summary>
#pragma warning disable IDE0052 // ɾ��δ����˽�г�Ա
        private readonly int m_MaxThread;
#pragma warning restore IDE0052 // ɾ��δ����˽�г�Ա

        public RigidBodyEngine(FP gravity, TSVector3 gridSize, TSVector3 gridCellSize, TSVector3 gridCenterOffset, int maxThreadCount)
        {
            m_MaxThread = maxThreadCount;

            Bodies = new List<RigidBody>();
            ForceAreas = new List<RigidForceArea>();
            Forces = new List<RigidForce>();
            Constraints = new List<RigidConstraint>();
            SIResolver = new RigidContactSIResolver();

            GravityForceArea = new RigidGravityForce(gravity);
            ForceAreas.Add(GravityForceArea);

            Collisions = new CollisionConstraint(gridSize, gridCellSize, gridCenterOffset);
            Collisions.MaxThread = maxThreadCount;
            Constraints.Add(Collisions);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(float gravity)
        {
            GravityForceArea.SetGravity(gravity);
        }

        ///<summary>
        /// ��ʼ��
        ///<summary>
        public void StartFrame()
        {
            foreach (RigidBody body in Bodies)
            {
                body.ClearAccumulators();
            }
        }

        internal System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public long ApplyForcesTime, GenerateContactsTime, IntegrateTime, PostUpdateTime;

        ///<summary>
        /// ����ִ��һ�ε���
        ///<summary>
        public void RunPhysics(FP dt)
        {
            sw.Restart();
            ApplyForces(dt);//�������µ����ͽ��ٶ� û������������䣬 ����ײ��
            sw.Stop();
            ApplyForcesTime = sw.ElapsedMilliseconds;

            sw.Restart();
            GenerateContacts(); //û�������ײʧЧ
            sw.Stop();
            GenerateContactsTime = sw.ElapsedMilliseconds;

            if (Collisions.CollisionData.Contacts.Count > 0)//û������������
                SIResolver.ResolveContacts(Collisions.CollisionData, dt);

            sw.Restart();
            Integrate(dt);//û�������ײʧЧ
            sw.Stop();
            IntegrateTime = sw.ElapsedMilliseconds;

            sw.Restart();
            PostUpdate();//û������ƶ�ʧЧ
            sw.Stop();
            PostUpdateTime = sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// �������и����ڲ�
        /// </summary>
        public void CalculateDerivedData()
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                if (Bodies[i].IsStatic)
                    continue;
                Bodies[i].CalculateDerivedData();
                Bodies[i].UnityBody.CalculateInternals();
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyForces(FP dt)
        {
            for (int i = 0; i < ForceAreas.Count; i++)
            {
                for (int j = 0; j < Bodies.Count; j++)
                {
                    if (Bodies[j].IsStatic)
                        continue;
                    if (!Bodies[j].UseAreaForce)
                        goto J;
                    ForceAreas[i].UpdateForce(Bodies[j], dt);//�������µ���
                J: Bodies[j].ApplyForceToVelocity(dt);//���½��ٶ�
                }
            }
        }


        ///<summary>
        /// ������ײ����
        ///<summary>
        private void GenerateContacts()
        {
            for (int i = 0; i < Constraints.Count; i++)
            {
                Constraints[i].GenerateContacts();//CollisionConstraint��
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="dt"></param>
        private void Integrate(FP dt)
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                Bodies[i].Integrate(dt);
            }
        }

        /// <summary>
        /// ѭ��������
        /// </summary>
        private void PostUpdate()
        {
            CalculateDerivedData();
        }
    }
}