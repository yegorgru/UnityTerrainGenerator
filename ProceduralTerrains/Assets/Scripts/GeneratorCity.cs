using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorCity
{
    public TileCity GenerateCityTile(Vector2 coordinates, int widthOfRegion, int lengthOfRegion, Transform transform)
    {
        float xOffset = widthOfRegion / -2f + 0.5f;
        float yOffset = lengthOfRegion / -2f + 0.5f;

        Vector2 viewedChunkCoord = new Vector2(xOffset + coordinates.x, yOffset + coordinates.y);

        TileCity chunk = new TileCity(viewedChunkCoord, 240, transform, 100f);

        RoadItem[,] roadItems = GenerateRoadMap(10, 10, 10);
        return chunk;
    }

    public RoadItem[,] GenerateRoadMap(int width, int length, int steps)
    {
        RoadItem[,] roadMap = new RoadItem[width, length];
        int x = width / 2;
        int y = length / 2;
        roadMap[x, y] = RoadItem.CreateStartRoadItem();

        Queue<Vector2Int> queueToProcess = new Queue<Vector2Int>();
        queueToProcess.Enqueue(new Vector2Int(x + 1, y));
        queueToProcess.Enqueue(new Vector2Int(x - 1, y));
        queueToProcess.Enqueue(new Vector2Int(x, y + 1));
        queueToProcess.Enqueue(new Vector2Int(x, y - 1));

        for (int step = 0; step < steps; ++step)
        {
            if (queueToProcess.Count == 0)
            {
                return roadMap;
            }
            Queue<Vector2Int> queueToProcessNext = new Queue<Vector2Int>();
            while (queueToProcess.Count != 0)
            {
                Vector2Int v = queueToProcess.Dequeue();
                RoadItem roadItem = roadMap[v.x, v.y];
                if (v.x < 1 || v.x > width - 2 || v.y < 1 || v.y > length - 2)
                {
                    continue;
                }
                RoadItem up = roadMap[v.x + 1, v.y];
                RoadItem right = roadMap[v.x, v.y + 1];
                RoadItem down = roadMap[v.x - 1, v.y];
                RoadItem left = roadMap[v.x, v.y - 1];
                roadItem.Process(up, right, down, left);
                roadMap[v.x, v.y] = roadItem;
                if (!up.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x + 1, v.y));
                }
                if (!right.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y + 1));
                }
                if (!down.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x - 1, v.y));
                }
                if (!left.IsProcessed())
                {
                    queueToProcessNext.Enqueue(new Vector2Int(v.x, v.y - 1));
                }
            }
            queueToProcess = queueToProcessNext;
        }
        return roadMap;
    }
}

public struct RoadItem
{
    static public RoadItem CreateStartRoadItem()
    {
        RoadItem roadItem = new RoadItem();
        roadItem.roadUp = true;
        roadItem.roadDown = true;
        roadItem.isProcessed = true;
        return roadItem;
    }

    private bool roadUp;
    private bool roadRight;
    private bool roadDown;
    private bool roadLeft;
    private bool isProcessed;
    static private float newRoadChance = 0.5f;

    public RoadItem(bool roadUp = false, bool roadRight = false, bool roadDown = false, bool roadLeft = false, bool isProcessed = false)
    {
        this.roadUp = roadUp;
        this.roadRight = roadRight;
        this.roadDown = roadDown;
        this.roadLeft = roadLeft;
        this.isProcessed = isProcessed;
    }

    public void Process(RoadItem up, RoadItem right, RoadItem down, RoadItem left)
    {
        if (!up.IsProcessed())
        {
            roadUp = Random.Range(0f, 1f) < newRoadChance;
        }
        else if(up.roadDown)
        {
            roadUp = true;
        }
        if (!down.IsProcessed())
        {
            roadDown = Random.Range(0f, 1f) < newRoadChance;
        }
        else if (down.roadUp)
        {
            roadDown = true;
        }
        if (!left.IsProcessed())
        {
            roadLeft = Random.Range(0f, 1f) < newRoadChance;
        }
        else if (left.roadRight)
        {
            roadLeft = true;
        }
        if (!right.IsProcessed())
        {
            roadRight = Random.Range(0f, 1f) < newRoadChance;
        }
        else if (right.roadLeft)
        {
            roadRight = true;
        }
        isProcessed = true;
    }

    public bool IsProcessed()
    {
        return isProcessed;
    }
}