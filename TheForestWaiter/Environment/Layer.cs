namespace TheForestWaiter.Environment
{
    class Layer
    {
        public string Type { get; set; }
        public string Name { get; set;  }
        public int[] Data { get; set; }
        public MapObject[] Objects { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
    }
}
