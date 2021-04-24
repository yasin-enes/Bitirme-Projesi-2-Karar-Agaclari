namespace BehaviorDesigner.Runtime.Tasks
{
    public class Basarisizliga_Kadar : Decorator
    {
        // Alt nesnenin çalışmasının bittikten sonraki durumu.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Alt görev hata döndürene kadar çalışmaya devam eder.
            return executionStatus == TaskStatus.Success || executionStatus == TaskStatus.Inactive;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Bir alt görev çalışmasını bitirdikten sonra yürütme durumunu güncellemeliyiz.
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            // Yürütme durumunu başlangıç değerlerine sıfırlarız.
            executionStatus = TaskStatus.Inactive;
        }
    }
}