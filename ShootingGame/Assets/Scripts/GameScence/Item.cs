using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    /// <summary>
    /// 아이템에 대해서 enmu으로 정의
    /// </summary>
    public enum eItemType//자료형 정의 (같은 문자열을 적지만 않으면 문제를 일으키지 않는다.)
    {
        None,
        PowerUP,
        HpRecovery
    }

    [SerializeField] eItemType ItemType;

    float moveSpeed;//움직이는 속도
    Vector3 moveDirection;//움직일 방향

    [SerializeField] float minSpeed = 1;
    [SerializeField] float maxSpeed = 3;

    Limiter limiter;
    //float createTime = 0.0f;
    //float itemLifetime = 10.0f;
    private void Awake()
    {
        //int iBox = (int)0.001f;
        //float fBox = (float)100;
        //숫자를 숫자로 변환시 
        // string sBox = 100.ToString();
        //int iBox2 = int.Parse("100");
        //글자를 숫자로 혹은 숫자를 글자로 변경시에는 함수를 이용해야만 변경이 가능

        //int value = (int)eItemType.None;
        string sValue = eItemType.HpRecovery.ToString();//Hp

        //숫자를 enum 자료형으로 변경
        //eItemType eValue1 = (eItemType)1;
        //글자를 enum 자료형으로 변경
        eItemType eValue2 = (eItemType)System.Enum.Parse(typeof(eItemType), "PowerUP");

        moveSpeed = Random.Range(minSpeed, maxSpeed); //1~3까지 어떤 사이값의 속도
        moveDirection.x = Random.Range(-1.0f, 1.0f);
        moveDirection.y = Random.Range(-1.0f, 1.0f);

        moveDirection.Normalize();//벡터에서 힘을 버리고 방향만 지시
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
        //var rData = limiter.IsReflectItem(transform.position); //var변수의 값을 보려면 변수에 마우스를 올려서 확인하는것이 좋다

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
