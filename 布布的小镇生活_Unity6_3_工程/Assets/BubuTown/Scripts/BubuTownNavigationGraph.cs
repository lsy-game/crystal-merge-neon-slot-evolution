using System.Collections.Generic;
using UnityEngine;

namespace BubuTown
{
    public sealed class BubuTownNavigationGraph : MonoBehaviour
    {
        public BubuTownNavigationAnchor[] Anchors = new BubuTownNavigationAnchor[0];

        private void Awake()
        {
            RefreshAnchors();
        }

        private void OnValidate()
        {
            RefreshAnchors();
        }

        public void RefreshAnchors()
        {
            Anchors = FindObjectsByType<BubuTownNavigationAnchor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        public BubuTownNavigationAnchor FindAnchor(string anchorId)
        {
            if (Anchors == null || Anchors.Length == 0)
            {
                RefreshAnchors();
            }

            foreach (var anchor in Anchors)
            {
                if (anchor != null && anchor.AnchorId == anchorId)
                {
                    return anchor;
                }
            }

            return null;
        }

        public BubuTownNavigationAnchor ClosestAnchor(Vector3 worldPosition, string zoneId)
        {
            if (Anchors == null || Anchors.Length == 0)
            {
                RefreshAnchors();
            }

            BubuTownNavigationAnchor best = null;
            var bestDistance = float.MaxValue;
            foreach (var anchor in Anchors)
            {
                if (anchor == null || (!string.IsNullOrEmpty(zoneId) && anchor.ZoneId != zoneId))
                {
                    continue;
                }

                var distance = Vector3.SqrMagnitude(anchor.WorldTarget - worldPosition);
                if (distance < bestDistance)
                {
                    best = anchor;
                    bestDistance = distance;
                }
            }

            return best;
        }

        public BubuTownNavigationAnchor[] ConnectedAnchors(string anchorId)
        {
            var anchor = FindAnchor(anchorId);
            if (anchor == null || anchor.ConnectedAnchorIds == null)
            {
                return new BubuTownNavigationAnchor[0];
            }

            var result = new BubuTownNavigationAnchor[anchor.ConnectedAnchorIds.Length];
            for (var i = 0; i < anchor.ConnectedAnchorIds.Length; i++)
            {
                result[i] = FindAnchor(anchor.ConnectedAnchorIds[i]);
            }

            return result;
        }

        public BubuTownNavigationAnchor[] BuildRoute(string startAnchorId, string endAnchorId)
        {
            RefreshAnchors();
            var start = FindAnchor(startAnchorId);
            var end = FindAnchor(endAnchorId);
            if (start == null || end == null)
            {
                return new BubuTownNavigationAnchor[0];
            }

            var queue = new Queue<string>();
            var visited = new HashSet<string>();
            var previous = new Dictionary<string, string>();
            queue.Enqueue(start.AnchorId);
            visited.Add(start.AnchorId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (currentId == end.AnchorId)
                {
                    return RebuildRoute(previous, start.AnchorId, end.AnchorId);
                }

                var current = FindAnchor(currentId);
                if (current == null || current.ConnectedAnchorIds == null)
                {
                    continue;
                }

                foreach (var nextId in current.ConnectedAnchorIds)
                {
                    if (string.IsNullOrEmpty(nextId) || visited.Contains(nextId))
                    {
                        continue;
                    }

                    if (FindAnchor(nextId) == null)
                    {
                        continue;
                    }

                    visited.Add(nextId);
                    previous[nextId] = currentId;
                    queue.Enqueue(nextId);
                }
            }

            return new BubuTownNavigationAnchor[0];
        }

        private BubuTownNavigationAnchor[] RebuildRoute(Dictionary<string, string> previous, string startAnchorId, string endAnchorId)
        {
            var ids = new List<string>();
            var current = endAnchorId;
            ids.Add(current);
            while (current != startAnchorId && previous.ContainsKey(current))
            {
                current = previous[current];
                ids.Add(current);
            }

            ids.Reverse();
            var route = new List<BubuTownNavigationAnchor>();
            foreach (var id in ids)
            {
                var anchor = FindAnchor(id);
                if (anchor != null)
                {
                    route.Add(anchor);
                }
            }

            return route.ToArray();
        }
    }
}
