using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum eEnemyType
    {
        EnemyA,
        EnemyB,
        EnemyC,
        EnemyBoss,
    }

    [SerializeField] protected eEnemyType enemyType;
    //private 자식에게 아무데이터도 전달 혹은 활용할수 있도록 제공하지 않음
    //protected 선언시 자식도 활용할 수 있도록 해줌
    #region 프로텍트 데이터들
    [SerializeField, Tooltip("적의 이동속도")] protected float moveSpeed;
    [SerializeField, Tooltip("적의 체력")] protected float hp;
    protected GameObject fabExplosion;
    protected GameManager gameManager;
    protected SpriteRenderer spriteRenderer;
    protected bool isDied = false;//적기가 죽고나면 더이상 기능을 반복 실행하지 않도록 해줌
    #endregion

    #region 프리베이트 데이터들
    Sprite defaultSprite;
    [SerializeField, Tooltip("적의 피격 스프라이트")] Sprite hitSprite;


    bool haveItem = false;

    [Header("아이템 보유시 컬러")]
    [SerializeField] Color colorHaveItem;
    #endregion

    [Header("파괴시 점수")]
    [SerializeField, Tooltip("적들별 점수 설정")] protected int score;//자신이 파괴되었을때 몇점을 올려줄것인지
    //이렇게 하면 안됨
    //기체 하나당 Resources 폴더를 한번 검색하기 때문에
    //100마리면 100번 1000마리면 1000번 검색하여 성능에 이슈를 일으킴
    //GameObject fabExplosion;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        defaultSprite = spriteRenderer.sprite;

        if (haveItem == true)
        {
            spriteRenderer.color = colorHaveItem;
        }

        gameManager = GameManager.Instance;
        //get set기능을 이용하는 방법
        fabExplosion = gameManager.FabExplosion;

        //fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion1");
        //이런식으로 리소스 안에 있는 데이터를 가져오는 경우에는 확장자를 입력하면 안됨
    }
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        moving();
        shooting();
    }
    protected virtual void moving()
    {
        //transform.position += transform.rotation * Vector3.down * moveSpeed * Time.deltaTime;
        //transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        transform.position -= transform.up * moveSpeed * Time.deltaTime;

        //transform, x y z
        //up 각도를 계산후 0 1 0
        //right 각도를 계산후 1 0 0
        //forward 각도를 계산후 0 0 1

        //vector에는 추가로
        //down 0 -1 0
        //left -1 0 0
        //back 0 0 -1
    }

    protected virtual void shooting()
    {

    }

    public virtual void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;
        }
        hp -= _damage;

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);//삭제를 예약
            //터지는연출이 들어갈 자리
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            //매니저를 호출 후 현재 내 위치를 전당하면 매니저가 아이템을 그 위치에 만들어줌
            if (haveItem == true)
            {
                gameManager.creatItem(transform.position);
            }

            gameManager.AddScore(score);
            gameManager.AddKillCount();
            //a b c 모두가 체력이 다르다
            //gameManager.AddScore(score);
        }

        else
        {
            spriteRenderer.sprite = hitSprite;
            //약간의 시간이 지난뒤에 어떤 함수를 실행하고 싶을때
            Invoke("setDefaultSprite", 0.04f);
        }
    }

    private void setDefaultSprite()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    public void setItem()
    {
        haveItem = true;
    }
}
