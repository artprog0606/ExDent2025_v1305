using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Core.Services.Tools.Recalculate
{
    public class RecalculateItemsToSum<TId>
        where TId : IEquatable<TId>
    {
        private readonly IList<RecalculateItem<TId>> _sourceItems;

        /// <summary>
        /// Допустимое отклонение
        /// <value>Должно быть положительным</value>
        /// </summary>
        public float AcceptableDifference { get; set; }

        /// <summary>
        /// Не менять кол-во у позиций
        /// </summary>
        public bool NoChangeAmount { get; set; }

        /// <summary>
        /// Не разделять позиции на части
        /// </summary>
        public bool NotSeparate { get; set; }

        /// <summary>
        /// Округление до какого значения
        /// </summary>
        public float? RoundNumbers { get; set; }

        /// <exception cref="ArgumentNullException">Если параметр <paramref name="items"/> равен null</exception>
        public RecalculateItemsToSum(IEnumerable<RecalculateItem<TId>> items)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));
            
            AcceptableDifference = 0f;

            _sourceItems = items as IList<RecalculateItem<TId>> ?? items.ToList();
        }

        /// <exception cref="ArgumentException"></exception>
        public IList<RecalculateResultItem<TId>> ToSum(float sum)
        {
            return ToSum(sum, out _);
        }

        /// <exception cref="ArgumentException"></exception>
        public IList<RecalculateResultItem<TId>> ToSum(float sum, out float difference)
        {
            if (!_ValidatePrice(sum))
                throw new ArgumentException(nameof(sum),
                    $"Не корректные входные данные: параметр {nameof(sum)}={sum} не соответствует указанной точности {RoundNumbers}.");
            if (_sourceItems.Count == 0)
            {
                difference = 0f;
                return _sourceItems.ToResultItems();
            }
            
            if (_sourceItems.Any(x => !_ValidatePrice(x.Price)))
                throw new ArgumentException(nameof(_sourceItems),
                    $"Не корректные входные данные: цены позиций не соответствует указанной точности {RoundNumbers}.");

            var resultItems = _sourceItems.ToResultItems();

            // частный случай приведения к 0
            if (sum == 0f)
                resultItems = resultItems.ResetPriceToZero();

            if (!IsAcceptableDifference(resultItems, sum))
                // без изменения кол-ва
                _ToSum(sum, resultItems);

            if (!NoChangeAmount &&
                !IsAcceptableDifference(resultItems, sum) && 
                _sourceItems.Any(item => item.Quantity % 1 > 0))
            {
                // предыдущие распределения не сработали
                // округляем кол-во до целых и пробуем снова

                resultItems = resultItems.ResetToPristine().CeilingQuantityToInteger();

                _ToSum(sum, resultItems);
            }

            if (!NoChangeAmount &&
                !IsAcceptableDifference(resultItems, sum))
            {
                // предыдущие распределения не сработали
                // приводим кол-во к единице и пробуем снова

                resultItems = resultItems.ResetToPristine().SetQuantityToOne();

                _ToSum(sum, resultItems);
            }

            difference = GetDifference(resultItems, sum);
            return resultItems;
        }

        private float GetDifference(IList<RecalculateResultItem<TId>> resultItems, float sum)
        {
            resultItems = resultItems ?? throw new ArgumentNullException(nameof(resultItems));
            return (float)Math.Round(sum - resultItems.Sum(x => (float)Math.Round(x.ResultQuantity * x.ResultPrice, 2)), 2);
        }

        private bool IsAcceptableDifference(IList<RecalculateResultItem<TId>> items, float sum)
            => Math.Abs(GetDifference(items, sum)) <= Math.Abs(AcceptableDifference);


        private void _ToSum(float sum, IList<RecalculateResultItem<TId>> items)
        {
            _DivideDifference(GetDifference(items, sum), items);

            if (!IsAcceptableDifference(items, sum))
                _SetDifferenceToItem(GetDifference(items, sum), items, sum);
        }

        /// <summary>
        /// Раскидываем разницу по позициям
        /// </summary>
        /// <param name="difference">разница</param>
        /// <param name="items">позиции</param>
        private void _DivideDifference(float difference, IList<RecalculateResultItem<TId>> items)
        {
            var productsTotal = items.Sum(x => (float)Math.Round(x.ResultQuantity * x.ResultPrice, 2));

            foreach (var item in items)
            {    
                if (RoundNumbers.HasValue)
                    item.ResultPrice = (float)(Math.Round((item.ResultPrice + item.ResultPrice / productsTotal * difference) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2));
                else
                    item.ResultPrice = (float)Math.Round(item.ResultPrice + item.ResultPrice / productsTotal * difference, 2);
            }
        }

        /// <summary>
        /// Разницу скидываем на одну позицию
        /// </summary>
        /// <param name="difference">разница</param>
        /// <param name="items">позиции</param>
        /// <param name="sum">сумма к которой необходимо привести</param>
        private void _SetDifferenceToItem(float difference, IList<RecalculateResultItem<TId>> items, float sum)
        {
            foreach (var item in items
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                // в приоритет ставим элементы с кол-вом равным 1
                .OrderByDescending(x => x.ResultQuantity == 1f)
                // далее по приоритету с целым кол-вом
                .ThenByDescending(x => x.ResultQuantity % 1 == 0)
                // берем впервую очередь с большей ценой
                .ThenByDescending(x => x.ResultPrice))
            {
                var newPrice = RoundNumbers.HasValue
                    ? (float)(Math.Round((item.ResultPrice + difference / item.ResultQuantity) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2))
                    : (float)Math.Round(item.ResultPrice + difference / item.ResultQuantity, 2);
                if (newPrice > 0f || sum < 0f)
                {
                    item.ResultPrice = newPrice;
                    break;
                }
            }

            // разделяем одну позицию на две, чтобы скинуть на нее разницу
            if (!NotSeparate && !IsAcceptableDifference(items, sum))
            {
                difference = GetDifference(items, sum);

                var itemsForSeparate = items
                    // берем впервую очередь с большей ценой
                    .OrderByDescending(x => x.ResultPrice)
                    // с большим кол-вом (стараясь не разделять 1.5 на 1 и 0.5)
                    .ThenByDescending(x => x.ResultQuantity)
                    // только с кол-вом больше 1
                    .Where(x => x.ResultQuantity > 1f)
                    // чтобы цена не ушла в "минус" (можно в минус если sum отрицательная)
                    .Where(x => x.ResultPrice + difference > 0f || sum < 0f)
                    .ToList();

                if (itemsForSeparate.Count > 0)
                {
                    var currentItem = 0;
                    do
                    {
                        var separateItem = itemsForSeparate[currentItem];
                        var pristineAmount = separateItem.ResultQuantity; // чтобы не потерять точность при мат.операциях
                        var newItem = separateItem.Clone();
                        newItem.ResultQuantity = 1f;
                        separateItem.ResultQuantity -= newItem.ResultQuantity;

                        var newPrice = RoundNumbers.HasValue
                            ? (float)(Math.Round((newItem.ResultPrice + difference) / RoundNumbers.Value, 0, MidpointRounding.AwayFromZero) * Math.Round(RoundNumbers.Value, 2))
                            : (float)Math.Round(newItem.ResultPrice + difference, 2);
                        
                        if (newPrice > 0f || sum < 0f)
                            newItem.ResultPrice = newPrice;
                        
                        int indexSeparateItem = int.MinValue;
                        for (var index = 0; index < items.Count; index++)
                        {
                            if (ReferenceEquals(items[index], separateItem))
                            {
                                indexSeparateItem = index;
                                break;
                            }
                        }
                        items.Insert(indexSeparateItem + 1, newItem);

                        if (!IsAcceptableDifference(items, sum))
                        {
                            // откатываем
                            separateItem.ResultQuantity = pristineAmount;
                            items.RemoveAt(indexSeparateItem + 1);
                        }

                        currentItem++;
                    } while (currentItem < itemsForSeparate.Count && !IsAcceptableDifference(items, sum));
                }
            }
        }

        /// <summary>
        /// Валидирует, что цена соответствует указанной точности
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns></returns>
        private bool _ValidatePrice(float value)
        {
            if (!RoundNumbers.HasValue)
                return true;

            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            var valueStr = value.ToString(invariantCulture);
            var RoundNumbersStr = RoundNumbers.Value.ToString(invariantCulture);
            var decimalSeparator = invariantCulture.NumberFormat.NumberDecimalSeparator;

		
            var indexValueStr = valueStr.IndexOf(decimalSeparator);
            var indexRoundNumbersStr = RoundNumbersStr.IndexOf(decimalSeparator);

            return indexRoundNumbersStr < 0 
                ? value % RoundNumbers.Value == 0
                : indexValueStr < 0 || valueStr.Substring(indexValueStr).Length <= RoundNumbersStr.Substring(indexRoundNumbersStr).Length;
        }
    }

    public class RecalculateItem<TId>
        where TId : IEquatable<TId>
    {
        public RecalculateItem(TId id, float price, float quantity)
        {
            Id = id;
            Price = price;
            Quantity = quantity;
        }
        public TId Id { get; }
        public float Price { get; }
        public float Quantity { get; }
        
        public RecalculateItem<TId> Clone()
        {
            return new RecalculateItem<TId>(Id, Price, Quantity);
        }
    }

    public class RecalculateResultItem<TId> : RecalculateItem<TId>
        where TId : IEquatable<TId>
    {
        public RecalculateResultItem(TId id, float price, float quantity) : base(id, price, quantity)
        {
            ResultPrice = price;
            ResultQuantity = quantity;
        }
        
        public RecalculateResultItem(RecalculateItem<TId> recalculateItem) 
            : base(recalculateItem.Id, recalculateItem.Price, recalculateItem.Quantity)
        {
            ResultPrice = recalculateItem.Price;
            ResultQuantity = recalculateItem.Quantity;
        }
        
        public float ResultPrice { get; set; }
        public float ResultQuantity { get; set; }
        
        public RecalculateResultItem<TId> Clone()
        {
            return new RecalculateResultItem<TId>(this)
            {
                ResultPrice = ResultPrice,
                ResultQuantity = ResultQuantity,
            };
        }

        public void ResetToPristine()
        {
            ResultPrice = Price;
            ResultQuantity = Quantity;
        }
    }
}