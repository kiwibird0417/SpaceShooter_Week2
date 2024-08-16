using UnityEngine;


//08.14(Wed) 총알 벽 충돌하면, 사라지는 부분
public class RemoveBullet : MonoBehaviour
{

    public GameObject sparkEffect;



    void OnCollisionEnter(Collision coll)
    {
        // 충돌한 물체 파악(Tag)
        if (coll.collider.CompareTag("BULLET"))
        {
            // 충돌 정보 불러오기
            ContactPoint cp = coll.GetContact(0);

            // 충돌 좌표
            Vector3 _point = cp.point;

            // 법선 벡터(방향의 각도를 구해서 --> instantiate의 각도를 계산하자)
            Vector3 _normal = -cp.normal;

            // 법선 벡터가 가리키는 방향의 각도(쿼터니언)를 계산
            // 형식: Quaternion rot = Quaternion.LookRotation(벡터);
            Quaternion _rot = Quaternion.LookRotation(_normal);

            // 스파크 이펙트 생성
            GameObject _obj = Instantiate(sparkEffect, _point, _rot);
            Destroy(_obj, 0.5f);

            Destroy(coll.gameObject);
        }

        //deprecated!(X)
        /*
        // 충돌한 물체 파악(Tag)
        if (coll.collider.tag == "BULLET")      // 사용 금지(불필요한 메모리 할당)
        {
            //총알 삭제
            Destroy(coll.gameObject);
        }
        */
    }


    // 충돌 콜백 함수
    /*
        {조건}
        1. 양쪽 다 Collider 갖고 있다.
        2. 이동하는 게임 오브젝트에 Rigidbody 있어야 함.

        # IsTrigger 언체크
        OnCollisionEnter
        OnCollisionStay
        OnCollisionExit

        # IsTrigger 체크
        OnTriggerEnter
        OnTriggerStay
        OnTriggerExit
    */
}
