using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvantShop.Core.Services.FullSearch.Core
{
    public class BaseSearcher<T> : BaseSearch<T> where T : BaseDocument
    {
        #region Fields

        private readonly int _hitsLimit;
        private readonly List<ESearchDeep> _deepLimits;
        private IndexSearcher _searcher;
        
        #endregion

        #region Ctor

        public BaseSearcher(int hitsLimit, ESearchDeep deepLimit) : this(hitsLimit, new List<ESearchDeep> { deepLimit }, string.Empty)
        {
        }

        public BaseSearcher(int hitsLimit, ESearchDeep deepLimit, string path) : this(hitsLimit, new List<ESearchDeep> { deepLimit }, path)
        {
        }


        public BaseSearcher(int hitsLimit, List<ESearchDeep> deepLimits) : this(hitsLimit, deepLimits, string.Empty)
        {
        }

        public BaseSearcher(int hitsLimit, List<ESearchDeep> deepLimits, string path) : base(path)
        {
            _hitsLimit = hitsLimit;
            _deepLimits = deepLimits;
            try
            {
                _searcher = new IndexSearcher(_luceneDirectory, true);
            }
            catch (FileNotFoundException) // вызывается если папки или индекса еще нет
            {
                LuceneSearch.CreateNewIndex<T>();
                base.InitDirectory();
                _searcher = new IndexSearcher(_luceneDirectory, true);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
        }

        #endregion

        public SearchResult SearchItems(string phrase, string field = "", bool ifError = false, bool minimizeSearchResults = false)
        {
            if (_searcher == null)
                return new SearchResult(phrase);

            try
            {
                return _search(phrase, field, minimizeSearchResults);
            }
            catch (FileNotFoundException ex)
            {
                Debug.Log.Error(ex);
                if (ifError) 
                    return new SearchResult(phrase);

                LuceneSearch.CreateNewIndex<T>();

                return SearchItems(phrase, field, true, minimizeSearchResults);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return new SearchResult(phrase);
        }

        private SearchResult _search(string phrase, string field = "", bool minimizeSearchResults = false)
        {
            phrase = StringHelper.ReplaceCirilikSymbol(phrase).ToLower();
            var fields = GetFields(typeof(T));

            ScoreDoc[] hits;

            if (minimizeSearchResults)
                hits = ProcessMinimized(phrase);
            else
            {
                hits = !string.IsNullOrEmpty(field)
                    ? ProcessSingle(field, fields, phrase)
                    : ProcessMulti(fields, phrase);
            }

            var searchResults = new SearchResult(phrase);
            if (hits != null)
            {
                searchResults.Hits = hits.Length;
                searchResults.SearchResultItems = hits.Select(x =>
                {
                    var doc = _searcher.Doc(x.Doc);
                    return new SearchResultItem
                    {
                        Id = doc.Get(nameof(BaseDocument.Id)).TryParseInt(),
                        Score = x.Score,
                    };
                }).ToList();
            }
            return searchResults;
        }
        
        protected virtual ScoreDoc[] ProcessMulti(string[] fields, string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            var mergedQuery = new BooleanQuery();
            mergedQuery = ProcessCondition(mergedQuery);

            var booleanQuery = new BooleanQuery();
            var phrasePrepared = phrase.RemoveSymvolsExt(" ").Trim().ToLower();

            if (string.IsNullOrEmpty(phrasePrepared))
                return null;

            // exact phrase 
            var exactPhraseQuery = GetExactPhraseQuery(phrase, fields);
            booleanQuery.Add(exactPhraseQuery, Occur.SHOULD);
            
            // exact phrasePrepared 
            if (_deepLimits.Contains(ESearchDeep.StrongPhase))
            {
                var query = GetExactPhraseQuery(phrasePrepared, fields);
                if (query != null)
                    booleanQuery.Add(query, Occur.SHOULD);
            }

            // phrase 
            if (_deepLimits.Contains(ESearchDeep.SepareteWords))
            {
                var query = GetSeparateWordsQuery(phrasePrepared, fields);
                if (query != null)
                    booleanQuery.Add(query, Occur.SHOULD);
            }

            // search q*
            if (_deepLimits.Contains(ESearchDeep.WordsStartFrom) || _deepLimits.Contains(ESearchDeep.WordsBetween))
            {
                var phrasePreparedAndParsed = PrepareString(phrasePrepared);

                if (!string.IsNullOrEmpty(phrasePreparedAndParsed) && phrasePreparedAndParsed != phrasePrepared)
                {
                    var q = GetWordsStartFromQuery(phrasePrepared, fields);
                    if (q != null)
                        booleanQuery.Add(q, Occur.SHOULD);
                }

                var query =
                    !string.IsNullOrEmpty(phrasePreparedAndParsed)
                        ? GetWordsStartFromQuery(phrasePreparedAndParsed, fields)
                        : GetWordsStartFromQueryByFieldName(phrase);
                
                if (query != null)
                    booleanQuery.Add(query, Occur.SHOULD);
            }

            // search *q*
            if (_deepLimits.Contains(ESearchDeep.WordsBetween))
            {
                var phrasePreparedAndParsed = PrepareString(phrasePrepared);
                
                if (!string.IsNullOrEmpty(phrasePreparedAndParsed) && phrasePreparedAndParsed != phrasePrepared)
                {
                    var q = GetWordsBetweenQuery(phrasePrepared, fields);
                    if (q != null)
                        booleanQuery.Add(q, Occur.SHOULD);
                }

                var query =
                    !string.IsNullOrEmpty(phrasePreparedAndParsed)
                        ? GetWordsBetweenQuery(phrasePreparedAndParsed, fields)
                        : GetWordsStartFromQueryByFieldName(phrase);
                
                if (query != null)
                    booleanQuery.Add(query, Occur.SHOULD);
            }

            mergedQuery.Add(booleanQuery, Occur.MUST);

            var result = _searcher.Search(mergedQuery, null, _hitsLimit, Sort.RELEVANCE).ScoreDocs;

            return result;
        }
        
        protected virtual ScoreDoc[] ProcessMinimized(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            var mergedQuery = new BooleanQuery();

            mergedQuery = ProcessCondition(mergedQuery);
            var query = new BooleanQuery();
            var phraseClean = phrase.Trim(); // RemoveSymvolsExt(" ")

            if (string.IsNullOrEmpty(phraseClean))
                return null;

            query = MinimizedResultsProcess(phraseClean, query);
            mergedQuery.Add(query, Occur.MUST);

            var result = _searcher.Search(mergedQuery, null, _hitsLimit, Sort.RELEVANCE).ScoreDocs;

            return result;
        }

        protected virtual ScoreDoc[] ProcessSingle(string searchField, string[] fields, string phrase)
        {
            if (!fields.Contains(searchField))
                throw new SearchException($"Field {searchField} is not a search field");

            phrase = phrase.RemoveSymvolsExt(" ").Trim();
            var mergedQuery = new BooleanQuery();

            var parser = new QueryParser(CurrentVersion, searchField, _analyzer);
            var query = ParseQuery(phrase, parser);
            mergedQuery.Add(query, Occur.MUST);

            var searchLike = string.Join(" ", phrase.Trim().Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*"));
            var parserLike = new QueryParser(CurrentVersion, searchField, _analyzer);
            var queryLike = ParseQuery(searchLike, parserLike);
            mergedQuery.Add(queryLike, Occur.MUST);

            return _searcher.Search(query, _hitsLimit).ScoreDocs;
        }

        private Query GetExactPhraseQuery(string phrase, string[] fields)
        {
            var parser = new MultiFieldQueryParser(CurrentVersion, fields, _analyzer);
            var query = ParseQuery("\"" + phrase + "\"", parser);
            query.Boost = 1000;
            
            return query;
        }

        private Query GetSeparateWordsQuery(string phrase, string[] fields)
        {
            phrase = RemoveStopWords(phrase);

            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            var searchQueryPhrase = string.Join(" ", phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            searchQueryPhrase = "(" + searchQueryPhrase + ")";
            var parser = new MultiFieldQueryParser(CurrentVersion, fields, _analyzer);
            var query = ParseQuery(searchQueryPhrase, parser);
            query.Boost = 100;

            return query;
        }

        private Query GetWordsStartFromQuery(string phrase, string[] fields)
        {
            phrase = RemoveStopWords(phrase);

            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            var searchLike = string.Join(" ", phrase.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(x => x + "*"));

            var parser = new MultiFieldQueryParser(CurrentVersion, fields, _analyzer);
            var query = ParseQuery(searchLike, parser);
            query.Boost = 10;

            // тоже самое, но выставляет boost по полю
            // var booleanQuery = new BooleanQuery();
            //
            // var lowPriorityFields = new List<string>() {nameof(ProductDocument.Desc), nameof(ProductDocument.Tags)};
            // var mediumPriorityFields = new List<string>() {nameof(ProductDocument.MainCategoryName), nameof(ProductDocument.ParentCategoryName)};
            //
            // foreach (var field in fields)
            // {
            //     var query = new QueryParser(CurrentVersion, field, _analyzer).Parse(searchLike);
            //     query.Boost =
            //         lowPriorityFields.Contains(field)
            //             ? 1f
            //             : mediumPriorityFields.Contains(field) ? 5 : 10;
            //     booleanQuery.Add(query, Occur.SHOULD);
            // }
            //
            // return booleanQuery;
            
            return query;
        }

        private Query GetWordsStartFromQueryByFieldName(string phrase)
        {
            var searchLike = string.Join(" ", phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim() + "*"));

            var parser = new QueryParser(CurrentVersion, nameof(ProductDocument.NameNotAnalyzed), new KeywordAnalyzer());
            var query = ParseQuery(searchLike, parser);
            query.Boost = 1001;
            
            return query;
        }

        private Query GetWordsBetweenQuery(string phrase, string[] fields)
        {
            phrase = RemoveStopWords(phrase);

            if (string.IsNullOrWhiteSpace(phrase))
                return null;

            var search2Like = string.Join(" ", phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => "*" + x + "*"));

            var parser = new MultiFieldQueryParser(CurrentVersion, fields, _analyzer) {AllowLeadingWildcard = true};
            var query = ParseQuery(search2Like, parser);
            query.Boost = 999;

            return query;
        }

        private BooleanQuery MinimizedResultsProcess(string phrase, BooleanQuery mergedQuery)
        {
            var searchQ = string.Join(" ",
                phrase.RemoveSymvolsExt(" ").Trim()
                    .Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => "+" + x));

            foreach (var field in new List<string> { nameof(ProductDocument.ArtNo), nameof(ProductDocument.Name), nameof(ProductDocument.MainCategoryName), nameof(ProductDocument.ParentCategoryName) })
            {
                var parser = new QueryParser(CurrentVersion, field, _analyzer);
                var query = ParseQuery(searchQ, parser);
                mergedQuery.Add(query, Occur.SHOULD);
            }

            return mergedQuery;
        }

        protected virtual BooleanQuery ProcessCondition(BooleanQuery bq)
        {
            return bq;
        }

        protected virtual List<string> GetIgnoredFields()
        {
            return null;
        }

        private string[] GetFields(Type type)
        {
            var fields = new List<string>();
            var ignoredFields = GetIgnoredFields();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (ignoredFields != null && ignoredFields.Contains(property.Name))
                    continue;

                var attributes = property.GetCustomAttributes(true).OfType<SearchField>();
                foreach (var attr in attributes)
                {
                    if (attr.CombinedSearchFields.Length > 0)
                    {
                        for (var i = 0; i < attr.CombinedSearchFields.Length; i++)
                            fields.Add(attr.CombinedSearchFields[i]);
                    }
                    else
                        fields.Add(property.Name);
                }
            }
            return fields.ToArray();
        }

        /// <summary> 
        /// Parse the given query string to a Lucene Query object 
        /// </summary> 
        /// <param name="phrase">The query string</param> 
        /// <param name="parser">The Lucene QueryParser</param> 
        /// <returns>A Lucene Query object</returns> 
        protected Query ParseQuery(string phrase, QueryParser parser)
        {
            Query q;
            try
            {
                q = parser.Parse(phrase);
            }
            catch (ParseException e)
            {
                Debug.Log.Warn("Query parser exception", e);
                q = null;
            }

            if (q == null || string.IsNullOrEmpty(q.ToString()))
            {
                try
                {
                    var cooked = QueryParser.Escape(phrase.ToLower());
                    q = parser.Parse(cooked);
                }
                catch
                {
                    q = null;
                }
            }
            return q;
        }

        protected string PrepareString(string str)
        {
            var query = ParseQuery(str, new QueryParser(CurrentVersion, "", _analyzer));
            return query?.ToString();
        }

        private string RemoveStopWords(string str)
        {
            TokenStream tokenStream = new StandardTokenizer(CurrentVersion, new StringReader(str.Trim()));
            tokenStream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(CurrentVersion), tokenStream, RUSSIAN_STOP_WORDS_30);
            StringBuilder sb = new StringBuilder();
            var charTermAttribute = tokenStream.GetAttribute<ITermAttribute>();
            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                String term = charTermAttribute.Term.ToString();

                sb.Append(term + " ");
            }
            return sb.ToString().Trim();
        }


        #region  IDisposable Support

        private bool _disposed; // To detect redundant calls

        // IDisposable

        ~BaseSearcher()// the finalizer
        {
            Dispose(false);
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public override void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_searcher != null)
                {
                    _searcher.Dispose();
                    _searcher = null;
                }
            }
            _disposed = true;
            base.Dispose(disposing);
        }

        #endregion
    }
}