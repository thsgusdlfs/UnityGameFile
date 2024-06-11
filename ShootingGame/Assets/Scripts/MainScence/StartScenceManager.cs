using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.IO;//c#
using Newtonsoft.Json;
using UnityEngine.SceneManagement;//Scene을 사용하는 경우에만 사용

public class StartScenceManager : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnRanking;
    [SerializeField] Button btnExitRanking;
    [SerializeField] Button btnExit;
    [SerializeField] GameObject viewRank;

    [Header("랭크 프리팹")]
    [SerializeField] GameObject fabRank;
    [SerializeField] Transform contents;

    private void Awake()
    {
        Tool.isStartingMainScene = true;
        #region 수업내용
        //btnStart.onClick.AddListener(function);
        //UnityAction<float> action = (float _value) => 
        //{
        //    Debug.Log($"람다식이 실행되었음 => {_value}");
        //};

        //람다식
        //이름없는 함수
        //action.Invoke(0.75f);//람다식은 특정 이벤튼나 invoke를 통해서 실행가능
        //btnStart.onClick.AddListener(() =>
        //{
        //    gameStart(1, 2);
        //}); 
        //람다식 사용방식
        #endregion
        btnStart.onClick.AddListener(gameStart);
        btnRanking.onClick.AddListener(showRanking);
        btnExitRanking.onClick.AddListener(() => { viewRank.SetActive(false); });
        btnExit.onClick.AddListener(gameExit);
        #region 수업내용
        //json
        //string 문자열, 키와 벨류
        //{key:value}

        //save기능, 씬과 씬을 이동할때 가지고 가야하는 데이터가 있다면 사용

        //PlayerPrefs를 이용하여 유니티에 저장하는 방법
        //PlayerPrefs//유니티가 꺼져도 데이터를 보관하도록 유니티 내무에 저장

        //PlayerPrefs.SetInt("test", 999);//숫자 데이터 1개만 저장할땐 setint, setfloat로 저장하면 된다.
        //데이터를 삭제하지 않는한 //test 999 데이터가 유지됨, 단 게임을 삭제하면 이 데이터는 삭제되고 불러올수 없음
        //
        //int value = PlayerPrefs.GetInt("test", -1);//데이터가 없는 key값이면 int의 디폴트인 0을 출력
        //Debug.Log(value);

        //PlayerPrefs.HasKey("test"); // 특정 key값으로 키값 데이터가 있는지 없는지를 확인하기 위한 코드

        //PlayerPrefs.DeleteAll(); //모든 Playerprefs의 데이터를 삭제
        //PlayerPrefs.DeleteKey("test"); //특정 key값의 Playerprefs의 데이터를 삭제

        //string path = Application.streamingAssetsPath;//os에 따라 읽기전용으로 사용됨
        //~/Assets/StreamingAssets
        //File.WriteAllText(path + "/abc.json","테스트22");
        //File.Delete(path + "/abc.json");
        //string result = File.ReadAllText(path + "/abc.json");
        //Debug.Log(result);

        //path = Application.persistentDataPath;//R/W가 가능한 폴더위치
        //Debug.Log(path);
        //~/AppData/LocalLow/DefaultCompanmy/Class6

        //string path = Application.persistentDataPath + "/Jsons";//R/W가 가능한 폴더 위치
        //~/AppData/LocalLow/DefaultCompany/Class6/Jsons

        //if (Directory.Exists(path) == false)
        //{ 
        //    Directory.CreateDirectory(path);
        //}
        //if (File.Exists(path + "Test/abc.json") == true)
        //{
        //    string result = File.ReadAllText(path + "Test/abc.json");
        //}
        //else// 저장한 파일이 존재하지 않음
        //{
        //    //새로운 저장 위치와 데이터를 만들어 줘야함

        //    File.Create(path + "/Test");//폴더를 만들어줌
        //}

        //cUserData cUserData = new cUserData();
        //cUserData.Name = "가나다";
        //cUserData.Score = 100;
        //cUserData cUserData2 = new cUserData();
        //cUserData.Name = "라마바";
        //cUserData.Score = 200;

        //List<cUserData> listUserData = new List<cUserData>();
        //listUserData.Add(new cUserData() { Name = "가나다", Score = 100});
        //listUserData.Add(new cUserData() { Name = "라마바", Score = 200});

        //string jsonData = JsonConvert.SerializeObject(listUserData);

        //List<cUserData> afterData = JsonConvert.DeserializeObject<List<cUserData>>(jsonData);
        //Debug.Log(jsonData);
        //Debug.Log(afterData);

        //string jsonData = JsonUtility.ToJson(cUserData);
        //Debug.Log(jsonData);
        //{"Name":"가나다","Score":100}
        //cUserData user2 = new cUserData();
        //user2 = JsonUtility.FromJson<cUserData>(jsonData);
        //string jsonData = JsonUtility.ToJson(listUserData);
        //{} //리스트 데이터를 Json화 할수가 없다.
        //JsonUtility는 list를 json으로 변경하면 트러블이 존재함
        #endregion

        initRankView();
        viewRank.SetActive(false);
    }

    /// <summary>
    /// 랭크가 저장되어 있다면 저장된 랭크 데이터를 이용해서 랭크뷰를 만들어주고
    /// 랭크가 저장되어 있지 않다면 비어있는 랭크를 만들어 랭크뷰를 만들어줌
    /// </summary>
    private void initRankView()
    {
        List<cUserData> listUserData = null;
        clearRankView();
        if (PlayerPrefs.HasKey(Tool.rankKey) == true)//랭크 데이터가 저장이 되어있었다면
        {
            listUserData = JsonConvert.DeserializeObject<List<cUserData>>(PlayerPrefs.GetString(Tool.rankKey));
        }
        else
        {
            listUserData = new List<cUserData>();
            int rankcount = Tool.rankCount;
            for (int iNum = 0; iNum < rankcount; ++iNum)
            {
                listUserData.Add(new cUserData() { Name = "None", Score = 0 });
            }

            string value = JsonConvert.SerializeObject(listUserData);
            PlayerPrefs.SetString(Tool.rankKey, value);
        }

        int count = listUserData.Count;
        for (int iNum = 0; iNum < count; ++iNum)
        {
            cUserData data = listUserData[iNum];

            GameObject go = Instantiate(fabRank, contents);
            FabRanking gosc = go.GetComponent<FabRanking>();
            gosc.SetData((iNum + 1).ToString(), data.Name, data.Score);
        }
    }

    private void clearRankView()
    {
        int count = contents.childCount;
        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Destroy(contents.GetChild(iNum).gameObject);
        }
    }

    private void gameStart()
    {
        //SceneManager.LoadScene(0); //메인씬//0->0 게임을 다시시작하는 경우에 사용
        FunctionFade.Instance.ActiveFade(true, () => {
            SceneManager.LoadScene(1);
            FunctionFade.Instance.ActiveFade(false);
        });
    }

    private void showRanking()
    {
        viewRank.SetActive(true);
    }

    private void gameExit()
    {
        //에디터에서 플레이를 끄는 방법, 에디터 전용기능
        //빌드를 통해서 밖으로 가지고 나가서는 안되는 기능이다.
        //에디터에서만 사용됨을 알려주기 위해서 전처리를 해줄 필요가 있음
        //전처리, 코드가 조건에 의해서 본인이 없는것처럼 혹은 있는것처럼 동작하게 해줌

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else //유니티 에디터에서 실행하지 않았을때
        Application.Quit();
        //게임을 빌드했을때 게임이 종료됨
#endif
    }
}
