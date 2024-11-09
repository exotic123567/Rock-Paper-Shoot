using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialClassDisplayForPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] playerClassPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayPlayerClass(int color, uint playerClass)
    {
        int playerIndex = (int) playerClass;
        
    }
}
