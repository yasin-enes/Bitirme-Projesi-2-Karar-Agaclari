using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Tutorials
{
    public class Nesneyi_Gor : Conditional
    {
        // Hedef nesne
        public SharedGameObject targetObject;
        // Ajanın görüş açısı (Derece Cinsinden)
        public SharedFloat fieldOfViewAngle = 90;
        // Ajanın görüş mesafesi
        public SharedFloat viewDistance = 1000;
        // Görüş alanına giren nesne
        public SharedGameObject returnedObject;

        public override TaskStatus OnUpdate()
        {
            returnedObject.Value = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
            if (returnedObject.Value != null)
            {
                // Bir nesne bulunursa başarı döndür
                return TaskStatus.Success;
            }
            // Görüş alanında bir nesne yok dolayısıyla hata döndür
            return TaskStatus.Failure;
        }

        private GameObject WithinSight(GameObject targetObject, float fieldOfViewAngle, float viewDistance)
        {
            if (targetObject == null)
            {
                return null;
            }

            var direction = targetObject.transform.position - transform.position;
            direction.y = 0;
            var angle = Vector3.Angle(direction, transform.forward);
            if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
            {
                // İsabet ajanının mevcut ajanın görüşü dahilinde olması gerekiyor
                if (LineOfSight(targetObject))
                {
                    return targetObject; // görüş alanında olan nesneyi döndür
                }
            }
            return null;
        }

        private bool LineOfSight(GameObject targetObject)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, targetObject.transform.position, out hit))
            {
                if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = Color.yellow;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;

            var halfFOV = fieldOfViewAngle.Value * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * Owner.transform.forward;
            UnityEditor.Handles.DrawSolidArc(Owner.transform.position, Owner.transform.up, beginDirection, fieldOfViewAngle.Value, viewDistance.Value);

            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}