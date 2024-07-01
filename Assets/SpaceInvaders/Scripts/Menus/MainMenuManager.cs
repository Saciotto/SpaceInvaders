using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
