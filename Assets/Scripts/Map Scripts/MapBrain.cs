using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapBrain : Singleton<MapBrain>
{
    //Genetic algorithm parameters
    [Header("Genetic Algorithm Parameters")]
    [SerializeField, Range(10, 100)]
    private int populationSize = 20;
    [SerializeField, Range(1, 100)]
    private int crossoverRate = 100;
    [SerializeField, Range(1, 100)]
    private int mutationRate = 0;
    [SerializeField, Range(1, 100)]
    private int generationLimit = 10;

    private double crossoverRatePercent;
    private double mutationRatePercent;

    //Algorithm variables
    private List<MapInitializer> currentGeneration;
    private int totalFitnessThisGeneration;
    private int bestFitnessScoreOfAllTime = 0;
    private MapInitializer bestMap = null;
    private int bestMapGenerationNumber = 0;
    private int generationNumber = 1;

    //Fitness parameters
    [SerializeField]
    private int fitnessCornerMin = 6, fitnessCornerMax = 12;
    [SerializeField, Range(1, 3)]
    private int fitnessCornerWeight = 1;
    [SerializeField, Range(1, 3)]
    private int fitnessNearCornerWeight = 1;
    [SerializeField, Range(1, 5)]
    private int fitnessPathWeight = 1;
    [SerializeField, Range(0.3f, 1f)]
    private float fitnessObstacleWeight = 1;

    //Map start parameters
    [Header("Map Vizualization")]
    [SerializeField, Range(3, 30)]
    private int mapWidth = 11;
    [SerializeField, Range(3, 30)]
    private int mapLength = 11;
    private Vector3 startPosition, exitPosition;
    private MapGrid grid;
    [SerializeField]
    private Direction startPositionEdge = Direction.Left;
    [SerializeField]
    private Direction exitPositionEdge = Direction.Right;
    [SerializeField]
    private bool randomStartAndExit = false;
    [SerializeField, Range(4, 9)]
    private int numberOfKnightPieces = 7;

    //Visualize grid
    public MapVisualizer mapVisualizer;
    private DateTime startDate, endDate;
    private bool isAlgorithmRunning = false;
    public bool IsAlgorithmRunning { get => isAlgorithmRunning; }

    private void Start()
    {
        mutationRatePercent = mutationRate / 100d;
        crossoverRatePercent = crossoverRate / 100d;
        RunAlgorithm();
    }

    public void RunAlgorithm()
    {
        UIController.Instance.ResetScreen();
        ResetAlgorithmVariables();
        mapVisualizer.ClearMap();
        grid = new MapGrid(mapWidth, mapLength);
        MapHelper.RandomlyChooseAndSetStartAndExit(grid, ref startPosition, ref exitPosition, randomStartAndExit, startPositionEdge, exitPositionEdge);
        isAlgorithmRunning = true;
        startDate = DateTime.Now;
        FindOptimalSolution(grid);
    }

    private void ResetAlgorithmVariables()
    {
        totalFitnessThisGeneration = 0;
        bestFitnessScoreOfAllTime = 0;
        bestMapGenerationNumber = 0;
        generationNumber = 0;
        bestMap = null;
    }

    private void FindOptimalSolution(MapGrid grid)
    {
        currentGeneration = new List<MapInitializer>(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            var map = new MapInitializer(grid, numberOfKnightPieces);
            map.CreateMap(startPosition, exitPosition, true);
            currentGeneration.Add(map);
        }

        StartCoroutine(GeneticAlgorithm());
    }

    private IEnumerator GeneticAlgorithm()
    {
        totalFitnessThisGeneration = 0;
        int bestFitnessScoreThisGeneration = 0;
        MapInitializer bestMapThisGeneration = null;

        foreach (var candidate in currentGeneration)
        {
            candidate.FindPath();
            candidate.Repair();
            var fitnessScore = CalculateFitness(candidate.ReturnMapData());
            totalFitnessThisGeneration += fitnessScore;
            if (fitnessScore > bestFitnessScoreThisGeneration)
            {
                bestFitnessScoreThisGeneration = fitnessScore;
                bestMapThisGeneration = candidate;
            }
        }

        if (bestFitnessScoreThisGeneration > bestFitnessScoreOfAllTime)
        {
            bestFitnessScoreOfAllTime = bestFitnessScoreThisGeneration;
            bestMap = bestMapThisGeneration.DeepClone();
            bestMapGenerationNumber = generationNumber;
        }

        generationNumber++;
        yield return new WaitForEndOfFrame();
        UIController.Instance.SetLoadingValue(generationNumber / (float)generationLimit);

        Debug.Log("Current generation " 
            + generationNumber 
            + " score: " 
            + bestMapThisGeneration);

        if (generationNumber < generationLimit)
        {
            var nextGeneration = new List<MapInitializer>();
            while (nextGeneration.Count < populationSize)
            {
                var parent1 = currentGeneration[RouletteWheelSelection()];
                var parent2 = currentGeneration[RouletteWheelSelection()];
                MapInitializer child1, child2;

                CrossoverParents(parent1, parent2, out child1, out child2);
                child1.AddMutation(mutationRatePercent);
                child2.AddMutation(mutationRatePercent);

                nextGeneration.Add(child1);
                nextGeneration.Add(child2);
            }
            currentGeneration = nextGeneration;
            StartCoroutine(GeneticAlgorithm());
        }
        else
        {
            ShowResults();
        }
    }

    private void ShowResults()
    {
        isAlgorithmRunning = false;
        Debug.Log("Best solution at generation " 
            + bestMapGenerationNumber 
            + " with score: " 
            + bestFitnessScoreOfAllTime);

        var data = bestMap.ReturnMapData();
        mapVisualizer.VisualizeMap(bestMap.Grid, data, true);

        UIController.Instance.HideLoadingScreen();

        Debug.Log("Path length: " + data.path);
        Debug.Log("Corners count: " + data.cornersList.Count);

        endDate = DateTime.Now;
        long elapsedTicks = endDate.Ticks - startDate.Ticks;
        var elapsedSpan = new TimeSpan(elapsedTicks);
        Debug.Log("Time needed to run this algorithm: " + elapsedSpan.TotalSeconds);
    }

    private void CrossoverParents(MapInitializer parent1, MapInitializer parent2, out MapInitializer child1, out MapInitializer child2)
    {
        child1 = parent1.DeepClone();
        child2 = parent2.DeepClone();

        if (Random.value < crossoverRatePercent)
        {
            int numBits = parent1.ObstaclesArray.Length;
            int crossoverIndex = Random.Range(0, numBits);

            for (int i = crossoverIndex; i < numBits; i++)
            {
                child1.PlaceObstacle(i, parent2.IsObstacleAt(i));
                child2.PlaceObstacle(i, parent1.IsObstacleAt(i));
            }
        }
    }

    private int RouletteWheelSelection()
    {
        int randomValue = Random.Range(0, totalFitnessThisGeneration);
        for (int i = 0; i < populationSize; i++)
        {
            randomValue -= CalculateFitness(currentGeneration[i].ReturnMapData());
            if (randomValue <= 0)
            {
                return i;
            }
        }
        return populationSize - 1;
    }

    private int CalculateFitness(MapData mapData)
    {
        int numberOfObstacles = mapData.obstacleArray.Where(isObstacle => isObstacle).Count();
        int fitnessScore = mapData.path.Count * fitnessPathWeight + (int)(numberOfObstacles * fitnessObstacleWeight);
        int cornersCount = mapData.cornersList.Count;
        if (cornersCount >= fitnessCornerMin && cornersCount <= fitnessCornerMax)
        {
            fitnessScore += cornersCount * fitnessCornerWeight;
        }
        else if (cornersCount > fitnessCornerMax)
        {
            fitnessScore -= fitnessCornerWeight * (cornersCount - fitnessCornerMax);
        }
        else if (cornersCount < fitnessCornerMin)
        {
            fitnessScore -= fitnessCornerWeight * fitnessCornerMin;
        }
        fitnessScore -= mapData.cornersNearEachOther * fitnessNearCornerWeight;
        return fitnessScore;
    }
}
