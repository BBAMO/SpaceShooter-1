using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총소리에 사용할 오디오 음원
    public AudioClip fireSfx;

    // AudioSource 컴포넌트를 저장할 변수
    private new AudioSource audio;
    // Muzzle Flash의 MeshRenderer 컴포넌트
    private MeshRenderer muzzleFlash;
    void Start()
    {
        audio = GetComponent<AudioSource>();

        // FirePos 하위에 있는 MuzzleFlash의 Material 컴포넌트를 추출
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        // 처음 시작할 때 비활성화
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        void Fire()
        {
            // bullet 프리팹을 동적으로 생성
            Instantiate(bullet, firePos.position, firePos.rotation);

            // 총소리 발생
            audio.PlayOneShot(fireSfx, 1.0f);

            // 총구 화염 효과 코루틴 함수 호출
            StartCoroutine(ShowMuzzleFlash()); // StartCoroutine("ShowMuzzleFash"); 가능하나 GC 발생
        }

        IEnumerator ShowMuzzleFlash()
        {
            // MuzzleFlash 활성화
            muzzleFlash.enabled = true;

            // 0.2초 동안 대기(정지)하는 동안 메시지 루프로 제어권을 양보
            yield return new WaitForSeconds(0.2f);

            // MuzzleFlash 비활성화
            muzzleFlash.enabled = false;
        }
    }
}
