using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

namespace Objects.Scripts
{
    [Action("IsComputerTurn")]
    public class IsComputerTurn : BasePrimitiveAction
    {
        public override TaskStatus OnUpdate()
        {
            if (Settings.currentPlayer == Settings.ComputerName)
            {
                return TaskStatus.COMPLETED;
            }
            return TaskStatus.FAILED;
        }
    }
}