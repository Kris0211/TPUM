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
    }
}
