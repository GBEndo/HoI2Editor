﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HoI2Editor.Parsers;
using HoI2Editor.Properties;
using HoI2Editor.Utilities;

namespace HoI2Editor.Models
{
    /// <summary>
    ///     指揮官データ群
    /// </summary>
    public static class Leaders
    {
        #region 公開プロパティ

        /// <summary>
        ///     マスター指揮官リスト
        /// </summary>
        public static List<Leader> Items { get; }

        /// <summary>
        ///     国タグと指揮官ファイル名の対応付け
        /// </summary>
        public static Dictionary<Country, string> FileNameMap { get; }

        /// <summary>
        ///     使用済みIDリスト
        /// </summary>
        public static HashSet<int> IdSet { get; }

        /// <summary>
        ///     階級名
        /// </summary>
        public static string[] RankNames { get; private set; }

        #endregion

        #region 内部フィールド

        /// <summary>
        ///     読み込み済みフラグ
        /// </summary>
        private static bool _loaded;

        /// <summary>
        ///     遅延読み込み用
        /// </summary>
        private static readonly BackgroundWorker Worker = new BackgroundWorker();

        /// <summary>
        ///     編集済みフラグ
        /// </summary>
        private static bool _dirtyFlag;

        /// <summary>
        ///     国家ごとの編集済みフラグ
        /// </summary>
        private static readonly bool[] DirtyFlags = new bool[Enum.GetValues(typeof (Country)).Length];

        /// <summary>
        ///     指揮官リストファイルの編集済みフラグ
        /// </summary>
        private static bool _dirtyListFlag;

        #endregion

        #region 公開定数

        /// <summary>
        ///     指揮官特性値
        /// </summary>
        public static readonly uint[] TraitsValues =
        {
            LeaderTraits.LogisticsWizard,
            LeaderTraits.DefensiveDoctrine,
            LeaderTraits.OffensiveDoctrine,
            LeaderTraits.WinterSpecialist,
            LeaderTraits.Trickster,
            LeaderTraits.Engineer,
            LeaderTraits.FortressBuster,
            LeaderTraits.PanzerLeader,
            LeaderTraits.Commando,
            LeaderTraits.OldGuard,
            LeaderTraits.SeaWolf,
            LeaderTraits.BlockadeRunner,
            LeaderTraits.SuperiorTactician,
            LeaderTraits.Spotter,
            LeaderTraits.TankBuster,
            LeaderTraits.CarpetBomber,
            LeaderTraits.NightFlyer,
            LeaderTraits.FleetDestroyer,
            LeaderTraits.DesertFox,
            LeaderTraits.JungleRat,
            LeaderTraits.UrbanWarfareSpecialist,
            LeaderTraits.Ranger,
            LeaderTraits.Mountaineer,
            LeaderTraits.HillsFighter,
            LeaderTraits.CounterAttacker,
            LeaderTraits.Assaulter,
            LeaderTraits.Encircler,
            LeaderTraits.Ambusher,
            LeaderTraits.Disciplined,
            LeaderTraits.ElasticDefenceSpecialist,
            LeaderTraits.Blitzer
        };

        /// <summary>
        ///     指揮官特性名
        /// </summary>
        public static readonly string[] TraitsNames =
        {
            "TRAIT_LOGWIZ",
            "TRAIT_DEFDOC",
            "TRAIT_OFFDOC",
            "TRAIT_WINSPE",
            "TRAIT_TRICKS",
            "TRAIT_ENGINE",
            "TRAIT_FORBUS",
            "TRAIT_PNZLED",
            "TRAIT_COMMAN",
            "TRAIT_OLDGRD",
            "TRAIT_SEAWOL",
            "TRAIT_BLKRUN",
            "TRAIT_SUPTAC",
            "TRAIT_SPOTTE",
            "TRAIT_TNKBUS",
            "TRAIT_CRPBOM",
            "TRAIT_NGHTFL",
            "TRAIT_FLTDES",
            "TRAIT_DSRFOX",
            "TRAIT_JUNGLE",
            "TRAIT_URBAN",
            "TRAIT_FOREST",
            "TRAIT_MOUNTAIN",
            "TRAIT_HILLS",
            "TRAIT_COUNTER",
            "TRAIT_ASSAULT",
            "TRAIT_ENCIRCL",
            "TRAIT_AMBUSH",
            "TRAIT_DELAY",
            "TRAIT_TATICAL",
            "TRAIT_BREAK"
        };

