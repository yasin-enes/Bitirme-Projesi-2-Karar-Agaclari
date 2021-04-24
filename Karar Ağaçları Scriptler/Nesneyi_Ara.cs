using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class Nesneyi_Ara : NavMeshMovement
    {
        // Varış noktası ile mevcut konum arasındaki minimum mesafe
        public SharedFloat minWanderDistance = 20;
        //Varış noktası ile mevcut konum arasındaki maksimum mesafe
        public SharedFloat maxWanderDistance = 20;
        // Ajanın dönme miktarı
        public SharedFloat wanderRate = 1;
        // Ajanın her bir hedefte duraklaması gereken minimum süre
        public SharedFloat minPauseDuration = 0;
        //Aracının her bir hedefte duraklaması gereken maksimum süre (devre dışı bırakmak için sıfır yaz)
        public SharedFloat maxPauseDuration = 0;
        // Maksimum deneme sayısı
        public SharedInt targetRetries = 1;
        // Ajanın görüş açısı (derece olarak)
        public SharedFloat fieldOfViewAngle = 90;
        // Ajanın görebileceği mesafe
        public SharedFloat viewDistance = 30;
        // Görüş çizgisi kontrolü yapılırken göz ardı edilecek nesnelerin maskelemesi
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        // Ses duyulursa arama sona ermeli mi kontrolü
        public SharedBool senseAudio = true;
        // Ajanın ne kadar uzağı işitebileceğini belirle
        public SharedFloat hearingRadius = 30;
        // Pivot konumuna göre raycast uzaklığı
        public SharedVector3 offset;
        // Pivot konumuna göre hedef raycast uzaklığı
        public SharedVector3 targetOffset;
        // Aradığımız nesnelerin maskelemesi
        public LayerMask objectLayerMask;
        // Kontrol
        public SharedBool useTargetBone;
        // Kimlik saptama
        public HumanBodyBones targetBone;
        // Ses kaynağı ne kadar uzaksa, ajanın onu duyma olasılığı o kadar düşük olur.
        // Ajanın duyabileceği minimum duyulabilirlik seviyesi için atama yap
        public SharedFloat audibilityThreshold = 0.05f;
        // Bulunan nesne
        public SharedGameObject returnedObject;

        private float pauseTime;
        private float destinationReachTime;

        // Bir nesne görünene veya duyulana kadar aramaya devam et (senseAudio etkin olmalı)
        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                // Temsilci, yalnızca maksimum duraklatma süresi 0'dan büyükse hedefte duraklamalı
                if (maxPauseDuration.Value > 0)
                {
                    if (destinationReachTime == -1)
                    {
                        destinationReachTime = Time.time;
                        pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                    }
                    if (destinationReachTime + pauseTime <= Time.time)
                    {
                        // Yalnızca bir hedef ayarlanmışsa zamanı sıfırla.
                        if (TrySetTarget())
                        {
                            destinationReachTime = -1;
                        }
                    }
                }
                else
                {
                    TrySetTarget();
                }
            }

            // Görünürde herhangi bir nesne olup olmadığını tespit et
            returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
            // If an object was seen then return success
            if (returnedObject.Value != null)
            {
                return TaskStatus.Success;
            }
            // Herhangi bir nesnenin ses aralığında olup olmadığını algılama (etkinleştirilmişse)
            if (senseAudio.Value)
            {
                returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
                // Bir nesne duyulduysa başarı döndür
                if (returnedObject.Value != null)
                {
                    return TaskStatus.Success;
                }
            }

            // Hiçbir nesne görülmedi veya duyulmadı, bu yüzden aramaya devam et
            return TaskStatus.Running;
        }

        private bool TrySetTarget()
        {
            var direction = transform.forward;
            var validDestination = false;
            var attempts = targetRetries.Value;
            var destination = transform.position;
            while (!validDestination && attempts > 0)
            {
                direction = direction + Random.insideUnitSphere * wanderRate.Value;
                destination = transform.position + direction.normalized * Random.Range(minWanderDistance.Value, maxWanderDistance.Value);
                validDestination = SamplePosition(destination);
                attempts--;
            }
            if (validDestination)
            {
                SetDestination(destination);
            }
            return validDestination;
        }

        // Genel değişkenleri sıfırla
        public override void OnReset()
        {
            base.OnReset();

            minWanderDistance = 20;
            maxWanderDistance = 20;
            wanderRate = 2;
            minPauseDuration = 0;
            maxPauseDuration = 0;
            targetRetries = 1;
            fieldOfViewAngle = 90;
            viewDistance = 30;
            senseAudio = true;
            hearingRadius = 30;
            audibilityThreshold = 0.05f;
        }
    }
}