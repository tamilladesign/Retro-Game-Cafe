using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
{
    public GameObject menuPanel;
   

    public void OpenPanel()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void ReturnToGame()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
