using System.Collections.Generic;
using UnityEngine;

public class RandomFeature : MonoBehaviour
{
    [SerializeField] private List<GameObject> featuresList;
    public int featureListCount {  get { return featuresList.Count; } }
    public void InitialGenerationRandomFeatures(GameObject[,] cellsObjects, Cell[,] cells)
    {
        ClearAllFeatures(cellsObjects, cells);
        for (int i = 0; i < cellsObjects.GetLength(0); i++)
        {
            for (int j = 0; j < cellsObjects.GetLength(1); j++)
            {
                int indexRandom = Random.Range(0, featuresList.Count);
                GameObject cell = Instantiate(GenerateRandomFeature(indexRandom), cellsObjects[i, j].transform.position, Quaternion.identity, cellsObjects[i, j].transform);
                cells[i,j].feature = cell.GetComponent<Feature>();
                cells[i, j].feature.colorFeature = (ColorFeature)indexRandom;
            }
        }

    }

    public void RandomFillOneLine(GameObject[,] cellsObjects, Cell[,] cells, int row = 0)
    {
        for (int i = 0; i < cellsObjects.GetLength(1); i++)
        {
            Feature f = cellsObjects[row, i].GetComponentInChildren<Feature>();
            if (f != null)
                Destroy(f.gameObject);
            int indexRandom = Random.Range(0, featuresList.Count);
            GameObject cell = Instantiate(GenerateRandomFeature(indexRandom), cellsObjects[row, i].transform.position, Quaternion.identity, cellsObjects[row, i].transform);
            cells[row, i].feature = cell.GetComponent<Feature>();
            cells[row, i].feature.colorFeature = (ColorFeature)indexRandom;

        }
    }

    public void ClearAllFeatures(GameObject[,] cellsObjects, Cell[,] cells)
    {
        for (int i = 0; i < cellsObjects.GetLength(0); i++)
        {
            for (int j = 0; j < cellsObjects.GetLength(1); j++)
            {
                Feature f = cellsObjects[i, j].GetComponentInChildren<Feature>();
                if (f != null)
                    Destroy(f.gameObject);
                cells[i, j].feature = null;
            }
        }
    }

    public void FillUpLineWithEmptyCell(GameObject[,] cellsObjects, Cell[,] cells)
    {
        for (int i = 0; i < cellsObjects.GetLength(1); i++)
        {
            Feature f = cellsObjects[0, i].GetComponentInChildren<Feature>();
            if (f == null)
            {
                Vector3 positionSpawn = new Vector3(cellsObjects[0, i].transform.position.x, cellsObjects[0, i].transform.position.y + 200, cellsObjects[0, i].transform.position.z);
                int indexRandom = Random.Range(0,featuresList.Count);
                GameObject cell = Instantiate(GenerateRandomFeature(indexRandom), positionSpawn, Quaternion.identity, cellsObjects[0, i].transform);
                cells[0, i].feature = cell.GetComponent<Feature>();
                cells[0, i].feature.colorFeature = (ColorFeature)indexRandom;
            }
        }
    }

    public GameObject GenerateRandomFeature(int index)
    {
        return featuresList[index];
    }

}
