
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public GameObject gameOverPanel;

    [Header("Som de Game Over")]
    public AudioClip somGameOver;

    [Range(0f, 1f)]
    public float volumeGameOver = 0.8f;

    private AudioSource audioGameOver;

    void Awake()
    {
        Instance = this;

        // Primeiro cria o AudioSource.
        audioGameOver = gameObject.AddComponent<AudioSource>();

        // Depois configura suas propriedades.
        audioGameOver.playOnAwake = false;
        audioGameOver.loop = false;
        audioGameOver.ignoreListenerPause = true;
        audioGameOver.spatialBlend = 0f;
    }

    public void MostrarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogError("GameOverPanel não está associado no Inspector!");

        if (somGameOver != null)
        {
            audioGameOver.Stop();
            audioGameOver.PlayOneShot(
                somGameOver,
                volumeGameOver
            );
        }

        Time.timeScale = 0f;
    }

    public void Reiniciar()
    {
        PlayerMovement jogador =
            FindFirstObjectByType<PlayerMovement>();

        if (jogador == null)
        {
            Debug.LogError("PlayerMovement não foi encontrado!");
            return;
        }

        // Interrompe o som de Game Over.
        audioGameOver.Stop();

        // Reativa o tempo do jogo.
        Time.timeScale = 1f;

        // Fecha o painel.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Retorna o jogador ao último checkpoint.
        jogador.ReiniciarNoCheckpoint();

        Debug.Log("Jogador reiniciado no último checkpoint!");
    }

    public void IrParaMenu()
    {
        audioGameOver.Stop();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void SairDoJogo()
    {
        audioGameOver.Stop();

        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
