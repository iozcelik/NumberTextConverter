using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumberTextConverter;

namespace NumberTextConverterTest {
    [TestClass]
    public class ConverterTest {
        [TestMethod]
        public void DollarTest() {
            var result = Converter.ConvertAmountToText("745,13 $");
            Assert.AreEqual(" seven hundred forty  five  dollars  thirteen  cents ", result);
        }

        [TestMethod]
        public void LiraTest() {
            var result = Converter.ConvertAmountToText("₺1630,29");
            Assert.AreEqual(" one thousand  six hundred thirty  liras  twenty  nine  kuruses ", result);
        }
    }
}
