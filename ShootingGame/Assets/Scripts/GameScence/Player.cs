using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    [Header("�÷��̾��"), SerializeField, Tooltip("�÷��̾��� �̵��ӵ�")] float moveSpeed;
    // Start is called before the first frame update

    Vector3 moveDir;

    [Header("�Ѿ�")]
    [SerializeField, Tooltip("�÷��̾� �Ѿ˵����� Level1")] GameObject fabBullet;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField, Tooltip("�÷��̾� �Ѿ˵����� Level2")] GameObject fabBullet2;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField, Tooltip("�÷��̾� �Ѿ˵����� Level3")] GameObject fabBullet3;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField, Tooltip("�÷��̾� �Ѿ˵����� Level4")] GameObject fabBullet4;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField, Tooltip("�÷��̾� �Ѿ˵����� Level5")] GameObject fabBullet5;//�÷��̾ �����ؼ� ����� ���� �Ѿ�
    [SerializeField, Tooltip("�Ѿ� ������Ʈ�� ��ġ")] Transform dynamicObject;
    [SerializeField, Tooltip("�ڵ���� �¿���")] bool autoFire = false;//�ڵ��߻� ���
    [SerializeField, Tooltip("�Ѿ��� �߻� ���ð�")] float fireRateTime = 0.5f;//�ش� �ð��� ������ �Ѿ��� �߻��

    float fireTimer = 0;

    GameManager gameManager;
    GameObject fabExplosion;
    SpriteRenderer spriteRenderer;
    Limiter limiter;

    [SerializeField] int maxHp = 3;
    [SerializeField] int curHp;
    int beforeHp;
    bool invincivilty;//��������
    [SerializeField]float invinciviltyTime = 1f;//�����ð�
    float invinciviltyTimer;

    [Header("�÷��̾� ����")]
    [SerializeField] int minLevel = 1;
    [SerializeField] int maxLevel = 5;
    [SerializeField, Range(1, 5)] int curLevel = 1;

    [SerializeField] Transform shootTrs;
    [SerializeField] float distanceBullet;//2���� �̻�� �Ѿ��� �߽����κ���  �������� �Ÿ�//�÷��̾� ���濡�� �߻�
    [SerializeField] float angleBullet;//4���� �̻�� �Ѿ��� ȸ���� ��;
    [SerializeField] Transform shootTrsLevel4; //4������ �Ѿ��� �߻�� ��ġ
    [SerializeField] Transform shootTrsLevel5; //5������ �Ѿ��� �߻�� ��ġ

    private void OnValidate()//�ν����Ϳ��� � ���� ������ ����� ȣ��
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
        if (collision.tag == Tool.GetTag(GameTags.Enemy))//�±׸� ���ڷ� ���ٺ��� ��Ÿ�� ������ ����� ����.
        {
            //ü�� ����
            hit();
            //ü���� 0�� �Ǹ� ������ ����
            //������ ��ũ���� �Ǹ� �̸� �Է��ϴ� ���
            //���� �޴����� 1~10�� ��ũ ���

            //ª���ð� �����Ǵ� ���

            //������ ��ȭ�ڵ� ����
        }
        else if (collision.tag == Tool.GetTag(GameTags.Item))
        {
            Item item = collision.GetComponent<Item>();
            Destroy(item.gameObject);//�� �Լ��� �� �Լ��� ��� ������ ��ġ�� �Ǹ� �����ش޶�� �����ϴ� ���
            if (item.GetItemType() == Item.eItemType.PowerUP)
            {
                curLevel++;
                if (curLevel > maxLevel)
                {
                    curLevel = maxLevel;
                }
                //�߻�ü�� �߰��ǵ��� ����
            }
            else if (item.GetItemType() == Item.eItemType.HpRecovery)
            {
                //ü�� ȸ��
                curHp++;
                if (curHp > maxHp)
                {
                    curHp = maxHp;
                }
                gameManager.SetHp(maxHp, curHp);
            }
        }
    }

    //Awake���� �ڵ带 �ѹ� �����ϰ� �ϴ� ���
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

    private void checkinvincivilty()//�����϶��� �����ϸ� �����ð��� ������ ���� �ٽ� ������ Ǯ����
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
        if (_value == true)//������ �Ȱ�ó�� ������ �ٿ� �������� �����̶� �˷���
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
    /// �÷��̾� ��ü�� �⵿�� �����մϴ�.0
    /// </summary>
    private void moving()
    {
        moveDir.y = Input.GetAxisRaw("Vertical");//�� Ȥ�� �Ʒ� �Է�
        moveDir.x = Input.GetAxisRaw("Horizontal"); //������ Ȥ�� ���� �Է�

        transform.position += moveDir * Time.deltaTime * moveSpeed;
        //transform.postion => ���� ������ ��ǥ
        //transform.localPosition => �� �����Ͱ� Root �����Ͷ�� �˾Ƽ� ���� ������ ��ǥ�� ���
        //                           �� �����Ͱ� �ڽ� �����Ͷ�� �θ�κ����� �Ÿ��� ������ ��ǥ�� ���
    }

    /// <summary>
    /// � �ִϸ��̼��� �������� �Ķ���͸� ���� �մϴ�.
    /// </summary>
    private void doAnimation()//�ϳ��� �Լ����� �ϳ��� ��ɸ� �־��ִ°��� ����
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
            //�����ð��� ������ �Ѿ� �ѹ� �߻�
            fireTimer += Time.deltaTime;//1�ʰ� ������ 1�� �� �� �ֵ��� �Ҽ������� fireTimer�� ����
            if (fireTimer > fireRateTime)
            {
                createBullet();
                fireTimer = 0;
            }
        }
    }

    private void createBullet()//�Ѿ��� �����Ѵ�
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
        //�������¶�� �������� ���� ����
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
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            Explosion goSc = go.GetComponent<Explosion>();
            //���簢��
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//���� ��ü�� �̹��� ���̸� �־���

            gameManager.GameOver();
        }
    }
}
