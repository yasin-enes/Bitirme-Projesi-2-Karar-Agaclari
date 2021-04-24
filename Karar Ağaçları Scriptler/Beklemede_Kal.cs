using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class Beklemede_Kal : Action
    {
        // Bekleme süresi
        public SharedFloat waitTime = 1;
        // Bekleme rastgele olmalı mı kontrolü
        public SharedBool randomWait = false;
        //Rastgele bekleme etkinse minimum bekleme süresi
        public SharedFloat randomWaitMin = 1;
        // Rastgele bekleme etkinse maksimum bekleme süresi
        public SharedFloat randomWaitMax = 1;

        // Bekleme süresi
        private float waitDuration;
        // Görevin beklemeye başladığı zaman.
        private float startTime;
        // Görevin duraklatıldığı zamanı ata.
        // Böylece duraklatılan zaman bekleme süresine katkıda bulunmaz.
        private float pauseTime;

        public override void OnStart()
        {
            // Remember the start time.
            startTime = Time.time;
            if (randomWait.Value)
            {
                waitDuration = Random.Range(randomWaitMin.Value, randomWaitMax.Value);
            }
            else
            {
                waitDuration = waitTime.Value;
            }
        }

        public override TaskStatus OnUpdate()
        {
            // Görev, başlatıldığından beri waitDuration süresi geçtiyse beklet.
            if (startTime + waitDuration < Time.time)
            {
                return TaskStatus.Success;
            }
            // Aksi takdirde beklemeye devam et.
            return TaskStatus.Running;
        }

        public override void OnPause(bool paused)
        {
            if (paused)
            {
                // Davranışın duraklatıldığı zamanı ata
                pauseTime = Time.time;
            }
            else
            {
                // Yeni bir başlangıç zamanı bulmak için Time.time ve pauseTime arasındaki farkı ekle.
                startTime += (Time.time - pauseTime);
            }
        }

        public override void OnReset()
        {
            // Genel fonksiyonları orijinal değerlerine sıfırla.
            waitTime = 1;
            randomWait = false;
            randomWaitMin = 1;
            randomWaitMax = 1;
        }
    }
}