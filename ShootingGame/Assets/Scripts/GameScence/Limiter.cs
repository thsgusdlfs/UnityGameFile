using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화면 경계의 비율에 대해서 설정
/// </summary>
public class Limiter : MonoBehaviour
{
    [Header("화면 경계")]
    [SerializeField, Tooltip("화면경계의 최소비율")] Vector2 viewPortLimitMin;
    [SerializeField, Tooltip("화면경계의 최대비율")] Vector2 viewPortLimitMax;
    
    [Header("보스용 화면 경계")]
    [SerializeField, Tooltip("보스 화면경계의 최소비율")] Vector2 viewPortLimitMinBoss;
    [SerializeField, Tooltip("보스 화면경계의 최대비율")] Vector2 viewPortLimitMaxBoss;

    Vector2 worldPosLimitMin;//실제 데이터는 이 변수가 가지고 있음
    Vector2 worldPosLimitMax;//실제 데이터는 이 변수가 가지고 있음
    public Vector2 WorldPosLimitMin//이 데이터는 변수로 보이지만 함수로 작동
    {
        get
        {
            return worldPosLimitMin;
        }
    }

    public Vector2 WorldPosLimitMax => worldPosLimitMax;

    Camera cam;
    GameManager gameManager;

    private void Start()
    {
        cam = Camera.main;
        gameManager = GameManager.Instance;
        gameManager._Limiter = this;

        initWorldPos();
    }

    /// <summary>
    /// 게임 시작시 뷰포인트의 화면 경계 변수들을 월드 포지션으로 초기화 합니다.
    /// </summary>
    private void initWorldPos()
    {
        //뷰포트 데이터
        worldPosLimitMin = cam.ViewportToWorldPoint(viewPortLimitMin);
        worldPosLimitMax = cam.ViewportToWorldPoint(viewPortLimitMax);

    }

    /// <summary>
    /// 화면내의 (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x))포지션을 체크해서 최소 최대값을 초과시에 초과하지 않는 좌표로 다시 옮겨준다.
    /// </summary>
    /// <param name="_limitPosition"></param>
    /// <returns></returns>
    public Vector3 checkMovePosition(Vector3 _limitPosition, bool _isBoss = false)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(_limitPosition);

        //조건 연산자, 삼항연산자, 다항식

        if (viewPortPos.x < (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x))
        {
            viewPortPos.x = (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x);
        }
        else if (viewPortPos.x > (_isBoss == false ? viewPortLimitMax.x : viewPortLimitMaxBoss.x))
        {
            viewPortPos.x = (_isBoss == false ? viewPortLimitMax.x : viewPortLimitMaxBoss.x);
        }

        if (viewPortPos.y < viewPortLimitMin.y)
        {
            viewPortPos.y = viewPortLimitMin.y;
        }
        else if (viewPortPos.y > viewPortLimitMax.y)
        {
            viewPortPos.y = viewPortLimitMax.y;
        }

        return cam.ViewportToWorldPoint(viewPortPos);
    }

    public bool checkMovePosition(Vector3 _limitPosition)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(_limitPosition);

        //조건 연산자, 삼항연산자, 다항식

        if (viewPortPos.x < viewPortLimitMinBoss.x || viewPortPos.x > viewPortLimitMaxBoss.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public (bool _x, bool _y) IsReflectItem(Vector3 _pos, Vector3 _dir)
    {
        bool rX = false;
        bool rY = false;
        if ((_pos.x < WorldPosLimitMin.x && _dir.x < 0) || (_pos.x > worldPosLimitMax.x && _dir.x > 0))
        {
            rX = true;
        }
        else if ((_pos.y < WorldPosLimitMin.y && _dir.y < 0) || (_pos.y > worldPosLimitMax.y && _dir.y > 0))
        {
            rY = true;
        }
        return (rX, rY);
    }
}
