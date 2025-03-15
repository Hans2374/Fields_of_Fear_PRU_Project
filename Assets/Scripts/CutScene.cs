// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class CutScene : MonoBehaviour
// {
//     public GameObject frame1, frame2, frame3, frame4, frame5, frame6; // Các frame riêng biệt
//     AudioManager audioManager;
//     public float frameDuration = 6f; // Thời gian mỗi frame hiển thị

//     private void Awake()
//     {
//         audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
//     }

//     private void Start()
//     {
//         StartCoroutine(PlayCutscene());
//     }

//     private IEnumerator PlayCutscene()
//     {
//         yield return ShowFrame(frame1, () => audioManager.PlaySFX(audioManager.carMove));
//         yield return ShowFrame(frame2, () => audioManager.PlaySFX(audioManager.carFail));
//         yield return ShowFrame(frame3, () => audioManager.PlaySFX(audioManager.getHit));
//         yield return ShowFrame(frame4, () => audioManager.PlaySFX(audioManager.carIncrease));
//         yield return ShowFrame(frame5, () => audioManager.PlaySFX(audioManager.crops));
//         yield return ShowFrame(frame6, () => audioManager.PlaySFX(audioManager.crops));

//         // Sau khi cutscene kết thúc, chuyển scene
//         SceneManager.LoadSceneAsync(2);
//     }

//     private IEnumerator ShowFrame(GameObject frame, System.Action playSound)
//     {
//         audioManager.StopSFX(); // Dừng âm thanh trước đó

//         if (frame != null) frame.SetActive(true); // Hiển thị frame mới
//         playSound?.Invoke(); // Phát âm thanh mới

//         yield return new WaitForSeconds(frameDuration);
//     }

//     public void SkipCutscene()
//     {
//         StopAllCoroutines(); // Dừng cutscene
//         audioManager.StopSFX(); // Dừng âm thanh khi skip
//         SceneManager.LoadSceneAsync(2); // Chuyển ngay đến scene tiếp theo
//     }
// }

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public GameObject frame1, frame2, frame3, frame4, frame5, frame6; // Các frame riêng biệt
    private GameObject[] frames;
    AudioManager audioManager;
    public float frameDuration = 3f; // Thời gian mỗi frame hiển thị
    public float fadeDuration = 1f; // Thời gian fade-in

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        frames = new GameObject[] { frame1, frame2, frame3, frame4, frame5, frame6 };
    }

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        yield return ShowFrame(frame1, () => audioManager.PlaySFX(audioManager.carMove));
        yield return ShowFrame(frame2, () => audioManager.PlaySFX(audioManager.carFail));
        yield return ShowFrame(frame3, () => audioManager.PlaySFX(audioManager.getHit));
        yield return ShowFrame(frame4, () => audioManager.PlaySFX(audioManager.carIncrease));
        yield return ShowFrame(frame5, () => audioManager.PlaySFX(audioManager.crops));
        yield return ShowFrame(frame6, () => audioManager.PlaySFX(audioManager.crops));

        // Sau khi cutscene kết thúc, chuyển scene
        SceneManager.LoadSceneAsync(2);
    }

    private IEnumerator ShowFrame(GameObject frame, System.Action playSound)
    {
        audioManager.StopSFX(); // Dừng âm thanh trước đó

        if (frame != null)
        {
            // Đảm bảo frame có CanvasGroup
            CanvasGroup canvasGroup = frame.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = frame.AddComponent<CanvasGroup>();
            }
            
            frame.SetActive(true); // Hiển thị frame
            canvasGroup.alpha = 0; // Bắt đầu từ trong suốt

            // Fade-in hiệu ứng
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1;

            playSound?.Invoke(); // Phát âm thanh mới

            yield return new WaitForSeconds(frameDuration);
        }
    }

    public void SkipCutscene()
    {
        StopAllCoroutines(); // Dừng cutscene
        audioManager.StopSFX(); // Dừng âm thanh khi skip

        foreach (GameObject frame in frames)
        {
            if (frame != null)
            {
                frame.SetActive(false); // Tắt toàn bộ frame
            }
        }

        SceneManager.LoadSceneAsync(2); // Chuyển ngay đến scene tiếp theo
    }
}
