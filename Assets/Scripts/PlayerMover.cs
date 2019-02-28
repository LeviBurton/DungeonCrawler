using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : Mover
{
   // PlayerCompass m_playerCompass;

    protected override void Awake()
    {
        base.Awake();

      //  m_playerCompass = Object.FindObjectOfType<PlayerCompass>().GetComponent<PlayerCompass>();
    }

    protected override void Start()
    {
        base.Start();

        UpdateBoard();
    }

    void UpdateBoard()
    {
        //if (m_board != null)
        //{
        //    m_board.UpdatePlayerNode();
        //}
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        // disable PlayerCompass arrows
        //if (m_playerCompass != null)
        //{
        //    m_playerCompass.ShowArrows(false);
        //}

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
