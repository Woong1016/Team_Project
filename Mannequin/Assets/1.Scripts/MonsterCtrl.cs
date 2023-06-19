using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public Transform[] destinations; // �̵��� ���������� ���� �迭
    private int currentDestinationIndex = 0; // ���� ������ �ε���

    public float chaseDistance = 10f; // �÷��̾ ������ �Ÿ�
    public float returnDistance = 15f; // ���� ���͸� ������ �Ÿ�

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;

    private bool isChasing = false; // �÷��̾ �Ѿư��� ������ ����

    private Vector3 initialPosition; // ������ �ʱ� ��ġ
    private Quaternion initialRotation; // ������ �ʱ� ȸ��

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
            // �÷��̾ �Ѿư�
            agent.destination = playerTr.position;

            // �÷��̾���� �Ÿ��� ���� �Ÿ����� �־����� �߰� ����
            if (distanceToPlayer > returnDistance)
            {
                isChasing = false;
                agent.destination = destinations[currentDestinationIndex].position;
            }
        }
        else
        {
            // ������ ���� �̵�
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Length;
                agent.destination = destinations[currentDestinationIndex].position;
            }

            // �÷��̾���� �Ÿ��� �߰� �Ÿ� �̳��̸� �÷��̾ �Ѿư�
            if (distanceToPlayer < chaseDistance)
            {
                isChasing = true;
            }
        }
    }

    // ���� �ʱ�ȭ
    public void ResetMonster()
    {
        isChasing = false;
        agent.destination = destinations[currentDestinationIndex].position;
        monsterTr.position = initialPosition;
        monsterTr.rotation = initialRotation;
    }
}
