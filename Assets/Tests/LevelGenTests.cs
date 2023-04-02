using System;
using LevelGeneration;
using NUnit.Framework;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

namespace Tests
{
    public class LevelGenTests
    {
        /*
         * Acceptance Tests
         */
        
        // The tests prefixed with "RoomPriority" test the room priority system, making sure that each time the generator
        // attempts to place multiple rooms in a single area, the room with the higher priority remains.

        // Optional rooms should have the lowest priority, and be overwritten by everything.
        [Test]
        public void RoomPriorityOptional()
        {
            var generator = new GridWorldGenerator();

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.Optional);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpen);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.LeftRightOpen);
        }

        // LeftRightOpen rooms should only overwrite Optional rooms.
        [Test]
        public void RoomPriorityLeftRightOpen()
        {
            var generator = new GridWorldGenerator();

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.LeftRightOpen);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpen);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.LeftRightOpen);
        }

        // AllOpen rooms should only overwrite Optional and LeftRightOpen rooms.
        [Test]
        public void RoomPriorityAllOpen()
        {
            var generator = new GridWorldGenerator();

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpen);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpen);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpen);
        }

        // AllOpenSpawn rooms should overwrite everything.
        [Test]
        public void RoomPriorityAllOpenSpawn()
        {
            var generator = new GridWorldGenerator();

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenSpawn);
        }

        // AllOpenExit rooms should overwrite everything.
        [Test]
        public void RoomPriorityAllOpenExit()
        {
            var generator = new GridWorldGenerator();

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.Optional);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenSpawn);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);

            generator.ClearRoomTypes();
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.AllOpenExit);
            generator.SetRoom(0, 0, GridWorldGenerator.RoomType.LeftRightOpen);
            Assert.AreEqual(generator.GetRoom(0, 0), GridWorldGenerator.RoomType.AllOpenExit);
        }
        
        // The tests prefixed with "IsOccupied" check that the map is able to correctly identify when one of its tiles
        // is occupied (meaning that it has a collider preventing entities from entering it) and when it is not.

        [Test]
        public void IsOccupiedDetectsFullTile()
        {
            var map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
            map.SetTile(TileType.Grass, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Dirt, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Sand, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Floor, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Path, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Inkwell, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.OpenBook, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.ClosedBook, 0, 0);
            Assert.IsFalse(map.IsTileOccupied(0, 0));
        }
        
        [Test]
        public void IsOccupiedDoesNotDetectEmptyTile()
        {
            var map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
            map.SetTile(TileType.Tree, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Wall, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Bush, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Bin, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Bucket, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Cabinet, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.DownChair, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Dresser, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.RightChair, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
            map.SetTile(TileType.Table, 0, 0);
            Assert.IsTrue(map.IsTileOccupied(0, 0));
        }

        /*
         * White Box Tests for the following function:
         * private static void ValidateRoom(string name, string room, int roomI, bool horizontalMustBeOpen, bool verticalMustBeOpen)
         * {
         *     const int verticalOpeningEnd = VerticalOpeningStart + VerticalOpeningSize;
         *     const int horizontalOpeningEnd = HorizontalOpeningStart + HorizontalOpeningSize;
         *     
         *     for (var y = 0; y < RoomHeight; y++)
         *     {
         *         if (verticalMustBeOpen && y is 0 or RoomHeight - 1)
         *         {
         *             for (var x = VerticalOpeningStart; x < verticalOpeningEnd; x++)
         *             {
         *                 var xChar = room[x + y * RoomWidth];
         *                 var charCategory = CharCategories[xChar];
         *                 if (charCategory is TileCategory.Floor or TileCategory.Ground) continue;
         *  
         *                 throw new ArgumentException($"Room set '{name}' is missing vertical opening in room {roomI}");
         *             }
         *         }
         *  
         *         if (horizontalMustBeOpen && y is >= HorizontalOpeningStart and < horizontalOpeningEnd)
         *         {
         *             var leftChar = room[y * RoomWidth];
         *             var leftCharCategory = CharCategories[leftChar];
         *             var rightChar = room[RoomWidth - 1 + y * RoomWidth];
         *             var rightCharCategory = CharCategories[rightChar];
         *  
         *             if (leftCharCategory is not TileCategory.Floor and not TileCategory.Ground ||
         *                 rightCharCategory is not TileCategory.Floor and not TileCategory.Ground)
         *             {
         *                 throw new ArgumentException($"Room set '{name}' is missing horizontal opening in room {roomI}");
         *             }
         *         }
         *     }
         * }
         */

        // Along with "ValidateVertical", achieves statement coverage.
        [Test]
        public void ValidateHorizontal()
        {
            var validRoom = new Room(RoomCategory.Indoor,
                "##########" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "__________" +
                "##########");

            var invalidRoom = new Room(RoomCategory.Indoor,
                "##########" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "_________#" +
                "##########");

            var invalidRoom2 = new Room(RoomCategory.Indoor,
                "##########" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "#_________" +
                "##########");

            Assert.DoesNotThrow(() => { Rooms.ValidateRoom(nameof(validRoom), validRoom, 0, true, false); });

            Assert.Throws<ArgumentException>(() =>
            {
                Rooms.ValidateRoom(nameof(invalidRoom), invalidRoom, 0, true, false);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Rooms.ValidateRoom(nameof(invalidRoom2), invalidRoom2, 0, true, false);
            });
        }

        [Test]
        public void ValidateVertical()
        {
            var validRoom = new Room(RoomCategory.Indoor,
                "####__####" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "#________#" +
                "####__####");

            var invalidRoom = new Room(RoomCategory.Indoor,
                "##########" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "#________#" +
                "####__####");

            var invalidRoom2 = new Room(RoomCategory.Indoor,
                "####__####" +
                "#____~~__#" +
                "#________#" +
                "#__~~____#" +
                "#________#" +
                "#________#" +
                "#________#" +
                "##########");

            Assert.DoesNotThrow(() => { Rooms.ValidateRoom(nameof(validRoom), validRoom, 0, false, true); });

            Assert.Throws<ArgumentException>(() =>
            {
                Rooms.ValidateRoom(nameof(invalidRoom), invalidRoom, 0, false, true);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                Rooms.ValidateRoom(nameof(invalidRoom2), invalidRoom2, 0, false, true);
            });
        }

        /*
         * Integration Tests
         */

        // Tests "Map" and "GridWorldGenerator". Uses the riskiest-first method, the map and generator classes
        // are the most likely to have problems in my opinion, due to their relative complexity compared to the
        // rest of the project. Therefore, I'm testing interactions between them first.
        [Test]
        public void BorderIsFullyFilled()
        {
            var map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
            map.OnLoad += data =>
            {
                for (var y = -1; y <= data.Size.y + 2; y++)
                for (var x = -1; x <= data.Size.x + 2; x++)
                {
                    if ((y != -1 || y != data.Size.y + 2) && (x != -1 || x != data.Size.x + 2)) continue;

                    Assert.IsTrue(map.IsTileOccupied(x, y));
                }
            };
            map.Generate(new GridWorldGenerator());
        }

        // Tests "Map" and "GridWorldGenerator". Uses the riskiest-first method, for the same reasons as the previous test.
        [Test]
        public void PlayerStartIsUnobstructed()
        {
            var map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
            map.OnLoad += data =>
            {
                Assert.IsFalse(map.IsTileOccupied((int)data.StartPosition.x, (int)data.StartPosition.y));
            };
            map.Generate(new GridWorldGenerator());
        }

        // Tests "Map" and "GridWorldGenerator". Uses the riskiest-first method, for the same reasons as the previous test.
        [Test]
        public void PlayerEndIsUnobstructed()
        {
            var map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
            map.OnLoad += data =>
            {
                Assert.IsFalse(map.IsTileOccupied((int)data.EndPosition.x, (int)data.EndPosition.y));
            };
            map.Generate(new GridWorldGenerator());
        }
    }
}