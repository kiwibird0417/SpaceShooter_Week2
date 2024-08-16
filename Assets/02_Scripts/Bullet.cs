using UnityEngine;

public class Bullet : MonoBehaviour
{
    //1. 물리 엔진 적용하기
    private Rigidbody rb;


    void Start()
    {
        //Rigidbody 컴포넌트를 들고 온다.
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * 1200.0f); //AddRelativeForce(전진 방향 * 속도)
    }

    //로직이 없으면 꼭 Update 삭제!!!!!!!
}
