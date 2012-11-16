using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Celebrity3._2
{
    class VideoLog
    {
        string _outputrankvideofolder;
        string _time;
        string _celebrityname;
        string _videoqueryscorefilefolder = @"D:\result\OutPut\videoqueryscorefile\videoqueryscorefile";
        string _outputvideofolder = @"D:\result\OutPut\outputvideo\outputvideo";
        List<string> _newshotqueryword = new List<string>();
        List<double> _newshotquerywordscore = new List<double>();

        List<string> _query = new List<string>();
        List<string> _videourl = new List<string>();
        List<double> VideoQueryScore = new List<double>();
        List<string> VideoQuery = new List<string>();

        List<string> _videourlsort = new List<string>();
        List<double> _videourlsortscore = new List<double>();

        public VideoLog(string time, string outputrankvideo, string celebrityname)
        {
            _time = time;
            _outputrankvideofolder = outputrankvideo;
            _celebrityname = celebrityname;
        }

        public void AddVideo(string query, string videourl)
        {

            _query.Add(query);
            _videourl.Add(videourl);
        }

        public void SetNewsHotQuery(List<string> newshotqueryword, List<double> newshotquerywordscore)
        {
            _newshotqueryword = newshotqueryword;
            _newshotquerywordscore = newshotquerywordscore;
        }

        public void GetVideoUrlScore()
        {
            int index = 0;
            int scoreindex = 0;
            double score = 0;
            string query = null;
            double sum = 0;
            for (int i = 0; i < _videourl.Count; i++)
            {
                score = 0;
                query = _query[i];
                scoreindex = VideoQuery.IndexOf(query);
                if (scoreindex >= 0) score = VideoQueryScore[scoreindex];
                index = _videourlsort.IndexOf(_videourl[i]);
                sum += score;
                if (index >= 0)
                {
                    _videourlsortscore[index] += score;
                }
                else
                {
                    _videourlsort.Add(_videourl[i]);
                    _videourlsortscore.Add(score);
                }
            }

            for (int i = 0; i < _videourlsortscore.Count; i++)
            {
                _videourlsortscore[i] = _videourlsortscore[i] / sum;
            }

            //sort
            double tempscore = 0;
            string temp = null;
            for (int i = _videourlsort.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (_videourlsortscore[j] > _videourlsortscore[j - 1])
                    {
                        tempscore = _videourlsortscore[j];
                        _videourlsortscore[j] = _videourlsortscore[j - 1];
                        _videourlsortscore[j - 1] = tempscore;

                        temp = _videourlsort[j];
                        _videourlsort[j] = _videourlsort[j - 1];
                        _videourlsort[j - 1] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Set video Score
        /// </summary>
        /// <param name="args"></param>
        public void SetVideoQueryScore()
        {
            string timeformed = _time;
            timeformed = timeformed.Replace("/", "_");
            string videoqueryscorefile = _videoqueryscorefilefolder + "_" + _celebrityname + "_" + timeformed + ".txt";
            double score = 0;
            foreach (string query in _query)
            {
                score = 0;
                if (!VideoQuery.Contains(query))
                {
                    for (int i = 0; i < _newshotquerywordscore.Count; i++)
                    {
                        if (query.Contains(_newshotqueryword[i])) score += _newshotquerywordscore[i];
                    }
                    VideoQuery.Add(query);
                    VideoQueryScore.Add(score);
                }
            }

            string tempquery = null;
            double tempcount = 0;
            //bubble

            for (int i = VideoQuery.Count - 1; i > 0; i--)
            {
                for (int j = i; j > 0; j--)
                {
                    if (VideoQueryScore[j] > VideoQueryScore[j - 1])
                    {
                        tempcount = VideoQueryScore[j];
                        VideoQueryScore[j] = VideoQueryScore[j - 1];
                        VideoQueryScore[j - 1] = tempcount;

                        tempquery = VideoQuery[j];
                        VideoQuery[j] = VideoQuery[j - 1];
                        VideoQuery[j - 1] = tempquery;
                    }
                }
            }



            FileStream fs = new FileStream(videoqueryscorefile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < VideoQuery.Count; i++)
            {
                sw.Write(VideoQuery[i]);
                sw.Write('\t');
                sw.Write(VideoQueryScore[i]);
                sw.WriteLine();
            }
            sw.Close();
            fs.Close();
        }

        public void OUtPutRankVideo()
        {
            string timeformed = _time;
            timeformed = timeformed.Replace("/", "_");
            string _outputrankvideo = _outputrankvideofolder + "_" + _celebrityname + "_" + timeformed + ".txt";
            FileStream fs = new FileStream(_outputrankvideo, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string url = null;

            List<int> index_query = new List<int>();
            int index = 0;
            for (int i = 0; i < _videourlsort.Count; i++)
            {
                url = _videourlsort[i];
                sw.Write(url);
                sw.Write('\t');
                sw.Write(_videourlsortscore[i]);
                sw.Write('\t');


                //find the indexlist
                index = _videourl.IndexOf(url, 0);
                while (index >= 0)
                {
                    index_query.Add(index);
                    if (index != _videourl.Count - 1)
                    {
                        index = _videourl.IndexOf(url, index + 1);
                    }
                    else
                        index = -1;
                }

                sw.Write(index_query.Count);
                sw.Write('\t');

                foreach (int index_que in index_query)
                {
                    sw.Write(_query[index_que]);
                    sw.Write('\t');
                }
                sw.WriteLine();

                index_query.Clear();

            }

        }

        /// <summary>
        /// output the video according to the newsquery
        /// </summary>
        /// <param name="args"></param>
        public void OutPutVidoeResult()
        {
            string timeformed = _time;
            timeformed = timeformed.Replace("/", "_");
            string outputvideo = _outputvideofolder + "_" + _celebrityname + "_" + timeformed + ".txt";
            FileStream fs = new FileStream(outputvideo, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string query = null;
            int index = -1;

            for (int i = 0; i < VideoQuery.Count; i++)
            {
                query = VideoQuery[i];
                if (VideoQueryScore[i] > 0)
                {
                    do
                    {
                        index = _query.IndexOf(query, index + 1);
                        if (index >= 0)
                        {
                            sw.Write(query);
                            sw.Write('\t');
                            sw.Write(_videourl[index]);
                            sw.WriteLine();
                        }
                    } while (index >= 0 && index < _query.Count - 1);

                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
        }

        public void Clear()
        {
            _newshotqueryword.Clear();
            _newshotquerywordscore.Clear();
            _query.Clear();
            _videourl.Clear();
            VideoQuery.Clear();
            VideoQueryScore.Clear();
            _videourlsort.Clear();
            _videourlsortscore.Clear();
        }

        public void SetTime(string time)
        {
            _time = time;
        }
        public void SetName(string celebrityname)
        {
            _celebrityname = celebrityname;
        }

        public List<string> GetQuery { get { return _query; } }
        public List<string> GetUrl { get { return _videourl; } }
        public List<string> GetVideoUrl { get { return _videourl; } }


    }
}
