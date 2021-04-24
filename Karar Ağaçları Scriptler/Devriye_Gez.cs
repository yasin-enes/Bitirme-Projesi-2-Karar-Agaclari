using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class Devriye_Gez : NavMeshMovement
    {
        // Ajan yol noktalarını rastgele devriye gezmeli mi kontrolü
        public SharedBool randomPatrol = false;
        // Ajanın bir ara noktaya vardığında duraklaması gereken süre
        public SharedFloat waypointPauseDuration = 0;
        // Hareket edilecek olan yol noktaları
        public SharedGameObjectList waypoints;

        // Geçiş noktaları dizisi içinde ilerlediğimiz geçerli indeks
        private int waypointIndex;
        private float waypointReachedTime;

        public override void OnStart()
        {
            base.OnStart();

            // başlangıçta en yakın ara noktaya doğru hareket et.
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Value.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance)
                {
                    distance = localDistance;
                    waypointIndex = i;
                }
            }
            waypointReachedTime = -1;
            SetDestination(Target());
        }

        // Ara nokta dizisinde belirtilen farklı ara noktaların etrafında devriye gez.  
        // Her zaman çalışan bir görev durumu döndürmeliyiz. Yoksa sonsuz çevrime giriyor.
        public override TaskStatus OnUpdate()
        {
            if (waypoints.Value.Count == 0)
            {
                return TaskStatus.Failure;
            }
            if (HasArrived())
            {
                if (waypointReachedTime == -1)
                {
                    waypointReachedTime = Time.time;
                }
                // ara noktaları değiştirmeden önce gerekli süreyi bekle.
                if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
                {
                    if (randomPatrol.Value)
                    {
                        if (waypoints.Value.Count == 1)
                        {
                            waypointIndex = 0;
                        }
                        else
                        {
                            // aynı yol noktasının seçilmesini önle.
                            var newWaypointIndex = waypointIndex;
                            while (newWaypointIndex == waypointIndex)
                            {
                                newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                            }
                            waypointIndex = newWaypointIndex;
                        }
                    }
                    else
                    {
                        waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                    }
                    SetDestination(Target());
                    waypointReachedTime = -1;
                }
            }

            return TaskStatus.Running;
        }

        // Geçerli ara nokta indeks konumunu döndür
        private Vector3 Target()
        {
            if (waypointIndex >= waypoints.Value.Count)
            {
                return transform.position;
            }
            return waypoints.Value[waypointIndex].transform.position;
        }

        // Genel değişkenleri sıfırla
        public override void OnReset()
        {
            base.OnReset();

            randomPatrol = false;
            waypointPauseDuration = 0;
            waypoints = null;
        }

        // 
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (waypoints == null || waypoints.Value == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i) {
                if (waypoints.Value[i] != null) {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
                    UnityEditor.Handles.SphereCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1);
#else
                    UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
#endif
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}