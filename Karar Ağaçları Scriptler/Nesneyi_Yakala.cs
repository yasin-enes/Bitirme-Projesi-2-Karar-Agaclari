using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class Nesneyi_Yakala : NavMeshMovement
    {
        // Aranan nesne
        public SharedGameObject target;
        // Hedef yoksa hedefin konumunu kullan
        public SharedVector3 targetPosition;

        public override void OnStart()
        {
            base.OnStart();

            SetDestination(Target());
        }

        // Hedefi kovalarız ve yakalayınca başarı döndürürüz.
        // Hedefe ulaşmadığımız sürece yürütmeye devam ederiz. 
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                return TaskStatus.Success;
            }

            SetDestination(Target());

            return TaskStatus.Running;
        }

        // Hedef yok ise targetPosition nesnesini döndürürüz.
        private Vector3 Target()
        {
            if (target.Value != null)
            {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}