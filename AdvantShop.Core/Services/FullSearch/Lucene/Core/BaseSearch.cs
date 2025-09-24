using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System.IO;
using System.Web.Hosting;
using Version = Lucene.Net.Util.Version;

namespace AdvantShop.Core.Services.FullSearch
{
    // docs: https://lucene.apache.org/core/2_9_4/queryparsersyntax.html
    // docs: https://lucenenet.apache.org/docs/3.0.3/dc/df9/class_lucene_1_1_net_1_1_analysis_1_1_per_field_analyzer_wrapper.html
    
    public abstract class BaseSearch<T> : IDisposable where T : BaseDocument
    {
        protected Lucene.Net.Store.Directory _luceneDirectory;
        protected Analyzer _analyzer;
        protected readonly string _dataFolder;

        protected static readonly HashSet<string> RUSSIAN_STOP_WORDS_30 = new HashSet<string>()
        {
            "а", "без", "более", "бы", "был", "была", "были", "было", "быть", "в",
            "вам", "вас", "весь", "во", "вот", "все", "всего", "всех", "вы", "где",
            "да", "даже", "для", "до", "его", "ее", "ей", "ею", "если", "есть",
            "еще", "же", "за", "здесь", "и", "из", "или", "им", "их", "к", "как",
            "ко", "когда", "кто", "ли", "либо", "мне", "может", "мы", "на", "надо",
            "наш", "не", "него", "нее", "нет", "ни", "них", "но", "ну", "о", "об",
            "однако", "он", "она", "они", "оно", "от", "очень", "по", "под", "при",
            "с", "со", "так", "также", "такой", "там", "те", "тем", "то", "того",
            "тоже", "той", "только", "том", "ты", "у", "уже", "хотя", "чего", "чей",
            "чем", "что", "чтобы", "чье", "чья", "эта", "эти", "это", "я"
        };

        /// <summary>
        /// The App Data folder - or the folder where the lucene folder is placed under
        /// </summary>
        public string DataFolder => _dataFolder;

        /// <summary>
        /// Constructor that will initialise the LuceneDirectory
        /// </summary>
        protected BaseSearch(string path)
        {
            _dataFolder = string.IsNullOrWhiteSpace(path)
                ? BasePath(typeof(T).Name)
                : path;
            
            InitDirectory();
            
            if (IndexWriter.IsLocked(_luceneDirectory)) 
                IndexWriter.Unlock(_luceneDirectory);
            
            var lockFilePath = Path.Combine(_dataFolder, "write.lock");
            
            if (File.Exists(lockFilePath)) 
                File.Delete(lockFilePath);
            
            _analyzer = new PerFieldAnalyzerWrapper(
                new RussianAnalyzer(CurrentVersion),
                new[]
                {
                    new KeyValuePair<string, Analyzer>(nameof(ProductDocument.NameNotAnalyzed), new KeywordAnalyzer()),
                    new KeyValuePair<string, Analyzer>(nameof(ProductDocument.ArtNoNotAnalyzed), new KeywordAnalyzer()),
                    new KeyValuePair<string, Analyzer>(nameof(ProductDocument.OfferArtNoNotNotAnalyzed), new KeywordAnalyzer()),
                });
        }

        protected void InitDirectory()
        {
            var di = new DirectoryInfo(_dataFolder);
            if (!di.Exists)
                di.Create();
            
            _luceneDirectory = FSDirectory.Open(di, new SimpleFSLockFactory());
        }

        public Version CurrentVersion => Version.LUCENE_30;

        public static string BasePath(string str)
        {
            var root = HostingEnvironment.MapPath("~/App_Data/Lucene");
            var path = Path.Combine(root, str);
            return path;
        }

        #region  IDisposable Support

        private bool _disposed; // To detect redundant calls

        // IDisposable

        ~BaseSearch()// the finalizer
        {
            Dispose(false);
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_analyzer != null)
                {
                    _analyzer.Dispose();
                    _analyzer = null;
                }
                if (_luceneDirectory != null)
                {
                    _luceneDirectory.Dispose();
                    _luceneDirectory = null;
                }
            }
            _disposed = true;
        }
        #endregion
    }
}