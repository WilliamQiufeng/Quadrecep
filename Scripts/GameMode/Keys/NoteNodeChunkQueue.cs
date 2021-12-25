using System.Collections.Concurrent;
using System.Collections.Generic;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class NoteNodeChunkQueue
    {
        private const int ChunkTimeInterval = 200;
        private const int TimeBeforeEntrance = 50;
        private int _currentChunkTime = -2;
        private readonly ConcurrentDictionary<int, ConcurrentQueue<NoteNode>> _chunks = new();
        private readonly ConcurrentQueue<NoteNode> _instantPullNext = new();

        public void Enqueue(NoteNode node)
        {
            var chunkTime = GetChunkTime(node.FirstAppearanceTime);
            if (chunkTime < _currentChunkTime)
            {
                _instantPullNext.Enqueue(node);
            }
            else
            {
                _chunks.GetOrAdd(chunkTime, _ => new ConcurrentQueue<NoteNode>()).Enqueue(node);
            }
        }

        public void Pull(float time, ref List<NoteNode> nodes)
        {
            while (!_instantPullNext.IsEmpty)
            {
                if (!_instantPullNext.TryDequeue(out var node)) continue;
                nodes.Add(node);
            }

            while (GetChunkTime(time + TimeBeforeEntrance) >= _currentChunkTime && !_chunks.IsEmpty)
            {
                if (_chunks.ContainsKey(_currentChunkTime))
                {
                    var res = _chunks.TryRemove(_currentChunkTime, out var chunk);
                    if (res)
                    {
                        nodes.AddRange(chunk);
                    }
                }

                _currentChunkTime++;
            }
        }

        private int GetChunkTime(float time) => Mathf.FloorToInt(time / ChunkTimeInterval);
    }
}