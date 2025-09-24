using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Model.Enums;
using AdvantShop.Core.Services.Bonuses.Model.Rules;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Notification;
using AdvantShop.Core.Services.Bonuses.Notification.Template;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Transaction = AdvantShop.Core.Services.Bonuses.Model.Transaction;

namespace AdvantShop.Core.Services.Bonuses
{
    public class BonusSystemService
    {
        private const string BonusFirstPercentCacheKey = "BonusSystem.BonusFirstPercent";
        private const string BonusGradesCacheKey = "BonusSystem.BonusGrades";

        #region Bonus card api methods

        public static Card GetCard(long? cardId)
        {
            if (cardId == null)
                return null;

            return GetCard(cardId.Value);
        }

        public static Card GetCard(long cardnumber)
        {
            return CardService.Get(cardnumber);
        }

        public static Card GetCardByPhone(string phone)
        {
            if (phone.IsNullOrEmpty())
                return null;

            var card = CardService.GetByPhone(phone);

            return card;
        }

        public static Card GetCard(Guid? customerId)
        {
            if (customerId == null || customerId == Guid.Empty)
                return null;

            var card = CardService.Get(customerId.Value);

            return card;
        }

        public static long AddCard(Card card)
        {
            card.CardNumber = GenerateCardNumber(card.CardNumber);
            card.GradeId = BonusSystem.DefaultGrade;
            card.CreateOn = DateTime.Now;
            CardService.Add(card);           
            return card.CardNumber;
        }

        public static long GenerateCardNumber(long cardNumber)
        {
            if (cardNumber != 0) return cardNumber;
            var count = 0;
            var from = BonusSystem.CardFrom;
            var to = BonusSystem.CardTo;

            cardNumber = GetRandom(from, to);
            while (CardService.Get(cardNumber) != null)
            {
                cardNumber = GetRandom(from, to);
                count++;
                if (count == 50)
                {
                    throw new BlException(LocalizationService.GetResource("Admin.Cards.AddUpdateCard.Error.CanNotGenerate"), "CardNumber");
                }
            }
            return cardNumber;
        }


        public static bool MakeBonusPurchase(long cardNumber, Order order)
        {
            var totalPrice = order.OrderItems.Sum(x => x.Price * x.Amount);
            var totalDiscount = order.TotalDiscount;

            var productsPrice = totalPrice - totalDiscount;
            var price = productsPrice -
                        order.OrderItems.Where(x => !x.AccrueBonuses)
                            .Sum(x => (x.Price - x.Price / totalPrice * totalDiscount) * x.Amount);

            var sumPrice = BonusSystem.BonusType == EBonusType.ByProductsCostWithShipping
                    ? price + order.ShippingCost
                    : price;

            if (sumPrice < 0)
                return false;

            return MakeBonusPurchase(cardNumber, (decimal)price, (decimal)sumPrice, order);
        }

        public static bool MakeBonusPurchase(long cardNumber, ShoppingCart cart, float shippingPrice, Order order)
        {
            var totalPrice = cart.TotalPrice;
            var totalDiscount = cart.TotalDiscount;

            var productsPrice = totalPrice - totalDiscount;
            var price = productsPrice -
                        cart.Where(x => !x.Offer.Product.AccrueBonuses)
                            .Sum(x => (x.PriceWithDiscount - x.PriceWithDiscount / totalPrice * totalDiscount) * x.Amount);


            var sumPrice = BonusSystem.BonusType == EBonusType.ByProductsCostWithShipping
                    ? price + shippingPrice
                    : price;

            if (sumPrice < 0)
                return false;
            
            return MakeBonusPurchase(cardNumber, (decimal)price, (decimal)sumPrice, order);
        }


