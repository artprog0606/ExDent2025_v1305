using AdvantShop.Core.Services.Shipping.Rules;
using AdvantShop.Repository.Currencies;
using NUnit.Framework;

namespace AdvantShop.Core.Test.ShippingRules
{
    public class FiltersTest
    {
        [Test]
        public void ByCost_NegativeByNull()
        {
            //Arrange
            var objWithNullCost = new SimpleObjectForRule();
            var filterByZero = new FilterByCost(0f, 1f, true);
            var filterByNegativeValue = new FilterByCost(-10f, 1f, true);
            var filterByPositiveValue = new FilterByCost(10f, 1f, true);
            
            //Act
            var negativeByZero = filterByZero.Check(objWithNullCost);
            var negativeByNegativeValue = filterByNegativeValue.Check(objWithNullCost);
            var negativeByPositiveValue = filterByPositiveValue.Check(objWithNullCost);
            
            //Assert
            Assert.IsNull(objWithNullCost.Cost);
            Assert.IsFalse(negativeByZero);
            Assert.IsFalse(negativeByNegativeValue);
            Assert.IsFalse(negativeByPositiveValue);
        }

        [Test]
        public void ByCost_Positive()
        {
            //Arrange
            var currency = new Currency() {Rate = 1f};
            var cost = 10f;
            var objWithTenCost = new SimpleObjectForRule(default, default, cost, currency);
            var filterByCost = new FilterByCost(cost, currency.Rate, true);
            
            //Act
            var positiveFilter = filterByCost.Check(objWithTenCost);
            
            //Assert
            Assert.AreEqual(objWithTenCost.Cost, cost);
            Assert.IsTrue(positiveFilter);
        }

        [Test]
        public void ByCost_PositiveByMultiCurrency()
        {
            //Arrange
            var costInCurrencyObject = 10f;
            var currencyOfObject = new Currency() {Rate = 1f};
            var objWithTenCost = new SimpleObjectForRule(default, default, costInCurrencyObject, currencyOfObject);
            var currencyOfFilter= new Currency() {Rate = 2f};
            var filterByCost = new FilterByCost(5f, currencyOfFilter.Rate, true);
            
            //Act
            var positiveFilter = filterByCost.Check(objWithTenCost);
            
            //Assert
            Assert.AreNotEqual(currencyOfObject.Rate, currencyOfFilter.Rate);
            Assert.AreEqual(objWithTenCost.Cost, costInCurrencyObject);
            Assert.IsTrue(positiveFilter);
        }
   
        [Test]
        public void ByCost_NegativeByNullCurrency()
        {
            //Arrange
            var cost = 10f;
            var objWithTenCost = new SimpleObjectForRule(default, default, cost, null);
            var filterByCost = new FilterByCost(cost, 1f, true);
            
            //Act
            var negativeFilter = filterByCost.Check(objWithTenCost);
            
            //Assert
            Assert.IsNull(objWithTenCost.Currency);
            Assert.AreEqual(objWithTenCost.Cost, cost);
            Assert.IsFalse(negativeFilter);
        }
     
        [Test]
        public void ByCost_NegativeByLess()
        {
            //Arrange
            var currency = new Currency() {Rate = 1f};
            var cost = 0f;
            var objWithZeroCost = new SimpleObjectForRule(default, default, cost, currency);
            var filterByCost = new FilterByCost(10f, currency.Rate, true);
            
            //Act
            var negativeByLess = filterByCost.Check(objWithZeroCost);
            
            //Assert
            Assert.AreEqual(objWithZeroCost.Cost, cost);
            Assert.IsFalse(negativeByLess);
        }
        
        [Test]
        public void ByCost_NegativeByMore()
        {
            //Arrange
            var currency = new Currency() {Rate = 1f};
            var cost = 100f;
            var objWithHundredCost = new SimpleObjectForRule(default, default, cost, currency);
            var filterByCost = new FilterByCost(10f, currency.Rate, true);
            
            //Act
            var negativeByMore = filterByCost.Check(objWithHundredCost);
            
            //Assert
            Assert.AreEqual(objWithHundredCost.Cost, cost);
            Assert.IsFalse(negativeByMore);
        }
    }
}