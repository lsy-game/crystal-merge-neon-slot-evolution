using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownNavigationRouteDebugger : MonoBehaviour
    {
        public BubuTownNavigationGraph Graph;
        public string StartAnchorId = "nav.central_plaza";
        public string EndAnchorId = "nav.garage_entry";
        public string[] RequiredWaypointAnchorIds = new string[0];
        public BubuTownNavigationAnchor[] LastRoute = new BubuTownNavigationAnchor[0];

        private void Awake()
        {
            RefreshRoute();
        }

        private void OnValidate()
        {
            RefreshRoute();
        }

        public void RefreshRoute()
        {
            if (Graph == null)
            {
                var graphs = FindObjectsByType<BubuTownNavigationGraph>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                Graph = graphs.Length > 0 ? graphs[0] : null;
            }

            LastRoute = Graph != null ? BuildWaypointRoute() : new BubuTownNavigationAnchor[0];
        }

        public bool HasValidRoute()
        {
            return LastRoute != null && LastRoute.Length >= 2;
        }

        private BubuTownNavigationAnchor[] BuildWaypointRoute()
        {
            if (RequiredWaypointAnchorIds == null || RequiredWaypointAnchorIds.Length == 0)
            {
                return Graph.BuildRoute(StartAnchorId, EndAnchorId);
            }

            var route = new System.Collections.Generic.List<BubuTownNavigationAnchor>();
            var currentStart = StartAnchorId;
            for (var i = 0; i <= RequiredWaypointAnchorIds.Length; i++)
            {
                var currentEnd = i < RequiredWaypointAnchorIds.Length ? RequiredWaypointAnchorIds[i] : EndAnchorId;
                var segment = Graph.BuildRoute(currentStart, currentEnd);
                if (segment == null || segment.Length < 2)
                {
                    return new BubuTownNavigationAnchor[0];
                }

                for (var j = 0; j < segment.Length; j++)
                {
                    if (route.Count > 0 && j == 0)
                    {
                        continue;
                    }

                    route.Add(segment[j]);
                }

                currentStart = currentEnd;
            }

            return route.ToArray();
        }
    }
}
