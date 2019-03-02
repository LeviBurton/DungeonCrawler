using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : Mover
{
    // PlayerCompass m_playerCompass;

    Board board;

    protected override void Awake()
    {
        base.Awake();

        //  m_playerCompass = Object.FindObjectOfType<PlayerCompass>().GetComponent<PlayerCompass>();
        board = FindObjectOfType<Board>();
        m_graphView = board.GetComponent<GraphView>();
    }

    protected override void Start()
    {
        base.Start();

        UpdateBoard();
        UpdateCurrentNode();
    }

    void UpdateBoard()
    {
        if (board != null)
        {
            board.UpdatePlayerNode();
        }
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        // run the parent class MoveRoutine
        yield return StartCoroutine(base.MoveRoutine(destinationPos, delayTime));

        // update the Board's PlayerNode
        UpdateBoard();

        //// enable PlayerCompass arrows
        //if (m_playerCompass != null)
        //{
        //    m_playerCompass.ShowArrows(true);
        //}

        // make sure this runs before we finish the turn
        base.finishMovementEvent.Invoke();
    }
}
