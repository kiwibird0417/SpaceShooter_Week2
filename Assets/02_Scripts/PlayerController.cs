#pragma warning disable CS0108

using System;
using System.Collections;
using UnityEngine;

//9-4. Random 들고오기
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{

    // 1-1. 전역변수 선언
    private float v;
    private float h;
    private float r;

    // 4. 속도를 전역변수로 선언!
    [SerializeField]
    private float moveSpeed = 8.0f;

    [SerializeField]
    private float turnSpeed = 200.0f;
    //============================================================================

    // 7. 총알 발사 관련
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;


    //============================================================================
    // 5. Animator 컴포넌트 저장할 변수 선언
    [NonSerialized]                 // == [HideInInspector]               
    public Animator animator;

    // 6. Animator Hash 추출
    // Hash 값이 int형으로 반환됨.
    private readonly int hashForward = Animator.StringToHash("forward");
    private readonly int hashStrafe = Animator.StringToHash("strafe");

    //============================================================================

    //08.14(수)
    // 8. 오디오
    [SerializeField] private AudioClip fireSfx;
    private AudioSource audio;
    //private new AudioSource audio1;              //메모리 할당 아니고, 그냥 키워드라고 알려주는 것.

    //============================================================================

    //08.16(금)
    // 9. 총구 효과 활성화시키기
    //MeshRenderer을 활성/비활성화시키면 편하다.
    //FirePos의 차일드로 그냥 코드로 들고오면 된다.
    public MeshRenderer muzzleFlash;


    //============================================================================
    void Start()
    {
        //3. FPS 확인해보기
        //Application.targetFrameRate = 60;       //60FPS 고정, 이건 변경 가능

        //5-2. 애니메이터는 Start에서 처리
        //animator = this.gameObject.GetComponent<Animator>();  //this.gameObject == Player, <> 안에 추출하고 싶은 컴포넌트 타입을 지정해주면 사용 가능.
        animator = GetComponent<Animator>();

        //8-1.audio 추가
        audio = GetComponent<AudioSource>();

        //9-1. MuzzelFlash의 MeshRenderer 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    //============================================================================
    // 매 프레임마다 호출
    // --> 너무 복잡한 함수를 쓰면, 오래 걸림, 가독성 떨어짐.
    void Update()
    {
        InputAxis();
        Locomotion();
        Animation();

        // 7-1. 총알 발사
        Fire();

    }


    private void Fire()
    {
        // 총알 프리팹을 이용해서 런타임에서 동적을 생성
        // 왼쪽 마우스를 눌렀을 때!(0), 오른쪽(1), 가운데 휠(2)
        if (Input.GetMouseButtonDown(0))
        {
            // Instantiate(생성할 객체, 위치, 각도)
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);      //10-1. .rotation의 타입이 Quaternion!

            //8-2. 총소리 발생
            audio.PlayOneShot(fireSfx, 0.8f);

            //9-2. 총구 효과 보여주기
            StartCoroutine(ShowMuzzleFlash());
        }
    }

    // 코루틴(Co-routine)
    IEnumerator ShowMuzzleFlash()
    {
        //9-2. 총구 효과 보여주기
        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;

        //9-4. Texture Offset 변경
        //총구 발사 이미지, 변경하기
        //(0,0) (0.5, 0) (0.5, 0.5) (0, 0.5)
        // ★★★Random.Range(0,2) = (0, 1) * 0.5★★★
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        //9-5. Scale 변경
        //muzzleFlash.transform.localScale = new Vector3(x, y, z);
        //안 좋다: 길어~ muzzleFlash.transform.localScale = new Vector3(1, 1, 1) * Random.Range();
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.0f, 3.0f);

        //10-2. Z축 회전
        muzzleFlash.transform.localRotation = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));


        // 9-3. 잠깐 기다렸다가 비활성화되도록!!(코루틴 이용)
        // Waiting ...
        // 메인 루프한테 프로세스양보
        // yield return [조건]
        yield return new WaitForSeconds(0.2f);


        // MuzzleFlash 비활성화
        muzzleFlash.enabled = false;
    }

    private void Animation()
    {
        //============================================================================
        // 5-1. 애니메이션 파라메터 전달
        // 밑의 방식대로, 선언 변수값으로 접근하면 속도가 느려짐.
        // 그래서 Animator Hash 추출 사용.
        //animator.SetFloat("forward", v);        //상하 이동 animator.SetFloat("선언 변수값 이름", v)
        //animator.SetFloat("strafe", h);         //좌우 이동

        animator.SetFloat(hashForward, v);        //상하 이동 animator.SetFloat("선언 변수값 이름", v)
        animator.SetFloat(hashStrafe, h);         //좌우 이동
    }

    private void Locomotion()
    {
        // 2. 이동 처리 로직 : Vector 덧셈 연산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //Debug.Log("비정규화" + moveDir.magnitude);                   //magnitude는 벡터 크기(값)를 가져오기
        //Debug.Log("정규화" + moveDir.normalized.magnitude);        //정규화 시킨 값! 와!

        transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);        //일정한 속도이동.  //★3-2. Time.deltaTime 추가! 꼭 필요하다(프레임워크에 따라 제공해주기 위해.)

        //============================================================================
        // 4. 회전 처리 로직
        transform.Rotate(Vector3.up * Time.deltaTime * r * turnSpeed);      //transform.Rotate(y축(기준) * Time.deltaTime(일정하게) * 마우스 각도 * turnSpeed(설정한 속도));
    }

    private void InputAxis()
    {
        // 1. 입력 키보드에 따라, 함수 실행
        // 축(Axis) 값을 받아옴. (-1.0f ~0.0 ~ +1.0f)
        v = Input.GetAxis("Vertical");          //Project Settings의 Input Manager에 있는 키워드에 해당하는 걸 받아오는 것!!! 굉장하네.
        h = Input.GetAxis("Horizontal");
        r = Input.GetAxis("Mouse X");           //마우스 회전 각도
    }
}

// 10. Quaternion 쿼터니언(사 원수)
/*
    Quaternion 쿼터니언(사 원수) x, y, z, w
    복소수 사차원 벡터

    오일러 회전 (오일러각 Euler) 0 ~ 360
    x -> y -> z

    짐벌락(Gimbal Lock) 발생
    -> 방지하기 위해서, Quaternion 사용.

    Quaternion.Euler(30, 45, -15)
    Quaternion.LookRotation(벡터)       //벡터도 3차원 좌표계값을 가진다~
    Quaternion.identity                 //원래 오브젝트가 갖고 있던 각도 들고 오기
*/

/*
    //★외워야 할 키워드
    //01. x,y,z축 중심만 사용.
    //(-)값을 곱하면 반대방향 표현이 가능하다.
    Vector3.forward == Vector3(0, 0, 1)
    Vector3.up      == Vector3(0, 1, 0)
    Vector3.right   == Vector3(1, 0, 0)

    Vector3.one     == Vector3(1, 1, 1)
    Vector3.zero    == Vector3(0, 0, 0)
*/


//11. NPC(적) 인공지능 만들기
/*
    유한 상태 머신(Finite State Machine : FSM)
--> 생성되고서, 주기 이후에, 사망이 있다.
(NPC가 많아질수록, 연결성이 많아져서 문제점이 될 수도 있음.)




*/

