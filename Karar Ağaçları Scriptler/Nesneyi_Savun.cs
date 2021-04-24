using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using HelpURL = BehaviorDesigner.Runtime.Tasks.HelpURLAttribute;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    public class Nesneyi_Savun : NavMeshTacticalGroup
    {
        //Savunacak nesne
        public SharedGameObject defendObject;
        // Ajanları konumlandırmak için savunma nesnesinin çevresindeki yarıçap
        public SharedFloat radius = 3;
        // Savunmak için savunma nesnesinin çevresindeki yarıçap 
        public SharedFloat defendRadius = 10;
        // Ajanların savunma nesnesinden koruyabileceği maksimum mesafe
        public SharedFloat maxDistance = 15;

        private float theta;

        protected override void AddAgentToGroup(Behavior agent, int index)
        {
            base.AddAgentToGroup(agent, index);

            // 2 * PI = 360 derece
            theta = 2 * Mathf.PI / agents.Count;
        }

        protected override int RemoveAgentFromGroup(Behavior agent)
        {
            var index = base.RemoveAgentFromGroup(agent);

            // 2 * PI = 360 derece
            theta = 2 * Mathf.PI / agents.Count;

            return index;
        }

        public override TaskStatus OnUpdate()
        {
            var baseStatus = base.OnUpdate();
            if (baseStatus != TaskStatus.Running || !started)
            {
                return baseStatus;
            }

            // Ajanın bir hedefi varsa hedefe saldır
            if (tacticalAgent.TargetTransform != null)
            {
                // Hedef savunma nesnesinden çok uzaklaşırsa saldırmayı bırak.
                if ((transform.position - defendObject.Value.transform.position).magnitude > maxDistance.Value || !tacticalAgent.TargetDamagable.IsAlive())
                {
                    tacticalAgent.TargetTransform = null;
                    tacticalAgent.TargetDamagable = null;
                    tacticalAgent.AttackPosition = false;
                }
                else
                {
                    // Hedef mesafe içinde. Ona doğru ilerle.
                    tacticalAgent.AttackPosition = true;
                    if (MoveToAttackPosition())
                    {
                        tacticalAgent.TryAttack();
                    }
                }
            }
            else
            {
                // Olası hedef dönüşümleri arasında döngü yapıp her bir ajana en yakın dönüşümün hangisi olduğunu belirle.
                for (int i = targetTransforms.Count - 1; i > -1; --i)
                {
                    // Hedef hayatta olmalı.
                    if (targets[i].IsAlive())
                    {
                        // Hedef çok yaklaşırsa saldırmaya başla.
                        if ((transform.position - targetTransforms[i].position).magnitude < defendRadius.Value)
                        {
                            tacticalAgent.TargetDamagable = targets[i];
                            tacticalAgent.TargetTransform = targetTransforms[i];
                        }
                    }
                    else
                    {
                        // Hedef artık hayatta değil - listeden kaldır.
                        targets.RemoveAt(i);
                        targetTransforms.RemoveAt(i);
                    }
                }
            }

            // Ajan saldırmıyor. Savunma nesnesinin yanına git.
            if (!tacticalAgent.AttackPosition)
            {
                var targetPosition = defendObject.Value.transform.TransformPoint(radius.Value * Mathf.Sin(theta * formationIndex), 0, radius.Value * Mathf.Cos(theta * formationIndex));
                tacticalAgent.UpdateRotation(true);
                tacticalAgent.SetDestination(targetPosition);
                if (tacticalAgent.HasArrived())
                {
                    // Savunan nesneden uzak dur.
                    var direction = targetPosition - defendObject.Value.transform.position;
                    direction.y = 0;
                    tacticalAgent.RotateTowards(Quaternion.LookRotation(direction));
                }
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();

            defendObject = null;
            radius = 5;
            defendRadius = 10;
        }
    }
}