        #endregion

        #region 初期化

        /// <summary>
        ///     静的コンストラクタ
        /// </summary>
        static Leaders()
        {
            // マスター指揮官リスト
            Items = new List<Leader>();

            // 国タグと指揮官ファイル名の対応付け
            FileNameMap = new Dictionary<Country, string>();

            // 使用済みIDリスト
            IdSet = new HashSet<int>();

            // 階級
            RankNames = new[] { "", Resources.Rank3, Resources.Rank2, Resources.Rank1, Resources.Rank0 };
        }

        #endregion

        #region ファイル読み込み

        /// <summary>
        ///     指揮官ファイルの再読み込みを要求する
        /// </summary>
        public static void RequestReload()
        {
            _loaded = false;
        }

        /// <summary>
        ///     指揮官ファイル群を再読み込みする
        /// </summary>
        public static void Reload()
        {
            // 読み込み前なら何もしない
            if (!_loaded)
            {
                return;
            }

            _loaded = false;

            Load();
        }

        /// <summary>
        ///     指揮官ファイル群を読み込む
        /// </summary>
        public static void Load()
        {
            // 既に読み込み済みならば何もしない
            if (_loaded)
            {
                return;
            }

            // 読み込み途中ならば完了を待つ
            if (Worker.IsBusy)
            {
                WaitLoading();
                return;
            }

            LoadFiles();
        }

        /// <summary>
        ///     指揮官ファイル群を遅延読み込みする
        /// </summary>
        /// <param name="handler">読み込み完了イベントハンドラ</param>
        public static void LoadAsync(RunWorkerCompletedEventHandler handler)
        {
            // 既に読み込み済みならば完了イベントハンドラを呼び出す
            if (_loaded)
            {
                handler?.Invoke(null, new RunWorkerCompletedEventArgs(null, null, false));
                return;
            }

            // 読み込み完了イベントハンドラを登録する
            if (handler != null)
            {
                Worker.RunWorkerCompleted += handler;
            }

            // 読み込み途中ならば戻る
            if (Worker.IsBusy)
            {
                return;
            }

            // ここで読み込み済みならば既に完了イベントハンドラを呼び出しているので何もせずに戻る
            if (_loaded)
            {
                return;
            }

            // 遅延読み込みを開始する
            Worker.DoWork += OnWorkerDoWork;
            Worker.RunWorkerAsync();
        }

