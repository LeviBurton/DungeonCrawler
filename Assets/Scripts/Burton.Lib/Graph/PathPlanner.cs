using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burton.Lib.Graph
{
    public enum EPlanStatus { NoClosestNodeFound = -1 }

    public class PathPlanner
    {
        // The NavGraph
        SparseGraph<GraphNode, GraphEdge> Graph;

        // current graph search algorithm
        Graph_SearchTimeSliced<GraphEdge> CurrentSearch;

        public ESearchStatus CycleOnce()
        {
            ESearchStatus Result = CurrentSearch.CycleOnce();
            if (Result == ESearchStatus.TargetNotFound)
            {
            
            }
            else if (Result == ESearchStatus.TargetFound)
            {
            
                //if the search was for an item type then the final node in the path will
                //represent a giver trigger. Consequently, it's worth passing the pointer
                //to the trigger in the extra info field of the message. (The pointer
                //will just be NULL if no trigger)

                //void* pTrigger =
                //m_NavGraph.GetNode(m_pCurrentSearch->GetPathToTarget().back()).ExtraInfo();

                //Dispatcher->DispatchMsg(SEND_MSG_IMMEDIATELY,
                //                        SENDER_ID_IRRELEVANT,
                //                        m_pOwner->ID(),
                //                        Msg_PathReady,
                //                        pTrigger);
            }

            return Result;
        }
    }
}
