using Burton.Lib.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Burton.Lib.Unity
{
    [ExecuteInEditMode]
    public class UnityPathManager : MonoBehaviour
    {
        
        public bool DrawSearchPaths = true;
        public int NumSearchCyclesPerUpdate;

        public Color StartNodeColor;
        public Color EndNodeColor;
        public Color SearchPathColor = Color.green;
        public Color FrontierPathColor = Color.yellow;

        public Vector3 TargetPosition;

        public List<UnityPathPlanner> SearchRequests = new List<UnityPathPlanner>();

        public void ClearSearches()
        {
            SearchRequests.Clear();
        }

        private void DrawPathsToTargets()
        {
            Gizmos.color = SearchPathColor;

            for (int CurSearchIndex = 0; CurSearchIndex < SearchRequests.Count; CurSearchIndex++)
            {
                var SearchRequest = SearchRequests[CurSearchIndex];

                if (!SearchRequest.PathToTarget.Any())
                    continue;

                var StartNode = SearchRequest.Graph.GetNode(SearchRequest.Search.SourceNodeIndex);
                var TargetNode = SearchRequest.Graph.GetNode(SearchRequest.Search.TargetNodeIndex);

                for (int i = 0; i < SearchRequest.PathToTarget.Count; i++)
                {
                    var PathEdge = SearchRequest.PathToTarget[i];
                    var FromNode = SearchRequest.Graph.GetNode(PathEdge.FromIndex);
                    var ToNode = SearchRequest.Graph.GetNode(PathEdge.ToIndex);

                    Vector3 SpherePosition = new Vector3(FromNode.Position.x, (FromNode.Position.y + SearchRequest.UnityGraph.TileWidth / 4), FromNode.Position.z);

                    Gizmos.color *= 1.25f;
                    Gizmos.DrawSphere(SpherePosition + (Vector3.up * 0.5f), 0.10f);
                    Gizmos.color = SearchPathColor;
                    Gizmos.DrawLine(FromNode.Position + (Vector3.up * 0.5f), ToNode.Position + (Vector3.up * 0.5f));
                }
            }
        }

        void Start()
        {
            SearchRequests = new List<UnityPathPlanner>();
        }

        void Update()
        {
            UpdateSearches();
        }

        public void Register(UnityPathPlanner PathPlanner)
        {
            if (!SearchRequests.Contains(PathPlanner))
            {
                SearchRequests.Add(PathPlanner);
            }
        }

        public void UnRegister(UnityPathPlanner PathPlanner)
        {
            SearchRequests.Remove(PathPlanner);
        }

        public int GetNumActiveSearches()
        {
            return SearchRequests.Count;
        }

        public void UpdateSearches()
        {
            int NumCyclesRemaining = NumSearchCyclesPerUpdate;
            int CurSearchIndex = 0;

            while (NumCyclesRemaining-- > 0 && SearchRequests.Any())
            {
                var SearchRequest = SearchRequests[CurSearchIndex];
                ESearchStatus Result = SearchRequest.CycleOnce();

                if (Result == ESearchStatus.TargetFound)
                {
                    //SearchRequests.RemoveAt(CurSearchIndex);
                }
                else if (Result == ESearchStatus.TargetNotFound)
                {
                    //SearchRequests.RemoveAt(CurSearchIndex);
                }
                else
                {
                    // go to next search
                    CurSearchIndex++;
                }

                // if we are at the end, reset to beginning.
                if (CurSearchIndex >= SearchRequests.Count)
                {
                    CurSearchIndex = 0;
                }
            }
        }
    }
}