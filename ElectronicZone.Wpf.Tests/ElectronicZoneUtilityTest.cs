using ElectronicZone.Wpf.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ElectronicZone.Wpf.Tests
{
    [TestClass]
    public class ElectronicZoneUtilityTest
    {
        //[TestMethod]
        //public void Test_GetUTCFormattedDate()
        //{
        //    // Arrange
        //    string expectedDate = "2019-04-06", date = "04/06/2019";

        //    // Act
        //    DateTimeUtility dateTimeUtility = new DateTimeUtility();
        //    string actual = dateTimeUtility.GetUTCFormattedDate(date);

        //    // Assert
        //    Assert.AreEqual(expectedDate, actual);
        //}

        [TestMethod]
        public void Test_GetMonthStartDate() {
            // Arrange
            DateTime expDate = new DateTime(2019, 4, 1);
            DateTime date = new DateTime(2019,4,6);

            // Act
            DateTimeUtility dtUtility = new DateTimeUtility();
            DateTime monthStartDate = dtUtility.GetMonthStartDate(date);

            // Assert
            Assert.AreEqual(expDate, monthStartDate);
        }
    }
}
