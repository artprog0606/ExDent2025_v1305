using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Tools.Recalculate;
using AdvantShop.Core.Test.ExtensionsForTests;
using NUnit.Framework;

namespace AdvantShop.Core.Test
{
    [TestFixture]
    public class RecalculateItemsToSumTest
    {
        private float GetDifference(IEnumerable<RecalculateResultItem<int>> items, float sum)
        {
            return (float)Math.Round(sum - items.Sum(x => (float)Math.Round(x.ResultQuantity * x.ResultPrice, 2)), 2);
        }

        [Test]
        public void Test_NullReferenceException()
        {
            //Arrange, Act, Assert
            Assert.Catch<ArgumentNullException>(() => { new RecalculateItemsToSum<int>(null); });
        }

        [Test]
        public void Test_PositiveSum()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);

            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(10);

            //Assert
            Assert.Greater(recalculateResultItems.Sum(x => x.ResultPrice * x.ResultQuantity), 0);
        }

        [Test]
        public void Test_ZeroSum()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);

            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(0);

            //Assert
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultPrice * x.ResultQuantity), 0f);
        }

        [Test]
        public void Test_NegativeSum()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
                new RecalculateItem<int>(id: 0, price: 100, quantity: 1),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);

            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(-10f);

            //Assert
            Assert.Less(recalculateResultItems.Sum(x => x.ResultPrice * x.ResultQuantity), 0);
        }

        [Test]
        public void Test_AcceptableDifferenceZero()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 30, quantity: 3),
                new RecalculateItem<int>(id: 0, price: 100, quantity: 2),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(290-50);

            //Assert
            var difference = GetDifference(recalculateResultItems, 240);
            Assert.AreEqual(difference, 0);
        }

        [Test]
        public void Test_AcceptableDifferenceOneCent()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0.01f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(100);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 100);
            Assert.LessOrEqual(difference, recalculateItemsToSum.AcceptableDifference);
            Assert.GreaterOrEqual(difference, 0);
        }
        
        [Test]
        public void Test_PriceHaveNotChangeIfDifferenceIsZero()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(120);
        
            //Assert
            Assert.AreEqual(recalculateResultItems.ElementAt(0).Price, 40);
        }
        
        [Test]
        public void Test_SourceNoEqualsResult()
        {
            //Arrange
            var sourceItems = new List<RecalculateResultItem<int>>
            {
                new RecalculateResultItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(120);
        
            //Assert
            Assert.IsFalse(Equals(sourceItems, recalculateResultItems));
            Assert.IsFalse(ReferenceEquals(sourceItems, recalculateResultItems));
        }
        
        [Test]
        public void Test_NoChange()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(120);
        
            //Assert
            Assert.IsTrue(recalculateResultItems
               .All(x => 
                    x.Quantity == x.ResultQuantity 
                    && x.Price == x.ResultPrice));
        }
        
        [Test]
        public void Test_NoChangeAmount()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.NoChangeAmount = true;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(100);
        
            //Assert
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.IsTrue(recalculateResultItems
                         .GroupBy(x => x.Id)
                         .All(g => 
                              g.First().Quantity.Equals(g.Sum(x => x.ResultQuantity))));
        }
        
        /// <summary>
        /// Тест на раскидывание наценки, которую можно сделать только с разделением позиции
        /// </summary>
        [Test]
        public void Test_SeparateItems()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 167, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 367, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 359, quantity: 30),
                new RecalculateItem<int>(id: 0, price: 130, quantity: 6),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;
            recalculateItemsToSum.NotSeparate = false;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(45_770);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 45_770);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count + 1);
        }
        
        /// <summary>
        /// Тест на раскидыванием наценки, которую можно сделать только с разделением позиции (больше никак на 100%)
        /// </summary>
        [Test]
        public void Test_ToSumOnlyWithSeparateItems()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.NoChangeAmount = true;
            recalculateItemsToSum.AcceptableDifference = 0f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(100);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 100);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count + 1);
        }
        
        /// <summary>
        /// Тест на неудачу раскидывания наценки без функции разделения позиции
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems()
        {
            // если тест падает из-за того что удается раскидать скидку из-за новых реализаций алгоритма, нужно 
            // отключить эти функции в блоке настроек RecalculateItemsToSum<int>
            
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 167, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 367, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 359, quantity: 30),
                new RecalculateItem<int>(id: 0, price: 130, quantity: 6),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            // настраиваем
            recalculateItemsToSum.AcceptableDifference = 0f;
            recalculateItemsToSum.NotSeparate = true;
            recalculateItemsToSum.NoChangeAmount = true;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(45_770);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 45_770);
            Assert.AreNotEqual(difference, 0);
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count);
        }
        
        /// <summary>
        /// Тест2 на неудачу раскидывания наценки без функции разделения позиции (больше никак не раскидать на 100%)
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems2()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 40, quantity: 3),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;
            recalculateItemsToSum.NotSeparate = true;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(100);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 100);
            Assert.AreEqual(difference, 0);
            Assert.AreNotEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.IsFalse(recalculateResultItems
               .All(x => 
                    x.Quantity == x.ResultQuantity 
                    && x.Price == x.ResultPrice));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count);
        }
        
        /// <summary>
        /// Распределение наценки с округлением
        /// <remarks>
        /// https://task.advant.shop/adminv3/tasks#?modal=27983
        /// </remarks>
        /// </summary>
        [Test]
        public void Test_Round()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 167, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 367, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 359, quantity: 30),
                new RecalculateItem<int>(id: 0, price: 130, quantity: 6),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;
            recalculateItemsToSum.RoundNumbers = 1f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(45_770f);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 45_770f);
            Assert.AreEqual(difference, 0);
            Assert.IsTrue(recalculateResultItems.All(x => x.Price % 1 == 0));
            
            // может сборить, т.к. к сути теста не относится (может привести к сумме путем изменения кол-ва)
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count + 1);
        }
        
        /// <summary>
        /// Распределение наценки с округлением
        /// <remarks>
        /// https://task.advant.shop/adminv3/tasks#?modal=27983
        /// </remarks>
        /// </summary>
        [Test]
        public void Test_Round_Task_27983()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 55, quantity: 50),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0.1f;
            recalculateItemsToSum.RoundNumbers = 0.01f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(2_750f + 137.5f);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 2_750f + 137.5f);
            Assert.LessOrEqual(difference, recalculateItemsToSum.AcceptableDifference);
            
            // может сборить, т.к. результат может измениться при изменении логики RecalculateItemsToSum<int> (может привести к сумме путем изменения кол-ва или выделения позиции)
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
            Assert.AreEqual(recalculateResultItems.Count, sourceItems.Count);
        }
        
        /// <summary>
        /// Распределение наценки с округлением
        /// <remarks>
        /// https://task.advant.shop/adminv3/tasks#?modal=27983
        /// </remarks>
        /// </summary>
        [Test]
        public void Test_Round_Task_27983_2()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 167, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 367, quantity: 60),
                new RecalculateItem<int>(id: 0, price: 359, quantity: 30),
                new RecalculateItem<int>(id: 0, price: 130, quantity: 6),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0.1f;
            recalculateItemsToSum.RoundNumbers = 0.01f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(45_770f);
        
            //Assert
            var difference = GetDifference(recalculateResultItems, 45_770f);
            Assert.LessOrEqual(difference, recalculateItemsToSum.AcceptableDifference);
            
            // может сборить, т.к. к сути теста не относится (проблема была, что итоговая сумма сильно разнилась)
            Assert.AreEqual(recalculateResultItems.Sum(x => x.ResultQuantity), sourceItems.Sum(x => x.Quantity));
        }
        
        /// <summary>
        /// 
        /// <remarks>
        /// https://task.advant.shop/adminv3/tasks#?modal=28255
        /// </remarks>
        /// </summary>
        [Test]
        public void Test_Round_Task_28255()
        {
            //Arrange
            var sourceItems = new List<RecalculateItem<int>>
            {
                new RecalculateItem<int>(id: 0, price: 800, quantity: 0.8f),
                new RecalculateItem<int>(id: 0, price: 650, quantity: 0.3f),
                new RecalculateItem<int>(id: 0, price: 770, quantity: 1),
                new RecalculateItem<int>(id: 0, price: 790, quantity: 0.7f),
                new RecalculateItem<int>(id: 0, price: 850, quantity: 0.8f),
                new RecalculateItem<int>(id: 0, price: 600, quantity: 1),
                new RecalculateItem<int>(id: 0, price: 850, quantity: 1.4f),
                new RecalculateItem<int>(id: 0, price: 550, quantity: 0.5f),
                new RecalculateItem<int>(id: 0, price: 770, quantity: 0.7f),
                new RecalculateItem<int>(id: 0, price: 770, quantity: 0.7f),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            recalculateItemsToSum.AcceptableDifference = 0f;
        
            //Act
            var recalculateResultItems = recalculateItemsToSum.ToSum(5_944.57f - 262.62f);
        
            //Assert
            // Падало при приведении к сумме
            
            var difference = GetDifference(recalculateResultItems, 5_944.57f - 262.62f);
            Assert.AreEqual(difference, 0);
        }
        
        public class RecalculateItemSetterPrice<TId>: RecalculateItem<TId> 
            where TId : IEquatable<TId>
        {
            public RecalculateItemSetterPrice(TId id, float price, float quantity) : base(id, price, quantity)
            {
            }
            
            public float SetPrice
            {
                get => base.Price;
                set
                {
                    var propertyInfo= this.GetPropertyInfo("Price", bindingAttr: null);
                    var backingField = propertyInfo.GetBackingField();
                    this.SetFieldValue(backingField, value);
                }
            }
        }
        /// <summary>
        /// Тест валидации входных цен
        /// </summary>
        [Test]
        public void Test_ValidatePrice()
        {
            //Arrange
            var sourceItems = new List<RecalculateItemSetterPrice<int>>
            {
                new RecalculateItemSetterPrice<int>(id: 0, price: 0f, quantity: 60),
            };
            var recalculateItemsToSum = new RecalculateItemsToSum<int>(sourceItems);
            
            //Act, Assert

            recalculateItemsToSum.RoundNumbers = null;
            
            var recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.01f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f);
            sourceItems[0].SetPrice = 10f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(10f);
            
            
            
            recalculateItemsToSum.RoundNumbers = 0.01f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f);
            sourceItems[0].SetPrice = 10f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(10f);
            
            
            
            recalculateItemsToSum.RoundNumbers = 0.1f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 10f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(10f);
            
            
            
            recalculateItemsToSum.RoundNumbers = 1f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 10f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(10f);
            sourceItems[0].SetPrice = 120f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(120f);
            sourceItems[0].SetPrice = 1001f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(1001f);
            sourceItems[0].SetPrice = 200f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(200f);
            sourceItems[0].SetPrice = 11000f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(11000f);
         
            
            
            recalculateItemsToSum.RoundNumbers = 10f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 10f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(10f);
            sourceItems[0].SetPrice = 120f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(120f);
            sourceItems[0].SetPrice = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 200f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(200f);
            sourceItems[0].SetPrice = 11000f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(11000f);
        
            
            
            recalculateItemsToSum.RoundNumbers = 100f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(10f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(120f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 200f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(200f);
            sourceItems[0].SetPrice = 11000f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(11000f);

            
            
            recalculateItemsToSum.RoundNumbers = 1000f;
            
            sourceItems[0].SetPrice = recalculateItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value);
            sourceItems[0].SetPrice = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(0f);
            sourceItems[0].SetPrice = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(10f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 100f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(100f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(120f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateItemsToSum.ToSum(recalculateItemsToSum.RoundNumbers.Value); });
            sourceItems[0].SetPrice = 11000f;
            recalculateOrderItems = recalculateItemsToSum.ToSum(11000f);
        }
    }
}