        /// <summary>
        ///     読み込み完了まで待機する
        /// </summary>
        public static void WaitLoading()
        {
            while (Worker.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     遅延読み込み処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            LoadFiles();
        }

        /// <summary>
        ///     指揮官ファイル群を読み込む
        /// </summary>
        private static void LoadFiles()
        {
            Items.Clear();
            IdSet.Clear();
            FileNameMap.Clear();

            switch (Game.Type)
            {
                case GameType.HeartsOfIron2:
                case GameType.ArsenalOfDemocracy:
                    if (!LoadHoI2())
                    {
                        return;
                    }
                    break;

                case GameType.DarkestHour:
                    if (!LoadDh())
                    {
                        return;
                    }
                    break;
            }

            // 編集済みフラグを解除する
            _dirtyFlag = false;

            // 読み込み済みフラグを設定する
            _loaded = true;
        }

        /// <summary>
        ///     指揮官ファイル群を読み込む(HoI2/AoD/DH-MOD未使用時)
        /// </summary>
        /// <returns>読み込みに失敗すればfalseを返す</returns>
        private static bool LoadHoI2()
        {
            List<string> filelist = new List<string>();
            string folderName;
            bool error = false;

            // 保存フォルダ内の指揮官ファイルを読み込む
            if (Game.IsExportFolderActive)
            {
                folderName = Game.GetExportFileName(Game.LeaderPathName);
                if (Directory.Exists(folderName))
                {
                    foreach (string fileName in Directory.GetFiles(folderName, "*.csv"))
                    {
                        try
                        {
                            // 指揮官ファイルを読み込む
                            LoadFile(fileName);

                            // 指揮官ファイル一覧に読み込んだファイル名を登録する
                            string name = Path.GetFileName(fileName);
                            if (!string.IsNullOrEmpty(name))
                            {
                                filelist.Add(name.ToLower());
                            }
                        }
                        catch (Exception)
                        {
                            error = true;
                            Log.Error("[Leader] Read error: {0}", fileName);
                            if (MessageBox.Show($"{Resources.FileReadError}: {fileName}",
                                Resources.EditorLeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Error)
                                == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // MODフォルダ内の指揮官ファイルを読み込む
            if (Game.IsModActive)
            {
                folderName = Game.GetModFileName(Game.LeaderPathName);
                if (Directory.Exists(folderName))
                {
                    foreach (string fileName in Directory.GetFiles(folderName, "*.csv"))
                    {
                        try
                        {
                            // 指揮官ファイルを読み込む
                            LoadFile(fileName);

                            // 指揮官ファイル一覧に読み込んだファイル名を登録する
                            string name = Path.GetFileName(fileName);
                            if (!string.IsNullOrEmpty(name))
                            {
                                filelist.Add(name.ToLower());
                            }
                        }
                        catch (Exception)
                        {
                            error = true;
                            Log.Error("[Leader] Read error: {0}", fileName);
                            if (MessageBox.Show($"{Resources.FileReadError}: {fileName}",
                                Resources.EditorLeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Error)
                                == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // バニラフォルダ内の指揮官ファイルを読み込む
            folderName = Path.Combine(Game.FolderName, Game.LeaderPathName);
            if (Directory.Exists(folderName))
            {
                foreach (string fileName in Directory.GetFiles(folderName, "*.csv"))
                {
                    // MODフォルダ内で読み込んだファイルは無視する
                    string name = Path.GetFileName(fileName);
                    if (string.IsNullOrEmpty(name) || filelist.Contains(name.ToLower()))
                    {
                        continue;
                    }

                    try
                    {
                        // 指揮官ファイルを読み込む
                        LoadFile(fileName);
                    }
                    catch (Exception)
                    {
                        error = true;
                        Log.Error("[Leader] Read error: {0}", fileName);
                        if (MessageBox.Show($"{Resources.FileReadError}: {fileName}",
                            Resources.EditorLeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Error)
                            == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
            }

            return !error;
        }

        /// <summary>
        ///     指揮官ファイル群を読み込む(DH-MOD使用時)
        /// </summary>
        /// <returns>読み込みに失敗すればfalseを返す</returns>
        private static bool LoadDh()
        {
            // 指揮官リストファイルが存在しなければ従来通りの読み込み方法を使用する
            string listFileName = Game.GetReadFileName(Game.DhLeaderListPathName);
            if (!File.Exists(listFileName))
            {
                return LoadHoI2();
            }

            // 指揮官リストファイルを読み込む
            IEnumerable<string> fileList;
            try
            {
                fileList = LoadList(listFileName);
            }
            catch (Exception)
            {
                Log.Error("[Leader] Read error: {0}", listFileName);
                MessageBox.Show($"{Resources.FileReadError}: {listFileName}",
                    Resources.EditorLeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool error = false;
            foreach (string fileName in fileList.Select(name => Game.GetReadFileName(Game.LeaderPathName, name)))
            {
                try
                {
                    // 指揮官ファイルを読み込む
                    LoadFile(fileName);
                }
                catch (Exception)
                {
                    error = true;
                    Log.Error("[Leader] Read error: {0}", fileName);
                    if (MessageBox.Show($"{Resources.FileReadError}: {fileName}",
                        Resources.EditorLeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Error)
                        == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
            }

            return !error;
        }

        /// <summary>
        ///     指揮官リストファイルを読み込む(DH)
        /// </summary>
        private static IEnumerable<string> LoadList(string fileName)
        {
            Log.Verbose("[Leader] Load: {0}", Path.GetFileName(fileName));

            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // 空行
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // コメント行
                    if (line[0] == '#')
                    {
                        continue;
                    }

                    list.Add(line);
                }
            }
            return list;
        }

        /// <summary>
        ///     指揮官ファイルを読み込む
        /// </summary>
        /// <param name="fileName">対象ファイル名</param>
        private static void LoadFile(string fileName)
        {
            Log.Verbose("[Leader] Load: {0}", Path.GetFileName(fileName));

            using (CsvLexer lexer = new CsvLexer(fileName))
            {
                // 空ファイルを読み飛ばす
                if (lexer.EndOfStream)
                {
                    return;
                }

                // ヘッダ行読み込み
                lexer.SkipLine();

                // ヘッダ行のみのファイルを読み飛ばす
                if (lexer.EndOfStream)
                {
                    return;
                }

                // 1行ずつ順に読み込む
                Country country = Country.None;
                while (!lexer.EndOfStream)
                {
                    Leader leader = ParseLine(lexer);

                    // 空行を読み飛ばす
                    if (leader == null)
                    {
                        continue;
                    }

                    Items.Add(leader);

                    if (country == Country.None)
                    {
                        country = leader.Country;
                        if (country != Country.None && !FileNameMap.ContainsKey(country))
                        {
                            FileNameMap.Add(country, lexer.FileName);
                        }
                    }
                }

                ResetDirty(country);
            }
        }

        /// <summary>
        ///     指揮官定義行を解釈する
        /// </summary>
        /// <param name="lexer">字句解析器</param>
        /// <returns>指揮官データ</returns>
        private static Leader ParseLine(CsvLexer lexer)
        {
            string[] tokens = lexer.GetTokens();

            // 空行を読み飛ばす
            if (tokens == null)
            {
                return null;
            }

            // トークン数が足りない行は読み飛ばす
            if (tokens.Length != (Misc.EnableRetirementYearLeaders ? 19 : 18))
            {
                Log.Warning("[Leader] Invalid token count: {0} ({1} L{2})", tokens.Length, lexer.FileName, lexer.LineNo);
                // 末尾のxがない/余分な項目がある場合は解析を続ける
                if (tokens.Length < (Misc.EnableRetirementYearLeaders ? 18 : 17))
                {
                    return null;
                }
            }

            // 名前指定のない行は読み飛ばす
            if (string.IsNullOrEmpty(tokens[0]))
            {
                return null;
            }

            Leader leader = new Leader();
            int index = 0;

            // 名前
            leader.Name = tokens[index];
            index++;

            // ID
            int id;
            if (!int.TryParse(tokens[index], out id))
            {
                Log.Warning("[Leader] Invalid id: {0} [{1}] ({1} L{2})", tokens[index], leader.Name, lexer.FileName,
                    lexer.LineNo);
                return null;
            }
            leader.Id = id;
            index++;

            // 国家
            if (string.IsNullOrEmpty(tokens[index]) || !Countries.StringMap.ContainsKey(tokens[index].ToUpper()))
            {
                Log.Warning("[Leader] Invalid country: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id, leader.Name,
                    lexer.FileName, lexer.LineNo);
                return null;
            }
            leader.Country = Countries.StringMap[tokens[index].ToUpper()];
            index++;

            // 任官年
            for (int i = 0; i < 4; i++)
            {
                int rankYear;
                if (int.TryParse(tokens[index], out rankYear))
                {
                    leader.RankYear[i] = rankYear;
                }
                else
                {
                    leader.RankYear[i] = 1990;
                    Log.Warning("[Leader] Invalid rank{0} year: {1} [{2}: {3}] ({4} L{5})", i, tokens[index], leader.Id,
                        leader.Name, lexer.FileName, lexer.LineNo);
                }
                index++;
            }

            // 理想階級
            int idealRank;
            if (int.TryParse(tokens[index], out idealRank) && 0 <= idealRank && idealRank <= 3)
            {
                leader.IdealRank = (LeaderRank) (4 - idealRank);
            }
            else
            {
                leader.IdealRank = LeaderRank.None;
                Log.Warning("[Leader] Invalid ideal rank: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // 最大スキル
            int maxSkill;
            if (int.TryParse(tokens[index], out maxSkill))
            {
                leader.MaxSkill = maxSkill;
            }
            else
            {
                leader.MaxSkill = 0;
                Log.Warning("[Leader] Invalid max skill: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // 指揮官特性
            uint traits;
            if (uint.TryParse(tokens[index], out traits))
            {
                leader.Traits = traits;
            }
            else
            {
                leader.Traits = LeaderTraits.None;
                Log.Warning("[Leader] Invalid trait: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // スキル
            int skill;
            if (int.TryParse(tokens[index], out skill))
            {
                leader.Skill = skill;
            }
            else
            {
                leader.Skill = 0;
                Log.Warning("[Leader] Invalid skill: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id, leader.Name,
                    lexer.FileName, lexer.LineNo);
            }
            index++;

            // 経験値
            int experience;
            if (int.TryParse(tokens[index], out experience))
            {
                leader.Experience = experience;
            }
            else
            {
                leader.Experience = 0;
                Log.Warning("[Leader] Invalid experience: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // 忠誠度
            int loyalty;
            if (int.TryParse(tokens[index], out loyalty))
            {
                leader.Loyalty = loyalty;
            }
            else
            {
                leader.Loyalty = 0;
                Log.Warning("[Leader] Invalid loyalty: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id, leader.Name,
                    lexer.FileName, lexer.LineNo);
            }
            index++;

            // 兵科
            int branch;
            if (int.TryParse(tokens[index], out branch))
            {
                leader.Branch = (Branch) (branch + 1);
            }
            else
            {
                leader.Branch = Branch.None;
                Log.Warning("[Leader] Invalid branch: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id, leader.Name,
                    lexer.FileName, lexer.LineNo);
            }
            index++;

            // 画像ファイル名
            leader.PictureName = tokens[index];
            index++;

            // 開始年
            int startYear;
            if (int.TryParse(tokens[index], out startYear))
            {
                leader.StartYear = startYear;
            }
            else
            {
                leader.StartYear = 1930;
                Log.Warning("[Leader] Invalid start year: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // 終了年
            int endYear;
            if (int.TryParse(tokens[index], out endYear))
            {
                leader.EndYear = endYear;
            }
            else
            {
                leader.EndYear = 1970;
                Log.Warning("[Leader] Invalid end year: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                    leader.Name, lexer.FileName, lexer.LineNo);
            }
            index++;

            // 引退年
            if (Misc.EnableRetirementYearLeaders)
            {
                int retirementYear;
                if (int.TryParse(tokens[index], out retirementYear))
                {
                    leader.RetirementYear = retirementYear;
                }
                else
                {
                    leader.RetirementYear = 1999;
                    Log.Warning("[Leader] Invalid retirement year: {0} [{1}: {2}] ({3} L{4})", tokens[index], leader.Id,
                        leader.Name, lexer.FileName, lexer.LineNo);
                }
            }
            else
            {
                leader.RetirementYear = 1999;
            }

            return leader;
        }

        #endregion

        #region ファイル書き込み

        /// <summary>
        ///     指揮官ファイル群を保存する
        /// </summary>
        /// <returns>保存に失敗すればfalseを返す</returns>
        public static bool Save()
        {
            // 編集済みでなければ何もしない
            if (!IsDirty())
            {
                return true;
            }

            // 読み込み途中ならば完了を待つ
            if (Worker.IsBusy)
            {
                WaitLoading();
            }

            // 指揮官リストファイルを保存する
            if ((Game.Type == GameType.DarkestHour) && IsDirtyList())
            {
                try
                {
                    SaveList();
                }
                catch (Exception)
                {
                    string fileName = Game.GetWriteFileName(Game.DhLeaderListPathName);
                    Log.Error("[Leader] Write error: {0}", fileName);
                    MessageBox.Show($"{Resources.FileReadError}: {fileName}",
                        Resources.EditorLeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            bool error = false;
            foreach (Country country in Countries.Tags
                .Where(country => DirtyFlags[(int) country] && country != Country.None))
            {
                try
                {
                    // 指揮官ファイルを保存する
                    SaveFile(country);
                }
                catch (Exception)
                {
                    error = true;
                    string fileName = Game.GetWriteFileName(Game.LeaderPathName, Game.GetLeaderFileName(country));
                    Log.Error("[Leader] Write error: {0}", fileName);
                    if (MessageBox.Show($"{Resources.FileWriteError}: {fileName}",
                        Resources.EditorLeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Error)
                        == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
            }

            // 保存に失敗していれば戻る
            if (error)
            {
                return false;
            }

            // 編集済みフラグを解除する
            _dirtyFlag = false;

            return true;
        }

        /// <summary>
        ///     指揮官リストファイルを保存する (DH)
        /// </summary>
        private static void SaveList()
        {
            // データベースフォルダが存在しなければ作成する
            string folderName = Game.GetWriteFileName(Game.DatabasePathName);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string fileName = Game.GetWriteFileName(Game.DhLeaderListPathName);
            Log.Info("[Leader] Save: {0}", Path.GetFileName(fileName));

            // 登録された指揮官ファイル名を順に書き込む
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                foreach (string name in FileNameMap.Select(pair => pair.Value))
                {
                    writer.WriteLine(name);
                }
            }

            // 編集済みフラグを解除する
            ResetDirtyList();
        }

        /// <summary>
        ///     指揮官ファイルを保存する
        /// </summary>
        /// <param name="country">国タグ</param>
        private static void SaveFile(Country country)
        {
            // 指揮官フォルダが存在しなければ作成する
            string folderName = Game.GetWriteFileName(Game.LeaderPathName);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string name = Game.GetLeaderFileName(country);
            string fileName = Path.Combine(folderName, name);
            Log.Info("[Leader] Save: {0}", name);

            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                int lineNo = 2;

                // ヘッダ行を書き込む
                writer.WriteLine(
                    Misc.EnableRetirementYearLeaders
                        ? "Name;ID;Country;Rank 3 Year;Rank 2 Year;Rank 1 Year;Rank 0 Year;Ideal Rank;Max Skill;Traits;Skill;Experience;Loyalty;Type;Picture;Start Year;End Year;Retirement Year;x"
                        : "Name;ID;Country;Rank 3 Year;Rank 2 Year;Rank 1 Year;Rank 0 Year;Ideal Rank;Max Skill;Traits;Skill;Experience;Loyalty;Type;Picture;Start Year;End Year;x");

                // 指揮官定義行を順に書き込む
                foreach (Leader leader in Items.Where(leader => leader.Country == country))
                {
                    // 不正な値が設定されている場合は警告をログに出力する
                    if (leader.Branch == Branch.None)
                    {
                        Log.Warning("[Leader] Invalid branch: {0} {1} ({2} L{3})", leader.Id, leader.Name, name, lineNo);
                    }
                    if (leader.IdealRank == LeaderRank.None)
                    {
                        Log.Warning("[Leader] Invalid ideal rank: {0} {1} ({2} L{3})", leader.Id, leader.Name, name,
                            lineNo);
                    }

                    // 指揮官定義行を書き込む
                    if (Misc.EnableRetirementYearLeaders)
                    {
                        writer.WriteLine(
                            "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};x",
                            leader.Name,
                            leader.Id,
                            Countries.Strings[(int) leader.Country],
                            leader.RankYear[0],
                            leader.RankYear[1],
                            leader.RankYear[2],
                            leader.RankYear[3],
                            leader.IdealRank != LeaderRank.None ? IntHelper.ToString(4 - (int) leader.IdealRank) : "",
                            leader.MaxSkill,
                            leader.Traits,
                            leader.Skill,
                            leader.Experience,
                            leader.Loyalty,
                            leader.Branch != Branch.None ? IntHelper.ToString((int) (leader.Branch - 1)) : "",
                            leader.PictureName,
                            leader.StartYear,
                            leader.EndYear,
                            leader.RetirementYear);
                    }
                    else
                    {
                        writer.WriteLine(
                            "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};x",
                            leader.Name,
                            leader.Id,
                            Countries.Strings[(int) leader.Country],
                            leader.RankYear[0],
                            leader.RankYear[1],
                            leader.RankYear[2],
                            leader.RankYear[3],
                            leader.IdealRank != LeaderRank.None ? IntHelper.ToString(4 - (int) leader.IdealRank) : "",
                            leader.MaxSkill,
                            leader.Traits,
                            leader.Skill,
                            leader.Experience,
                            leader.Loyalty,
                            leader.Branch != Branch.None ? IntHelper.ToString((int) (leader.Branch - 1)) : "",
                            leader.PictureName,
                            leader.StartYear,
                            leader.EndYear);
                    }

                    // 編集済みフラグを解除する
                    leader.ResetDirtyAll();

                    lineNo++;
                }
            }

            ResetDirty(country);
        }

        #endregion

        #region 指揮官リスト操作

        /// <summary>
        ///     指揮官リストに項目を追加する
        /// </summary>
        /// <param name="leader">挿入対象の項目</param>
        public static void AddItem(Leader leader)
        {
            Log.Info("[Leader] Add leader: ({0}: {1}) <{2}>", leader.Id, leader.Name,
                Countries.Strings[(int) leader.Country]);

            Items.Add(leader);
        }

        /// <summary>
        ///     指揮官リストに項目を挿入する
        /// </summary>
        /// <param name="leader">挿入対象の項目</param>
        /// <param name="position">挿入先の項目</param>
        public static void InsertItem(Leader leader, Leader position)
        {
            int index = Items.IndexOf(position) + 1;

            Log.Info("[Leader] Insert leader: {0} ({1}: {2}) <{3}>", index, leader.Id, leader.Name,
                Countries.Strings[(int) leader.Country]);

            Items.Insert(index, leader);
        }

        /// <summary>
        ///     指揮官リストから項目を削除する
        /// </summary>
        /// <param name="leader">削除対象の項目</param>
        public static void RemoveItem(Leader leader)
        {
            Log.Info("[Leader] Remove leader: ({0}: {1}) <{2}>", leader.Id, leader.Name,
                Countries.Strings[(int) leader.Country]);

            Items.Remove(leader);

            // 使用済みIDリストから削除する
            IdSet.Remove(leader.Id);
        }

        /// <summary>
        ///     指揮官リストの項目を移動する
        /// </summary>
        /// <param name="src">移動元の項目</param>
        /// <param name="dest">移動先の項目</param>
        public static void MoveItem(Leader src, Leader dest)
        {
            int srcIndex = Items.IndexOf(src);
            int destIndex = Items.IndexOf(dest);

            Log.Info("[Leader] Move leader: {0} -> {1} ({2}: {3}) <{4}>", srcIndex, destIndex, src.Id, src.Name,
                Countries.Strings[(int) src.Country]);

            if (srcIndex > destIndex)
            {
                // 上へ移動する場合
                Items.Insert(destIndex, src);
                Items.RemoveAt(srcIndex + 1);
            }
            else
            {
                // 下へ移動する場合
                Items.Insert(destIndex + 1, src);
                Items.RemoveAt(srcIndex);
            }
        }

        #endregion

        #region ID操作

        /// <summary>
        ///     未使用の指揮官IDを取得する
        /// </summary>
        /// <param name="country">対象の国タグ</param>
        /// <returns>指揮官ID</returns>
        public static int GetNewId(Country country)
        {
            // 対象国の指揮官IDの最大値+1から検索を始める
            int id = 1;
            if (country != Country.None)
            {
                List<int> ids = Items.Where(leader => leader.Country == country).Select(leader => leader.Id).ToList();
                if (ids.Any())
                {
                    id = ids.Max() + 1;
                }
            }
            // 未使用IDが見つかるまでIDを1ずつ増やす
            while (IdSet.Contains(id))
            {
                id++;
            }
            return id;
        }

        #endregion

        #region 編集済みフラグ操作

        /// <summary>
        ///     編集済みかどうかを取得する
        /// </summary>
        /// <returns>編集済みならばtrueを返す</returns>
        public static bool IsDirty()
        {
            return (_dirtyFlag || _dirtyListFlag);
        }

        /// <summary>
        ///     指揮官リストファイルが編集済みかどうかを取得する
        /// </summary>
        /// <returns>編集済みならばtrueを返す</returns>
        private static bool IsDirtyList()
        {
            return _dirtyListFlag;
        }

        /// <summary>
        ///     編集済みかどうかを取得する
        /// </summary>
        /// <param name="country">国タグ</param>
        /// <returns>編集済みならばtrueを返す</returns>
        public static bool IsDirty(Country country)
        {
            return DirtyFlags[(int) country];
        }

        /// <summary>
        ///     編集済みフラグを設定する
        /// </summary>
        /// <param name="country">国タグ</param>
        public static void SetDirty(Country country)
        {
            DirtyFlags[(int) country] = true;
            _dirtyFlag = true;
        }

        /// <summary>
        ///     指揮官リストファイルの編集済みフラグを設定する
        /// </summary>
        public static void SetDirtyList()
        {
            _dirtyListFlag = true;
        }

        /// <summary>
        ///     編集済みフラグを解除する
        /// </summary>
        /// <param name="country">国タグ</param>
        private static void ResetDirty(Country country)
        {
            DirtyFlags[(int) country] = false;
        }

        /// <summary>
        ///     指揮官リストファイルの編集済みフラグを解除する
        /// </summary>
        private static void ResetDirtyList()
        {
            _dirtyListFlag = false;
        }

        #endregion
    }
}