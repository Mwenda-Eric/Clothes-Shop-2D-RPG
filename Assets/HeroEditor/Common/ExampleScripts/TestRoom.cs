using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.HeroEditor.Common.ExampleScripts
{
    public class TestRoom : MonoBehaviour
    {
        public string ReturnSceneName;

        public void Awake()
        {
            Physics.gravity = new Vector3(0, -12.5f, 0);
            Physics.defaultSolverIterations = 8;
            Physics.defaultSolverVelocityIterations = 2;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                #if UNITY_EDITOR

                if (UnityEditor.EditorBuildSettings.scenes.All(i => !i.path.Contains(ReturnSceneName)))
                {
                    throw new Exception("Please add the following scene to Build Settings: " + ReturnSceneName);
                }

                #endif

                SceneManager.LoadScene(ReturnSceneName);
            }
        }
    }
}