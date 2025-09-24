using System;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Test
{
    [TestFixture]
    public class RecalculateOrderItemsToSumTest
    {
        private float GetDifference(IList<OrderItem> orderItems, float sum)
        {
            return (float)Math.Round(sum - orderItems.Sum(x => (float)Math.Round(x.Amount * x.Price, 2)), 2);
        }

        [Test]
        public void Test_NotNullReferenceException()
        {
            //Arrange
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(null);

            //Act
            var items = recalculateOrderItemsToSum.ToSum(0);

            //Assert
            Assert.NotNull(items);
            Assert.IsFalse(items.Any());
        }

        [Test]
        public void Test_PositiveSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 100, Amount = 1},
                new OrderItem { OrderItemID = 2, Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10);

            //Assert
            Assert.Greater(recalculateOrderItems.Sum(x => x.Price * x.Amount), 0);
        }

        [Test]
        public void Test_ZeroSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 100, Amount = 1},
                new OrderItem { OrderItemID = 2, Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0);

            //Assert
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Price * x.Amount), 0f);
        }

        [Test]
        public void Test_NegativeSum()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 100, Amount = 1},
                new OrderItem { OrderItemID = 2, Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act, Assert
            Assert.Catch<ArgumentOutOfRangeException>(() => { var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(-10); });
        }

        [Test]
        public void Test_AcceptableDifferenceZero()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 30, Amount = 3},
                new OrderItem { OrderItemID = 2, Price = 100, Amount = 2},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(290-50);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 240);
            Assert.AreEqual(difference, 0);
        }

        [Test]
        public void Test_AcceptableDifferenceOneCent()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0.01f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 100);
            Assert.LessOrEqual(difference, recalculateOrderItemsToSum.AcceptableDifference);
            Assert.GreaterOrEqual(difference, 0);
        }

        [Test]
        public void Test_PriceHaveNotChangeIfDifferenceIsZero()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120);

            //Assert
            Assert.AreEqual(recalculateOrderItems.ElementAt(0).Price, 40);
        }

        [Test]
        public void Test_SourceNotChange()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120);

            //Assert
            Assert.IsFalse(Equals(sourceOrderItems, recalculateOrderItems));
            Assert.IsFalse(ReferenceEquals(sourceOrderItems, recalculateOrderItems));
        }

        [Test]
        public void Test_NoChangeAmount()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.NoChangeAmount = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));

        }

        /// <summary>
        /// Тест на раскидывание наценки, которую можно сделать только с разделением позиции
        /// </summary>
        [Test]
        public void Test_SeparateItems()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 167, Amount = 60},
                new OrderItem { OrderItemID = 2, Price = 367, Amount = 60},
                new OrderItem { OrderItemID = 3, Price = 359, Amount = 30},
                new OrderItem { OrderItemID = 4, Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = false;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 45_770);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
        }

        /// <summary>
        /// Тест на раскидыванием наценки, которую можно сделать только с разделением позиции (больше никак на 100%)
        /// </summary>
        [Test]
        public void Test_ToSumOnlyWithSeparateItems()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.NoChangeAmount = true;
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 100);
            Assert.AreEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
        }

        /// <summary>
        /// Тест на неудачу раскидывания наценки без функции разделения позиции
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems()
        {
            // если тест падает из-за того что удается раскидать скидку из-за новых реализаций алгоритма, нужно 
            // отключить эти функции в блоке настроек RecalculateOrderItemsToSum
            
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 167, Amount = 60},
                new OrderItem { OrderItemID = 2, Price = 367, Amount = 60},
                new OrderItem { OrderItemID = 3, Price = 359, Amount = 30},
                new OrderItem { OrderItemID = 4, Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            // настраиваем
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = true;
            recalculateOrderItemsToSum.NoChangeAmount = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 45_770);
            Assert.AreNotEqual(difference, 0);
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
        }

        /// <summary>
        /// Тест2 на неудачу раскидывания наценки без функции разделения позиции (больше никак не раскидать на 100%)
        /// </summary>
        [Test]
        public void Test_FailWithoutSeparateItems2()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 100);
            Assert.AreEqual(difference, 0);
            Assert.AreNotEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
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
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 167, Amount = 60},
                new OrderItem { OrderItemID = 2, Price = 367, Amount = 60},
                new OrderItem { OrderItemID = 3, Price = 359, Amount = 30},
                new OrderItem { OrderItemID = 4, Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.RoundNumbers = 1f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770f);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 45_770f);
            Assert.AreEqual(difference, 0);
            Assert.IsTrue(recalculateOrderItems.All(x => x.Price % 1 == 0));
            
            // может сборить, т.к. к сути теста не относится (может привести к сумме путем изменения кол-ва)
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count + 1);
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
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 55, Amount = 50},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0.1f;
            recalculateOrderItemsToSum.RoundNumbers = 0.01f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(2_750f + 137.5f);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 2_750f + 137.5f);
            Assert.LessOrEqual(difference, recalculateOrderItemsToSum.AcceptableDifference);
            
            // может сборить, т.к. результат может измениться при изменении логики RecalculateOrderItemsToSum (может привести к сумме путем изменения кол-ва или выделения позиции)
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
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
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 167, Amount = 60},
                new OrderItem { OrderItemID = 2, Price = 367, Amount = 60},
                new OrderItem { OrderItemID = 3, Price = 359, Amount = 30},
                new OrderItem { OrderItemID = 4, Price = 130, Amount = 6},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0.1f;
            recalculateOrderItemsToSum.RoundNumbers = 0.01f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(45_770f);

            //Assert
            var difference = GetDifference(recalculateOrderItems, 45_770f);
            Assert.LessOrEqual(difference, recalculateOrderItemsToSum.AcceptableDifference);
            
            // может сборить, т.к. к сути теста не относится (проблема была, что итоговая сумма сильно разнилась)
            Assert.AreEqual(recalculateOrderItems.Sum(x => x.Amount), sourceOrderItems.Sum(x => x.Amount));
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
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 800, Amount = 0.8f},
                new OrderItem { OrderItemID = 2, Price = 650, Amount = 0.3f},
                new OrderItem { OrderItemID = 3, Price = 770, Amount = 1},
                new OrderItem { OrderItemID = 4, Price = 790, Amount = 0.7f},
                new OrderItem { OrderItemID = 5, Price = 850, Amount = 0.8f},
                new OrderItem { OrderItemID = 6, Price = 600, Amount = 1},
                new OrderItem { OrderItemID = 7, Price = 850, Amount = 1.4f},
                new OrderItem { OrderItemID = 8, Price = 550, Amount = 0.5f},
                new OrderItem { OrderItemID = 9, Price = 770, Amount = 0.7f},
                new OrderItem { OrderItemID = 10, Price = 770, Amount = 0.7f},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(5_944.57f - 262.62f);

            //Assert
            // Падало при приведении к сумме
            
            var difference = GetDifference(recalculateOrderItems, 5_944.57f - 262.62f);
            Assert.AreEqual(difference, 0);
        }
        
        /// <summary>
        /// Тест валидации входных цен
        /// </summary>
        [Test]
        public void Test_ValidatePrice()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 0f, Amount = 60},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            
            //Act, Assert

            recalculateOrderItemsToSum.RoundNumbers = null;
            
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.01f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f);
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 0.01f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f);
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 0.1f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            
            
            
            recalculateOrderItemsToSum.RoundNumbers = 1f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            sourceOrderItems[0].Price = 120f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f);
            sourceOrderItems[0].Price = 1001f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f);
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
         
            
            
            recalculateOrderItemsToSum.RoundNumbers = 10f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 10f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f);
            sourceOrderItems[0].Price = 120f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f);
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
        
            
            
            recalculateOrderItemsToSum.RoundNumbers = 100f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 200f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(200f);
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);

            
            
            recalculateOrderItemsToSum.RoundNumbers = 1000f;
            
            sourceOrderItems[0].Price = recalculateOrderItemsToSum.RoundNumbers.Value;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value);
            sourceOrderItems[0].Price = 0.01f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.01f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0f);
            sourceOrderItems[0].Price = 0.011f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.011f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.1000001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.1000001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 0.11f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(0.11f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 10f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(10f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 100f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 120f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(120f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 1001f;
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(1001f); });
            Assert.Catch<ArgumentException>(() => { recalculateOrderItems = recalculateOrderItemsToSum.ToSum(recalculateOrderItemsToSum.RoundNumbers.Value); });
            sourceOrderItems[0].Price = 11000f;
            recalculateOrderItems = recalculateOrderItemsToSum.ToSum(11000f);
        }
        
        
        /// <summary>
        /// Тест проверяет, что значения веса и габаритов поменялось в соответствии с новым кол-вом
        /// </summary>
        [Test]
        public void Test_ChangeAmount()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 40, Amount = 3, Weight = 100, Height = 10, Width = 100, Length = 100},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);
            recalculateOrderItemsToSum.AcceptableDifference = 0f;
            recalculateOrderItemsToSum.NotSeparate = true;

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(100);

            //Assert
            Assert.AreNotEqual(recalculateOrderItems[0].Amount, sourceOrderItems[0].Amount);
            Assert.AreEqual(recalculateOrderItems[0].Weight, 300f);
            Assert.AreEqual(recalculateOrderItems[0].Height, 30f);
            Assert.AreEqual(recalculateOrderItems[0].Width, 100f);
            Assert.AreEqual(recalculateOrderItems[0].Length, 100f);
        }

        //Тест на не изменение цены позиции с запретом скидок
        [Test]
        public void Test_NoChangePriceForNoDiscountsFlag()
        {
            //Arrange
            var order = new Order()
            {
                OrderCurrency = new OrderCurrency()
                {
                    CurrencyValue = 1,
                    EnablePriceRounding = false,
                },
                Coupon = null,
                Certificate = null,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {OrderItemID = 1, Price = 100, Amount = 1, DoNotApplyOtherDiscounts = true},
                    new OrderItem {OrderItemID = 2, Price = 100, Amount = 1},
                },
                BonusCost = 50,
                Sum = 150,
            };
            var recalculateOrderItemsToSum = order.GetOrderItemsWithDiscountsAndFee();

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.GetItems();

            //Assert
            Assert.AreEqual(recalculateOrderItems.Single(x => x.OrderItemID == 1).Price, 100f);
            var difference = GetDifference(recalculateOrderItems, 150);
            Assert.AreEqual(difference, 0);
        }
   
        //Проверка на то, что алгоритм не теряет позиции с флагом DoNotApplyOtherDiscounts
        [Test]
        public void Test_NoDiscountsFlag_NoMissingElements()
        {
            //Arrange
            var sourceOrderItems = new List<OrderItem>
            {
                new OrderItem { OrderItemID = 1, Price = 100, Amount = 1, DoNotApplyOtherDiscounts = true},
                new OrderItem { OrderItemID = 2, Price = 100, Amount = 1},
            };
            var recalculateOrderItemsToSum = new RecalculateOrderItemsToSum(sourceOrderItems);

            //Act
            var recalculateOrderItems = recalculateOrderItemsToSum.ToSum(150);

            //Assert
            Assert.AreEqual(recalculateOrderItems.Count, sourceOrderItems.Count);
        }
    }
}
