using System;
using System.Collections.Generic;
using System.Numerics;

namespace AltV.Net.EntitySync.SpatialPartitions
{
    public class Grid : SpatialPartition
    {
        private static readonly float Tolerance = 0.013F; //0.01318359375F;

        // x-index, y-index, col shapes
        private readonly IEntity[][][] entityAreas;

        private readonly int maxX;

        private readonly int maxY;

        private readonly int areaSize;

        private readonly int xOffset;

        private readonly int yOffset;

        /// <summary>
        /// The constructor of the grid spatial partition algorithm
        /// </summary>
        /// <param name="maxX">The max x value</param>
        /// <param name="maxY">The max y value</param>
        /// <param name="areaSize">The Size of a grid area</param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public Grid(int maxX, int maxY, int areaSize, int xOffset, int yOffset)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            this.areaSize = areaSize;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        public override void Add(IEntity entity)
        {
            var entityPositionX = entity.Position.X + xOffset;
            var entityPositionY = entity.Position.Y + yOffset;
            var range = entity.Range;
            if (range == 0 || entityPositionX < 0 || entityPositionY < 0 ||
                entityPositionX > maxX ||
                entityPositionY > maxY) return;

            // we actually have a circle but we use this as a square for performance reasons
            // we now find all areas that are inside this square
            var squareMaxX = entityPositionX + range;
            var squareMaxY = entityPositionY + range;
            var squareMinX = entityPositionX - range;
            var squareMinY = entityPositionY - range;
            // We first use starting y index to start filling
            var startingYIndex = (int) Math.Floor(squareMinY / areaSize);
            // We now define starting x index to start filling
            var startingXIndex = (int) Math.Floor(squareMinX / areaSize);
            // Also define stopping indexes
            var stoppingYIndex =
                (int) Math.Ceiling(squareMaxY / areaSize);
            var stoppingXIndex =
                (int) Math.Ceiling(squareMaxX / areaSize);
            // Now fill all areas from min {x, y} to max {x, y}
            lock (entityAreas)
            {
                for (var i = startingYIndex; i <= stoppingYIndex; i++)
                {
                    for (var j = startingXIndex; j <= stoppingXIndex; j++)
                    {
                        var length = entityAreas[j][i].Length;
                        Array.Resize(ref entityAreas[j][i], length + 1);
                        entityAreas[j][i][length] = entity;
                    }
                }
            }
        }

        public override void Remove(IEntity entity)
        {
            var entityPositionX = entity.Position.X + xOffset;
            var entityPositionY = entity.Position.Y + yOffset;
            var range = entity.Range;
            var id = entity.Id;
            var type = entity.Type;
            if (range == 0 || entityPositionX < 0 || entityPositionY < 0 ||
                entityPositionX > maxX ||
                entityPositionY > maxY) return;

            // we actually have a circle but we use this as a square for performance reasons
            // we now find all areas that are inside this square
            var squareMaxX = entityPositionX + range;
            var squareMaxY = entityPositionY + range;
            var squareMinX = entityPositionX - range;
            var squareMinY = entityPositionY - range;
            // We first use starting y index to start filling
            var startingYIndex = (int) Math.Floor(squareMinY / areaSize);
            // We now define starting x index to start filling
            var startingXIndex = (int) Math.Floor(squareMinX / areaSize);
            // Also define stopping indexes
            var stoppingYIndex =
                (int) Math.Ceiling(squareMaxY / areaSize);
            var stoppingXIndex =
                (int) Math.Ceiling(squareMaxX / areaSize);
            // Now remove entity from all areas from min {x, y} to max {x, y}
            //lock (entityAreas)
            //{
                for (var i = startingYIndex; i <= stoppingYIndex; i++)
                {
                    for (var j = startingXIndex; j <= stoppingXIndex; j++)
                    {
                        var arr = entityAreas[i][j];
                        var length = arr.Length;
                        int k;
                        var found = false;
                        for (k = 0; k < length; k++)
                        {
                            var currEntity = arr[k];
                            if (currEntity.Id != id || currEntity.Type != type) continue;
                            found = true;
                            break;
                        }

                        if (!found) continue;
                        var newLength = length - 1;
                        for (var l = k; l < newLength; l++)
                        {
                            arr[l] = arr[l + 1];
                        }

                        Array.Resize(ref arr, newLength);
                    }
                }
            //}
        }

