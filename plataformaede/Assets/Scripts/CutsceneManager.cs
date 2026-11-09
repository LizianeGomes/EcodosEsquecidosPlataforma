using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Vídeo")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Próxima cena")]
    [SerializeField] private string proximaCena = "SampleScene";

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("Video Player não encontrado!");
            return;
        }

        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;

        videoPlayer.loopPointReached += AoTerminarVideo;

        videoPlayer.Play();
    }

    private void AoTerminarVideo(VideoPlayer vp)
    {
        SceneManager.LoadScene(proximaCena);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= AoTerminarVideo;
    }
}