using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph;
using System;

public class GameManager : MonoBehaviour
{
    Board board;

    void Awake()
    {
        board = FindObjectOfType<Board>();
    }

}

