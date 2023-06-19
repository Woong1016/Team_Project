using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public Transform[] destinations; // 이동할 목적지들을 담은 배열
    private int currentDestinationIndex = 0; // 현재 목적지 인덱스

    public float chaseDistance = 10f; // 플레이어를 감지할 거리
    public float returnDistance = 15f; // 동선 복귀를 시작할 거리

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;

    private bool isChasing = false; // 플레이어를 쫓아가는 중인지 여부

    private Vector3 initialPosition; // 몬스터의 초기 위치
    private Quaternion initialRotation; // 몬스터의 초기 회전

    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        initialPosition = monsterTr.position;
        initialRotation = monsterTr.rotation;

        agent.destination = destinations[currentDestinationIndex].position;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(monsterTr.position, playerTr.position);

        if (isChasing)
        {
            // 플레이어를 쫓아감
            agent.destination = playerTr.position;

            // 플레이어와의 거리가 일정 거리보다 멀어지면 추격 종료
            if (distanceToPlayer > returnDistance)
            {
                isChasing = false;
                agent.destination = destinations[currentDestinationIndex].position;
            }
        }
        else
        {
            // 동선을 따라 이동
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Length;
                agent.destination = destinations[currentDestinationIndex].position;
            }

            // 플레이어와의 거리가 추격 거리 이내이면 플레이어를 쫓아감
            if (distanceToPlayer < chaseDistance)
            {
                isChasing = true;
            }
        }
    }

    // 몬스터 초기화
    public void ResetMonster()
    {
        isChasing = false;
        agent.destination = destinations[currentDestinationIndex].position;
        monsterTr.position = initialPosition;
        monsterTr.rotation = initialRotation;
    }
}
