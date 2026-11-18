using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FinalCutscene : MonoBehaviour
{
    [Header("Referências")]
    public VideoPlayer videoPlayer;

    [Header("Cena do Menu")]
    public string nomeCenaMenu = "Menu";

    private bool videoFinalizado = false;

    private void Start()
    {
        videoPlayer.loopPointReached += AoTerminarVideo;
    }

    private void AoTerminarVideo(VideoPlayer vp)
    {
        if (videoFinalizado)
            return;

        videoFinalizado = true;

        SceneManager.LoadScene(nomeCenaMenu);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= AoTerminarVideo;
    }
}