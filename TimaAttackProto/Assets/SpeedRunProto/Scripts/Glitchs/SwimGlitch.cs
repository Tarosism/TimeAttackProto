using UnityEngine;
using MoreMountains.CorgiEngine;

public class SwimGlitch : MonoBehaviour
{
    private Transform playerTransform;
    private CharacterHorizontalMovement playerMovement;
    private float flipInterval = 0.1f; // 뒤집는 간격 (초)
    private float nextFlipTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // "Player" 태그를 가진 오브젝트와 충돌한 경우
        {
            playerTransform = collision.transform;
            playerMovement = collision.gameObject.GetComponent<CharacterHorizontalMovement>();
            playerMovement.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // "Player" 태그를 가진 오브젝트가 트리거에서 벗어난 경우
        {
            playerTransform = null; // 효과 중지
            playerMovement.enabled = true;
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            playerTransform.position += new Vector3(0.3f, 0, 0); // 천천히 오른쪽으로 이동

            if (Time.time > nextFlipTime)
            {
                playerTransform.localScale = new Vector3(-playerTransform.localScale.x, playerTransform.localScale.y, playerTransform.localScale.z); // 좌우 뒤집기
                nextFlipTime = Time.time + flipInterval;
            }
        }
    }
}
