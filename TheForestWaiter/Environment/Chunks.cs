using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Entities;

namespace TheForestWaiter.Environment
{
    class Chunks
    { 
        public const float CHUNK_WIDTH = 200;
        public const int LOAD_DISTANCE_CHUNKS = 5;
        public int CurrentChunkId { get; private set; }

        public int TotalChunks => _totalChunks;
        private readonly int _totalChunks;
        private readonly Chunk[] _chunkArray;
        private readonly Chunk[] _activeChunks;

        public Chunks(World world)
        {
            int worldSize = (world.Tiles.GetLength(0) * World.TILE_SIZE);
            _totalChunks = (int)(worldSize / CHUNK_WIDTH);
            _chunkArray = new Chunk[TotalChunks];
            _activeChunks = new Chunk[LOAD_DISTANCE_CHUNKS * 2 + 1];

            for (int i = 0; i < _chunkArray.Length; i++)
                _chunkArray[i] = new Chunk();

            for (int i = 0; i < _activeChunks.Length; i++)
                _activeChunks[i] = _chunkArray[i];
            CurrentChunkId = LOAD_DISTANCE_CHUNKS;
        }

        private static int GetChunkIdAt(Vector2f location) => (int)(location.X / CHUNK_WIDTH);
        
        public void LoadChunksAt(Vector2f location)
        {
            var lastChunkId = CurrentChunkId;
            var loadChunkId = GetChunkIdAt(location);
            if (loadChunkId == lastChunkId)
                return;

            for (int i = -LOAD_DISTANCE_CHUNKS; i <= LOAD_DISTANCE_CHUNKS; i++)
            {
                var chunkId = loadChunkId + i;

                if (chunkId > -1 && chunkId < _chunkArray.Length)
                {
                    _activeChunks[i + LOAD_DISTANCE_CHUNKS] = _chunkArray[chunkId];
                }
                else
				{
                    _activeChunks[i + LOAD_DISTANCE_CHUNKS] = null;
				}
            }

            CurrentChunkId = loadChunkId;
        }

        public void Update(float time)
        {
            foreach(var i in _activeChunks)
            {
                i?.Update(time);
            }
        }

        public void Draw(RenderWindow win)
        {
            foreach (var i in _activeChunks)
            {
                i?.Draw(win);
            }
        }

        public void Add(StaticObject obj)
        {
            var chunk = GetChunkIdAt(obj.Position);

            if (chunk >= 0 && chunk < TotalChunks)
            { 
                _chunkArray[chunk].Objects.Add(obj);
            }
        }

        public IEnumerable<int> GetActiveChunks()
        {
            for (int i = -LOAD_DISTANCE_CHUNKS; i <= LOAD_DISTANCE_CHUNKS; i++)
            {
                yield return CurrentChunkId + i;
            }
        }
    }
}
