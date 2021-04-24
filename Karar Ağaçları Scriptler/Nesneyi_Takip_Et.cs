using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class Nesneyi_Takip_Et : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is following")]
        // Takip edilen nesne
        public SharedGameObject target;
        //Hedef belirtilen mesafeden fazla ise hedefe doğru ilerlemeye başla
        public SharedFloat moveDistance = 2;

        private Vector3 lastTargetPosition;
        private bool hasMoved;

        public override void OnStart()
        {
            base.OnStart();

            lastTargetPosition = target.Value.transform.position + Vector3.one * (moveDistance.Value + 1);
            hasMoved = false;
        }

        // Hedefi takip et. Belirlenen mesafeye ulaş.
        // Ancak hiçbir zaman hedefe ulaşılamayacağı için görev başarıya hiçbir zaman döndürmez.
        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
            {
                return TaskStatus.Failure;
            }

            // Hedef moveDistance değişkeninden fazla hareket etmişse harekete devam et.
            var targetPosition = target.Value.transform.position;
            if ((targetPosition - lastTargetPosition).magnitude >= moveDistance.Value)
            {
                SetDestination(targetPosition);
                lastTargetPosition = targetPosition;
                hasMoved = true;
            }
            else
            {
                // Hedef moveDistance değişkeninden az hareket etmişse hareketi bırak.
                if (hasMoved && (targetPosition - transform.position).magnitude < moveDistance.Value)
                {
                    Stop();
                    hasMoved = false;
                    lastTargetPosition = targetPosition;
                }
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            moveDistance = 2;
        }
    }
}