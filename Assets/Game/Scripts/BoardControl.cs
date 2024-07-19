using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControl : MonoBehaviour
{
    [SerializeField] private Board board;
    public bool canTouch = false;

    private void Start()
    {
        board = GetComponent<Board>();
    }

    public void InitialGenerationRandomFeatureOnBoard()
    {
        StartCoroutine(InitSpawnAndMoveFeatureUpLineCoroutine());
    }


    public void MoveDownFeatureOnBoard(Feature feature)
    {
        if (canTouch == true)
        {
            board.CheckNearFeature(feature);
            StartCoroutine(MoveDownAndSpawnFeatureCoroutine());
            Destroy(feature.gameObject);
        }
    }

    private IEnumerator InitSpawnAndMoveFeatureUpLineCoroutine()
    {
        int count = 0;
        while (board.CheckUpLineHasNullFeature())
        {
            yield return new WaitForSeconds(0.1f);
            board.FillUpStates();
            yield return new WaitForSeconds(0.5f);
            board.MoveFeatureDown();
            yield return new WaitForSeconds(0.1f);
            canTouch = false;
            count++;
            if (count == board.Row - 1)
            {
                yield return new WaitForSeconds(0.05f);
                board.FillUpStates();
                canTouch = true;
                yield break;
            }
        }
    }

    private IEnumerator MoveDownAndSpawnFeatureCoroutine()
    {
        canTouch = false;
        yield return null;
        while (board.CheckFillCellFeatures())
        {
            yield return new WaitForSeconds(0.15f);
            board.MoveFeatureDown();
            yield return new WaitForSeconds(0.2f);
            board.FillUpStates();
            yield return new WaitForSeconds(0.2f);
        }
        if (board.CheckUpLineHasNullFeature())
        {
            yield return new WaitForSeconds(0.2f);
            board.FillUpStates();
            yield return new WaitForSeconds(0.4f);
            canTouch = true;
        }
    }


}
