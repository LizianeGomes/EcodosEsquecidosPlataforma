using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool pausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado)
                Despausar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        pausado = true;
    }

    public void Despausar()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        pausado = false;
    }

    public void VoltarAoMenuPrincipal()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
}