        /// <summary>
        /// Списание бонусов
        /// </summary>
        /// <param name="cardNumber">Номер карты</param>
        /// <param name="purchaseFullAmount"></param>
        /// <param name="purchaseAmount">Сумма, из которой расчитывались бонусы</param>
        /// <param name="order">Заказ</param>
        public static bool MakeBonusPurchase(long cardNumber, decimal purchaseFullAmount, decimal purchaseAmount, Order order)
        {
            // Сколько бонусов списать
            var bonusAmount = (decimal) order.BonusCost;

            using (var scope = new TransactionScope())
            {
                var card = CardService.Get(cardNumber);
                
                if (card == null || card.Blocked)
                    return false;

                var p = PurchaseService.GetByOrderId(order.OrderID);
                //Продажа с таким номером заказа уже существует
                if (p != null)
                    return false;
                
                
                var bonusBalance = BonusService.ActualSum(card.CardId);

                //Нельзя списать больше чем имеется бонусов
                var diff = Math.Round((float) bonusBalance, 2) - Math.Round((float) bonusAmount, 2);
                
                if (bonusAmount > 0 && diff < -1f)
                {
                    Debug.Log.Info($"Списание бонусов для заказа {order.OrderID} не произойдет потому, что нельзя списать {bonusAmount} бонусов. Это больше чем имеется {bonusBalance}.");
                    return false;
                }
                
                // 137,5 - 138 = -0,5
                // if (diff != 0)
                //     bonusAmount = balance;

                var comment = "Заказ № " + (BonusSystem.UseOrderId ? order.OrderID.ToString() : order.Number) + " в магазине " +
                              SettingsMain.SiteUrlPlain;

                var purchase = new Purchase
                {
                    CardId = card.CardId,
                    CreateOn = DateTime.Now,
                    CreateOnCut = DateTime.Now,
                    PurchaseAmount = purchaseAmount,
                    PurchaseFullAmount = purchaseFullAmount,
                    CashAmount = 0,
                    NewBonusAmount = 0,
                    Comment = comment,
                    Status = EPuchaseState.Hold,
                    OrderId = order.OrderID
                };
                purchase.Id = PurchaseService.Add(purchase);

                var subtractBonuses = SubtractBonuses(card.CardId, bonusAmount, comment, purchase.Id);
                purchase.BonusAmount = subtractBonuses;
                purchase.BonusBalance = bonusBalance - subtractBonuses;

                purchase.CashAmount = purchaseAmount - subtractBonuses;
                if (purchase.CashAmount < 0)
                    purchase.CashAmount = 0;

                if (!BonusSystem.ProhibitAccrualAndSubstractBonuses || subtractBonuses == 0)
                {
                    var newBonusAmount = order.Coupon == null || !BonusSystem.ForbidOnCoupon
                        ? PriceService.SimpleRoundPrice(card.Grade.BonusPercent * purchase.CashAmount / 100, order.OrderCurrency)
                        : 0;
                    purchase.NewBonusAmount = newBonusAmount;
                }

                PurchaseService.Update(purchase);
                scope.Complete();
            }
            return true;
        }

        /// <summary>
        /// Списание необходимой суммы бонусов с карты
        /// </summary>
        /// <returns>Кол-во списанных бонусов</returns>
        public static decimal SubtractBonuses(Guid cardId, decimal subtractAmount, string basis = "", int? byPurchaseId = null, List<Bonus> bonuses = null, bool executeModules = true)
        {
            if (subtractAmount < 0m) throw new ArgumentException("It cannot be negative.", nameof(subtractAmount));
            
            bonuses = bonuses 
                      ?? BonusService.Actual(cardId)
                                     .OrderBy(x => x.EndDate ?? DateTime.MaxValue)
                                     .ToList();
            
            var balance = bonuses.Sum(x => x.Amount);
            
            decimal subtractSumAll = 0m;
            foreach (var bonus in bonuses)
            {
                decimal subtractSum;
                if (subtractAmount <= 0) break;
                if (bonus.Amount <= subtractAmount)
                {
                    subtractSum = bonus.Amount;
                    bonus.Amount = 0m;
                    subtractAmount -= subtractSum;
                    bonus.Status = EBonusStatus.Zero;
                    BonusService.Update(bonus);
                }
                else
                {
                    subtractSum = subtractAmount;
                    bonus.Amount -= subtractSum;
                    subtractAmount = 0;
                    bonus.Status = EBonusStatus.Substract;
                    BonusService.Update(bonus);
                }

                balance -= subtractSum;
                subtractSumAll += subtractSum;

                var transLog = Transaction.Factory(cardId, subtractSum, basis,
                    EOperationType.SubtractBonus, balance, byPurchaseId, bonus.Id, null);
                TransactionService.Create(transLog, executeModules);
            }

            return subtractSumAll;
        }

