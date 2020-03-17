using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab;  // 폭발 prefab
    public LayerMask levelMask;     // layermask 추가
    private bool exploded = false;  // 폭발 여부 확인

    // Start is called before the first frame update
    void Start()
    {
        // 3초 후 폭탄 폭발
        // 1 : 메서드 이름, 2: 시간
        Invoke("Explode", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 폭발 메서드
    void Explode(){
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);  // 1   spawns an explosion at the bomb's position

        // 폭발 범위 추가
        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));

        GetComponent<MeshRenderer>().enabled = false;   // 2    disables the mesh renderer, making the bomb invisible
        
        exploded = true;

        transform.Find("Collider").gameObject.SetActive(false); //3     disables the collider, allowing players to move through and walk into an explosion        
        Destroy(gameObject, .3f);       // 4  destorys the bomb after 0.3seconds
    }

    // 더 큰 폭발 생성
    private IEnumerator CreateExplosions(Vector3 direction){
        // 1 (2칸 방향으로 폭발을 늘려주도록 for문 실행)
        for (int i = 1; i < 3; i++){
            // 2 (raycasthit을 통해 맞았는지 안맞았는지의 위치를 확인해줌)
            RaycastHit hit; //??
            // 3 (layers에 추가해주고 blocks에 설정해준 blocks를 제외하고 폭발이 퍼져나가도록 설정함)
            Physics.Raycast(transform.position + new Vector3(0, .5f, 0), direction, out hit, i, levelMask);
            // 4
            if (!hit.collider){
                //5 //6 (폭발 생성)
                Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
            }else{
                // 7
                break;
            }
            // 쉽게 말하면 폭발이 2칸 방향인데, 만약 첫번째 칸에서 폭발을 못할 경우 그다음 칸도 못하도록 막은거임

            // 8
            yield return new WaitForSeconds(.05f);
        }
    }

    // 연쇄 폭발
    public void OnTriggerEnter(Collider other){
        // 1 & 2    (폭발 여부 확인)
        if (!exploded && other.CompareTag("Explosion")){            
            CancelInvoke("Explode");    // 2
            Explode();                  // 3
        }
    }
}
