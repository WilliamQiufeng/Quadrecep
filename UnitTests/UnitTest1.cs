using Godot;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NUnit.Framework;
using Path = Quadrecep.Gameplay.Path;

namespace UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(500, 1, 0, 3000, 1, 1, -130, 100, true)]
        [TestCase(500, 1, 0, 3000, 1, 1, 300, 400, true)]
        [TestCase(500, 1, 0, 100, 1, 1, 300, 400, true)]
        [TestCase(500, 1, 0, 1000, 1, 0.5f, -100, 400, true)]
        [TestCase(500, 1, 0, 1000, 1, 1, -200, 400, false)]
        [TestCase(500, 1, 0, 3000, 1, 0, 0, 600, true)]
        [TestCase(500, 1, 0, 3000, 0, 1, 0, 0, true)]
        [TestCase(500, 1, 0, 3000, 0, 0, 512, 300, true)]
        [TestCase(500, 1, 0, 3000, 0, 0, -1, -1, false)]
        public void Test1(float sv, float factor, float st, float et, float dx, float dy, float sx, float sy, bool res)
        {
            var path = new Path(sv, factor, st, et, new Vector2(dx, dy), new(sx, sy), null);
            var cutPath = Path.CutVisiblePath(path, new Vector2(0, 0), new(1024, 600));
            TestContext.WriteLine($"Original Path: {path}", OutputLevel.Information);
            if (cutPath == null)
            {
                TestContext.WriteLine("Not Passing the region", OutputLevel.Information);
                Assert.IsTrue(!res);
                return;
            }
            TestContext.WriteLine($"Cut Path: {cutPath}", OutputLevel.Information);
            Assert.IsTrue(res);
        }
    }
}