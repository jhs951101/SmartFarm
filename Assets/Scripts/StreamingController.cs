using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamingController : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;

    private VideoPlayer streamingPlayer;

    void Awake()
    {
        streamingPlayer = GetComponent<VideoPlayer>();
    }

    IEnumerator PlayVideo()
    {
        streamingPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);

        while (!streamingPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }

        rawImage.texture = streamingPlayer.texture;
        streamingPlayer.Play();
    }

    public void Reset()
    {
        streamingPlayer.Stop();
        StopCoroutine("PlayVideo");
        StartCoroutine("PlayVideo");
    }
}
