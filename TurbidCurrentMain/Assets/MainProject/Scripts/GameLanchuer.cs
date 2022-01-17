using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent
{
    public class GameLanchuer : MonoBehaviour
    {
        private void Awake()
        {
            DirectorManager.Instance.ChangeDirector(new InitDirector());
        }
        private void OnEnable()
        {
            
        }
    }
}