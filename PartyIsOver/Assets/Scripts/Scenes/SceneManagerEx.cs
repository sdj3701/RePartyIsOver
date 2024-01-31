using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx 
{
    public BaseScene CurrentScene{ get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.SceneType type)
    {
        //���� ����ߴ� ���� �����ְ� ���� ������ �̵�
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    //Define�� �ִ� ���ڿ��� �̾Ƴ��� ����̴�. LoadScene�� ���ڿ��� �޾ƾ��ϱ� ������ �̷��� ��� ���
    public string GetSceneName(Define.SceneType type)
    {
        string name = System.Enum.GetName(typeof(Define.SceneType),type);
        return name;
    }

    public string GetCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.name;
    }

    public Scene GetCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene;
    }

    public GameObject GetCurrentSceneRootGameObject()
    {
        GameObject currentScene = GetCurrentScene().GetRootGameObjects()[0];
        return currentScene;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }

}
