using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//����ϴ� �̸��� ��������� �ش� ���� �ν��Ͻ��� �صθ� �̱������� ���� �� �� �ִ�.
    [Header("�����")]                            //���� ���۽ÿ� null�̱⿡ �����͸� ä������Ѵ�.
    [SerializeField, Tooltip("���� ����Ʈ")] List<GameObject> listEnemy;
    [SerializeField] GameObject fabBoss;

    [Header("�� ���� ����")]
    [SerializeField, Tooltip("���� ���� ����")] bool isSpawn = false;//������ �����ϰų� ���ϴ� ������ ������ �̰���
                                                               //true�� �����ϸ� ������ ������ �ϴ� ��
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;

    WaitForSeconds halfTime = new WaitForSeconds(0.5f);
    bool isSpawnBoss = false; //������ ���������� Ȯ���ϱ����� ����
    bool IsSpawnBoss
    {
        set
        {
            isSpawnBoss = value;
            StartCoroutine(sliderColorChange(value));
        }
    }

    IEnumerator sliderColorChange(bool _spawnBoss)
    {
        float timer = 0.0f;

        while (timer < 1.0f)//���ǹ��� ���̶�� �ݺ�
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                timer = 1.0f;
            }
            if (_spawnBoss == true)
            {
                imgSliderFill.color = Color.Lerp(sliderDefaultColor, sliderBossSpawnColor, timer);
            }
            else
            {
                imgSliderFill.color = Color.Lerp(sliderBossSpawnColor, sliderDefaultColor, timer);
            }
            yield return null;
        }

    }

    //true�� �����ϸ� ������ ���̻� ������ �ʰ��ϴ� �뵵�� Ȱ��

    [Header("�� ���� �ð�")]
    float enemySpwanTimer = 0.0f;
    [SerializeField, Range(0.1f, 5f)] float spwanTime = 1.0f;

    [Header("�� ���� ��ġ")]
    [SerializeField] Transform trsSpwanPosition;
    [SerializeField] Transform trsDynamicObject;

    [Header("��Ӿ�����")]
    [SerializeField] List<GameObject> listItem;

    [Header("���Ȯ��")]
    [SerializeField, Range(0.0f, 100.0f)] float itemDropRate;

    [Header("ü�� ������")]
    [SerializeField] FunctionHp functionHP;
    [SerializeField] Slider slider;
    [SerializeField] Image imgSliderFill;


    [Header("���� ������")]
    [SerializeField] Transform trsBossPosition;

    public Transform TrsBossPosition => trsBossPosition;

    Limiter limiter;

    [Header("���� ��������")]
    [SerializeField] int killCount = 100;
    int curKillCount = 0;
    [SerializeField] TMP_Text textSlider;

    [SerializeField] float bossSpawnTime = 0.0f;
    [SerializeField] float bossSpawnTimer = 0.0f;

    [Header("����")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("���ӽ��� �ؽ�Ʈ")]
    [SerializeField] TMP_Text gameStartText;

    [Header("���ӿ����޴�")]
    [SerializeField] GameObject objgameOverMenu;
    [SerializeField] TMP_Text textGameOverMenuScore;
    [SerializeField] TMP_Text textGameOverMenuRank;
    [SerializeField] TMP_Text textGameOverMenuBtn;
    [SerializeField] TMP_InputField IFGameOverMenuRank;
    [SerializeField] Button btnGameOverMenu;
    public Limiter _Limiter
    {
        get { return limiter; }
        set { limiter = value; }
    }

    Player player;

    public Player _Player
    {
        get { return player; }
        set { player = value; }
    }

    public bool GetPlyerPosition(out Vector3 _pos)
    {
        _pos = default;
        if (player == null)
        {
            return false;
        }
        else
        {
            _pos = player.transform.position;
            return true;
        }
    }

    GameObject fabExplosion;//�Ŵ����� ������ ���۷����� ����������//private, �������� ���� ���۷��� �����͸� ������ ����
    //�Լ��� ����� ��� ���������� �����͸� ������ ���� ���� �����ʹ� ������ ���� ����
    //���� �����͸� ������ �ִ� ������ private�� �����ϰ� ������ ���� Ȥ�� �����;��Ҷ��� �Լ��μ� ��밡��
    public GameObject FabExplosion
    {
        get
        {
            return fabExplosion;
        }
        set { fabExplosion = value; }
    }

    ////�ν������� ���� ������ ������ �� �Լ��� ���� ȣ��
    //OnValidate()�� ������ ������� �ʾƵ� �����Ѵ�.
    //private void OnValidate()
    //{
    //    if (Application.isPlaying == false) return; // ������ �������� �ƴ϶�� �Ѿ���� �ϴ� �ڵ�
    //    Debug.Log("���� �����");

    //    if (spwanTime <= 0.1)
    //    { 

    //    }
    //}

    private void Awake()
    {
        if (Tool.isStartingMainScene == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        //1���� �����ؾ� �Ѵ�

        if (Instance == null)
        {
            Instance = this;
        }
        else//�ν��Ͻ��� �̹� �����Ѵٸ�
        {
            //Destroy(this);//�̷��� ������Ʈ�� �ƴ� ������Ʈ�� ������
            Destroy(gameObject);//������Ʈ�� �������鼭 ��ũ��Ʈ�� ���� ������
        }
        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");

        initSlider();
    }
    private void initSlider()
    {
        //ųī��Ʈ ����
        //slider.minValue = 0;
        //slider.maxValue = killCount;
        //slider.value = 0;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //Ÿ�̸� ����
        slider.minValue = 0;
        slider.maxValue = bossSpawnTime;
        slider.value = 0;
        modifySlider();
    }

    void Start()
    {
        StartCoroutine(doStartText());
    }

    IEnumerator doStartText()
    {
        Color color = gameStartText.color;
        color.a = 0f;
        gameStartText.color = color;

        while (true)
        {
            color = gameStartText.color;
            color.a += Time.deltaTime;
            if (color.a > 1.0f)
            {
                color.a = 1.0f;
            }
            gameStartText.color = color;

            if (color.a == 1.0f)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            color = gameStartText.color;
            color.a -= Time.deltaTime;
            if (color.a < 0.0f)
            {
                color.a = 0.0f;
            }
            gameStartText.color = color;

            if (color.a == 0.0f)
            {
                break;
            }
            yield return null;
        }
        Destroy(gameStartText.gameObject);

        gameStart = true;
        isSpawn = true;
    }

    void Update()
    {
        if (gameStart == false) return;
        createEnemy();
        checkTimer();
    }

    private void checkTimer()
    {
        if (isSpawnBoss == false)
        {
            bossSpawnTimer += Time.deltaTime;
            modifySlider();
            if (bossSpawnTimer >= bossSpawnTime)
            {
                checkSpawnBoss();
            }
        }
    }

    private void createEnemy()
    {
        if (isSpawn == false) return;

        enemySpwanTimer += Time.deltaTime;
        if (enemySpwanTimer > spwanTime)
        {
            //���� ����
            int count = listEnemy.Count;//���� ���� 0~2
            int iRand = Random.Range(0, count);//0,3
            //float �����͸� �̿��Ͽ� ������ ���ÿ��� 0.0 ~ 1.0 ���� ������ ���� ���� ����
            //int �����͸� �̿��ؼ� ������ ����ϸ� 0 ~ 1 �̶�� �Է½� �ƽ������� -1�� �����͸� ������ ����
            //int min max�� ����϶� max-1�� ����� ��, �����϶��� max+1

            Vector3 defaultPos = trsSpwanPosition.position; //y=7;
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x =>-2.4 ~ 2.4
            defaultPos.x = x;

            GameObject go = Instantiate(listEnemy[iRand], defaultPos, Quaternion.identity, trsDynamicObject);
            //����Ƽ���� ���� �������� ����
            //Instantiate(listEnemy[iRand], new Vector3(0,7,0),Quaternion.identity, trsDynamicObject);
            //�ڵ带 ���� ���� ��ġġ��
            //������ ��ġ, ���̳��� ������Ʈ ��ġ�� �ʿ�

            //�ֻ����� ����
            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                //���Ⱑ �������� ������ ����
                Enemy goSc = go.GetComponent<Enemy>();
                goSc.setItem();
            }

            enemySpwanTimer = 0.0f;

        }
    }

    public void creatItem(Vector3 _pos)
    {
        int count = listItem.Count;
        int iRand = Random.Range(0, count);
        Instantiate(listItem[iRand], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void creaItem(Vector3 _pos, Item.eItemType _type)//0���� 1�Ŀ���, 2 ü��ȸ��
    {
        if (_type == Item.eItemType.None) return;
        Instantiate(listItem[(int)_type - 1], _pos, Quaternion.identity, trsDynamicObject);
    }

    public void SetHp(float _maxHp, float _curHp)
    {
        functionHP.SetHp(_maxHp, _curHp);
    }

    public void AddKillCount()
    {
        curKillCount++;
        //modifySlider();
        //checkSpawnBoss();
    }

    public void AddScore(int _value)//�ڽ��� �������� ���� 
    {
        score += _value;
        textScore.text = $"{score.ToString("d8")}";
    }

    private void modifySlider()
    {
        //ųī��Ʈ ����
        //slider.value = curKillCount;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //Ÿ�̸� ����
        slider.value = bossSpawnTimer;
        textSlider.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        //ųī��Ʈ ����
        //if (isSpawnBoss == false && curKillCount >= killCount)
        //{
        //    isSpawn = false;
        //    isSpawnBoss = true;

        //    GameObject go = Instantiate(fabBoss, trsSpwanPosition.position, Quaternion.identity, trsDynamicObject);
        //}

        //Ÿ�̸� ����
        if (isSpawnBoss == false)
        {
            isSpawn = false;
            IsSpawnBoss = true;
            GameObject go = Instantiate(fabBoss, trsSpwanPosition.position, Quaternion.identity, trsDynamicObject);
            //���� ü���� �ִ� ������ �����ߴ���
            EnemyBoss goSc = go.GetComponent<EnemyBoss>();
            setSliderBossType(goSc.Hp);
        }
    }

    private void setSliderBossType(float _maxHp)
    {
        slider.maxValue = _maxHp;
        slider.value = _maxHp;
        textSlider.text = $"{(int)_maxHp} / {(int)_maxHp}";
    }

    public void modifyBossHp(float _hp)
    {
        slider.value = _hp;
        textSlider.text = $"{(int)_hp} / {(int)slider.maxValue}";
    }

    public void KillBoss()
    {
        bossSpawnTimer = 0.0f;
        bossSpawnTime += 10;

        spwanTime -= 0.1f;
        if (spwanTime < 0.01f)
        {
            spwanTime = 0.01f;
        }
        //���̵� ���� ����� �߰��ϸ� ��

        isSpawn = true;
        initSlider();
        IsSpawnBoss = false;
    }

    public void GameOver()
    {
        List<cUserData> listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));

        int rank = -1;//0�� 1��
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData userData = listUserData[iNum];
            if (userData.Score < score)//�������� ��ũ���� ���ٸ� �ش� ��ũ�� �����ؾ���
            {
                rank = iNum;
                break;
            }
        }

        textGameOverMenuScore.text = $"���� : {score.ToString("d8")}";

        //�÷��̾ ��ũ�� ������� Ȯ��
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"��ŷ : {rank + 1}��";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "���";
        }
        else //��ũ�ȿ� ���� ���ߴٸ� �̸��� ���� �ʿ䰡 ����
        {
            textGameOverMenuRank.text = $"��ũ�� ���� ���߽��ϴ�.";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "���θ޴���";
        }


        btnGameOverMenu.onClick.AddListener(() =>
        {
            //��ũ���� �ߴٸ� ��ũ�� �̸��� ����
            if (rank != -1)
            {
                string name = IFGameOverMenuRank.text;

                if (name == string.Empty)
                {
                    name = "AAA";
                }

                cUserData newRank = new cUserData();
                newRank.Score = score;
                newRank.Name = name;

                listUserData.Insert(rank, newRank);
                listUserData.RemoveAt(listUserData.Count - 1);

                string value = JsonConvert.SerializeObject(listUserData);

                PlayerPrefs.SetString(Tool.rankKey, value);
            }

            FunctionFade.Instance.ActiveFade(true, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                FunctionFade.Instance.ActiveFade(false);
            });
        });
        objgameOverMenu.SetActive(true);
    }
}
