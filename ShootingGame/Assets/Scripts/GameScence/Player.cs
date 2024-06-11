using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    [Header("플레이어설정"), SerializeField, Tooltip("플레이어의 이동속도")] float moveSpeed;
    // Start is called before the first frame update

    Vector3 moveDir;

    [Header("총알")]
    [SerializeField, Tooltip("플레이어 총알데이터 Level1")] GameObject fabBullet;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField, Tooltip("플레이어 총알데이터 Level2")] GameObject fabBullet2;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField, Tooltip("플레이어 총알데이터 Level3")] GameObject fabBullet3;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField, Tooltip("플레이어 총알데이터 Level4")] GameObject fabBullet4;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField, Tooltip("플레이어 총알데이터 Level5")] GameObject fabBullet5;//플레이어가 복제해서 사용할 원본 총알
    [SerializeField, Tooltip("총알 오브젝트의 위치")] Transform dynamicObject;
    [SerializeField, Tooltip("자동사격 온오프")] bool autoFire = false;//자동발사 기능
    [SerializeField, Tooltip("총알의 발사 대기시간")] float fireRateTime = 0.5f;//해당 시간이 지나면 총알이 발사됨

    float fireTimer = 0;

    GameManager gameManager;
    GameObject fabExplosion;
    SpriteRenderer spriteRenderer;
    Limiter limiter;

    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;
    bool invincivilty;//무적상태
    [SerializeField]float invinciviltyTime = 1f;//무적시간
    float invinciviltyTimer;

    [Header("플레이어 레벨")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel = 1;

    [SerializeField] Transform shootTrs;
    [SerializeField] float distanceBullet;//2레벨 이상시 총알이 중심으로부터  벌어지는 거리//플레이어 전방에서 발사
    [SerializeField] float angleBullet;//4레벨 이상시 총알이 회전된 값;
    [SerializeField] Transform shootTrsLevel4; //4레벨시 총알이 발사될 위치
    [SerializeField] Transform shootTrsLevel5; //5레벨시 총알이 발사될 위치

    private void OnValidate()//인스펙터에서 어떤 값이 변동이 생기면 호출
    {
        if (Application.isPlaying == false)
        {
            return;
        }
        if (beforeHp != curHp)
        {
            beforeHp = curHp;
            GameManager.Instance.SetHp(maxHp, curHp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tool.GetTag(GameTags.Enemy))//태그를 글자로 적다보면 오타를 구분할 방법이 없다.
        {
            //체력 감소
            hit();
            //체력이 0이 되면 게임이 끝남
            //점수가 랭크인이 되면 이름 입력하는 기능
            //메인 메뉴에서 1~10등 랭크 출력

            //짧은시간 무적되는 기능

            //게이지 변화코드 실행
        }
        else if (collision.tag == Tool.GetTag(GameTags.Item))
        {
            Item item = collision.GetComponent<Item>();
            Destroy(item.gameObject);//이 함수는 이 함수가 모든 동작을 마치게 되면 삭제해달라고 예약하는 기능
            if (item.GetItemType() == Item.eItemType.PowerUP)
            {
                curLevel++;
                if (curLevel > maxLevel)
                {
                    curLevel = maxLevel;
                }
                //발사체가 추가되도록 설계
            }
            else if (item.GetItemType() == Item.eItemType.HpRecovery)
            {
                //체력 회복
                curHp++;
                if (curHp > maxHp)
                {
                    curHp = maxHp;
                }
                gameManager.SetHp(maxHp, curHp);
            }
        }
    }

    //Awake전에 코드를 한번 동작하게 하는 기능
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static void initCode() 
    //{
    //    Debug.Log("initCode");
    //}

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.transform.GetComponent<Animator>();
        curHp = maxHp;
        curLevel = minLevel;
    }

    private void Start()
    {
        //cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameManager = GameManager.Instance;
        fabExplosion = gameManager.FabExplosion;
        gameManager._Player = this;
    }

    // Update is called once per frame
    void Update()
    {
        moving();
        doAnimation();
        checkPlayerPos();

        shoot();
        checkinvincivilty();
    }

    private void checkinvincivilty()//무적일때만 동작하며 일정시간이 지나고 나면 다시 무적을 풀어줌
    {
        if (invincivilty == false) return;

        if (invinciviltyTimer > 0f)
        { 
            invinciviltyTimer -= Time.deltaTime;
            if (invinciviltyTimer < 0f)
            {
                setSprInvincivilty(false);
            }
        }
    }

    private void setSprInvincivilty(bool _value)
    {
        Color color = spriteRenderer.color;
        if (_value == true)//무적이 된것처럼 투명도를 줄여 유저에게 무적이라 알려줌
        {
            color.a = 0.5f;
            invincivilty = true;
            invinciviltyTimer = invinciviltyTime;
        }
        else
        {
            color.a = 1.0f;
            invincivilty = false;
            invinciviltyTimer = 0f;
        }
        spriteRenderer.color = color;
    }


    /// <summary>
    /// 플레이어 기체의 기동을 정의합니다.0
    /// </summary>
    private void moving()
    {
        moveDir.y = Input.GetAxisRaw("Vertical");//위 혹은 아래 입력
        moveDir.x = Input.GetAxisRaw("Horizontal"); //오른쪽 혹은 왼쪽 입력

        transform.position += moveDir * Time.deltaTime * moveSpeed;
        //transform.postion => 월드 포지션 좌표
        //transform.localPosition => 이 데이터가 Root 데이터라면 알아서 월드 포지션 좌표를 출력
        //                           이 데이터가 자식 데이터라면 부모로부터의 거리를 포지션 좌표로 출력
    }

    /// <summary>
    /// 어떤 애니메이션을 실행할지 파라미터를 전달 합니다.
    /// </summary>
    private void doAnimation()//하나의 함수에는 하나의 기능만 넣어주는것이 좋음
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }
    private void checkPlayerPos()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
        transform.position = limiter.checkMovePosition(transform.position, false);
    }

    private void shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            createBullet();
        }
        else if (autoFire == true)
        {
            //일정시간이 지나면 총알 한발 발사
            fireTimer += Time.deltaTime;//1초가 지나면 1이 될 수 있도록 소수점들이 fireTimer에 쌓임
            if (fireTimer > fireRateTime)
            {
                createBullet();
                fireTimer = 0;
            }
        }
    }

    private void createBullet()//총알을 생성한다
    {
        if (curLevel == 1)
        {
            GameObject go = Instantiate(fabBullet,shootTrs.position, Quaternion.identity, dynamicObject);
            Bullet goSc = go.GetComponent<Bullet>();
            goSc.ShootPlayer();
            //instBullet(shootTrs.position, Quaternion.identity);
        }
        if (curLevel == 2)
        {
            Instantiate(fabBullet2, shootTrs.position, Quaternion.identity, dynamicObject);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        if (curLevel == 3)
        {
            Instantiate(fabBullet3, shootTrs.position, Quaternion.identity, dynamicObject);
            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
        }
        if (curLevel == 4)
        {
            Instantiate(fabBullet4, shootTrs.position, Quaternion.identity, dynamicObject);
            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);

            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4LocalPos = shootTrsLevel4.localPosition;
            //lv4LocalPos.x *= -1;
            //lv4LocalPos += transform.position;
            //instBullet(lv4LocalPos, new Vector3(0, 0, -angleBullet));
        }
        else if (curLevel == 5)
        {
            Instantiate(fabBullet5, shootTrs.position, Quaternion.identity, dynamicObject);
            //instBullet(shootTrs.position, Quaternion.identity);
            //instBullet(shootTrs.position + new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //instBullet(shootTrs.position - new Vector3(distanceBullet, 0, 0), Quaternion.identity);
            //Vector3 lv4Pos = shootTrsLevel4.position;
            //instBullet(lv4Pos, new Vector3(0, 0, angleBullet));

            //Vector3 lv4LocalPos = shootTrsLevel4.localPosition;
            //lv4LocalPos.x *= -1;
            //lv4LocalPos += transform.position;
            //instBullet(lv4LocalPos, new Vector3(0, 0, -angleBullet));

            //Vector3 lv5Pos = shootTrsLevel5.position;
            //instBullet(lv5Pos, new Vector3(0, 0, angleBullet));
            //Vector3 lv5LocalPos = shootTrsLevel5.localPosition;

            //lv5LocalPos.x *= -1;
            //lv5LocalPos += transform.position;
            //instBullet(lv5LocalPos, new Vector3(0, 0, -angleBullet));
        }
    }

    private void instBullet(Vector3 _pos, Vector3 _angle)
    {
        GameObject go = Instantiate(fabBullet, _pos, Quaternion.Euler(_angle), dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }
    private void instBullet(Vector3 _pos, Quaternion _quat)
    {
        GameObject go = Instantiate(fabBullet, _pos, _quat, dynamicObject);
        Bullet goSc = go.GetComponent<Bullet>();
        goSc.ShootPlayer();
    }

    public void hit()
    {
        //무적상태라면 데미지를 받지 않음
        if (invincivilty == true) return;

        setSprInvincivilty(true);

        curHp--;
        if (curHp < 0)
        {
            curHp = 0;
        }
        GameManager.Instance.SetHp(maxHp, curHp);
        curLevel--;
        if (curLevel < minLevel)
        {
            curLevel = minLevel;
        }

        if (curHp == 0)
        {
            Destroy(gameObject);
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            Explosion goSc = go.GetComponent<Explosion>();
            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            gameManager.GameOver();
        }
    }
}