        /// <summary>
        /// Можно ли менять кол-во бонусов у заказа?
        /// </summary>
        public static bool CanChangeBonusAmount(Order order, Card card, Purchase purchase)
        {
            // Если заказ не оплачен и есть бонусная карта и у продажи статус != завершена и включена бонусная система

            return !order.Payed &&
                   card != null && !card.Blocked &&
                   (purchase == null || purchase.Status != EPuchaseState.Complete) && BonusSystem.IsEnabled;
                // && card.BonusesTotalAmount > 0
        }

        /// <summary>
        /// Запрос смс кода по номеру карты
        /// </summary>
        /// <param name="cardNumber">Номер карты</param>
        /// <returns></returns>
        //public static int GetSmsCode(long cardNumber)
        //{
        //    var card = CardService.Get(cardNumber);
        //    var smsCode = GenerateDigit(6);
        //    if (card != null)
        //    {
        //        var customer = CustomerService.GetCustomer(card.CardId);
        //        if (customer != null && customer.StandardPhone.HasValue)
        //            SmsService.Process(customer.StandardPhone.Value, ESmsType.OnSmsCode, new OnSmsCodeTempalte { Code = smsCode });
        //    }
        //    return smsCode;
        //}

        public static int GenerateDigit(int size)
        {
            var rnd = new Random();
            return rnd.Next(1, (int)Math.Pow(10, size));
        }

        /// <summary>
        /// Проверка занят ли телефон
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhoneExist(string phone)
        {
            var temp = CustomerService.GetCustomersByPhone(phone);

            return temp.Any();
        }

        /// <summary>
        /// Подтверждаем, что заказ оплачен
        /// </summary>
        public static bool Confirm(long? cardNumber, Order order)
        {
            var p = PurchaseService.GetByOrderId(order.OrderID);
            if (p == null) 
                return false;
            
            var card = CardService.Get(p.CardId);
            decimal balanceBonuses;
            using (TransactionScope scope = new TransactionScope())
            {
                if (p.Status != EPuchaseState.Hold) return false;
                p.Status = EPuchaseState.Complete;
                PurchaseService.Update(p);

                var bonus = new Bonus()
                {
                    CardId = card.CardId,
                    Amount = p.NewBonusAmount,
                    Status = EBonusStatus.Create,
                    Name = "Зачисление за " + p.Comment,
                    Description = p.Comment,
                };
                bonus.Id = BonusService.Add(bonus);
                balanceBonuses = BonusService.ActualSum(p.CardId);
                var tranLog = Transaction.Factory(p.CardId, p.NewBonusAmount, p.Comment, EOperationType.AddBonus, balanceBonuses, p.Id, bonus.Id, null);
                TransactionService.Create(tranLog);
                scope.Complete();
            }
            
            new ChangeGradeRule().Execute(p.CardId);
            
            var customer = CustomerService.GetCustomer(card.CardId);
            if (customer != null && (customer.StandardPhone.HasValue || !string.IsNullOrEmpty(customer.EMail)))
            {
                NotificationService.Process(p.CardId, ENotifcationType.OnPurchase, new OnPurchaseTempalte
                {
                    CompanyName = SettingsMain.ShopName,
                    PurchaseFull = p.PurchaseFullAmount,
                    Purchase = p.PurchaseAmount,
                    UsedBonus = p.BonusAmount,
                    AddBonus = p.NewBonusAmount,
                    Balance = balanceBonuses,
                    TotalSum = order.Sum,
                    ProductsSum = order.OrderItems.Sum(x => x.Price * x.Amount)
                });
            }
            return true;
        }