        public override void UpdateEntityPosition(IEntity entity, in Vector3 newPosition)
        {
            var oldEntityPositionX = entity.Position.X + xOffset;
            var oldEntityPositionY = entity.Position.Y + yOffset;
            var newEntityPositionX = newPosition.X + xOffset;
            var newEntityPositionY = newPosition.Y + yOffset;
            var range = entity.Range;
            var id = entity.Id;
            var type = entity.Type;
            if (range == 0 || oldEntityPositionX < 0 || oldEntityPositionY < 0 ||
                oldEntityPositionX > maxX ||
                oldEntityPositionY > maxY || newEntityPositionX < 0 || newEntityPositionY < 0 ||
                newEntityPositionX > maxX ||
                newEntityPositionY > maxY) return;

            // we actually have a circle but we use this as a square for performance reasons
            // we now find all areas that are inside this square
            var oldSquareMaxX = oldEntityPositionX + range;
            var oldSquareMaxY = oldEntityPositionY + range;
            var oldSquareMinX = oldEntityPositionX - range;
            var oldSquareMinY = oldEntityPositionY - range;
            // We first use starting y index to start filling
            var oldStartingYIndex = (int) Math.Floor(oldSquareMinY / areaSize);
            // We now define starting x index to start filling
            var oldStartingXIndex = (int) Math.Floor(oldSquareMinX / areaSize);
            // Also define stopping indexes
            var oldStoppingYIndex =
                (int) Math.Ceiling(oldSquareMaxY / areaSize);
            var oldStoppingXIndex =
                (int) Math.Ceiling(oldSquareMaxX / areaSize);

            // we actually have a circle but we use this as a square for performance reasons
            // we now find all areas that are inside this square
            var newSquareMaxX = newEntityPositionX + range;
            var newSquareMaxY = newEntityPositionY + range;
            var newSquareMinX = newEntityPositionX - range;
            var newSquareMinY = newEntityPositionY - range;
            // We first use starting y index to start filling
            var newStartingYIndex = (int) Math.Floor(newSquareMinY / areaSize);
            // We now define starting x index to start filling
            var newStartingXIndex = (int) Math.Floor(newSquareMinX / areaSize);
            // Also define stopping indexes
            var newStoppingYIndex =
                (int) Math.Ceiling(newSquareMaxY / areaSize);
            var newStoppingXIndex =
                (int) Math.Ceiling(newSquareMaxX / areaSize);

            //TODO: do later checking for overlaps between the grid areas in the two dimensional array
            //  --    --    --    --   
            // |xy|  |xy|  |yy|  |  |    
            // |yx|  |yx|  |yy|  |  |
            //  --    --    --    --  
            //
            //  --    --    --    --   
            // |xy|  |xy|  |yy|  |  |    
            // |yx|  |yx|  |yy|  |  |
            //  --    --    --    --  
            //
            //  --    --    --    --   
            // |yy|  |yy|  |yy|  |  |    
            // |yy|  |yy|  |yy|  |  |
            //  --    --    --    --  
            //
            //  --    --    --    --   
            // |  |  |  |  |  |  |  |    
            // |  |  |  |  |  |  |  |
            //  --    --    --    --  
            // Now we have to check if some of the (oldStartingYIndex, oldStoppingYIndex) areas are inside (newStartingYIndex, newStoppingYIndex)
            // Now we have to check if some of the (oldStartingXIndex, oldStoppingXIndex) areas are inside (newStartingXIndex, newStoppingXIndex)


            //lock (entityAreas)
            //{
                for (var i = oldStartingYIndex; i <= oldStoppingYIndex; i++)
                {
                    for (var j = oldStartingXIndex; j <= oldStoppingXIndex; j++)
                    {
                        //TODO: Now we check if (i,j) is inside the new position range, so we don't have to delete it
                        var arr = entityAreas[i][j];
                        var length = arr.Length;
                        int k;
                        var found = false;
                        for (k = 0; k < length; k++)
                        {
                            var currEntity = arr[k];
                            if (currEntity.Id != id || currEntity.Type != type) continue;
                            found = true;
                            break;
                        }

                        if (!found) continue;
                        var newLength = length - 1;
                        for (var l = k; l < newLength; l++)
                        {
                            arr[l] = arr[l + 1];
                        }

                        Array.Resize(ref arr, newLength);
                    }
                }

                for (var i = newStartingYIndex; i <= newStoppingYIndex; i++)
                {
                    for (var j = newStartingXIndex; j <= newStoppingXIndex; j++)
                    {
                        var length = entityAreas[j][i].Length;
                        Array.Resize(ref entityAreas[j][i], length + 1);
                        entityAreas[j][i][length] = entity;
                    }
                }
            //}
        }

