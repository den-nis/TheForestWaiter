using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;

namespace TheForestWaiter.Environment
{
    class Chunks : IUpdateDraw
    { 
        class Chunk : IUpdateDraw
        {
            public GameObjectContainer<StaticObject> Objects { get; } = new GameObjectContainer<StaticObject>();

            public void Draw(RenderWindow window)
            {
                Objects.Draw(window);
            }

            public void Update(float time)
            {
                Objects.Update(time);
            }

            public void Unloaded()
            {
                Objects.CleanupMarkedForDeletion();
            }
        }

        public const float CHUNK_WIDTH = 100;
        public const int LOAD_DISTANCE_CHUNKS = 5;
        private int TotalChunks { get; set; }
        private Chunk[] ChunkArray { get; set; }
        private Chunk[] ActiveChunks { get; set; }
        private int LastChunkId { get; set; }

        public Chunks(World world)
        {
            int worldSize = (world.Tiles.GetLength(0) * World.TILE_SIZE);
            TotalChunks = (int)(worldSize / CHUNK_WIDTH);
            ChunkArray = new Chunk[TotalChunks];
            ActiveChunks = new Chunk[LOAD_DISTANCE_CHUNKS * 2];

            for (int i = 0; i < ChunkArray.Length; i++)
                ChunkArray[i] = new Chunk();

            for (int i = 0; i < ActiveChunks.Length; i++)
                ActiveChunks[i] = ChunkArray[i]; 
        }

        private static int GetChunkIdAt(Vector2f location) => (int)(location.X / CHUNK_WIDTH);
        
        public void LoadChunksAt(Vector2f location)
        {
            var chunk = GetChunkIdAt(location);
            if (chunk == LastChunkId)
                return;
            

            if (chunk < LOAD_DISTANCE_CHUNKS)
                chunk = LOAD_DISTANCE_CHUNKS;

            for (int i = -LOAD_DISTANCE_CHUNKS; i <= LOAD_DISTANCE_CHUNKS; i++)
            {
                var chunkId = chunk + i;
                var previousChunk = LastChunkId + i;
                if (previousChunk < -LOAD_DISTANCE_CHUNKS || previousChunk > LOAD_DISTANCE_CHUNKS)
                    ChunkArray[previousChunk].Unloaded();

                ActiveChunks[i] = ChunkArray[chunkId];
            }

            LastChunkId = chunk;
        }

        public void Update(float time)
        {
            foreach(var i in ActiveChunks)
            {
                i.Update(time);
            }
        }

        public void Draw(RenderWindow win)
        {
            foreach (var i in ActiveChunks)
            {
                i.Draw(win);
            }
        }

        public void Add(StaticObject obj)
        {
            var chunk = GetChunkIdAt(obj.Position);
            ChunkArray[chunk].Objects.Add(obj);
        }
    }
}
