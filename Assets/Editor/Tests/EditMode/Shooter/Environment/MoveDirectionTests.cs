using System.Linq;
using NUnit.Framework;
using Shooter.Environment;
using Assert = UnityEngine.Assertions.Assert;

namespace Editor.Tests.EditMode.Shooter.Environment {
    public class MoveDirectionTests {
        [Test]
        public void MoveDirection_CorrectlyClassifiesCardinalAndIntercardinalDirections() {
            MoveDirection[] northerlyDirections = { MoveDirection.North, MoveDirection.NorthEast, MoveDirection.NorthWest };
            MoveDirection[] easternDirections = { MoveDirection.East, MoveDirection.NorthEast, MoveDirection.SouthEast };
            MoveDirection[] southerlyDirections = { MoveDirection.South, MoveDirection.SouthEast, MoveDirection.SouthWest };
            MoveDirection[] westerlyDirections = { MoveDirection.West, MoveDirection.NorthWest, MoveDirection.SouthWest };

            for (var i = 0; i < 8; i++) {
                var direction = (MoveDirection) (1 << i);
                Assert.AreEqual(northerlyDirections.Contains(direction), direction.IsNortherly());
                Assert.AreEqual(easternDirections.Contains(direction), direction.IsEasterly());
                Assert.AreEqual(southerlyDirections.Contains(direction), direction.IsSoutherly());
                Assert.AreEqual(westerlyDirections.Contains(direction), direction.IsWesterly());
            }
        }
        
        [Test]
        public void MoveDirection_CorrectlyDeterminesOppositeDirection() {
            Assert.AreEqual(MoveDirection.South, MoveDirection.North.Opposite());
            Assert.AreEqual(MoveDirection.SouthWest, MoveDirection.NorthEast.Opposite());
            Assert.AreEqual(MoveDirection.West, MoveDirection.East.Opposite());
            Assert.AreEqual(MoveDirection.NorthWest, MoveDirection.SouthEast.Opposite());
            Assert.AreEqual(MoveDirection.North, MoveDirection.South.Opposite());
            Assert.AreEqual(MoveDirection.NorthEast, MoveDirection.SouthWest.Opposite());
            Assert.AreEqual(MoveDirection.East, MoveDirection.West.Opposite());
            Assert.AreEqual(MoveDirection.SouthEast, MoveDirection.NorthWest.Opposite());
            Assert.AreEqual(MoveDirection.None, MoveDirection.None.Opposite());
        }
    }
}