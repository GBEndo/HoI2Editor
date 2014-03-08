﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HoI2Editor.Properties;

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
        public static List<Leader> Items { get; private set; }

        /// <summary>
        ///     国タグと指揮官ファイル名の対応付け
        /// </summary>
        public static Dictionary<Country, string> FileNameMap { get; private set; }

        /// <summary>
        ///     使用済みIDリスト
        /// </summary>
        public static HashSet<int> IdSet { get; private set; }

        /// <summary>
        ///     兵科名
        /// </summary>
        public static string[] BranchNames { get; private set; }

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
        ///     編集済みフラグ
        /// </summary>
        private static bool _dirtyFlag;

        /// <summary>
        ///     国家ごとの編集済みフラグ
        /// </summary>
        private static readonly bool[] DirtyFlags = new bool[Enum.GetValues(typeof (Country)).Length];

        /// <summary>
        ///     現在解析中のファイル名
        /// </summary>
        private static string _currentFileName = "";

        /// <summary>
        ///     現在解析中の行番号
        /// </summary>
        private static int _currentLineNo;

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

        #region 内部定数

        /// <summary>
        ///     CSVファイルの区切り文字
        /// </summary>
        private static readonly char[] CsvSeparator = {';'};

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

            // 兵科
            BranchNames = new[] {"", Resources.BranchArmy, Resources.BranchNavy, Resources.BranchAirforce};

            // 階級
            RankNames = new[] {"", Resources.Rank3, Resources.Rank2, Resources.Rank1, Resources.Rank0};
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
            // 読み込み済みならば戻る
            if (_loaded)
            {
                return;
            }

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
            var filelist = new List<string>();
            string folderName;
            bool error = false;

            // 保存フォルダ内の指揮官ファイルを読み込む
            if (Game.IsExportFolderActive)
            {
                folderName = Path.Combine(Game.ExportFolderName, Game.LeaderPathName);
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
                            if (!String.IsNullOrEmpty(name))
                            {
                                filelist.Add(name.ToLower());
                            }
                        }
                        catch (Exception)
                        {
                            error = true;
                            Debug.WriteLine("[Leader] Load failed: {0}", fileName);
                            Log.Write(String.Format("{0}: {1}\n\n", Resources.FileReadError, fileName));
                            if (MessageBox.Show(string.Format("{0}: {1}", Resources.FileReadError, fileName),
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
                folderName = Path.Combine(Game.ModFolderName, Game.LeaderPathName);
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
                            if (!String.IsNullOrEmpty(name))
                            {
                                filelist.Add(name.ToLower());
                            }
                        }
                        catch (Exception)
                        {
                            error = true;
                            Debug.WriteLine("[Leader] Load failed: {0}", fileName);
                            Log.Write(String.Format("{0}: {1}\n\n", Resources.FileReadError, fileName));
                            if (MessageBox.Show(string.Format("{0}: {1}", Resources.FileReadError, fileName),
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
                        Debug.WriteLine("[Leader] Load failed: {0}", fileName);
                        Log.Write(String.Format("{0}: {1}\n\n", Resources.FileReadError, fileName));
                        if (MessageBox.Show(string.Format("{0}: {1}", Resources.FileReadError, fileName),
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
                Debug.WriteLine("[Leader] Load failed: {0}", listFileName);
                Log.Write(String.Format("{0}: {1}\n\n", Resources.FileReadError, listFileName));
                MessageBox.Show(string.Format("{0}: {1}", Resources.FileReadError, listFileName),
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
                    Debug.WriteLine("[Leader] Load failed: {0}", fileName);
                    Log.Write(String.Format("{0}: {1}\n\n", Resources.FileReadError, fileName));
                    if (MessageBox.Show(string.Format("{0}: {1}", Resources.FileReadError, fileName),
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
            Debug.WriteLine(string.Format("[Leader] Load: {0}", Path.GetFileName(fileName)));

            var list = new List<string>();
            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // 空行
                    if (String.IsNullOrEmpty(line))
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
            Debug.WriteLine(string.Format("[Leader] Load: {0}", Path.GetFileName(fileName)));

            using (var reader = new StreamReader(fileName, Encoding.GetEncoding(Game.CodePage)))
            {
                _currentFileName = Path.GetFileName(fileName);
                _currentLineNo = 1;

                // 空ファイルを読み飛ばす
                if (reader.EndOfStream)
                {
                    return;
                }

                // ヘッダ行読み込み
                string line = reader.ReadLine();
                if (String.IsNullOrEmpty(line))
                {
                    return;
                }

                _currentLineNo++;
                var country = Country.None;

                while (!reader.EndOfStream)
                {
                    Leader leader = ParseLine(reader.ReadLine());

                    if (country == Country.None && leader != null)
                    {
                        country = leader.Country;
                        if (country != Country.None && !FileNameMap.ContainsKey(country))
                        {
                            FileNameMap.Add(country, Path.GetFileName(fileName));
                        }
                    }
                    _currentLineNo++;
                }

                ResetDirty(country);
            }
        }

        /// <summary>
        ///     指揮官定義行を解釈する
        /// </summary>
        /// <param name="line">対象文字列</param>
        /// <returns>指揮官データ</returns>
        private static Leader ParseLine(string line)
        {
            // 空行を読み飛ばす
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            string[] tokens = line.Split(CsvSeparator);

            // トークン数が足りない行は読み飛ばす
            if (tokens.Length != (Misc.EnableRetirementYearLeaders ? 19 : 18))
            {
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidTokenCount, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}\n", line));
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

            var leader = new Leader();
            int index = 0;

            // 名前
            leader.Name = tokens[index];
            index++;

            // ID
            int id;
            if (!int.TryParse(tokens[index], out id))
            {
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidId, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1}\n", tokens[index], leader.Name));
                return null;
            }
            leader.Id = id;
            index++;

            // 国家
            if (string.IsNullOrEmpty(tokens[index]) || !Countries.StringMap.ContainsKey(tokens[index].ToUpper()))
            {
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidCountryTag, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                    Log.Write(string.Format("{0}({1}): {2} L{3}\n", Resources.InvalidRankYear, RankNames[i],
                        _currentFileName, _currentLineNo));
                    Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidIdealRank, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidMaxSkill, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidTraits, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidSkill, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidExperience, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidLoyalty, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidBranch, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidStartYear, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidEndYear, _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
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
                    Log.Write(string.Format("{0}: {1} L{2}\n", Resources.InvalidEndYear, _currentFileName,
                        _currentLineNo));
                    Log.Write(string.Format("  {0}: {1} => {2}\n", leader.Id, leader.Name, tokens[index]));
                }
            }
            else
            {
                leader.RetirementYear = 1999;
            }

            Items.Add(leader);

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

            // TODO: 指揮官ファイルリストの保存

            bool error = false;
            foreach (Country country in Enum.GetValues(typeof (Country))
                .Cast<Country>()
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
                    Debug.WriteLine("[Leader] Save failed: {0}", fileName);
                    Log.Write(string.Format("{0}: {1}\n\n", Resources.FileWriteError, fileName));
                    if (MessageBox.Show(string.Format("{0}: {1}", Resources.FileWriteError, fileName),
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

            string fileName = Path.Combine(folderName, Game.GetLeaderFileName(country));
            Debug.WriteLine(string.Format("[Leader] Save: {0}", Path.GetFileName(fileName)));

            using (var writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                _currentFileName = fileName;
                _currentLineNo = 2;

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
                        Log.Write(String.Format("{0}: {1} L{2}\n", Resources.InvalidBranch, _currentFileName,
                            _currentLineNo));
                        Log.Write(String.Format("  {0}: {1}\n", leader.Id, leader.Name));
                    }
                    if (leader.IdealRank == LeaderRank.None)
                    {
                        Log.Write(String.Format("{0}: {1} L{2}\n", Resources.InvalidIdealRank, _currentFileName,
                            _currentLineNo));
                        Log.Write(String.Format("  {0}: {1}\n", leader.Id, leader.Name));
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
                            leader.IdealRank != LeaderRank.None
                                ? (4 - (int) leader.IdealRank).ToString(CultureInfo.InvariantCulture)
                                : "",
                            leader.MaxSkill,
                            leader.Traits,
                            leader.Skill,
                            leader.Experience,
                            leader.Loyalty,
                            leader.Branch != Branch.None
                                ? ((int) (leader.Branch - 1)).ToString(CultureInfo.InvariantCulture)
                                : "",
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
                            leader.IdealRank != LeaderRank.None
                                ? (4 - (int) leader.IdealRank).ToString(CultureInfo.InvariantCulture)
                                : "",
                            leader.MaxSkill,
                            leader.Traits,
                            leader.Skill,
                            leader.Experience,
                            leader.Loyalty,
                            leader.Branch != Branch.None
                                ? ((int) (leader.Branch - 1)).ToString(CultureInfo.InvariantCulture)
                                : "",
                            leader.PictureName,
                            leader.StartYear,
                            leader.EndYear);
                    }

                    // 編集済みフラグを解除する
                    leader.ResetDirtyAll();

                    _currentLineNo++;
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
            Items.Insert(index, leader);
        }

        /// <summary>
        ///     指揮官リストから項目を削除する
        /// </summary>
        /// <param name="leader">削除対象の項目</param>
        public static void RemoveItem(Leader leader)
        {
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
            return _dirtyFlag;
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
        ///     編集済みフラグを解除する
        /// </summary>
        /// <param name="country">国タグ</param>
        private static void ResetDirty(Country country)
        {
            DirtyFlags[(int) country] = false;
        }

        #endregion
    }
}