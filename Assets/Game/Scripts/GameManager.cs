using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private BoardControl boardControl;
    void Start()
    {
        board.GenerationBoard();
        EventManager.ClickUserOnFeatureEvent += boardControl.MoveDownFeatureOnBoard;
    }

    private void Update()
    {
        

    }
}
