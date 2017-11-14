using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DreamHeader : Attribute
    {
        public string HeaderName;

        public DreamHeader(string headerName)
        {
            this.HeaderName = headerName;
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

        [DreamHeader("revision")]
        public int Revision { get => int.Parse(GetHeader(() => Revision)); set => SetHeader(() => Revision, value.ToString()); }

        [DreamHeader("name")]
        public string Name { get => GetHeader(() => Name); set => SetHeader(() => Name, value); }

        [DreamHeader("rating")]
        public string Rating { get => GetHeader(() => Rating); set => SetHeader(() => Rating, value); }

        [DreamHeader("patchs")]
        public string PatchArchive { get => GetHeader(() => PatchArchive); set => SetHeader(() => PatchArchive, value); }

        [DreamHeader("ismodern")]
        public bool IsModern { get => GetHeader(() => IsModern) == "1"; set => SetHeader(() => IsModern, value ? "1" : "0"); }

        [DreamHeader("notab")]
        public bool PreventTabNameListing { get => GetHeader(() => PreventTabNameListing) == "1"; set => SetHeader(() => PreventTabNameListing, value ? "1" : "0"); }

        [DreamHeader("nonovelty")]
        public bool PreventSeasonalAvatars { get => GetHeader(() => PreventSeasonalAvatars) == "1"; set => SetHeader(() => PreventSeasonalAvatars, value ? "1" : "0"); }

        [DreamHeader("nowho")]
        public bool PreventPlayerListing { get => GetHeader(() => PreventPlayerListing) == "1"; set => SetHeader(() => PreventPlayerListing, value ? "1" : "0"); }

        [DreamHeader("allowshouts")]
        public bool AllowShouting { get => GetHeader(() => AllowShouting) == "1"; set => SetHeader(() => AllowShouting, value ? "1" : "0"); }

        [DreamHeader("forcesittable")]
        public bool StrictSittable { get => GetHeader(() => StrictSittable) == "1"; set => SetHeader(() => StrictSittable, value ? "1" : "0"); }

        [DreamHeader("swearfilter")]
        public bool UseSwearFilter { get => GetHeader(() => UseSwearFilter) == "1"; set => SetHeader(() => UseSwearFilter, value ? "1" : "0"); }

        [DreamHeader("parentalcontrols")]
        public bool EnforceParentalControls { get => GetHeader(() => EnforceParentalControls) == "1"; set => SetHeader(() => EnforceParentalControls, value ? "1" : "0"); }

        [DreamHeader("encoded")]
        public bool EncodeDream { get => GetHeader(() => EncodeDream) == "1"; set => SetHeader(() => EncodeDream, value ? "1" : "0"); }

        [DreamHeader("allowlf")]
        public bool AllowLeadFollow { get => GetHeader(() => AllowLeadFollow) == "1"; set => SetHeader(() => AllowLeadFollow, value ? "1" : "0"); }

        [DreamHeader("allowlarge")]
        public bool AllowLargeDreamSize { get => GetHeader(() => AllowLargeDreamSize) == "1"; set => SetHeader(() => AllowLargeDreamSize, value ? "1" : "0"); }

        [DreamHeader("allowjs")]
        public bool AllowJoinSummon { get => GetHeader(() => AllowJoinSummon) == "1"; set => SetHeader(() => AllowJoinSummon, value ? "1" : "0"); }

        [DreamHeader("allowfurl")]
        public bool AllowDreamURL { get => GetHeader(() => AllowDreamURL) == "1"; set => SetHeader(() => AllowDreamURL, value ? "1" : "0"); }

        public string GetHeader<T>(Expression<Func<T>> property) => 
            this.Headers[((DreamHeader)(((MemberExpression)property.Body).Member.GetCustomAttributes(typeof(DreamHeader), true).First())).HeaderName];

        public void SetHeader<T>(Expression<Func<T>> property, string value) => 
            this.Headers[((DreamHeader)(((MemberExpression)property.Body).Member.GetCustomAttributes(typeof(DreamHeader), true).First())).HeaderName] = value;

        public Floor GetFloorAt(int x, int y) => IsValidNonWallTile(x, y) ? (Floor)this.MapTiles[GetPosFrom(x, y)] :
            throw new InvalidCoordinatesException(x, y, $"The floor tile co-ordinate ({x}, {y}) is either invalid or is out of range.");

        public Object GetObjectAt(int x, int y) => IsValidNonWallTile(x, y) ? (Object)this.MapTiles[GetPosFrom(x, y) + (this.LayerSize)] :
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
            var header_footer = Extensions.PatternAt(input, header_eol).First() + header_eol.Length - 1; // body begins here

            var metadata = new StreamReader(new MemoryStream(input, 0, header_footer)).ReadToEnd()
                .Split('\n').Select(meta => meta.Split('='));

            this.Version = metadata.First()[0];
            this.Headers = metadata.GroupBy(x => x[0]).Select(y => y.Last()) // in case of duplicates, always use the last
                            .Where(element => element.Length == 2).Distinct().ToDictionary(t => t[0], t => t[1]);

            if (!new List<string> { "MAP V01.50 Furcadia" }.Contains(this.Version))
                throw new DreamVersionUnsupportedException($"The dream version '{this.Version}' is currently unsupported.");

            if (this.Version == "MAP V01.50 Furcadia") {
                var br = new BinaryReader(new MemoryStream(input), Encoding.GetEncoding(1252));
                br.BaseStream.Seek(header_footer, SeekOrigin.Begin);
                
                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Floor(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + this.LayerChunkSize, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Object(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 2) + 1, SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerChunkSize)
                    select new Wall(
                         x: (i + 1 / this.Height) * 2,
                         y: (i % this.Height),
                         id: br.ReadByte()));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 3), SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Region(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 4), SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Effect(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 5), SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Lighting(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));

                br.BaseStream.Seek(header_footer + (this.LayerChunkSize * 6), SeekOrigin.Begin);

                this.MapTiles.AddRange(
                    from i in Enumerable.Range(0, this.LayerSize)
                    select new Ambience(
                         x: (i / this.Height) * 2,
                         y: (i % this.Height),
                         id: BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0)));
            }

            return this;
        }

        public void Save(string fileName, bool overwrite = true)
        {
            if (File.Exists(fileName) && !overwrite)
                return;

            using (var fs = new FileStream(fileName, FileMode.Create)) {
                using (var bw = new BinaryWriter(fs, Encoding.GetEncoding(1252))) {
                    var header = new List<string> { this.Version };
                        header.AddRange(this.Headers.Select(h => h.Key + "=" + h.Value));
                        header.Add("BODY\n");

                    bw.Write(Encoding.GetEncoding(1252).GetBytes(string.Join("\n", header)));
                    
                    bw.Write(this.MapTiles.Where(tile => tile is Floor).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Object).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Wall).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).Select(us => (byte)us.Id).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Region).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Effect).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Lighting).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                    bw.Write(this.MapTiles.Where(tile => tile is Ambience).OrderBy(n => n.Location.X).ThenBy(n => n.Location.Y).SelectMany(us => BitConverter.GetBytes((ushort)us.Id)).ToArray());
                }
            }
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
