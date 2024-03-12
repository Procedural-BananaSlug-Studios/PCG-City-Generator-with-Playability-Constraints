using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;
    public Tile backupTile;
    private int iterations;

    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    // Initializes the grid
    void InitializeGrid()
    {
        float cellSize = 3.0f; // Replace with the size of your cells
        for (int x = 0; x < dimensions; x++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x * cellSize, 0, y * cellSize), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }
        StartCoroutine(CheckEntropy());
    }

    // Checks list and organizes it so that first element is the one with the least amount of options
    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);

        yield return new WaitForSeconds(0.125f);
        CollapseCell(tempGrid);
    }

    // Collapses a cell and updates the grid
    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        Cell cellToCollapse = tempGrid[randIndex];
        cellToCollapse.collapsed = true;
        try
        {
            int random = UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length);
            Tile selectedTile = cellToCollapse.tileOptions[random];
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }
        catch
        {
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, foundTile.transform.rotation);
        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int x = 0; x < dimensions; x++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                for (int z = 0; z < dimensions; z++) // Loop through each layer in the Z direction
                {
                    var index = z + (y * dimensions) + (x * dimensions * dimensions);
                    Debug.Log(index);
                    try
                    {

                        if (gridComponents[index].collapsed)
                        {
                            newGenerationCell[index] = gridComponents[index];
                        }
                        else
                        {
                            List<Tile> options = new List<Tile>();

                            foreach (Tile t in tileObjects)
                            {
                                options.Add(t);
                            }

                            if (x > 0)
                            {
                                Cell left = gridComponents[index - 1]; // Get cell to the left in the X direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in left.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].rightNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }

                            if (x < dimensions - 1)
                            {
                                Cell right = gridComponents[index + 1]; // get cell to the right in the X direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in right.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].leftNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }

                            if (y > 0)
                            {
                                Cell front = gridComponents[index - dimensions]; //get cell in front of the Y direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in front.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].backNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }

                            if (y < dimensions - 1)
                            {
                                Cell back = gridComponents[index + dimensions]; // get cell at the back in the Y direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in back.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].frontNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }

                            if (z > 0)
                            {
                                Cell down = gridComponents[index - dimensions * dimensions]; // Get cell in below in the Z direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in down.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].upNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }

                            if (z < dimensions - 1)
                            {
                                Cell up = gridComponents[index + dimensions * dimensions]; // Get cell at the above in the Z direction
                                List<Tile> validOptions = new List<Tile>();

                                foreach (Tile possibleOptions in up.tileOptions)
                                {
                                    var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                    var valid = tileObjects[validOption].downNeighbour;
                                    validOptions = validOptions.Concat(valid).ToList();
                                }

                                CheckValidity(options, validOptions);
                            }
                            Debug.Log(index);


                            Tile[] newTileList = new Tile[options.Count];

                            for (int i = 0; i < options.Count; i++)
                            {
                                newTileList[i] = options[i];
                            }

                            newGenerationCell[index].RecreateCell(newTileList);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error occurred at index: " + index);
                        Debug.LogError(e.Message);
                    }
                }
            }
>>>>>>> 134b11b909e5b13156bc9bc6ea5c53f1e9f7c38e
        }

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimensions * dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
    }


    // Checks if the options are valid
    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }

}
