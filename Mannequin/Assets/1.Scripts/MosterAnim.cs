using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosterAnim : MonoBehaviour
{
   public Animator animator;
    public float detectionDistance = 2f;
    public Transform playerTransform;

    private int animationState = 0; // 걷는 애니메이션: 0, 공격 애니메이션: 1

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 플레이어와의 거리를 계산
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionDistance)
        {
            // 플레이어가 일정 거리 이내에 있을 때, 공격 애니메이션으로 전환
            if (animationState != 1)
            {
                animationState = 1;
                animator.SetInteger("AnimationState", animationState);
            }
        }
        else
        {
            // 플레이어가 일정 거리 이내에 없을 때, 걷는 애니메이션으로 전환
            if (animationState != 0)
            {
                animationState = 0;
                animator.SetInteger("AnimationState", animationState);
            }
        }
    }
}
