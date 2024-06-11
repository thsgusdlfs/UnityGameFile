using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isShootEnemy = true;

    //적기에 닿았을때 or 플레이어에 닿았을때
    //몇초뒤에 사라진다고 명령했을때
    //화면밖으로 나갔을때

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)//collision은 상대 콜리전
    {
        if (isShootEnemy == false && collision.tag == "Enemy")
        {
            Destroy(gameObject);//총알이 닿는순간 총알이 삭제
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(1);
        }
        if (isShootEnemy == true && collision.tag == "Player")
        {
            Destroy(gameObject);//총알이 닿는순간 총알이 삭제
            Player player = collision.GetComponent<Player>();
            player.hit();
        }
    }

    void Start()
    {
        //Destroy(gameObject, 2.5f); //Destory(삭제할오브젝트명, 시간값);
        //일정시간후 오브젝트를 삭제하는 코드00 시간값을 생략하면 즉시 오브젝트를 제거한다
    }

    void Update()
    {
        //transform.position += new Vector3(0, 1, 0)* moveSpeed * Time.deltaTime;
        //transform.position += transform.rotation * Vector3.up * moveSpeed * Time.deltaTime;
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    public void ShootPlayer()
    {
        isShootEnemy = false;
    }
}
