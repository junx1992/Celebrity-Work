using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Celebrity3._2
{
    class Program
    {
        static string _celebrityname = null;
        static string _time = null;
        string _sessionlog_file = null;
        string _bigdaysfilefolder = @"D:\resource\bigdays";
        string _bigdaysfile = null;
        static string _stopwords = @"D:\resource\my reference\stopwords.txt";
        string _sessionlog_folder = @"D:\result\SessionlogDay";
        string _sessionlogfilename = @"SessionlogDay";

        static string _outputrankvideo = @"D:\result\OutPut\RankVideo\OutPutRankVideo";
        List<string> _weektime = new List<string>();

        NewsLog newslog = new NewsLog(_time, _stopwords, _celebrityname);
        VideoLog videolog = new VideoLog(_time, _outputrankvideo, _celebrityname);
        double Eps = 1.0e-10;
        List<string> _timetoint = new List<string>();
        Stack<string> _bigdays = new Stack<string>();

        public Program()
        {
            _timetoint.Add("0"); _timetoint.Add("1"); _timetoint.Add("2"); _timetoint.Add("3"); _timetoint.Add("4"); _timetoint.Add("5"); _timetoint.Add("6"); _timetoint.Add("7"); _timetoint.Add("8"); _timetoint.Add("9");
            _timetoint.Add("10"); _timetoint.Add("11"); _timetoint.Add("12"); _timetoint.Add("13"); _timetoint.Add("14"); _timetoint.Add("15"); _timetoint.Add("16"); _timetoint.Add("17"); _timetoint.Add("18"); _timetoint.Add("19");
            _timetoint.Add("20"); _timetoint.Add("21"); _timetoint.Add("22"); _timetoint.Add("23"); _timetoint.Add("24"); _timetoint.Add("25"); _timetoint.Add("26"); _timetoint.Add("27"); _timetoint.Add("28"); _timetoint.Add("29"); _timetoint.Add("30"); _timetoint.Add("31");
            //_celebrityname = "johnny depp";
            //_bigdaysfile = _bigdaysfile + _celebrityname + "_bigdays.txt";
            //newslog.SetTime(_time);
            //videolog.SetTime(_time);
            //newslog.SetName(_celebrityname);
            //videolog.SetName(_celebrityname);
        }

        public void TimeTurn()
        {
            if (_bigdays.Count > 0)
            {
                _time = _bigdays.Pop();
                newslog.Clear();
                videolog.Clear();
                newslog.SetTime(_time);
                videolog.SetTime(_time);
                newslog.SetName(_celebrityname);
                videolog.SetName(_celebrityname);
            }
        }

        public void Iterate()
        {
            List<double> hotwordscore1 = new List<double>();
            List<double> hotwordscore2 = new List<double>();
            for (int i = 0; i < newslog.GetNewsHotQueryWordScore.Count; i++)
            {
                hotwordscore1.Add(0);
                hotwordscore2.Add(0);
            }

            List<double> templist = new List<double>();
            double dif = 1;
            while (dif > Eps)
            {
                templist = newslog.GetNewsHotQueryWordScore;
                for (int i = 0; i < templist.Count; i++) hotwordscore1[i] = templist[i];
                newslog.ResetQueryScore();
                newslog.ResetQueryHotWord();
                templist = newslog.GetNewsHotQueryWordScore;
                for (int i = 0; i < templist.Count; i++) hotwordscore2[i] = templist[i];
                if (hotwordscore1.Count != hotwordscore2.Count)
                {
                    Console.WriteLine(false);
                    break;
                }
                else
                {
                    dif = 0;
                    for (int i = 0; i < hotwordscore1.Count; i++)
                    {
                        dif += Math.Abs(hotwordscore1[i] - hotwordscore2[i]);
                    }
                }
            }


        }

        public string TimeTransfer(string input_time)
        {
            string timeformed = null;
            string[] timesplit = input_time.Split('/');
            if (timesplit[0].Length == 1) timesplit[0] = "0" + timesplit[0];
            if (timesplit[1].Length == 1) timesplit[1] = "0" + timesplit[1];
            timeformed = timesplit[0] + timesplit[1];
            return timeformed;
        }

        public void GetWeekTime()
        {
            string time = _time;
            string[] timesplit = time.Split('/');
            int month = _timetoint.IndexOf(timesplit[0]);
            int day = _timetoint.IndexOf(timesplit[1]);
            string buffer_date = null;
            for (int i = 0; i < 6; i++)
            {
                if (month == 2)
                {
                    if (day < 28)
                    {
                        day++;

                    }
                    else
                    {
                        month++;
                        day = 0;
                    }
                    buffer_date = _timetoint[month] + "/" + _timetoint[day] + "/" + "2012";
                    _weektime.Add(buffer_date);
                }
                else
                {
                    if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                    {
                        if (day < 31)
                        {
                            day++;

                        }
                        else
                        {
                            month++;
                            day = 0;
                        }
                        buffer_date = _timetoint[month] + "/" + _timetoint[day] + "/" + "2012";
                        _weektime.Add(buffer_date);
                    }
                    else
                    {
                        if (day < 30)
                        {
                            day++;

                        }
                        else
                        {
                            month++;
                            day = 0;
                        }
                        buffer_date = _timetoint[month] + "/" + _timetoint[day] + "/" + "2012";
                        _weektime.Add(buffer_date);
                    }
                }
            }
        }

        public void InputSessionlog()
        {
            _sessionlog_file = _sessionlog_folder + @"\" + _sessionlogfilename + TimeTransfer(_time) + ".txt";
            FileStream fs = new FileStream(_sessionlog_file, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string buffer = null;

            int verticalblank = 1;
            int queryblank = 5;
            int urlblank = 15;
            int currentblank = 0;

            int verticalstart = 0;
            int verticalend = 0;
            int querystart = 0;
            int queryend = 0;
            int urlstart = 0;
            int urlend = 0;
            string vertical = null;
            string query = null;
            string url = null;

            while (sr.Peek() >= 0)
            {
                buffer = sr.ReadLine();
                if (buffer.Contains(_time))
                {
                    //get vertical, query, url
                    int i = 0;
                    currentblank = 0;
                    while (i < buffer.Length)
                    {

                        if (buffer[i++] == '\t')
                        {
                            currentblank++;
                            if (currentblank == queryblank)
                            {
                                querystart = i;
                            }
                            if (currentblank == urlblank)
                            {
                                urlstart = i;
                            }
                            if (currentblank == verticalblank)
                            {
                                verticalend = i - 2;
                            }
                            if (currentblank == queryblank + 1)
                            {
                                queryend = i - 2;
                            }
                            if (currentblank == urlblank + 1)
                            {
                                urlend = i - 2;
                            }
                        }

                    }

                    query = buffer.Substring(querystart, queryend - querystart + 1);
                    query = query.ToLower();
                    query = query.Trim();
                    url = buffer.Substring(urlstart, urlend - urlstart + 1);
                    vertical = buffer.Substring(verticalstart, verticalend - verticalstart + 1);
                    vertical = vertical.ToLower();

                    //selcet and add
                    if (vertical == "news")
                    {
                        if (url.Contains("http") && query.Contains(_celebrityname)) newslog.AddNews(query, url);
                    }
                    if (vertical == "video")
                    {
                        if (url.Contains("http") && query.Contains(_celebrityname)) videolog.AddVideo(query, url);
                    }
                }
            }
            sr.Close();
            fs.Close();
        }

        public void InputOtherVideoSessionlog()
        {
            foreach (string timetemp in _weektime)
            {
                _sessionlog_file = _sessionlog_folder + @"\" + _sessionlogfilename + TimeTransfer(timetemp) + ".txt";
                FileStream fs = new FileStream(_sessionlog_file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string buffer = null;

                int verticalblank = 1;
                int queryblank = 5;
                int urlblank = 15;
                int currentblank = 0;

                int verticalstart = 0;
                int verticalend = 0;
                int querystart = 0;
                int queryend = 0;
                int urlstart = 0;
                int urlend = 0;
                string vertical = null;
                string query = null;
                string url = null;

                while (sr.Peek() >= 0)
                {
                    buffer = sr.ReadLine();
                    if (buffer.Contains(timetemp))
                    {
                        //get vertical, query, url
                        int i = 0;
                        currentblank = 0;
                        while (i < buffer.Length)
                        {

                            if (buffer[i++] == '\t')
                            {
                                currentblank++;
                                if (currentblank == queryblank)
                                {
                                    querystart = i;
                                }
                                if (currentblank == urlblank)
                                {
                                    urlstart = i;
                                }
                                if (currentblank == verticalblank)
                                {
                                    verticalend = i - 2;
                                }
                                if (currentblank == queryblank + 1)
                                {
                                    queryend = i - 2;
                                }
                                if (currentblank == urlblank + 1)
                                {
                                    urlend = i - 2;
                                }
                            }

                        }

                        query = buffer.Substring(querystart, queryend - querystart + 1);
                        query = query.ToLower();
                        query = query.Trim();
                        url = buffer.Substring(urlstart, urlend - urlstart + 1);
                        vertical = buffer.Substring(verticalstart, verticalend - verticalstart + 1);
                        vertical = vertical.ToLower();

                        //selcet and add
                        //if (vertical == "news")
                        //{
                        //    if (url.Contains("http") && query.Contains(_celebrityname)) newslog.AddNews(query, url);
                        //}
                        if (vertical == "video")
                        {
                            if (url.Contains("http") && query.Contains(_celebrityname)) videolog.AddVideo(query, url);
                        }
                    }
                }
                sr.Close();
                fs.Close();
            }

        }

        public void InputBigDayFile()
        {

            FileStream fs = new FileStream(_bigdaysfile, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            string buffer = null;
            while (sw.Peek() >= 0)
            {
                buffer = sw.ReadLine();
                if (buffer != "")
                {
                    string[] split = buffer.Split('\t');
                    if (split[0][0] > '3' && split[0][0] <= '9') _bigdays.Push(split[0]);//after April
                }
            }
            sw.Close();
            sw.Close();
        }

        public void Run()
        {
            if (Directory.Exists(_bigdaysfilefolder))
            {
                string[] files = Directory.GetFiles(_bigdaysfilefolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string celebrity = name.Replace("_bigdays.txt", null);
                    _celebrityname = celebrity;
                    _bigdaysfile = file;
                    InputBigDayFile();
                    while (_bigdays.Count > 0)
                    {
                        TimeTurn();
                        InputSessionlog();
                        if (videolog.GetUrl.Count == videolog.GetVideoUrl.Count && newslog.GetUrl.Count == newslog.GetUrl.Count)
                        {
                            Console.WriteLine(_celebrityname + "  " + "News and Videos:" + _time + true);
                            Console.WriteLine("NewsQueryCount: " + newslog.GetQuery.Count);
                            Console.WriteLine("NewsUrlCount: " + newslog.GetUrl.Count);
                            Console.WriteLine("VideoQueryCount: " + videolog.GetQuery.Count);
                            Console.WriteLine("VideoUrlCount: " + videolog.GetUrl.Count);
                            newslog.SimpleCountQuery();
                            newslog.SetQueryHotWord();
                            newslog.SetUrlScore();
                            Iterate();
                            videolog.SetNewsHotQuery(newslog.GetNewsHotQueryWord, newslog.GetNewsHotQueryWordScore);
                            videolog.SetVideoQueryScore();
                            videolog.OutPutVidoeResult();
                            videolog.GetVideoUrlScore();
                            videolog.OUtPutRankVideo();
                            Console.WriteLine("Complete this day!");

                        }
                        else
                        {
                            if (newslog.GetUrl.Count != newslog.GetUrl.Count) Console.WriteLine(_celebrityname + "  " + "News :" + _time + "_News error");
                            if (videolog.GetUrl.Count != videolog.GetVideoUrl.Count) Console.WriteLine(_celebrityname + "  " + "Video :" + _time + "_Video error");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Input folder Error");
                return;
            }

        }


        static void Main(string[] args)
        {
            Program MyProgram = new Program();
            MyProgram.Run();            
        }
    }
}
