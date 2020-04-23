using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

namespace Objects.Scripts
{
    [Action("MakeComputerMove")]
    public class MakeComputerMove : BasePrimitiveAction
    {
        public override TaskStatus OnUpdate()
        {
            Settings.makeComputerMove = true;
            return TaskStatus.FAILED;
        }
    }
}