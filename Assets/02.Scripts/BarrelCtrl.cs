using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    const int MAX_HIT = 3;
    const float DELETE_TIME_EFFECT = 5.0f;
    const float DELETE_TIME_BARREL = 3.0f;
    const float FORCE_BARREL = 1500.0f;
    const float MASS_BARREL = 1.0f;
    private Collider[] colls = new Collider[10];

    // 무작위로 적용할 텍스처 배열
    public Texture[] textures;

    // 폭발 효과 파티클을 연결할 변수
    public GameObject expEffect;

    // 폭발 반경
    public float radius = 10.0f;

    // 하위에 있는 Mesh Renderer 컴포넌트를 저장할 변수
    private new MeshRenderer renderer;
    
    // 컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;

    // 총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0; 

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>(); 
        
        // 하위에 있는 MeshRenderer 컴포넌트 추출
        renderer = GetComponentInChildren<MeshRenderer>();

        // 난수 발생
        int idx = Random.Range(0, textures.Length);
        // 텍스처 지정
        renderer.material.mainTexture = textures[idx];
    }

    // 충돌 시 발생하는 콜백 함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            // hitCount++
            // if (hitCount == MAX_HIT)
            if (++hitCount ==3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, transform.position, Quaternion.identity);
        // 폭발 효과 파티클 5초 후에 제거
        Destroy(exp, 5.0f);

        // Rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        // rb.mass = 1.0f;
        // 위로 솟구치는 힘을 가함
        // rb.AddForce(Vector3.up * 1500.0f);

        // 간접 폭발력 전달
        IndirectDamage(tr.position);

        // 3초 후에 드럼통 제거
        Destroy(gameObject, DELETE_TIME_BARREL);
    }

    void IndirectDamage(Vector3 pos)
    {
        // 주변에 있는 드럼통을 모두 호출
        // Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);
        // GC 없이 사용하는 것으로 변경
        Physics.OverlapSphereNonAlloc(pos, radius, colls, 1 << 3);
        foreach(var coll in colls)
        {
            // coll 의 null 체크 : null 일 때는 아래 코드 건너 뜀
            if (coll != null)
            {
            // 폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            rb = coll.GetComponent<Rigidbody>();
            // 드럼통의 무게를 가볍게 함
            rb.mass = MASS_BARREL;
            // freezeRotation 제한값을 해제
            rb.constraints = RigidbodyConstraints.None;
            // 폭발력을 전달
            rb.AddExplosionForce(FORCE_BARREL, pos, radius, 1200.0f);
            }
            
        }
    }
}