        public override void UpdateEntityRange(IEntity entity, uint range)
        {
            throw new NotImplementedException();
        }

        //TODO: check if we can find a better way to pass a position and e.g. improve performance of this method by return type ect.
        public override IEnumerable<IEntity> Find(Vector3 position)
        {
            var posX = position.X + xOffset;
            var posY = position.Y + yOffset;

            if (posX < 0 || posY < 0 || posX > maxX || posY > maxY) yield break;

            var xIndex = (int) Math.Floor(posX / areaSize);

            var yIndex = (int) Math.Floor(posY / areaSize);

            // x2 and y2 only required for complete exact range check

            /*var x2Index = (int) Math.Ceiling(posX / areaSize);

            var y2Index = (int) Math.Ceiling(posY / areaSize);*/

            //lock (entityAreas)
            //{
                var areaEntities = entityAreas[xIndex][yIndex];

                for (int j = 0, innerLength = areaEntities.Length; j < innerLength; j++)
                {
                    var entity = areaEntities[j];
                    if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                    yield return entity;
                }

                /*if (xIndex != x2Index && yIndex == y2Index)
                {
                    var innerAreaEntities = entityAreas[x2Index][yIndex];

                    for (int j = 0, innerLength = innerAreaEntities.Length; j < innerLength; j++)
                    {
                        var entity = innerAreaEntities[j];
                        if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                        callback(entity);
                    }
                } else if (xIndex == x2Index && yIndex != y2Index)
                {
                    var innerAreaEntities = entityAreas[xIndex][y2Index];

                    for (int j = 0, innerLength = innerAreaEntities.Length; j < innerLength; j++)
                    {
                        var entity = innerAreaEntities[j];
                        if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                        callback(entity);
                    }
                } else if (xIndex != x2Index && yIndex != y2Index)
                {
                    var innerAreaEntities = entityAreas[x2Index][yIndex];

                    for (int j = 0, innerLength = innerAreaEntities.Length; j < innerLength; j++)
                    {
                        var entity = innerAreaEntities[j];
                        if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                        callback(entity);
                    }
                    
                    innerAreaEntities = entityAreas[x2Index][y2Index];

                    for (int j = 0, innerLength = innerAreaEntities.Length; j < innerLength; j++)
                    {
                        var entity = innerAreaEntities[j];
                        if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                        callback(entity);
                    }
                    
                    innerAreaEntities = entityAreas[xIndex][y2Index];

                    for (int j = 0, innerLength = innerAreaEntities.Length; j < innerLength; j++)
                    {
                        var entity = innerAreaEntities[j];
                        if (Vector3.Distance(entity.Position, position) > entity.Range) continue;
                        callback(entity);
                    }
                }*/
            //}
        }
    }
}