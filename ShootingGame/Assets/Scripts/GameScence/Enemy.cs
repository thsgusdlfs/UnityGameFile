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
    //private �ڽĿ��� �ƹ������͵� ���� Ȥ�� Ȱ���Ҽ� �ֵ��� �������� ����
    //protected ����� �ڽĵ� Ȱ���� �� �ֵ��� ����
    #region ������Ʈ �����͵�
    [SerializeField, Tooltip("���� �̵��ӵ�")] protected float moveSpeed;
    [SerializeField, Tooltip("���� ü��")] protected float hp;
    protected GameObject fabExplosion;
    protected GameManager gameManager;
    protected SpriteRenderer spriteRenderer;
    protected bool isDied = false;//���Ⱑ �װ��� ���̻� ����� �ݺ� �������� �ʵ��� ����
    #endregion

    #region ��������Ʈ �����͵�
    Sprite defaultSprite;
    [SerializeField, Tooltip("���� �ǰ� ��������Ʈ")] Sprite hitSprite;


    bool haveItem = false;

    [Header("������ ������ �÷�")]
    [SerializeField] Color colorHaveItem;
    #endregion

    [Header("�ı��� ����")]
    [SerializeField, Tooltip("���麰 ���� ����")] protected int score;//�ڽ��� �ı��Ǿ����� ������ �÷��ٰ�����
    //�̷��� �ϸ� �ȵ�
    //��ü �ϳ��� Resources ������ �ѹ� �˻��ϱ� ������
    //100������ 100�� 1000������ 1000�� �˻��Ͽ� ���ɿ� �̽��� ����Ŵ
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
        //get set����� �̿��ϴ� ���
        fabExplosion = gameManager.FabExplosion;

        //fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion1");
        //�̷������� ���ҽ� �ȿ� �ִ� �����͸� �������� ��쿡�� Ȯ���ڸ� �Է��ϸ� �ȵ�
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
        //up ������ ����� 0 1 0
        //right ������ ����� 1 0 0
        //forward ������ ����� 0 0 1

        //vector���� �߰���
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
            Destroy(gameObject);//������ ����
            //�����¿����� �� �ڸ�
            GameObject go = Instantiate(fabExplosion, transform.position, Quaternion.identity, transform.parent);
            //�Ŵ����κ��� �޾ƿ� ���� ������ �� ��ġ�� �����ϰ� �θ�� ������� ���̾ �������
            Explosion goSc = go.GetComponent<Explosion>();

            //���簢��
            goSc.setImageSize(spriteRenderer.sprite.rect.width);//���� ��ü�� �̹��� ���̸� �־���

            //�Ŵ����� ȣ�� �� ���� �� ��ġ�� �����ϸ� �Ŵ����� �������� �� ��ġ�� �������
            if (haveItem == true)
            {
                gameManager.creatItem(transform.position);
            }

            gameManager.AddScore(score);
            gameManager.AddKillCount();
            //a b c ��ΰ� ü���� �ٸ���
            //gameManager.AddScore(score);
        }

        else
        {
            spriteRenderer.sprite = hitSprite;
            //�ణ�� �ð��� �����ڿ� � �Լ��� �����ϰ� ������
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
