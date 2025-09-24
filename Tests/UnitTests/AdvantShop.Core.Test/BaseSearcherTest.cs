using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.FullSearch.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdvantShop.Core.Test
{
    public abstract class BaseSearchTest
    {
        public class TestDocument : ProductDocument
        {
        }

        private const string Path = "/index";
        protected string FullPath = "";

        protected List<TestDocument> Data;

        [OneTimeSetUp]
        public void Init()
        {
            var dir = Directory.GetCurrentDirectory();
            FullPath = dir + Path;

            using (var writer = new BaseWriter<TestDocument>(FullPath))
            {
                writer.AddUpdateItemsToIndex(Data);
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            if (Directory.Exists(FullPath))
                Directory.Delete(FullPath, true);
        }
    }

    [TestFixture]
    public class SearchByNameTest : BaseSearchTest
    {
        public SearchByNameTest()
        {
            Data = new List<TestDocument>
            {
                new TestDocument
                {
                    Id = 1, Name = "Футболка из смесового модала",
                    NameNotAnalyzed = "Футболка из смесового модала"
                },
                new TestDocument
                {
                    Id = 2, Name = "Платье с поясом \"Night\"",
                    NameNotAnalyzed = "Платье с поясом \"Night\""
                },
                new TestDocument
                {
                    Id = 3, Name = "Юбка с запахом и оборкой",
                    NameNotAnalyzed = "Юбка с запахом и оборкой"
                },
                // спецсимволы
                new TestDocument
                {
                    Id = 4, Name = "H&M+ Пижама-рубашка, футболка и шорты COOLMAX®",
                    NameNotAnalyzed = "H&M+ Пижама-рубашка, футболка и шорты COOLMAX®"
                },
                new TestDocument
                {
                    Id = 5, Name = "Tom.m Сандалии A-T49-79-B (Том М)-0",
                    NameNotAnalyzed = "Tom.m Сандалии A-T49-79-B (Том М)-0"
                },
                new TestDocument
                {
                    Id = 6, Name = "Tom.m Сандалии A-T49-79-B (Том М)-0",
                    NameNotAnalyzed = "Tom.m Сандалии A-T49-79-B (Том М)-0"
                },
                new TestDocument
                {
                    Id = 7, Name = "Ботинки КОТОФЕЙ 69-89",
                    NameNotAnalyzed = "Ботинки КОТОФЕЙ 69-89"
                },
                new TestDocument
                {
                    Id = 8, Name = "Побелить стену",
                    NameNotAnalyzed = "побелить стену"
                },
                new TestDocument
                {
                    Id = 9, Name = "Да И \"Нектар любви\" шу пуэр 2011 г. 357 гр",
                    NameNotAnalyzed = "Да И \"Нектар любви\" шу пуэр 2011 г. 357 гр"
                },
            };
        }

        #region Search by word

        /// <summary>
        /// Поиск по одному слову
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchWordSingle([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("платье").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(2, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        #endregion Search by word

        #region Search by phrase

        /// <summary>
        /// Поиск по простой фразе
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseSimple([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var res = reader.SearchItems("Футболка из смесового модала");
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                        Assert.AreEqual(1, res.SearchResultItems.Count);
                        Assert.AreEqual(1, res.SearchResultItems[0].Id);
                        break;

                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, res.SearchResultItems.Count);
                        Assert.AreEqual(1, res.SearchResultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе - название товара с пропущенным словом
        /// </summary>
        /// <remarks>
        /// Благочестие: не находит "Свечи восковые для домашней молитвы" по "Свечи для домашней молитвы"
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseMissedWord([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("Футболка из модала").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                        Assert.AreEqual(0, resultItems.Count);
                        break;

                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, resultItems.Count);
                        Assert.AreEqual(1, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе с кавычками
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseWithQuotes([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("Платье с поясом \"Night\"").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(2, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе, содержащей различные символы
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseWithSpecialSymbols([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("H&M+ Пижама-рубашка, футболка и шорты COOLMAX®").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(4, resultItems[0].Id);
                        break;

                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                        Assert.AreEqual(4, resultItems.Count);
                        Assert.AreEqual(4, resultItems[0].Id);
                        break;

                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(5, resultItems.Count);
                        Assert.AreEqual(4, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе, разделенной дефисом
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseSeparatedByHyphen([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("пижама-рубашка").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(4, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе, содержащей предлог
        /// </summary>
        /// <remarks>
        /// Благочестие: "икона в рамке" - находит все слова на в*, как-то не учитывать предлоги
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseWithPretext([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("платье с поясом").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(2, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе, состоящей из стоп слов
        /// </summary>
        /// <remarks>
        /// Task - 22059
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        // союзы в названии
        public void SearchPhraseWithStopWords([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("да и").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                        Assert.AreEqual(0, resultItems.Count);
                        break;

                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(9, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по фразе, поиск по корню слова (морфология)
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPhraseMorphology([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("побеливший слона").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                        Assert.AreEqual(0, resultItems.Count);
                        break;

                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(8, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        #endregion Search by phrase

        #region Search by numeric

        /// <summary>
        /// Поиск по числу
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchNumeric([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("49").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                        Assert.AreEqual(0, resultItems.Count);
                        break;

                    case ESearchDeep.WordsBetween:
                        Assert.IsTrue(resultItems.Count > 0);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск числа, разделенного дефисом
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchNumericSeparatedByHyphen([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("69-89").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(7, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        #endregion Search by numeric
    }

    [TestFixture]
    public class SearchByArtNoTest : BaseSearchTest
    {
        public SearchByArtNoTest()
        {
            Data = new List<TestDocument>
            {
                new TestDocument
                {
                    Id = 1, ArtNo = "0.9064", OfferArtNo = new[] {"0.9064"},
                    OfferArtNoNotNotAnalyzed = new[] {"0.9064"}
                },
                new TestDocument
                {
                    Id = 2, ArtNo = "10.41004N", OfferArtNo = new[] {"10.41004N"},
                    OfferArtNoNotNotAnalyzed = new[] {"10.41004N"}
                },
                new TestDocument
                {
                    Id = 3, ArtNo = "41000", OfferArtNo = new[] {"41000"},
                    OfferArtNoNotNotAnalyzed = new[] {"41000"}
                },
                new TestDocument
                {
                    Id = 4, ArtNo = "333.123.456", OfferArtNo = new[] {"333.123.456"},
                    OfferArtNoNotNotAnalyzed = new[] {"333.123.456"}
                },
                new TestDocument
                {
                    Id = 5, ArtNo = "AVG6789", OfferArtNo = new[] {"AVG6789"},
                    OfferArtNoNotNotAnalyzed = new[] {"AVG6789"}
                },
                new TestDocument
                {
                    Id = 6, ArtNo = "КК_2777-586/456", OfferArtNo = new[] {"КК_2777-586/456"},
                    OfferArtNoNotNotAnalyzed = new[] {"КК_2777-586/456"}
                },
                new TestDocument
                {
                    Id = 7, ArtNo = "1438/Зима-Осень", OfferArtNo = new[] {"1438/Зима-Осень"},
                    OfferArtNoNotNotAnalyzed = new[] {"1438/Зима-Осень"}
                },
                new TestDocument
                {
                    Id = 8, ArtNo = "Зима 1438-80", OfferArtNo = new[] {"Зима 1438-80"},
                    OfferArtNoNotNotAnalyzed = new[] {"Зима 1438-80"}
                },
                new TestDocument
                {
                    Id = 9, ArtNo = "666",
                    OfferArtNo = new[] {"SG000003504", "SG000003505", "SG000003506", "SG000003507"},
                    OfferArtNoNotNotAnalyzed = new[] {"SG000003504", "SG000003505", "SG000003506", "SG000003507"}
                }
            };
        }

        /// <summary>
        /// Поиск по артикулу, содержащему точки
        /// </summary>
        /// <remarks>
        /// Задача - 22210
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchWithDots([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("0.9064").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(1, resultItems[0].Id);
                        break;

                    case ESearchDeep.WordsBetween:
                        Assert.IsTrue(resultItems.Count > 0);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Проверка правильности сортировки поисковой выдачи при поиске по первым символам
        /// </summary>
        /// <remarks>
        /// Задача - 22135, должны показываться сначала те, которые начинаются с 41 (должна быть правильная сортировка, а не по id)
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchWordStartFromNumericSorting([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("41").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                        Assert.AreEqual(0, resultItems.Count);
                        break;

                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, resultItems.Count);
                        Assert.AreEqual(3, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }
        
        //[Test]
        public void SearchWordStartFromLiteralSorting([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("Зим").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, resultItems.Count);
                        Assert.AreEqual(8, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }
        
        /// <summary>
        /// Поиск по артикулу, содержащему точки
        /// </summary>
        /// <remarks>
        /// Задача - 22135 комментарий
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchPartOfArtNo([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("456").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, resultItems.Count);
                        Assert.AreEqual(4, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Проверка чувствительности к регистру
        /// </summary>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchCheckCaseSensitivity([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItemsUpper = reader.SearchItems("AVG6789").SearchResultItems;
                var resultItemsLower = reader.SearchItems("avg6789").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(resultItemsUpper.Count, resultItemsLower.Count);
                        Assert.AreEqual(1, resultItemsUpper.Count);
                        Assert.AreEqual(5, resultItemsUpper[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        /// <summary>
        /// Поиск по артикулам Оффера, когда артикул товара, не совападает с артикулом оффера
        /// </summary>
        /// <remarks>
        /// Задача - 22936
        /// </remarks>
        /// <param name="eSearchDeep">Глубина поиска</param>
        [Test]
        public void SearchByOfferArtNo([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItemsUpper = reader.SearchItems("SG000003504").SearchResultItems;
                var resultItemsLower = reader.SearchItems("sg000003504").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(resultItemsUpper.Count, resultItemsLower.Count);
                        Assert.AreEqual(1, resultItemsUpper.Count);
                        Assert.AreEqual(9, resultItemsUpper[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }
    }

    [TestFixture]
    public class SearchByTagsTest : BaseSearchTest
    {
        public SearchByTagsTest()
        {
            Data = new List<TestDocument>
            {
                new TestDocument
                {
                    Id = 1, Tags = new[] {"NEW IN"}
                },
                new TestDocument
                {
                    Id = 2, Tags = new[] {"re_style", "re-style", "re style", "re.style"}
                },
                new TestDocument
                {
                    Id = 3, Tags = new[] {"#trending \"RN\""}
                },
            };
        }

        [Test]
        public void SearchByPart([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems("NEW").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.IsTrue(resultItems.Count > 0);
                        Assert.AreEqual(1, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        [Test]
        public void SearchWordWithSeparators([Values] ESearchDeep eSearchDeep, [Values("re_style", "re-style", "re style", "re.style")] string word)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems(word).SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(2, resultItems[0].Id);
                        break;

                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(2, resultItems.Count);
                        Assert.AreEqual(2, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        [Test]
        public void SearchWordWithSymbols([Values] ESearchDeep eSearchDeep, [Values("#trending \"RN\"", "trending \"RN\"", "trending RN")] string word)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItems = reader.SearchItems(word).SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(1, resultItems.Count);
                        Assert.AreEqual(3, resultItems[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }

        [Test]
        public void SearchCheckCaseSensitivity([Values] ESearchDeep eSearchDeep)
        {
            using (var reader = new BaseSearcher<TestDocument>(100, eSearchDeep, FullPath))
            {
                var resultItemsUpper = reader.SearchItems("NEW IN").SearchResultItems;
                var resultItemsLower = reader.SearchItems("new in").SearchResultItems;
                switch (eSearchDeep)
                {
                    case ESearchDeep.StrongPhase:
                    case ESearchDeep.SepareteWords:
                    case ESearchDeep.WordsStartFrom:
                    case ESearchDeep.WordsBetween:
                        Assert.AreEqual(resultItemsUpper.Count, resultItemsLower.Count);
                        Assert.IsTrue(resultItemsUpper.Count > 0);
                        Assert.AreEqual(1, resultItemsUpper[0].Id);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(eSearchDeep), eSearchDeep, null);
                }
            }
        }
    }
}
