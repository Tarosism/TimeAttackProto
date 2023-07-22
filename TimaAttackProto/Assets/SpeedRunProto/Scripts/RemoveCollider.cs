using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCollider : MonoBehaviour
{
    Animator animator;
    EdgeCollider2D edgeCollider2D;

    void Start()
    {
        animator = GetComponent<Animator>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    public void OpenDoor()
    {
        edgeCollider2D.enabled = false; // 문의 콜라이더를 비활성화
    }

    public void OnDoorOpened()
    {
        edgeCollider2D.enabled = true; // 문의 콜라이더를 활성화
    }

    public void OpenStart()
    {
        StartCoroutine(DoorIsOpend());
    }

    IEnumerator DoorIsOpend()
    {
        OnDoorOpened();
        Debug.Log("door is opening");
        yield return new WaitForSeconds(3f);
        OpenDoor();
        Debug.Log("door is opend");
    }
}
