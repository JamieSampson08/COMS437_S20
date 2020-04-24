using Pada1.BBCore;
using Pada1.BBCore.Framework;

namespace Objects.Scripts
{
    [Condition("IsRequirementsMet")]
    [Help("Given a Setting's static variable, check to see it's true")]
    public class IsRequirementsMet : ConditionBase
    {
        public enum Requirements { WAITING_TO_LONG, LOST_DISCS, GAINED_DISCS };
        
        [InParam("Requirement")]
        [Help("What Setting's static variable are you wanting to check")]
        public Requirements givenRequirement;

        public override bool Check()
        {
            if (givenRequirement == Requirements.LOST_DISCS)
            {
                if (Settings.playerLosesDiscs)
                {
                    Settings.playerLosesDiscs = false;
                    return true;
                }
            }

            if (givenRequirement == Requirements.GAINED_DISCS)
            {
                if (Settings.playerGainsDiscs)
                {
                    Settings.playerGainsDiscs = false;
                    return true;
                }
            }

            if (givenRequirement == Requirements.WAITING_TO_LONG)
            {
                if (Settings.opponentWaiting)
                {
                    Settings.opponentWaiting = false;
                    return true;
                }
            }
            
            return false;
        }
    }
}