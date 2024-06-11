using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    /// <summary>
    /// �����ۿ� ���ؼ� enmu���� ����
    /// </summary>
    public enum eItemType//�ڷ��� ���� (���� ���ڿ��� ������ ������ ������ ����Ű�� �ʴ´�.)
    {
        None,
        PowerUP,
        HpRecovery
    }

    [SerializeField] eItemType ItemType;

    float moveSpeed;//�����̴� �ӵ�
    Vector3 moveDirection;//������ ����

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    Limiter limiter;
    //float createTime = 0.0f;
    //float itemLifetime = 10.0f;
    private void Awake()
    {
        //int iBox = (int)0.001f;
        //float fBox = (float)100;
        //���ڸ� ���ڷ� ��ȯ�� 
        // string sBox = 100.ToString();
        //int iBox2 = int.Parse("100");
        //���ڸ� ���ڷ� Ȥ�� ���ڸ� ���ڷ� ����ÿ��� �Լ��� �̿��ؾ߸� ������ ����

        //int value = (int)eItemType.None;
        string sValue = eItemType.HpRecovery.ToString();//Hp

        //���ڸ� enum �ڷ������� ����
        //eItemType eValue1 = (eItemType)1;
        //���ڸ� enum �ڷ������� ����
        eItemType eValue2 = (eItemType)System.Enum.Parse(typeof(eItemType), "PowerUP");

        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3���� � ���̰��� �ӵ�
        moveDirection.x = Random.Range(-1.0f, 1.0f);
        moveDirection.y = Random.Range(-1.0f, 1.0f);

        moveDirection.Normalize();//���Ϳ��� ���� ������ ���⸸ ����
    }

    void Start()
    {
        limiter = GameManager.Instance._Limiter;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        checkItempos();
        //itemdisapper(createTime,itemLifetime);
    }

    private void checkItempos()
    {
        (bool _x, bool _y) rData = limiter.IsReflectItem(transform.position, moveDirection);
        //var rData = limiter.IsReflectItem(transform.position); //var������ ���� ������ ������ ���콺�� �÷��� Ȯ���ϴ°��� ����

        if (rData._x == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.right);
        }
        if (rData._y == true)
        {
            moveDirection = Vector3.Reflect(moveDirection, Vector3.up);
        }
    }

    public eItemType GetItemType()
    {
        return ItemType;
    }

    //private void itemdisapper(float _itemTime, float _lifeTime)
    //{
    //    createTime += Time.deltaTime;
    //    if (_itemTime >= _lifeTime)
    //    {
    //        Destroy(gameObject);
    //        itemLifetime = 0.0f;
    //    }
    //}
}
