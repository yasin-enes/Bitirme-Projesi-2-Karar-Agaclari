namespace BehaviorDesigner.Runtime.Tasks
{
    public class Basari_Dondur : Decorator
    {
        // Alt nesnenin çalışmasının bittikten sonraki durumu.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Alt görev başarılı veya başarısız olana kadar yürütmeye devam ederiz.
            return executionStatus == TaskStatus.Inactive || executionStatus == TaskStatus.Running;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Bir alt görev çalışmasını bitirdikten sonra yürütme durumunu güncellemeliyiz.
            executionStatus = childStatus;
        }

        public override TaskStatus Decorate(TaskStatus status)
        {
            // Alt görev başarısız olsa bile başarı döndür.
            if (status == TaskStatus.Failure)
            {
                return TaskStatus.Success;
            }
            return status;
        }

        public override void OnEnd()
        {
            // Yürütme durumunu başlangıç değerlerine sıfırlarız.
            executionStatus = TaskStatus.Inactive;
        }
    }
}