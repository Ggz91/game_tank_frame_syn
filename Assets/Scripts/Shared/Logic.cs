﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logic : MonoBehaviour
{
#if _CLIENT_
    IEntity m_op_en = null;
    public IEntity GetOpEn()
    {
        return m_op_en;
    }
    public void SetOpEn(IEntity en)
    {
        m_op_en = en;
    }
#endif
    static Logic m_instance = null;
    public static Logic Instance()
    {
        return m_instance;
    }

    public INetManager GetNetMng()
    {
        return m_network_mng;
    }

    // Start is called before the first frame update
    INetManager m_network_mng = null;
    ISceneMng m_scene_mng = null;
    void Start()
    {
        DontDestroyOnLoad(this);
        m_scene_mng = new CSceneMng();
        INetManagerCallback net_callback = m_scene_mng as CSceneMng;
#if _CLIENT_
        m_network_mng = new ClientNetManager(net_callback);
#else
        m_network_mng = new ServerNetManager(net_callback);
#endif
        if (null != m_network_mng)
        {
            m_network_mng.Init();
        }
        else
        {
            Debug.LogError("fail to init network mng");
        }
        m_instance = this;

    }
#if _CLIENT_
    public void RequestEnterGame()
    {
        m_network_mng.Send((short)EventPredefined.MsgType.EMT_ENTER_GAME, new CEvent());
    }
#endif
    public void EnterInGame()
    {
#if _CLIENT_
        SceneManager.LoadScene("Scenes/InGameScene");
#endif
        m_scene_mng.Enter();
    }
    public void LeaveGame()
    {
        m_scene_mng.Leave();
#if _CLIENT_
        SceneManager.LoadScene("Scenes/EntryScene");
#endif
    }
    // Update is called once per frame
    void Update()
    {
        if (null != m_scene_mng && m_scene_mng.InScene)
        {
            m_scene_mng.Update();
        }
    }

    private void OnDestroy()
    {
        if (null != m_network_mng)
        {
            m_network_mng.Leave();
        }
        if (null != m_scene_mng)
        {
            m_scene_mng.Leave();
        }
    }
}
