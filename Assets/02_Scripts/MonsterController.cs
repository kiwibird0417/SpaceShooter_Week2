using System.Collections;
using UnityEngine;

//4. 인공지능 추적 기능
using UnityEngine.AI;

//8.16(금)
public class MonsterController : MonoBehaviour
{
    //1. 상태 선언
    //열거형 변수, State를 인스펙터 창에서 리스트 형태로 볼 수가 있다.
    //매우 편리하고, 많이 사용한다.
    public enum State
    {
        IDLE, TRACE, ATTACK, DIE
    }

    //================================================================================
    //2. 거리에 따라 애니메이션 변경하기
    // 현재 몬스터의 상태
    public State state;

    // 추적 사정거리
    [SerializeField] private float traceDist = 10.0f;

    // 공격 사정거리
    [SerializeField] private float attackDist = 2.0f;


    private Transform playerTr; //플레이어의 transform값 받아오기 위한 변수
    private Transform monsterTr;

    //================================================================================
    //2-2. 몬스터 죽음 상태 판단
    public bool isDie = false;

    //================================================================================
    //4-1. 
    private NavMeshAgent agent;

    //================================================================================
    //5. 애니메이션 할당
    private Animator animator;
    private readonly int hashIsTrace = Animator.StringToHash("IsTrace");

    //================================================================================
    void Start()
    {
        //2-1. player, monster 오브젝트 불러오기
        //이름으로 검색해서 게임 오브젝트 불러오는 법.
        //★Find계열 함수는 주의하자.
        //GameObject playerObj = GameObject.Find("Player");   //순차적으로 다 검색, 시간 많이 걸림...
        //Start나 Awake에서만 실행, Update에서는 매 프레임마다 호출되어서 안돼!

        //TAG로 찾는 게 더 좋다.
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        playerTr = playerObj?.GetComponent<Transform>();        //? : NULL이 아닐 때만 실행해라!

        monsterTr = transform;      // == GetComponent<Transform>();

        //4-2.
        agent = GetComponent<NavMeshAgent>();

        //5-1. animator
        animator = GetComponent<Animator>();

        //2-3. 코루틴 사용
        StartCoroutine(CheckMonsterState());

        //3-1. 몬스터 행동 정의
        StartCoroutine(MonsterAction());
    }

    //================================================================================
    //2-2. 코루틴 사용
    IEnumerator CheckMonsterState()
    {
        while (isDie == false)
        {
            // 몬스터와 플레이어 간 거리 계산 : 상태 값을 측정
            float distance = Vector3.Distance(monsterTr.position, playerTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }

            yield return new WaitForSeconds(0.3f);
            //주의사항: 제어권인 yield는 while 안에 넣자.
        }
    }

    //3. 몬스터 행동 정의
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //아이들링 로직 처리
                    agent.isStopped = true;

                    //5-2. 애니메이터 설정
                    animator.SetBool(hashIsTrace, false);
                    break;

                case State.TRACE:
                    //추적 상태 로직 처리
                    //4-3. 플레이어 추적하기
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;        //추적, 이동 상태

                    //5-3. 애니메이터 설정
                    animator.SetBool(hashIsTrace, true);
                    break;

                case State.ATTACK:
                    Debug.Log("공격");
                    break;

                case State.DIE:
                    break;

            }

            yield return new WaitForSeconds(0.3f);
        }
    }



}
