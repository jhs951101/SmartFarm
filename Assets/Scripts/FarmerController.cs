using UnityEngine;
using UnityEngine.UI;

public class FarmerController : MonoBehaviour
{
    [SerializeField]
    private Text commentText;

    private string[] comments;
    private int currentIndex;

    private float timeSpan;
    private float checkTime;

    void Start()
    {
        timeSpan = 0.0f;
        checkTime = 5.0f;

        comments = new string[4];
        comments[0] = "배추 10포기 세트 상품 판매합니다!";
        comments[1] = "오늘만 25% 할인해서 5만원!";
        comments[2] = "단돈 5만원에 배추 10포기 사가세요!";
        comments[3] = "네! 아주 좋습니다!";

        currentIndex = 0;
        commentText.text = comments[currentIndex];
    }

    void Update()
    {
        timeSpan += Time.deltaTime;

        if (timeSpan > checkTime)
        {
            currentIndex = (currentIndex + 1) % comments.Length;
            commentText.text = comments[currentIndex];

            timeSpan = 0;
        }
    }
}
