namespace ExampleTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TPUM.Class1 example = new TPUM.Class1();
            int sum = example.Add(2, 2);
            Assert.AreEqual(sum, 4);
        }

        [TestMethod]
        public void TestMethod2()
        {
            TPUM.Class1 example = new TPUM.Class1();
            int diff = example.Sub(5, 3);
            Assert.AreEqual(2, diff);
        }

        [TestMethod]
        public void TestMethod3()
        {
            TPUM.Class1 example = new TPUM.Class1();
            int diff = example.Add(-1760, 124);
            Assert.AreEqual(-1636, diff);
        }

        [TestMethod]
        public void TestMethod4()
        {
            TPUM.Class1 example = new TPUM.Class1();
            int diff = example.Sub(218496, -30127);
            Assert.AreEqual(248623, diff);
        }
    }
}
