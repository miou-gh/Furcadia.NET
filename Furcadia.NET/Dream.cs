using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Furcadia.NET
{
    public class Floor : MapTile
    {
        public Floor(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Object : MapTile
    {
        public Object(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Wall : MapTile
    {
        public Wall(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Region : MapTile
    {
        public Region(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Effect : MapTile
    {
        public Effect(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Lighting : MapTile
    {
        public Lighting(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class Ambience : MapTile
    {
        public Ambience(int x, int y, int id) : base(x, y, id)
        {
        }
    }

    public class MapTile
    {
        public Point Location { get; set; }
        public int Id { get; set; }

        public MapTile(int x, int y, int id)
        {
            this.Location = new Point(x, y);
            this.Id = id;
        }
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) : this()
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class Dream
    {
        internal string Version { get; set; }
        internal Dictionary<string, string> Headers = new Dictionary<string, string>();
        internal List<MapTile> MapTiles = new List<MapTile>();
        internal int InternalWidth => int.Parse(this.Headers["width"]);
        internal int InternalHeight => int.Parse(this.Headers["height"]);

        public int Width => int.Parse(this.Headers["width"]) * 2;
        public int Height => int.Parse(this.Headers["height"]);
        public int Revision => this.Headers.ContainsKey("revision") ? int.Parse(this.Headers["revision"]) : 1;

        public string Name => this.Headers.ContainsKey("name") ? this.Headers["name"] : "";
        public string Rating => this.Headers.ContainsKey("rating") ? this.Headers["rating"] : "";
        public string PatchArchive => this.Headers.ContainsKey("patchs") ? this.Headers["patchs"] : "";

        public bool IsModern => !this.Headers.ContainsKey("ismodern") ? false : this.Headers["ismodern"] == "1";
        public bool PreventTabNameListing => !this.Headers.ContainsKey("notab") ? false : this.Headers["notab"] == "1";
        public bool PreventSeasonalAvatars => !this.Headers.ContainsKey("nonovelty") ? false : this.Headers["nonovelty"] == "1";
        public bool PreventPlayerListing => !this.Headers.ContainsKey("nowho") ? false : this.Headers["nowho"] == "1";
        public bool AllowShouting => !this.Headers.ContainsKey("allowshouts") ? false : this.Headers["allowshouts"] == "1";
        public bool StrictSittable => !this.Headers.ContainsKey("forcesittable") ? false : this.Headers["forcesittable"] == "1";
        public bool UseSwearFilter => !this.Headers.ContainsKey("swearfilter") ? false : this.Headers["swearfilter"] == "1";
        public bool EnforceParentalControls => !this.Headers.ContainsKey("parentalcontrols") ? false : this.Headers["parentalcontrols"] == "1";
        public bool EncodeDream => !this.Headers.ContainsKey("encoded") ? false : this.Headers["encoded"] == "1";
        public bool AllowLeadFollow => !this.Headers.ContainsKey("allowlf") ? false : this.Headers["allowlf"] == "1";
        public bool AllowLargeDreamSize => !this.Headers.ContainsKey("allowlarge") ? false : this.Headers["allowlarge"] == "1";
        public bool AllowJoinSummon => !this.Headers.ContainsKey("allowjs") ? false : this.Headers["allowjs"] == "1";
        public bool AllowDreamURL => !this.Headers.ContainsKey("allowfurl") ? false : this.Headers["allowfurl"] == "1";

        public Floor GetFloorAt(int x, int y) => IsValidNonWallTile(x, y) ? (Floor)this.MapTiles[GetPosFrom(x, y) - 1] :
            throw new InvalidCoordinatesException(x, y, $"The floor tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Object GetObjectAt(int x, int y) => IsValidNonWallTile(x, y) ? (Object)this.MapTiles[GetPosFrom(x, y) - 1 + (this.LayerSize)] :
            throw new InvalidCoordinatesException(x, y, $"The object tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Wall GetWallAt(int x, int y) => IsWithinRange(x, y, true) ? (Wall)this.MapTiles[((this.InternalHeight * x) + y) + (this.LayerSize * 2)] :
            throw new InvalidCoordinatesException(x, y, $"The wall tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Region GetRegionAt(int x, int y) => IsValidNonWallTile(x, y) ? (Region)this.MapTiles[GetPosFrom(x, y) + (this.LayerSize * 4)] :
            throw new InvalidCoordinatesException(x, y, $"The region tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Effect GetEffectAt(int x, int y) => IsValidNonWallTile(x, y) ? (Effect)this.MapTiles[GetPosFrom(x, y) + (this.LayerSize * 5)] :
            throw new InvalidCoordinatesException(x, y, $"The effect tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Lighting GetLightingAt(int x, int y) => IsValidNonWallTile(x, y) ? (Lighting)this.MapTiles[GetPosFrom(x, y) + (this.LayerSize * 6)] :
            throw new InvalidCoordinatesException(x, y, $"The lighting tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Ambience GetAmbienceAt(int x, int y) => IsValidNonWallTile(x, y) ? (Ambience)this.MapTiles[GetPosFrom(x, y) + (this.LayerSize * 7)] :
            throw new InvalidCoordinatesException(x, y, $"The ambience tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Wall NorthEastWall(Wall wall) =>
            wall.Location.X % 2 == 0 ? this.GetWallAt(wall.Location.X + 1, wall.Location.Y) :
                                       this.GetWallAt(wall.Location.X, wall.Location.Y);

        public Wall NorthWestWall(Wall wall) =>
            wall.Location.X % 2 == 0 ? this.GetWallAt(wall.Location.X, wall.Location.Y) :
                                       this.GetWallAt(wall.Location.X - 1, wall.Location.Y);

        internal Dream()
        {
        }

        public static Dream FromFile(string fileName)
        {
            return new Dream().ParseFromBytes(File.ReadAllBytes(fileName));
        }

        public static Dream FromBytes(byte[] input)
        {
            return new Dream().ParseFromBytes(input);
        }

        private Dream ParseFromBytes(byte[] input)
        {
            var header_eol = new byte[] { 0x0A, 0x42, 0x4F, 0x44, 0x59, 0x0A }; // \nbody\n
            var header_footer = Extensions.PatternAt(input, header_eol).First() + header_eol.Length + 1; // body begins here

            var metadata = new StreamReader(new MemoryStream(input, 0, header_footer)).ReadToEnd()
                .Split('\n').Select(meta => meta.Split('='));

            this.Version = metadata.First()[0];
            this.Headers = metadata.GroupBy(x => x[0]).Select(y => y.Last()) // in case of duplicates, always use the last
                            .Where(element => element.Length == 2).Distinct().ToDictionary(t => t[0], t => t[1]);

            if (!new List<string> { "MAP V01.50 Furcadia" }.Contains(this.Version))
                throw new DreamVersionUnsupportedException($"The dream version '{this.Version}' is currently unsupported.");

            if (this.Version == "MAP V01.50 Furcadia") {
                var br = new BinaryReader(new MemoryStream(input), Encoding.GetEncoding(1252));
                br.ReadBytes(header_footer);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Floor(
                         x: ((i + i) / this.InternalHeight),
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + this.LayerChunkSize, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Object(
                         x: ((i + i) / this.InternalHeight),
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 2) - 1, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerChunkSize)
                    select new Wall(
                         x: ((i + 1) / this.InternalHeight),
                         y: i % this.InternalHeight,
                         id: br.ReadByte()));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 3) - 2, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Region(
                         x: ((i + i) / this.InternalHeight) * 2,
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 4) - 2, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Effect(
                         x: ((i + i) / this.InternalHeight) * 2,
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 5) - 2, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Lighting(
                         x: ((i + i) / this.InternalHeight) * 2,
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 6) - 2, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Ambience(
                         x: ((i + i) / this.InternalHeight) * 2,
                         y: (i + 1) % this.InternalHeight,
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));
            }

            return this;
        }

        private int LayerChunkSize => (this.InternalWidth * this.InternalHeight) * 2;
        private int LayerSize => this.InternalWidth * this.InternalHeight;

        private bool IsValidNonWallTile(int x, int y) => IsWithinRange(x, y) && x % 2 == 0;
        private bool IsWithinRange(int x, int y, bool wall = false) => x >= 0 && y >= 0 && x <= (this.InternalWidth * 2) - (!wall ? 2 : 1) && y <= this.InternalHeight - 1;

        internal int GetPosFrom(int x, int y) => (this.InternalHeight * (x / 2)) + y;
    }

    public static class Extensions
    {
        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (var i = 0; i < source.Length; i++)
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                    yield return i;
        }
    }


    [Serializable]
    public class DreamVersionUnsupportedException : Exception
    {
        public DreamVersionUnsupportedException() { }
        public DreamVersionUnsupportedException(string message) : base(message) { }
        public DreamVersionUnsupportedException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public class InvalidCoordinatesException : Exception
    {
        public Point Location { get; set; }

        public InvalidCoordinatesException(int x, int y) { this.Location = new Point(x, y); }
        public InvalidCoordinatesException(int x, int y, string message) : base(message) { this.Location = new Point(x, y); }
        public InvalidCoordinatesException(string message, Exception inner) : base(message, inner) { }
    }
}
