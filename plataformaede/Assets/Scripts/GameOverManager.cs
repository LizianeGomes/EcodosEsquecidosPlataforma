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

        audioGameOver = gameObject.AddComponent<AudioSource>();
        audioGameOver.playOnAwake = false;
        audioGameOver.loop = false;
        audioGameOver.ignoreListenerPause = true;
    }

    public void MostrarGameOver()
    {
        gameOverPanel.SetActive(true);

        if (somGameOver != null)
        {
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

        // Reativa o tempo do jogo.
        Time.timeScale = 1f;

        // Fecha o painel de Game Over.
        gameOverPanel.SetActive(false);

        // Retorna o jogador ao último checkpoint.
        jogador.ReiniciarNoCheckpoint();

        Debug.Log("Jogador reiniciado no último checkpoint!");
    }

    public void IrParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void SairDoJogo()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}