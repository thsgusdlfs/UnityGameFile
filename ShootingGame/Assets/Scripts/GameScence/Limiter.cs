using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ�� ����� ������ ���ؼ� ����
/// </summary>
public class Limiter : MonoBehaviour
{
    [Header("ȭ�� ���")]
    [SerializeField, Tooltip("ȭ������ �ּҺ���")] Vector2 viewPortLimitMin;
    [SerializeField, Tooltip("ȭ������ �ִ����")] Vector2 viewPortLimitMax;
    
    [Header("������ ȭ�� ���")]
    [SerializeField, Tooltip("���� ȭ������ �ּҺ���")] Vector2 viewPortLimitMinBoss;
    [SerializeField, Tooltip("���� ȭ������ �ִ����")] Vector2 viewPortLimitMaxBoss;

    Vector2 worldPosLimitMin;//���� �����ʹ� �� ������ ������ ����
    Vector2 worldPosLimitMax;//���� �����ʹ� �� ������ ������ ����
    public Vector2 WorldPosLimitMin//�� �����ʹ� ������ �������� �Լ��� �۵�
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
    /// ���� ���۽� ������Ʈ�� ȭ�� ��� �������� ���� ���������� �ʱ�ȭ �մϴ�.
    /// </summary>
    private void initWorldPos()
    {
        //����Ʈ ������
        worldPosLimitMin = cam.ViewportToWorldPoint(viewPortLimitMin);
        worldPosLimitMax = cam.ViewportToWorldPoint(viewPortLimitMax);

    }

    /// <summary>
    /// ȭ�鳻�� (_isBoss == false ? viewPortLimitMin.x : viewPortLimitMinBoss.x))�������� üũ�ؼ� �ּ� �ִ밪�� �ʰ��ÿ� �ʰ����� �ʴ� ��ǥ�� �ٽ� �Ű��ش�.
    /// </summary>
    /// <param name="_limitPosition"></param>
    /// <returns></returns>
    public Vector3 checkMovePosition(Vector3 _limitPosition, bool _isBoss = false)
    {
        Vector3 viewPortPos = cam.WorldToViewportPoint(_limitPosition);

        //���� ������, ���׿�����, ���׽�

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

        //���� ������, ���׿�����, ���׽�

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
