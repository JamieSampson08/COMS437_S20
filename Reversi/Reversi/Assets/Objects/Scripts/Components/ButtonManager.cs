using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void ButtonMoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