        /// <summary>
        /// Получить продажу
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static Purchase GetPurchase(long? cardNumber, string orderNumber, int orderId)
        {
            var p = PurchaseService.GetByOrderId(orderId);
            return p;
        }

        /// <summary>
        /// Отмена продажи
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static void CancelPurchase(long? cardNumber, string orderNumber, int orderId)
        {
            var p = PurchaseService.GetByOrderId(orderId);
            //Продажа не найдена
            if (p == null) return;
            //Отмена продажи возможна только в статусе ожидание
            // if (p.Status != EPuchaseState.Hold) return;
            PurchaseService.RollBack(p);
        }

        /// <summary>
        /// Обновление продажи при редатировании заказа
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="fullsum"></param>
        /// <param name="sum"></param>
        public static void UpdatePurchase(long cardNumber, decimal fullsum, decimal sum, Order order)
        {
            var purchase = PurchaseService.GetByOrderId(order.OrderID);

            #region Временно активируем бонусы 🩼

            var bonusesExpired = purchase.Transaction
                                         .Select(t => t.Bonus)
                                         .Where(b => b != null)
                                          // уже не действующие
                                         .Where(b => b.Status == EBonusStatus.Removed || b.EndDate < DateTime.Today)
                                         .ToList();
            var bonusesChangeStatus = new HashSet<int>();
            var bonusesChangeDate = new Dictionary<int, DateTime>();
            foreach (var bonus in bonusesExpired)
            {
                if (bonus.Status == EBonusStatus.Removed)
                {
                    bonusesChangeStatus.Add(bonus.Id);
                    bonus.Status = EBonusStatus.Substract;
                }

                if (bonus.EndDate < DateTime.Today)
                {
                    bonusesChangeDate.Add(bonus.Id, bonus.EndDate.Value);
                    bonus.EndDate = DateTime.Today;
                }
                
                BonusService.Update(bonus);
            }

            #endregion Временно активируем бонусы
            
            
            PurchaseService.RollBack(purchase);
            
            MakeBonusPurchase(cardNumber, fullsum, sum, order);

            
            #region Обратно деактивируем 🩼

            foreach (var bonusId in bonusesChangeStatus)
            {
                var bonus = BonusService.Get(bonusId);
                bonus.Status = EBonusStatus.Removed;
                BonusService.Update(bonus);
            }
            foreach (var change in bonusesChangeDate)
            {
                var bonus = BonusService.Get(change.Key);
                bonus.EndDate = change.Value;
                BonusService.Update(bonus);
            }

            #endregion Обратно деактивируем
        }

        /// <summary>
        /// Процент бонусов по умолчанию
        /// </summary>
        /// <returns></returns>
        public static decimal GetBonusDefaultPercent()
        {
            if (CacheManager.Contains(BonusFirstPercentCacheKey))
                return CacheManager.Get<decimal>(BonusFirstPercentCacheKey);

            var grade = GradeService.Get(BonusSystem.DefaultGrade);

            var percent = grade.BonusPercent;

            CacheManager.Insert(BonusFirstPercentCacheKey, percent);
            return percent;
        }

        /// <summary>
        /// Список грейдов компании
        /// </summary>
        public static List<Grade> GetGrades()
        {
            if (CacheManager.Contains(BonusGradesCacheKey))
                return CacheManager.Get<List<Grade>>(BonusGradesCacheKey);

            var grades = GradeService.GetAll();

            CacheManager.Insert(BonusGradesCacheKey, grades, 2);
            return grades;
        }


        #endregion

