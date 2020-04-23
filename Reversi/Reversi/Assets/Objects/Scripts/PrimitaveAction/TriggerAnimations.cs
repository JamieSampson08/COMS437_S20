using System;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using UnityEngine;

namespace Objects.Scripts
{
    [Action("TriggerOpponentAnimations")]
    public class TriggerAnimations : BasePrimitiveAction
    {
        [InParam("opponentAI")]
        public GameObject OpponentAI;

        public enum TriggerAction { AJ_ACTIONS };

        [InParam("trigger")]
        public TriggerAction Trigger;
        
        public override TaskStatus OnUpdate()
        {
            if (Trigger == TriggerAction.AJ_ACTIONS)
            {
                if (Settings.opponentWaiting)
                {
                    OpponentWaiting();
                    Settings.opponentWaiting = false;
                }
                if (Settings.playerGainsDiscs)
                {
                    PlayerGainsDiscs();
                    Settings.playerGainsDiscs = false;
                }
                if (Settings.playerLosesDiscs)
                {
                    PlayerLosesDiscs();
                    Settings.playerLosesDiscs = false;
                }
            }

            return TaskStatus.COMPLETED;
        } 

        private void PlayerLosesDiscs()
        {
            OpponentAI.GetComponentInChildren<Animator>().SetTrigger("lostDiscs");
        }

        private void PlayerGainsDiscs()
        {
            OpponentAI.GetComponentInChildren<Animator>().SetTrigger("gainDiscs");
        }

        private void OpponentWaiting()
        {
            OpponentAI.GetComponentInChildren<Animator>().SetTrigger("waitingTooLong");
        }
    }
}