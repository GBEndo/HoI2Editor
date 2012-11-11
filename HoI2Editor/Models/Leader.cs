﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HoI2Editor.Models
{
    /// <summary>
    /// 指揮官データ
    /// </summary>
    internal class Leader
    {
        /// <summary>
        /// CSVファイルの区切り文字
        /// </summary>
        private static readonly char[] CsvSeparator = {';'};

        /// <summary>
        /// 指揮官特性値
        /// </summary>
        public static readonly uint[] TraitsValueTable =
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
        /// 指揮官特性文字列
        /// </summary>
        public static readonly string[] TraitsTextTable =
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

        /// <summary>
        /// 兵科文字列
        /// </summary>
        public static readonly string[] BranchTextTable = {"", "陸軍", "海軍", "空軍"};

        /// <summary>
        /// 階級文字列
        /// </summary>
        public static readonly string[] RankTextTable = {"", "少将", "中将", "大将", "元帥"};

        /// <summary>
        /// 国タグと指揮官ファイル名の対応付け
        /// </summary>
        public static readonly Dictionary<CountryTag, string> FileNameMap = new Dictionary<CountryTag, string>();

        /// <summary>
        /// 現在解析中のファイル名
        /// </summary>
        private static string _currentFileName = "";

        /// <summary>
        /// 現在解析中の行番号
        /// </summary>
        private static int _currentLineNo;

        /// <summary>
        /// 任官年
        /// </summary>
        private readonly int[] _rankYear = new int[4];

        /// <summary>
        /// 国タグ
        /// </summary>
        public CountryTag CountryTag { get; set; }

        /// <summary>
        /// 指揮官ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 画像ファイル名
        /// </summary>
        public string PictureName { get; set; }

        /// <summary>
        /// 初期スキル
        /// </summary>
        public int Skill { get; set; }

        /// <summary>
        /// 最大スキル
        /// </summary>
        public int MaxSkill { get; set; }

        /// <summary>
        /// 任官年
        /// </summary>
        public int[] RankYear
        {
            get { return _rankYear; }
        }

        /// <summary>
        /// 開始年
        /// </summary>
        public int StartYear { get; set; }

        /// <summary>
        /// 終了年
        /// </summary>
        public int EndYear { get; set; }

        /// <summary>
        /// 理想階級
        /// </summary>
        public LeaderRank IdealRank { get; set; }

        /// <summary>
        /// 指揮官特性
        /// </summary>
        public uint Traits { get; set; }

        /// <summary>
        /// 経験値
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// 忠誠度
        /// </summary>
        public int Loyalty { get; set; }

        /// <summary>
        /// 兵科
        /// </summary>
        public LeaderBranch Branch { get; set; }

        /// <summary>
        /// 指揮官ファイル群を読み込む
        /// </summary>
        /// <returns>指揮官リスト</returns>
        public static List<Leader> LoadLeaderFiles()
        {
            var leaders = new List<Leader>();

            FileNameMap.Clear();

            foreach (string fileName in Directory.GetFiles(Game.LeaderFolderName, "*.csv"))
            {
                LoadLeaderFile(fileName, leaders);
            }

            return leaders;
        }

        /// <summary>
        /// 指揮官ファイルを読み込む
        /// </summary>
        /// <param name="fileName">対象ファイル名</param>
        /// <param name="leaders">指揮官リスト</param>
        private static void LoadLeaderFile(string fileName, List<Leader> leaders)
        {
            _currentFileName = Path.GetFileName(fileName);
            _currentLineNo = 1;

            var reader = new StreamReader(fileName, Encoding.Default);
            // 空ファイルを読み飛ばす
            if (reader.EndOfStream)
            {
                return;
            }

            // ヘッダ行読み込み
            string line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            _currentLineNo++;
            var currentCountryTag = CountryTag.None;

            while (!reader.EndOfStream)
            {
                Leader leader = ParseLeaderLine(reader.ReadLine(), leaders);

                if (currentCountryTag == CountryTag.None && leader != null)
                {
                    currentCountryTag = leader.CountryTag;
                    if (!FileNameMap.ContainsKey(currentCountryTag))
                    {
                        FileNameMap.Add(currentCountryTag, Path.GetFileName(fileName));
                    }
                }
                _currentLineNo++;
            }
        }

        /// <summary>
        /// 指揮官定義行を解釈する
        /// </summary>
        /// <param name="line">対象文字列</param>
        /// <param name="leaders">指揮官リスト</param>
        /// <returns>指揮官データ</returns>
        private static Leader ParseLeaderLine(string line, List<Leader> leaders)
        {
            // 空行を読み飛ばす
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            string[] token = line.Split(CsvSeparator);

            // トークン数が足りない行は読み飛ばす
            if (token.Length != 18)
            {
                Log.Write(string.Format("項目数の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}\n\n", line));
                // 末尾のxがない/余分な項目がある場合は解析を続ける
                if (token.Length < 17)
                {
                    return null;
                }
            }

            // 名前指定のない行は読み飛ばす
            if (string.IsNullOrEmpty(token[0]))
            {
                return null;
            }

            var leader = new Leader();
            int id;
            if (!int.TryParse(token[1], out id))
            {
                Log.Write(string.Format("IDの異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}\n\n", token[1]));
                return null;
            }
            leader.Id = id;
            leader.Name = token[0];
            if (!Country.CountryTextMap.ContainsKey(token[2].ToUpper()))
            {
                Log.Write(string.Format("国タグの異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[2]));
                return null;
            }
            leader.CountryTag = Country.CountryTextMap[token[2].ToUpper()];
            for (int i = 0; i < 3; i++)
            {
                int rankYear;
                if (int.TryParse(token[3 + i], out rankYear))
                {
                    leader.RankYear[i] = rankYear;
                }
                else
                {
                    leader.RankYear[i] = 1990;
                    Log.Write(string.Format("{0}任官年の異常: {1} L{2} \n", RankTextTable[i + 1], _currentFileName,
                                            _currentLineNo));
                    Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[3 + i]));
                }
            }
            int idealRank;
            if (int.TryParse(token[7], out idealRank) && 0 <= idealRank && idealRank <= 3)
            {
                leader.IdealRank = (LeaderRank) (4 - idealRank);
            }
            else
            {
                leader.IdealRank = LeaderRank.None;
                Log.Write(string.Format("理想階級の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[7]));
            }
            int maxSkill;
            if (int.TryParse(token[8], out maxSkill))
            {
                leader.MaxSkill = maxSkill;
            }
            else
            {
                leader.MaxSkill = 1;
                Log.Write(string.Format("最大スキルの異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[8]));
            }
            uint traits;
            if (uint.TryParse(token[9], out traits))
            {
                leader.Traits = traits;
            }
            else
            {
                leader.Traits = LeaderTraits.None;
                Log.Write(string.Format("特性の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[8]));
            }
            int skill;
            if (int.TryParse(token[10], out skill))
            {
                leader.Skill = skill;
            }
            else
            {
                leader.Skill = 1;
                Log.Write(string.Format("スキルの異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[10]));
            }
            int experience;
            if (int.TryParse(token[11], out experience))
            {
                leader.Experience = experience;
            }
            else
            {
                leader.Experience = 1;
                Log.Write(string.Format("経験値の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[11]));
            }
            int loyalty;
            if (int.TryParse(token[12], out loyalty))
            {
                leader.Loyalty = loyalty;
            }
            else
            {
                leader.Loyalty = 1;
                Log.Write(string.Format("忠誠度の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[12]));
            }
            int branch;
            if (int.TryParse(token[13], out branch))
            {
                leader.Branch = (LeaderBranch) (branch + 1);
            }
            else
            {
                leader.Branch = LeaderBranch.None;
                Log.Write(string.Format("兵科の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[13]));
            }
            leader.PictureName = token[14];
            int startYear;
            if (int.TryParse(token[15], out startYear))
            {
                leader.StartYear = startYear;
            }
            else
            {
                leader.StartYear = 1930;
                Log.Write(string.Format("開始年の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[15]));
            }
            int endYear;
            if (int.TryParse(token[16], out endYear))
            {
                leader.EndYear = endYear;
            }
            else
            {
                leader.EndYear = 1970;
                Log.Write(string.Format("終了年の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                Log.Write(string.Format("  {0}: {1} => {2}\n\n", leader.Id, leader.Name, token[16]));
            }
            leaders.Add(leader);

            return leader;
        }

        /// <summary>
        /// 指揮官ファイル群を保存する
        /// </summary>
        /// <param name="leaders">指揮官リスト</param>
        /// <param name="dirtyFlags">編集フラグ </param>
        public static void SaveLeaderFiles(List<Leader> leaders, bool[] dirtyFlags)
        {
            foreach (
                CountryTag countryTag in
                    Enum.GetValues(typeof (CountryTag)).Cast<CountryTag>().Where(
                        countryTag => dirtyFlags[(int) countryTag]).Where(countryTag => countryTag != CountryTag.None))
            {
                SaveLeaderFile(leaders, countryTag);
            }
        }

        /// <summary>
        /// 指揮官ファイルを保存する
        /// </summary>
        /// <param name="leaders">指揮官リスト</param>
        /// <param name="countryTag">国タグ</param>
        private static void SaveLeaderFile(IEnumerable<Leader> leaders, CountryTag countryTag)
        {
            if (countryTag == CountryTag.None)
            {
                return;
            }

            _currentFileName = Path.GetFileName(Game.GetLeaderFileName(countryTag));
            _currentLineNo = 2;

            var writer = new StreamWriter(Game.GetLeaderFileName(countryTag), false, Encoding.Default);
            writer.WriteLine(
                "Name;ID;Country;Rank 3 Year;Rank 2 Year;Rank 1 Year;Rank 0 Year;Ideal Rank;Max Skill;Traits;Skill;Experience;Loyalty;Type;Picture;Start Year;End Year;x");

            foreach (
                Leader leader in
                    leaders.Where(leader => leader.CountryTag == countryTag).Where(leader => leader != null))
            {
                // 不正な値が設定されている場合は警告をログに出力する
                if (string.IsNullOrEmpty(leader.Name))
                {
                    Log.Write(string.Format("指揮官名の異常: {0} L{1} \n", _currentFileName, _currentLineNo));
                    Log.Write(string.Format("  {0}: {1}\n\n", leader.Id, leader.Name));
                }

                writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};x",
                                 leader.Name, leader.Id, Country.CountryTextTable[(int) leader.CountryTag],
                                 leader.RankYear[0], leader.RankYear[1], leader.RankYear[2], leader.RankYear[3],
                                 (int) leader.IdealRank, leader.MaxSkill, leader.Traits, leader.Skill, leader.Experience,
                                 leader.Loyalty, (int) leader.Branch, leader.PictureName, leader.StartYear,
                                 leader.EndYear);
                _currentLineNo++;
            }

            writer.Close();
        }
    }

    /// <summary>
    /// 指揮官特性値
    /// </summary>
    public static class LeaderTraits
    {
        /// <summary>
        /// 特性なし
        /// </summary>
        public const uint None = 0x00000000;

        /// <summary>
        /// 兵站管理
        /// </summary>
        public const uint LogisticsWizard = 0x00000001;

        /// <summary>
        /// 防勢ドクトリン
        /// </summary>
        public const uint DefensiveDoctrine = 0x00000002;

        /// <summary>
        /// 攻勢ドクトリン
        /// </summary>
        public const uint OffensiveDoctrine = 0x00000004;

        /// <summary>
        /// 冬期戦
        /// </summary>
        public const uint WinterSpecialist = 0x00000008;

        /// <summary>
        /// 伏撃
        /// </summary>
        public const uint Trickster = 0x00000010;

        /// <summary>
        /// 工兵
        /// </summary>
        public const uint Engineer = 0x00000020;

        /// <summary>
        /// 要塞攻撃
        /// </summary>
        public const uint FortressBuster = 0x00000040;

        /// <summary>
        /// 機甲戦
        /// </summary>
        public const uint PanzerLeader = 0x00000080;

        /// <summary>
        /// 特殊戦
        /// </summary>
        public const uint Commando = 0x00000100;

        /// <summary>
        /// 古典派
        /// </summary>
        public const uint OldGuard = 0x00000200;

        /// <summary>
        /// 海狼
        /// </summary>
        public const uint SeaWolf = 0x00000400;

        /// <summary>
        /// 封鎖線突破の達人
        /// </summary>
        public const uint BlockadeRunner = 0x00000800;

        /// <summary>
        /// 卓越した戦術家
        /// </summary>
        public const uint SuperiorTactician = 0x00001000;

        /// <summary>
        /// 索敵
        /// </summary>
        public const uint Spotter = 0x00002000;

        /// <summary>
        /// 対戦車攻撃
        /// </summary>
        public const uint TankBuster = 0x00004000;

        /// <summary>
        /// 絨毯爆撃
        /// </summary>
        public const uint CarpetBomber = 0x00008000;

        /// <summary>
        /// 夜間航空作戦
        /// </summary>
        public const uint NightFlyer = 0x00010000;

        /// <summary>
        /// 対艦攻撃
        /// </summary>
        public const uint FleetDestroyer = 0x00020000;

        /// <summary>
        /// 砂漠のキツネ
        /// </summary>
        public const uint DesertFox = 0x00040000;

        /// <summary>
        /// 密林のネズミ
        /// </summary>
        public const uint JungleRat = 0x00080000;

        /// <summary>
        /// 市街戦
        /// </summary>
        public const uint UrbanWarfareSpecialist = 0x00100000;

        /// <summary>
        /// レンジャー
        /// </summary>
        public const uint Ranger = 0x00200000;

        /// <summary>
        /// 山岳戦
        /// </summary>
        public const uint Mountaineer = 0x00400000;

        /// <summary>
        /// 高地戦
        /// </summary>
        public const uint HillsFighter = 0x00800000;

        /// <summary>
        /// 反撃戦
        /// </summary>
        public const uint CounterAttacker = 0x01000000;

        /// <summary>
        /// 突撃戦
        /// </summary>
        public const uint Assaulter = 0x02000000;

        /// <summary>
        /// 包囲戦
        /// </summary>
        public const uint Encircler = 0x04000000;

        /// <summary>
        /// 奇襲戦
        /// </summary>
        public const uint Ambusher = 0x08000000;

        /// <summary>
        /// 規律
        /// </summary>
        public const uint Disciplined = 0x10000000;

        /// <summary>
        /// 戦術的退却
        /// </summary>
        public const uint ElasticDefenceSpecialist = 0x20000000;

        /// <summary>
        /// 電撃戦
        /// </summary>
        public const uint Blitzer = 0x40000000;

        /// <summary>
        /// 陸軍特性
        /// </summary>
        public const uint ArmyTraits =
            LogisticsWizard | DefensiveDoctrine | OffensiveDoctrine | WinterSpecialist | Trickster | Engineer |
            FortressBuster | PanzerLeader | Commando | OldGuard | DesertFox | JungleRat | UrbanWarfareSpecialist |
            Ranger | Mountaineer | HillsFighter | CounterAttacker | Assaulter | Encircler | Ambusher | Disciplined |
            ElasticDefenceSpecialist | Blitzer;

        /// <summary>
        /// 海軍特性
        /// </summary>
        public const uint NavyTraits = OldGuard | SeaWolf | BlockadeRunner | SuperiorTactician | Spotter | NightFlyer;

        /// <summary>
        /// 空軍特性
        /// </summary>
        public const uint AirforceTraits =
            OldGuard | SuperiorTactician | Spotter | TankBuster | CarpetBomber | NightFlyer | FleetDestroyer;
    }

    /// <summary>
    /// 指揮官特性ID
    /// </summary>
    public enum LeaderTraitsId
    {
        LogisticsWizard, // 兵站管理
        DefensiveDoctrine, // 防勢ドクトリン
        OffensiveDoctrine, // 攻勢ドクトリン
        WinterSpecialist, // 冬期戦
        Trickster, // 伏撃
        Engineer, // 工兵
        FortressBuster, // 要塞攻撃
        PanzerLeader, // 機甲戦
        Commando, // 特殊戦
        OldGuard, // 古典派
        SeaWolf, // 海狼
        BlockadeRunner, // 封鎖線突破の達人
        SuperiorTactician, // 卓越した戦術家
        Spotter, // 索敵
        TankBuster, // 対戦車攻撃
        CarpetBomber, // 絨毯爆撃
        NightFlyer, // 夜間航空作戦
        FleetDestroyer, // 対艦攻撃
        DesertFox, // 砂漠のキツネ
        JungleRat, // 密林のネズミ
        UrbanWarfareSpecialist, // 市街戦
        Ranger, // レンジャー
        Mountaineer, // 山岳戦
        HillsFighter, // 高地戦
        CounterAttacker, // 反撃戦
        Assaulter, // 突撃戦
        Encircler, // 包囲戦
        Ambusher, // 奇襲戦
        Disciplined, // 規律
        ElasticDefenceSpecialist, // 戦術的退却
        Blitzer // 電撃戦
    }

    /// <summary>
    /// 兵科
    /// </summary>
    public enum LeaderBranch
    {
        None,
        Army, // 陸軍
        Navy, //海軍
        Airforce //空軍
    }

    /// <summary>
    /// 指揮官階級
    /// </summary>
    public enum LeaderRank
    {
        None,
        MajorGeneral, // 少将
        LieutenantGeneral, // 中将
        General, // 大将
        Marshal, // 元帥
    }
}