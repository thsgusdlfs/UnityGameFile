using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isShootEnemy = true;

    //���⿡ ������� or �÷��̾ �������
    //���ʵڿ� ������ٰ� ���������
    //ȭ������� ��������

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)//collision�� ��� �ݸ���
    {
        if (isShootEnemy == false && collision.tag == "Enemy")
        {
            Destroy(gameObject);//�Ѿ��� ��¼��� �Ѿ��� ����
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.Hit(1);
        }
        if (isShootEnemy == true && collision.tag == "Player")
        {
            Destroy(gameObject);//�Ѿ��� ��¼��� �Ѿ��� ����
            Player player = collision.GetComponent<Player>();
            player.hit();
        }
    }

    void Start()
    {
        //Destroy(gameObject, 2.5f); //Destory(�����ҿ�����Ʈ��, �ð���);
        //�����ð��� ������Ʈ�� �����ϴ� �ڵ�00 �ð����� �����ϸ� ��� ������Ʈ�� �����Ѵ�
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
