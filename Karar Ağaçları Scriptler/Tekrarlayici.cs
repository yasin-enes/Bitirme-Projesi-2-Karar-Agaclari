namespace BehaviorDesigner.Runtime.Tasks
{
    public class Tekrarlayici : Decorator
    {
        public SharedInt count = 1;
        public SharedBool repeatForever;
        public SharedBool endOnFailure;

        // Alt görevin çalıştırılma sayısı.
        private int executionCount = 0;
        // Alt görevin çalışması bittikten sonraki durumu
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Sayıma ulaşana veya alt görev başarısızlıkla sonuçlanıncaya kadar yürütmeye devam edip bir hata aldığımızda durdurmalıyız.
            return (repeatForever.Value || executionCount < count.Value) && (!endOnFailure.Value || (endOnFailure.Value && executionStatus != TaskStatus.Failure));
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Alt görev yürütmeyi tamamladı. Yürütme sayısını artırıp ve yürütme durumunu güncellemeliyiz.
            executionCount++;
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            // Değişkenleri başlangıç değerlerine sıfırlarız.
            executionCount = 0;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnReset()
        {
            // Public tanımlanmış fonksiyonları orijinal değerlerine sıfırlarız.
            count = 0;
            endOnFailure = true;
        }
    }
}