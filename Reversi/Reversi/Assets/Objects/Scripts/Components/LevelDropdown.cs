using System;
using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class LevelDropdown : MonoBehaviour
    {
        //Attach this script to a Dropdown GameObject
        TMP_Dropdown m_Dropdown;

        void Start()
        {
            //Fetch the DropDown component from the GameObject
            m_Dropdown = GetComponent<TMP_Dropdown>();

            //Add listener for when the state of the Dropdown changes, to take action
            m_Dropdown.onValueChanged.AddListener(delegate
            {
                string value = m_Dropdown.options[m_Dropdown.value].text;
                Settings.maxDepth = Int32.Parse(value.Split(' ')[1]);
                print("MaxDepth: " + Settings.maxDepth);
            });
        }
    }
}