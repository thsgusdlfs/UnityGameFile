using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//사용하는 이름은 상관없으나 해당 값을 인스턴스로 해두면 싱글톤임을 쉽게 알 수 있다.
    [Header("적기들")]                            //최초 제작시엔 null이기에 데이터를 채워줘야한다.
    [SerializeField, Tooltip("적기 리스트")] List<GameObject> listEnemy;
    [SerializeField] GameObject fabBoss;

    [Header("적 생성 여부")]
    [SerializeField, Tooltip("적들 생성 유무")] bool isSpawn = false;//보스가 등장하거나 원하는 사유가 있을때 이값을
                                                               //true로 변경하면 적들이 나오게 하는 값
    [SerializeField] Color sliderDefaultColor;
    [SerializeField] Color sliderBossSpawnColor;

    WaitForSeconds halfTime = new WaitForSeconds(0.5f);
    bool isSpawnBoss = false; //보스가 등장중인지 확인하기위한 변수
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

        while (timer < 1.0f)//조건문이 참이라면 반복
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

    //true로 변경하면 적들이 더이상 나오지 않게하는 용도로 활용

    [Header("적 생성 시간")]
    float enemySpwanTimer = 0.0f;
    [SerializeField, Range(0.1f, 5f)] float spwanTime = 1.0f;

    [Header("적 생성 위치")]
    [SerializeField] Transform trsSpwanPosition;
    [SerializeField] Transform trsDynamicObject;

    [Header("드롭아이템")]
    [SerializeField] List<GameObject> listItem;

    [Header("드롭확률")]
    [SerializeField, Range(0.0f, 100.0f)] float itemDropRate;

    [Header("체력 게이지")]
    [SerializeField] FunctionHp functionHP;
    [SerializeField] Slider slider;
    [SerializeField] Image imgSliderFill;


    [Header("보스 포지션")]
    [SerializeField] Transform trsBossPosition;

    public Transform TrsBossPosition => trsBossPosition;

    Limiter limiter;

    [Header("보스 출현조건")]
    [SerializeField] int killCount = 100;
    int curKillCount = 0;
    [SerializeField] TMP_Text textSlider;

    [SerializeField] float bossSpawnTime = 0.0f;
    [SerializeField] float bossSpawnTimer = 0.0f;

    [Header("점수")]
    [SerializeField] TMP_Text textScore;
    private int score;

    private bool gameStart = false;

    [Header("게임시작 텍스트")]
    [SerializeField] TMP_Text gameStartText;

    [Header("게임오버메뉴")]
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

    GameObject fabExplosion;//매니저가 원본의 레퍼런스를 가지고있음//private, 변수들은 실제 레퍼런스 데이터를 가지고 있음
    //함수는 실행될 기능 순서정도의 데이터만 가지고 있지 실제 데이터는 가지고 있지 않음
    //실제 데이터를 가지고 있는 변수는 private를 유지하고 정보를 전달 혹은 가져와야할때만 함수로서 사용가능
    public GameObject FabExplosion
    {
        get
        {
            return fabExplosion;
        }
        set { fabExplosion = value; }
    }

    ////인스펙터의 값이 변동이 있을때 이 함수가 강제 호출
    //OnValidate()는 게임이 실행되지 않아도 동작한다.
    //private void OnValidate()
    //{
    //    if (Application.isPlaying == false) return; // 게임이 실행중이 아니라면 넘어가도록 하는 코드
    //    Debug.Log("값이 변경됨");

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
        //1개만 존재해야 한다

        if (Instance == null)
        {
            Instance = this;
        }
        else//인스턴스가 이미 존재한다면
        {
            //Destroy(this);//이러면 오브젝트가 아닌 컴포넌트만 삭제됨
            Destroy(gameObject);//오브젝트가 지워지면서 스크립트도 같이 지워짐
        }
        fabExplosion = Resources.Load<GameObject>("Effect/Test/fabExplosion");

        initSlider();
    }
    private void initSlider()
    {
        //킬카운트 버전
        //slider.minValue = 0;
        //slider.maxValue = killCount;
        //slider.value = 0;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //타이머 버전
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
            //적을 생성
            int count = listEnemy.Count;//개의 적기 0~2
            int iRand = Random.Range(0, count);//0,3
            //float 데이터를 이용하여 랜덤을 사용시에는 0.0 ~ 1.0 까지 숫자중 랜덤 값을 리턴
            //int 데이터를 이용해서 랜덤을 사용하면 0 ~ 1 이라고 입력시 맥스값에서 -1한 데이터를 가지고 리턴
            //int min max값 양수일때 max-1로 출력이 됨, 음수일때는 max+1

            Vector3 defaultPos = trsSpwanPosition.position; //y=7;
            float x = Random.Range(limiter.WorldPosLimitMin.x, limiter.WorldPosLimitMax.x);//x =>-2.4 ~ 2.4
            defaultPos.x = x;

            GameObject go = Instantiate(listEnemy[iRand], defaultPos, Quaternion.identity, trsDynamicObject);
            //유니티에서 스폰 포지션을 지정
            //Instantiate(listEnemy[iRand], new Vector3(0,7,0),Quaternion.identity, trsDynamicObject);
            //코드를 통해 직접 위치치정
            //생성할 위치, 다이나믹 오브젝트 위치가 필요

            //주사위를 굴림
            float rate = Random.Range(0.0f, 100.0f);
            if (rate <= itemDropRate)
            {
                //적기가 아이템을 가지고 있음
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

    public void creaItem(Vector3 _pos, Item.eItemType _type)//0없음 1파워업, 2 체력회복
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

    public void AddScore(int _value)//자신이 몇점인지 전달 
    {
        score += _value;
        textScore.text = $"{score.ToString("d8")}";
    }

    private void modifySlider()
    {
        //킬카운트 버전
        //slider.value = curKillCount;
        //textSlider.text = $"{curKillCount.ToString("d4")} / {killCount.ToString("d4")}";

        //타이머 버전
        slider.value = bossSpawnTimer;
        textSlider.text = $"{((int)bossSpawnTimer).ToString("d4")} / {((int)bossSpawnTime).ToString("d4")}";
    }

    private void checkSpawnBoss()
    {
        //킬카운트 버전
        //if (isSpawnBoss == false && curKillCount >= killCount)
        //{
        //    isSpawn = false;
        //    isSpawnBoss = true;

        //    GameObject go = Instantiate(fabBoss, trsSpwanPosition.position, Quaternion.identity, trsDynamicObject);
        //}

        //타이머 버전
        if (isSpawnBoss == false)
        {
            isSpawn = false;
            IsSpawnBoss = true;
            GameObject go = Instantiate(fabBoss, trsSpwanPosition.position, Quaternion.identity, trsDynamicObject);
            //보스 체력은 최대 몇으로 시작했는지
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
        //난이도 증가 기능을 추가하면 됨

        isSpawn = true;
        initSlider();
        IsSpawnBoss = false;
    }

    public void GameOver()
    {
        List<cUserData> listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));

        int rank = -1;//0이 1등
        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; iNum++)
        {
            cUserData userData = listUserData[iNum];
            if (userData.Score < score)//내점수가 랭크보다 높다면 해당 랭크를 차지해야함
            {
                rank = iNum;
                break;
            }
        }

        textGameOverMenuScore.text = $"점수 : {score.ToString("d8")}";

        //플레이어가 랭크에 들었는지 확인
        if (rank != -1)
        {
            textGameOverMenuRank.text = $"랭킹 : {rank + 1}등";
            IFGameOverMenuRank.gameObject.SetActive(true);
            textGameOverMenuBtn.text = "등록";
        }
        else //랭크안에 들지 못했다면 이름을 적을 필요가 없음
        {
            textGameOverMenuRank.text = $"랭크인 하지 못했습니다.";
            IFGameOverMenuRank.gameObject.SetActive(false);
            textGameOverMenuBtn.text = "메인메뉴로";
        }


        btnGameOverMenu.onClick.AddListener(() =>
        {
            //랭크인을 했다면 랭크와 이름을 저장
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
