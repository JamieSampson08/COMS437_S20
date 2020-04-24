using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using UnityEngine;

namespace Objects.Scripts
{
    [Action("TriggerAnimation")]
    [Help("Given a character and a trigger name, set that trigger on the" +
          "character's animator to true.")]
    public class TriggerAnimation : BasePrimitiveAction
    {
            
        [InParam("character")]
        [Help("Object you are wanting to trigger the animation on")]
        public GameObject character;

        [InParam("Trigger Name")]
        [Help("Name of an animator trigger")]
        public string triggerName;

        private Animator _animator;
        
        public override TaskStatus OnUpdate()
        {
            _animator =  character.GetComponentInChildren<Animator>();
            ResetAllTriggers();
            
            _animator.SetTrigger(triggerName);
            return TaskStatus.COMPLETED;
        }
        
        void ResetAllTriggers()
        {
            foreach (AnimatorControllerParameter p in _animator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Trigger)
                {
                    _animator.ResetTrigger(p.name);
                }
            }
        }
    }
}