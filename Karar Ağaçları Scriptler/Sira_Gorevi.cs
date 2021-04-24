namespace BehaviorDesigner.Runtime.Tasks
{
    public class Sira_Gorevi : Composite
    {
        // Çalışan veya çalıştırmak üzere olan alt nesnenin (child) indeksi.
        private int currentChildIndex = 0;
        // Son çalışan alt nesnenin (child) görev durumu.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override bool CanExecute()
        {
            // Alt nesneler başarısız olmadıkça ve henüz çalıştırılmamış alt nesneler varsa her karede çalıştırmaya devam edebiliriz.
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Failure;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // Bir alt öğe çalıştırıldıktan sonra alt indeksi artırırız ve yürütme durumunu güncelleriz.
            currentChildIndex++;
            executionStatus = childStatus;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Geçerli alt indeksi iptal etmeye neden olan indekse atama yaparız.
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnEnd()
        {
            //Bütün alt nesneler çalıştı dolayısıyla değişkenleri başlangıç değerlerine döndürmeliyiz .
            executionStatus = TaskStatus.Inactive;
            currentChildIndex = 0;
        }
    }
}