        /// <summary>
        /// Расчет стоимости бонуса
        /// </summary>
        /// <param name="totalOrderPrice">Стоимость товаров со скидками и доставкой</param>
        /// <param name="productsPrice">Стоимость товаров со скидками </param>
        /// <param name="bonusAmount">Бонусы</param>
        public static float GetBonusCost(float totalOrderPrice, float productsPrice, float bonusAmount)
        {
            var sumPrice = BonusSystem.BonusType == EBonusType.ByProductsCostWithShipping
                    ? totalOrderPrice
                    : productsPrice;

            var bonusPrice = sumPrice > bonusAmount ? bonusAmount : sumPrice;

            if (BonusSystem.MaxOrderPercent == 100 || (bonusPrice * 100 / sumPrice) <= BonusSystem.MaxOrderPercent)
                return bonusPrice.SimpleRoundPrice();

            return (sumPrice * BonusSystem.MaxOrderPercent / 100).SimpleRoundPrice();
        }

        /// <summary>
        /// Расчет стоимости бонусов, которые будут начислены на карту
        /// </summary>
        /// <param name="priceWithShippingAndDiscount"></param>
        /// <param name="priceWhitDiscount"></param>
        /// <param name="bonusPercent"></param>
        public static float GetBonusPlus(float priceWithShippingAndDiscount, float priceWhitDiscount, decimal bonusPercent)
        {
            if (bonusPercent == 0)
                return 0;

            var price = BonusSystem.BonusType == EBonusType.ByProductsCostWithShipping
                    ? priceWithShippingAndDiscount
                    : priceWhitDiscount;

            return (price * (float)bonusPercent / 100).SimpleRoundPrice();
        }

        public static BonusCost GetBonusCost(ShoppingCart cart, float shippingPrice = 0, float appliedBonuses = 0, bool wantBonusCard = true)
        {
            if (cart.Coupon != null && BonusSystem.ForbidOnCoupon)
                return new BonusCost(0, 0);
            var bonusCard = GetCard(CustomerContext.CustomerId);
            return GetBonusCost(bonusCard, cart, shippingPrice, appliedBonuses, wantBonusCard);
        }

        /// <summary>
        /// Расчет bonusCost и bonusPlus (стоимость и сколько будет зачислено)
        /// </summary>
        public static BonusCost GetBonusCost(Card bonusCard, ShoppingCart cart, float shippingPrice = 0, float appliedBonuses = 0, bool wantBonusCard = true)
        {
            if (bonusCard != null && bonusCard.Blocked || cart.Coupon != null && BonusSystem.ForbidOnCoupon)
                return new BonusCost(0, 0);

            var cartTotalPrice = cart.TotalPrice;
            var cartTotalDiscount = cart.TotalDiscount;

            // Сколько боусов списать (bonusCost) расчитывается из цены товаров со скидкой (priceWithDiscount)
            // Сколько бонусов начислить (bonusPlus) расчитывается из товаров, у которых включено начисление бонусов (price)

            var priceWithDiscount = cartTotalPrice - cartTotalDiscount;

            var priceForBonusCost =
                priceWithDiscount - cart.Where(x => x.Offer.Product.DoNotApplyOtherDiscounts)
                                        .Sum(x => (x.PriceWithDiscount - x.PriceWithDiscount/cartTotalPrice*cartTotalDiscount)*x.Amount);
            
            var priceForBonusPlus = 
                priceWithDiscount - cart.Where(x => !x.Offer.Product.AccrueBonuses)
                                        .Sum(x => (x.PriceWithDiscount - x.PriceWithDiscount/cartTotalPrice*cartTotalDiscount)*x.Amount);

            float bonusPlus = 0;
            float bonusCost = 0;
            
            if (bonusCard != null)
            {
                if (appliedBonuses > 0 && bonusCard.BonusesTotalAmount > 0)
                {
                    var bonusAmount = (float)bonusCard.BonusesTotalAmount > appliedBonuses ? appliedBonuses : (float)bonusCard.BonusesTotalAmount;
                    bonusCost = GetBonusCost(priceForBonusCost + shippingPrice, priceForBonusCost, bonusAmount);
                    priceForBonusPlus -= bonusCost;
                }

                bonusPlus = GetBonusPlus(priceForBonusPlus + shippingPrice, priceForBonusPlus, bonusCard.Grade.BonusPercent);
            }
            else if (wantBonusCard)
            {
                bonusPlus =
                    //BonusSystem.BonusesForNewCard +
                    GetBonusPlus(priceForBonusPlus + shippingPrice, priceForBonusPlus, BonusSystem.BonusFirstPercent);
            }

            return new BonusCost(bonusCost, bonusPlus);
        }

