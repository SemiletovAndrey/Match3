using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int columnsCount;
    [SerializeField] private int rowsCount;
    [SerializeField] private int padding;

    private GameObject[,] _cellsObject;
    private Cell[,] _cells;

    private RectTransform _cellTransform;
    private RectTransform _areaRect;
    private Canvas _canvas;
    private float _cellSize;
    private RandomFeature _randomFeature;
    private BoardControl _boardControl;
    private bool _hasMoved = false;
    private List<Feature> _visitedFeatures = new List<Feature>();

    public int Row { get { return rowsCount; } }

    public void GenerationBoard()
    {
        _canvas = GetComponentInParent<Canvas>();
        _areaRect = GetComponent<RectTransform>();
        _cellTransform = cellPrefab.transform.gameObject.GetComponent<RectTransform>();
        _randomFeature = GetComponent<RandomFeature>();
        _boardControl = GetComponent<BoardControl>();
        _cellsObject = new GameObject[rowsCount, columnsCount];
        _cells = new Cell[rowsCount, columnsCount];
        _cellSize = _cellTransform.rect.width;
        GenerationCells(_cellsObject, _cells);
        _boardControl.InitialGenerationRandomFeatureOnBoard();
    }

    private void GenerationCells(GameObject[,] cellsObject, Cell[,] cells)
    {
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                float xPos = _areaRect.transform.position.x + (j * (_cellSize * _canvas.scaleFactor + padding));
                float yPos = _areaRect.transform.position.y + -i * (_cellSize * _canvas.scaleFactor + padding);
                cellsObject[i, j] = Instantiate(cellPrefab, new Vector2(xPos, yPos), Quaternion.identity, _areaRect);
                cells[i, j] = cellsObject[i, j].GetComponent<Cell>();
            }
        }
    }

    private bool CheckFillCellFeatures(Cell[,] cells)
    {
        for (int i = 1; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j].feature == null)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private bool CheckUpLineHasNullFeature(Cell[,] cells)
    {
        for (int i = 0; i < cells.GetLength(1); i++)
        {
            if (cells[0, i].feature == null)
            {
                return true;
            }
        }
        return false;
    }

    private bool CanFeatureDown(Cell[,] cells, int row, int column)
    {
        if (row < cells.GetLength(0) - 1 && cells[row + 1, column].feature == null && cells[row, column].feature != null)
        {
            return true;
        }
        return false;
    }

    private bool HasFeatureInCellObject(GameObject[,] cellsObject, int row, int column)
    {
        if (cellsObject[row, column].GetComponentInChildren<Feature>() != null)
            return true;
        return false;
    }

    private void MoveFeatureDown(GameObject[,] cellsObject, Cell[,] cells)
    {
        _hasMoved = false;
        int downNullFeatureCount = 0;
        int downFeatureCount = 0;
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            downNullFeatureCount = cells.GetLength(0) - 1;

            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (CanFeatureDown(cells, i, j) && HasFeatureInCellObject(cellsObject, i, j))
                {
                    _boardControl.canTouch = false;
                    while (cells[downNullFeatureCount, j].feature != null && downNullFeatureCount > 0)
                    {
                        downNullFeatureCount--;
                    }
                    downFeatureCount = downNullFeatureCount;
                    while (cells[downFeatureCount, j].feature == null && downFeatureCount >= 0)
                    {
                        downFeatureCount--;
                    }
                    cells[downNullFeatureCount, j].feature = cells[downFeatureCount, j].feature;
                    cells[downFeatureCount, j].feature = null;
                    GameObject currentCellObject = cellsObject[downFeatureCount, j];
                    GameObject nextCellObject = cellsObject[downNullFeatureCount, j];
                    RectTransform currentCellRectTransform = currentCellObject.transform.GetChild(0).GetComponent<RectTransform>();
                    RectTransform nextCellRectTransform = nextCellObject.GetComponent<RectTransform>();
                    AnimationLineWithDOTween(currentCellRectTransform);
                    currentCellRectTransform.SetParent(nextCellRectTransform, true);
                    _hasMoved = true;
                }
            }
        }
    }

    private void AnimationLineWithDOTween(RectTransform currentCellRectTransform)
    {
        Tween myTween = currentCellRectTransform.DOAnchorPos(Vector2.zero, 0.8f)
                                .SetEase(Ease.Linear)
                                .OnKill(CompleteAnimation);

    }

    private void CompleteAnimation()
    {
        _boardControl.canTouch = true;
    }


    public void FillUpStates()
    {
        StartCoroutine(FillUpStepsCoroutine());
    }

    private IEnumerator FillUpStepsCoroutine()
    {
        for (int i = 0; i < _cellsObject.GetLength(1); i++)
        {
            Feature f = _cellsObject[0, i].GetComponentInChildren<Feature>();
            if (f == null)
            {
                yield return new WaitForSeconds(0.04f);
                _boardControl.canTouch = false;
                int indexFeature = Random.Range(0, _randomFeature.featureListCount);
                GameObject cell = Instantiate(_randomFeature.GenerateRandomFeature(indexFeature), _cellsObject[0, i].transform.position, Quaternion.identity, _cellsObject[0, i].transform);
                _cells[0, i].feature = cell.GetComponent<Feature>();
                _cells[0, i].feature.colorFeature = (ColorFeature)indexFeature;
                yield return null;
            }

        }
    }


    public void CheckNearFeature(Feature feature)
    {
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                if (_cells[i, j].feature == feature)
                {
                    CheckNeighborsOneColor(i, j, feature);
                    if (_visitedFeatures.Count >= 3)
                    {
                        foreach (var idF in _visitedFeatures)
                        {
                            Destroy(idF.gameObject);
                        }
                    }
                }
            }
        }

        _visitedFeatures.Clear();
    }
    public void CheckNeighborsOneColor(int row, int column, Feature mainFeature)
    {
        if (IsInsideBoard(row, column) && !_visitedFeatures.Contains(_cells[row, column].feature))
        {
            _visitedFeatures.Add(_cells[row, column].feature);
            CheckAllNeighborOneColor(row, column, mainFeature);
        }
    }

    private void CheckAllNeighborOneColor(int row, int column, Feature mainFeature)
    {
        CheckNeighborOneColor(row + 1, column, mainFeature);
        CheckNeighborOneColor(row - 1, column, mainFeature);
        CheckNeighborOneColor(row, column + 1, mainFeature);
        CheckNeighborOneColor(row, column - 1, mainFeature);
    }

    private void CheckNeighborOneColor(int row, int column, Feature mainFeature)
    {
        if (IsInsideBoard(row, column))
        {
            Feature feature = _cells[row, column].feature;
            if (_cells[row, column].feature != null &&
                mainFeature.colorFeature == feature.colorFeature)
            {
                CheckNeighborsOneColor(row, column, mainFeature);
            }
        }
    }

    private bool IsInsideBoard(int row, int column)
    {
        return row >= 0 && row < _cells.GetLength(0) && column >= 0 && column < _cells.GetLength(1);
    }


    [ContextMenu("Clear all cell")]
    public void Clear()
    {
        _randomFeature.ClearAllFeatures(_cellsObject, _cells);
    }

    [ContextMenu("Create all featue")]
    public void CreateAllFeature()
    {
        _randomFeature.InitialGenerationRandomFeatures(_cellsObject, _cells);
    }

    [ContextMenu("Create one line feature")]
    public void CreateOneLine()
    {
        _randomFeature.RandomFillOneLine(_cellsObject, _cells);
    }

    [ContextMenu("Check feature")]
    public bool CheckUpLineHasNullFeature()
    {
        return CheckUpLineHasNullFeature(_cells);
    }

    [ContextMenu("Move Down feature")]
    public void MoveFeatureDown()
    {
        do
        {
            MoveFeatureDown(_cellsObject, _cells);
        }
        while (_hasMoved);

    }

    [ContextMenu("Fill Up Line With Empty Cell")]
    public void FillUpLineWithEmptyCell()
    {
        _randomFeature.FillUpLineWithEmptyCell(_cellsObject, _cells);
    }

    [ContextMenu("Check fill feature")]
    public bool CheckFillCellFeatures()
    {
        return CheckFillCellFeatures(_cells);
    }


}
