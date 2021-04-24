using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class Nesneyi_Duy : Conditional
    {
        // Fizik 2D motoru kullanılmalı mı
        public bool usePhysics2D;
        // Aradığımız nesne
        public SharedGameObject targetObject;
        // Aradığımız nesneler
        public SharedGameObjectList targetObjects;
        // Aradığımız nesnenin etiketi
        public SharedString targetTag;
        // Aradığımız nesnelerin maskelemesi
        public LayerMask objectLayerMask;
        // Duyma mesafesi
        public SharedFloat hearingRadius = 50;
        //Ses kaynağı ne kadar uzaksa o kadar az ihtimalle işitilir
        // Minimum mesafe için atama yapmalıyız
        public SharedFloat audibilityThreshold = 0.05f;
        // Pivot konumuna göre 
        public SharedVector3 offset;
        // Döndürülen objenin duyulması için
        public SharedGameObject returnedObject;

        // Nesne bulunursa başarı döndür
        public override TaskStatus OnUpdate()
        {
            if (targetObjects.Value != null && targetObjects.Value.Count > 0)
            { // Grup listesinde nesneler varsa, o listedeki nesneyi arayın
                GameObject objectFound = null;
                for (int i = 0; i < targetObjects.Value.Count; ++i)
                {
                    float audibility = 0;
                    GameObject obj;
                    if (Vector3.Distance(targetObjects.Value[i].transform.position, transform.position) < hearingRadius.Value)
                    {
                        if ((obj = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, targetObjects.Value[i], ref audibility)) != null)
                        {
                            objectFound = obj;
                        }
                    }
                }
                returnedObject.Value = objectFound;
            }
            else if (targetObject.Value == null)
            { // Hedef nesne yoksa işitme mesafesinde bir nesne olup olmadığını kontrol et
                if (usePhysics2D)
                {
                    returnedObject.Value = MovementUtility.WithinHearingRange2D(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
                }
                else
                {
                    returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
                }
            }
            else
            {
                GameObject target;
                if (!string.IsNullOrEmpty(targetTag.Value))
                {
                    target = GameObject.FindGameObjectWithTag(targetTag.Value);
                }
                else
                {
                    target = targetObject.Value;
                }
                if (Vector3.Distance(target.transform.position, transform.position) < hearingRadius.Value)
                {
                    returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, targetObject.Value);
                }
            }

            if (returnedObject.Value != null)
            {
                // Bir nesne duyulursa başarı döndür
                return TaskStatus.Success;
            }
            // Bir nesne duyulmazsa hata döndür
            return TaskStatus.Failure;
        }

        // Public tanımlı değişkenleri resetle
        public override void OnReset()
        {
            hearingRadius = 50;
            audibilityThreshold = 0.05f;
        }

        // İşitme yarıçapını belirle
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || hearingRadius == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, hearingRadius.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }

        public override void OnBehaviorComplete()
        {
            MovementUtility.ClearCache();
        }
    }
}