        public static void AcceptBonuses(Guid cardId, decimal amount, string reason, string name, DateTime? startDate, DateTime? endDate, int? purchaseId, bool sendSms, bool executeModules = true,
            string transactionIdempotenceKey = null)
        {
            if (amount < 0m) throw new ArgumentException("It cannot be negative.", nameof(amount));
            
            var tempBonus = new Bonus
            {
                CardId = cardId,
                Amount = amount,
                Description = reason,
                StartDate = startDate,
                EndDate = endDate,
                Name = name,
                Status = EBonusStatus.Create
            };

            tempBonus.Id = BonusService.Add(tempBonus);

            var bonusBalance = BonusService.ActualSum(cardId);

            var transLog = Transaction.Factory(cardId, tempBonus.Amount, reason, EOperationType.AddBonus, bonusBalance, purchaseId, tempBonus.Id, transactionIdempotenceKey);
            TransactionService.Create(transLog, executeModules);
            

            if (!sendSms)
                return;

            var customer = CustomerService.GetCustomer(cardId);
            if (customer is null)
                return;
            
            if (customer.StandardPhone is null 
                && customer.EMail.IsNullOrEmpty())
                return;

            NotificationService.Process(cardId, ENotifcationType.OnAddBonus, new OnAddBonusTempalte()
            {
                Bonus = tempBonus.Amount,
                CompanyName = SettingsMain.ShopName,
                Basis = reason,
                Balance = bonusBalance,
                BalanceWithNewBonus =
                    tempBonus.StartDate is null || tempBonus.StartDate <= DateTime.Today
                        ? (decimal?) null // шаблон сам заменит на Balance
                        : bonusBalance + amount
            });
        }

        public static void SubtractBonuses(Bonus bonus, Guid cardId, decimal amount, string reason, bool sendSms)
        {
            if (bonus.Amount <= amount)
                bonus.Amount = 0m;
            else
                bonus.Amount -= amount;
            
            bonus.Status = bonus.Amount == amount ? EBonusStatus.Zero : EBonusStatus.Substract;

            decimal balanceBonuses;
            using (var tr = new TransactionScope())
            {
                BonusService.Update(bonus);
                balanceBonuses = BonusService.ActualSum(cardId);
                var transLog = Transaction.Factory(cardId, amount, reason, EOperationType.SubtractBonus, balanceBonuses, null, bonus.Id, null);
                TransactionService.Create(transLog);
                tr.Complete();
            }

            if (!sendSms)
                return;

            var customer = CustomerService.GetCustomer(cardId);
            if (customer is null)
                return;
            
            if (customer.StandardPhone is null 
                && customer.EMail.IsNullOrEmpty())
                return;

            NotificationService.Process(cardId, ENotifcationType.OnSubtractBonus, new OnSubtractBonusTempalte
            {
                Bonus = amount,
                CompanyName = SettingsMain.ShopName,
                Balance = balanceBonuses,
                Basis = reason
            });
        }

        // TODO: bonus system should have default currency
        // public static Currency BonusSystemCurrency => CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);



        private static readonly Random Rnd = new Random();
        private static long GetRandom(long min, long max)
        {
            var randomLong = min + (long)(Rnd.NextDouble() * (max - min));
            return randomLong;
        }
    }

    public class BonusCost
    {
        public BonusCost(float bonusPrice, float bonusPlus)
        {
            BonusPrice = bonusPrice > 0 ? bonusPrice : 0;
            BonusPlus = bonusPlus > 0 ? bonusPlus : 0;
        }

        public float BonusPrice { get; private set; }
        public float BonusPlus { get; private set; }
    }
}