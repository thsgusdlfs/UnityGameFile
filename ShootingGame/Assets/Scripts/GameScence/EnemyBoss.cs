using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float Hp => hp;
    //public float Hp
    //{ 
    //    get 
    //    { 
    //        return hp;
    //    }
    //}

    Transform trsBossPosition;//도착할 위치

    bool isMovingTrsBossPosition = false;//보스가 원위치까지 이동을 완료했는지
    bool patternChange = false;//패턴을 바꾸고 그동안 유저가 극딜할 타이밍을 만들어줌

    Vector3 createPos = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    float timer = 0.0f;

    bool isSwayRight = false;

    [Header("현재위치에서 전방으로")]
    [SerializeField] private int pattern1Count = 10;
    [SerializeField] private float pattern1Reload = 0.5f;
    [SerializeField] private GameObject pattern1Fab;

    [Header("샷건")]
    [Header("샷건")]
    [SerializeField] private int pattern2Count = 5;
    [SerializeField] private float pattern2Reload = 0.3f;
    [SerializeField] private GameObject pattern2Fab;

    [Header("조준사격")]
    [SerializeField] private int pattern3Count = 30;
    [SerializeField] private float pattern3Reload = 0.1f;
    [SerializeField] private GameObject pattern3Fab;

    Limiter limiter;

    private int curPattern = 1;
    private int curPatternShootCount = 0;

    [Header("발사위치")]
    [SerializeField] List<Transform> trsShootPos;//pulic으로 선언하거나 시리얼라이즈 필드로 선언하여
                                                 //인스펙터에서 변형해서 사용시에는 따로 동적할당을 받을 필요가 없다.
    Animator anim;

    //[System.Serializable]//시스템으로 직렬화가 가능하게 해준다.
    //public class cPattern//여러분들이 정의
    //{
    //    public string explain;
    //    public int pattern3Count;
    //    public float pattern3Reload;
    //    public GameObject patternFab;
    //}
    //[SerializeField] List<cPattern> listPattern;

    protected override void Start()
    {
        gameManager = GameManager.Instance;
        trsBossPosition = gameManager.TrsBossPosition;
        fabExplosion = gameManager.FabExplosion;
        createPos = transform.position;
        anim = GetComponent<Animator>();
    }

    protected override void moving()
    {
        //base.moving(); base는 부모의 데이터를 그대로 사용하는 경우에는 해당 코드를 사용
        //float posX = Mathf.SmoothDamp(transform.position.x, trsBossPosition.position.x, ref velocityX, 1f); //smoothDamp; 시간에 따른 이동
        //float posY = Mathf.SmoothDamp(transform.position.y, trsBossPosition.position.y, ref velocityY, 1f);
        //transform.position = new Vector3(posX, posY, 0f);
        if (isMovingTrsBossPosition == false)
        {
            if (timer < 1.0f)
            {
                timer += Time.deltaTime;
                //선형보간
                //transform.position = Vector3.Lerp(createPos, trsBossPosition.position, timer);

                float posX = Mathf.SmoothStep(createPos.x, trsBossPosition.position.x, timer);
                float posY = Mathf.SmoothStep(createPos.y, trsBossPosition.position.y, timer);
                transform.position = new Vector3(posX, posY);

                if (timer >= 1.0f)
                {
                    isMovingTrsBossPosition = true;
                    timer = 0f;
                }
            }
            return;
        }
        //이동 완료후 좌우로 이동하면서 패턴공격
        if (isSwayRight == true)
        {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }
        checkMovingLimit();
    }

    protected override void shooting()
    {
        if (isMovingTrsBossPosition == false)
        {
            return;
        }

        timer += Time.deltaTime;

        if (patternChange == true)
        {
            if (timer >= 1.0f)
            {
                timer = 0.0f;
                patternChange = false;
            }
            return;
        }

        if (curPattern == 1)//전방으로 발사
        {
            if (timer >= pattern1Reload)
            {
                shootStraght();
                timer = 0.0f;
                if (curPatternShootCount >= pattern1Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 2)
        {
            if (timer >= pattern2Reload)
            {
                shootShotgun();
                timer = 0.0f;
                if (curPatternShootCount >= pattern2Count)
                {
                    curPattern++;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
        else if (curPattern == 3)
        {
            if (timer >= pattern3Reload)
            {
                shootGatling();
                timer = 0.0f;
                if (curPatternShootCount >= pattern3Count)
                {
                    curPattern = 1;
                    patternChange = true;
                    curPatternShootCount = 0;
                }
            }
        }
    }

    private void shootShotgun()
    {
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180f - 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180f + 15f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 - 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 + 30f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 - 45f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 + 35f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 - 60f)), transform.parent);
        Instantiate(pattern2Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, 180 + 60f)), transform.parent);

        curPatternShootCount++;
    }

    private void shootStraght()
    {
        int count = 4;
        for (int iNum = 0; iNum < count; iNum++)
        {
            Instantiate(pattern1Fab, trsShootPos[iNum].position,
                 Quaternion.Euler(new Vector3(0, 0, 180)), transform.parent);
        }
        curPatternShootCount++;
    }

    private void shootGatling()
    {
        //플레이어의 위치로 발사
        Vector3 playerPos;//디폴트 x = 0, y = 0, z = 0;
        if (gameManager.GetPlyerPosition(out playerPos) == true)
        {
            Vector3 distance = playerPos - transform.position;//플레이어 위치로부터 내 위치의 거리

            float angle = Quaternion.FromToRotation(Vector3.up, distance).eulerAngles.z;
            //플레이어와 보스와의 거리 오차를 이용해 y축 0도로 부터 오차 위치의 각도 z를 구함
            Instantiate(pattern3Fab, trsShootPos[4].position,
                Quaternion.Euler(new Vector3(0, 0, angle)), transform.parent);
        }
        curPatternShootCount++;
    }

    private void checkMovingLimit()
    {
        if (limiter == null)
        {
            limiter = gameManager._Limiter;
        }
        float posX = transform.position.x;//중점

        if (limiter.checkMovePosition(transform.position) == true)
        {
            isSwayRight = !isSwayRight;
        }
    }



    public override void Hit(float _damage)
    {
        if (isDied == true)
        {
            return;
        }
        hp -= _damage;
        gameManager.modifyBossHp(hp);

        if (hp <= 0)
        {
            isDied = true;
            Destroy(gameObject);
            //터지는연출이 들어갈 자리
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            //매니저로부터 받아온 폭발 연출을 내 위치에 생성하고 부모로 사용중인 레이어에 만들어줌
            Explosion goSc = go.GetComponent<Explosion>();

            //직사각형
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//현재 기체의 이미지 길이를 넣어줌

            gameManager.creaItem(transform.position, Item.eItemType.PowerUP);
            gameManager.creaItem(transform.position, Item.eItemType.HpRecovery);
            gameManager.AddScore(score);
            //매니저를 호출 후 현재 내 위치를 전당하면 매니저가 아이템을 그 위치에 만들어줌
            //보스가 죽었다고 전달 // 다시 적들이 출동하도록 설계
            gameManager.KillBoss();
        }
        else
        {
            //이 친구는 스프라이트 뿐만이 아니라 애니메이션을 통해서 동작함
            anim.SetTrigger("BossHit");
        }
    }
}
