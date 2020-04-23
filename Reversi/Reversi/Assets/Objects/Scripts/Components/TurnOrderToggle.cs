using UnityEngine;
using UnityEngine.UI;

namespace Objects.Scripts
{
    public class TurnOrderToggle : MonoBehaviour
    {
        //Attach this script to a Toogle GameObject
        Toggle m_Toggle;

        void Start()
        {
            //Fetch the Toggle GameObject
            m_Toggle = GetComponent<Toggle>();
            
            //Add listener for when the state of the Toggle changes, to take action
            m_Toggle.onValueChanged.AddListener(delegate {
                if (m_Toggle.isOn)
                {
                    Settings.turnOrder = 0;
                }
                else
                {
                    Settings.turnOrder = 1;
                }
                print("TurnOrder: " + Settings.turnOrder);
            });
        }
    }
}