using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    public class Nesneye_Saldir : NavMeshTacticalGroup
    {
        public override TaskStatus OnUpdate()
        {
            var baseStatus = base.OnUpdate();
            if (baseStatus != TaskStatus.Running || !started)
            {
                return baseStatus;
            }

            if (MoveToAttackPosition())
            {
                tacticalAgent.TryAttack();
            }

            return TaskStatus.Running;
        }
    }
}