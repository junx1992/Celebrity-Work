using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Celebrity3._2
{
    class NewsLog
    {
        string _celebrityname;
        string _stopwords;
        string _time;
        string _queryhotwordfilefolder = @"D:\result\OutPut\queryhotwordscorefile\queryhotwordscorefile";
        List<string> _query = new List<string>();
        List<string> _newsurl = new List<string>();

        List<string> _purequery = new List<string>();
        List<int> _purequerycount = new List<int>();

        List<string> _newsquerysort = new List<string>();
        List<double> _newsquerysortscore = new List<double>();
        List<string> _newshotqueryword = new List<string>();
        List<double> _newshotquerywordscore = new List<double>();

        List<string> _newsurlsort = new List<string>();
        List<double> _newsurlsortscore = new List<double>();

        public NewsLog(string time, string stopwords, string celebrityname)
        {
            _time = time;
            _stopwords = stopwords;
            _celebrityname = celebrityname;
        }

        public void AddNews(string query, string newsurl)
        {

            _query.Add(query);
            _newsurl.Add(newsurl);
            int index = _purequery.IndexOf(query);

            if (index >= 0)
            {
                _purequerycount[index]++;
            }
            else
            {
                _purequery.Add(query);
                _purequerycount.Add(1);
            }

        }

        /// <summary>
        /// simple count of the frequency
        /// </summary>
        /// <param name="args"></param>
        public void SimpleCountQuery()
        {
            string tempquery = null;
            double tempscore = 0;

            _newsquerysort = _purequery;
            for (int i = 0; i < _purequerycount.Count; i++)
            {
                _newsquerysortscore.Add((double)_purequerycount[i] / _query.Count);
            }

            //bubble


            for (int i = _newsquerysortscore.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (_newsquerysortscore[j] > _newsquerysortscore[j - 1])
                    {
                        tempscore = _newsquerysortscore[j];
                        _newsquerysortscore[j] = _newsquerysortscore[j - 1];
                        _newsquerysortscore[j - 1] = tempscore;

                        tempquery = _newsquerysort[j];
                        _newsquerysort[j] = _newsquerysort[j - 1];
                        _newsquerysort[j - 1] = tempquery;
                    }
                }
            }
        }

        public void SetQueryHotWord()
        {

            string timeformed = _time;
            timeformed = timeformed.Replace("/", "_");
            string queryhotwordscorefile = _queryhotwordfilefolder + "_" + _celebrityname + "_" + timeformed + ".txt";
            char[] separate = { ' ', ',', '.', ':', '\t', '&', '!', '@', '#', '$', '%', '-', '(', ')' };
            string query = null;
            int index = 0;
            double score = 0;
            double sum = 0;
            //get stopwords;
            HashSet<String> stops = new HashSet<String>();
            StreamReader sr = new StreamReader(_stopwords);
            while (sr.Peek() > 0)
            {
                stops.Add(sr.ReadLine());
            }
            sr.Close();


            for (int i = 0; i < _newsquerysort.Count; i++)
            {
                query = _newsquerysort[i];
                score = _newsquerysortscore[i];
                string[] query_sentence = query.Split(separate);
                string[] nameword = _celebrityname.Split(' ');
                bool ismeaningfulword = true;
                foreach (string word in query_sentence)
                {
                    ismeaningfulword = true;
                    foreach (string name in nameword)
                    {
                        if (word.Contains(name)) ismeaningfulword = false;
                    }
                    if (stops.Contains(word) || word == "") ismeaningfulword = false;
                    if (ismeaningfulword == true)
                    {
                        index = _newshotqueryword.IndexOf(word);
                        if (index >= 0)
                        {
                            _newshotquerywordscore[index] += score;
                        }
                        else
                        {
                            _newshotqueryword.Add(word);
                            _newshotquerywordscore.Add(score);
                        }
                    }

                }

            }


            foreach (double temp in _newshotquerywordscore)
            {
                sum += temp;
            }

            for (int i = 0; i < _newshotquerywordscore.Count; i++)
            {
                _newshotquerywordscore[i] = _newshotquerywordscore[i] / sum;
            }

            //bubble sort
            double tempscore = 0;
            string tempword = null;
            for (int i = _newshotqueryword.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (_newshotquerywordscore[j] > _newshotquerywordscore[j - 1])
                    {
                        tempscore = _newshotquerywordscore[j];
                        _newshotquerywordscore[j] = _newshotquerywordscore[j - 1];
                        _newshotquerywordscore[j - 1] = tempscore;

                        tempword = _newshotqueryword[j];
                        _newshotqueryword[j] = _newshotqueryword[j - 1];
                        _newshotqueryword[j - 1] = tempword;
                    }
                }
            }

            FileStream fs = new FileStream(queryhotwordscorefile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < _newshotqueryword.Count; i++)
            {
                sw.Write(_newshotqueryword[i]);
                sw.Write('\t');
                sw.Write(_newshotquerywordscore[i]);
                sw.WriteLine();
            }
            sw.Close();
            fs.Close();

        }

        public void ResetQueryHotWord()
        {
            _newshotqueryword.Clear();
            _newshotquerywordscore.Clear();
            SetQueryHotWord();
        }

        public void SetUrlScore()
        {
            string url = null;
            string query = null;
            double score = 0;
            int index = 0;

            double sum = 0;

            for (int i = 0; i < _newsurl.Count; i++)
            {
                url = _newsurl[i];
                query = _query[i];
                score = _newsquerysortscore[_newsquerysort.IndexOf(query)];
                sum += score;
                index = _newsurlsort.IndexOf(url);
                if (index >= 0)
                {
                    _newsurlsortscore[index] += score;
                }
                else
                {
                    _newsurlsort.Add(url);
                    _newsurlsortscore.Add(score);
                }
            }

            //normalize
            for (int i = 0; i < _newsurlsortscore.Count; i++)
            {
                _newsurlsortscore[i] = (double)_newsurlsortscore[i] / sum;
            }


            //bubble sort
            double tempscore = 0;
            string tempurl = null;
            for (int i = _newsurlsort.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (_newsurlsortscore[j] > _newsurlsortscore[j - 1])
                    {
                        tempscore = _newsurlsortscore[j];
                        _newsurlsortscore[j] = _newsurlsortscore[j - 1];
                        _newsurlsortscore[j - 1] = tempscore;

                        tempurl = _newsurlsort[j];
                        _newsurlsort[j] = _newsurlsort[j - 1];
                        _newsurlsort[j - 1] = tempurl;
                    }
                }
            }
        }

        public void ResetScore()
        {
            _newsurlsort.Clear();
            _newsurlsortscore.Clear();
            SetUrlScore();
        }

        public void ResetQueryScore()
        {
            int index_query = 0;
            int index_url = 0;
            string query = null;
            string url = null;
            double sum = 0;
            string tempquery = null;
            double tempscore = 0;
            //clear the original score

            for (int i = 0; i < _newsquerysortscore.Count; i++)
            {
                _newsquerysortscore[i] = 0;
            }

            for (int i = 0; i < _query.Count; i++)
            {
                query = _query[i];
                url = _newsurl[i];
                index_query = _newsquerysort.IndexOf(query);
                index_url = _newsurlsort.IndexOf(url);
                if (index_url < 0 || index_query < 0) Console.WriteLine(false);
                else
                {
                    sum += _newsurlsortscore[index_url];
                    _newsquerysortscore[index_query] += _newsurlsortscore[index_url];
                }
            }

            for (int i = 0; i < _newsquerysortscore.Count; i++)
            {
                _newsquerysortscore[i] = _newsquerysortscore[i] / sum;
            }

            //bubble sort
            for (int i = _newsquerysortscore.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (_newsquerysortscore[j] > _newsquerysortscore[j - 1])
                    {
                        tempscore = _newsquerysortscore[j];
                        _newsquerysortscore[j] = _newsquerysortscore[j - 1];
                        _newsquerysortscore[j - 1] = tempscore;

                        tempquery = _newsquerysort[j];
                        _newsquerysort[j] = _newsquerysort[j - 1];
                        _newsquerysort[j - 1] = tempquery;
                    }
                }
            }

        }

        public void SetTime(string time)
        {
            _time = time;
        }

        public void SetName(string celebrityname)
        {
            _celebrityname = celebrityname;
        }

        public void Clear()
        {
            _query.Clear();
            _newsurl.Clear();
            _purequery.Clear();
            _purequerycount.Clear();
            _newsquerysort.Clear();
            _newsquerysortscore.Clear();
            _newshotqueryword.Clear();
            _newshotquerywordscore.Clear();
            _newsurlsort.Clear();
            _newsurlsortscore.Clear();
        }

        public List<string> GetQuery { get { return _query; } }
        public List<string> GetUrl { get { return _newsurl; } }

        public List<string> GetPureQuery { get { return _purequery; } }
        public List<int> GetPureQueryCount { get { return _purequerycount; } }

        public List<string> GetNewsHotQueryWord { get { return _newshotqueryword; } }
        public List<double> GetNewsHotQueryWordScore { get { return _newshotquerywordscore; } }

    }
}
