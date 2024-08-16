using UnityEngine;
using Random = UnityEngine.Random;


//08.14(수) 데미지 로직 구현
public class Barrel : MonoBehaviour
{
    //1. 히트 횟수에 따라 조정.
    private int hitCount = 0;

    //2. 폭발효과
    public GameObject expEffect;

    //3. 텍스처 적용하기
    // 배열 타입으로 적용하여, 난수 적용.
    [SerializeField] private Texture[] textures;

    //3-2. meshRenderer 필요
    private new MeshRenderer renderer;

    //3-1. 텍스처 불러오기
    void Start()
    {
        // 3-2. 차일드에 있는 MeshRenderer 컴포넌트 추출
        renderer = GetComponentInChildren<MeshRenderer>();

        // 3-1. 텍스처를 선택하기 위한 난수 발생.
        // 배열 길이가 늘어날 수 있으므로, .Length 사용.
        int index = Random.Range(0, textures.Length);   // Random.Range(0,3) => 0, 1, 2

        //3-3.
        renderer.material.mainTexture = textures[index];


    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("BULLET"))
        {
            // 증감연산자를 이용해서 hitCount를 증가시키도록 하자~
            ++hitCount;
            if (hitCount >= 3)
            {
                // 폭발효과
                ExpBarrel();

                hitCount = 0;
            }
        }
    }

    //★★★★★난수★★★★★
    /*
        Random.Range(min, max)

        # 정수 Integer
        Random.Range(0, 10) => 0, 1, 2, ..., 9

        # 실수 Float
        Random.Range(0.0f, 10.0f) => 0.0f ~ 10.0f

    */


    private void ExpBarrel()
    {
        // 컴포넌트를 추가한다!
        var rb = this.gameObject.AddComponent<Rigidbody>();

        // 위치를 랜덤하게 만들도록 하자!~
        // 가상 공간 스피어 안에다가 위치 생성을 랜덤으로 한다는 것.
        Vector3 pos = Random.insideUnitSphere;      // 단위 구체의 랜덤값 리턴

        //폭발 (힘, 위치, 범위, 위방향으로 어디까지?)
        rb.AddExplosionForce(1500.0f, transform.position + pos, 10.0f, 1800.0f);

        //배럴 삭제
        Destroy(this.gameObject, 3.0f);

        //==================================================
        //충돌했을 때 폭발하도록 로직 구현
        // 폭발 이펙트 생성
        var obj = Instantiate(expEffect, transform.position, Quaternion.identity);        //Quaternion.identity == 원래 가지고 있는 각도 그대로 표현!
        Destroy(obj, 1.0f);
        //==================================================

    }
}
