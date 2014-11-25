﻿using System.Collections.Generic;

namespace HoI2Editor.Models
{
    /// <summary>
    ///     シナリオデータ
    /// </summary>
    public class Scenario
    {
        #region 公開プロパティ

        /// <summary>
        ///     シナリオ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     パネル画像名
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        ///     シナリオヘッダ
        /// </summary>
        public ScenarioHeader Header { get; set; }

        /// <summary>
        ///     シナリオグローバルデータ
        /// </summary>
        public ScenarioGlobalData GlobalData { get; set; }

        /// <summary>
        ///     発生済みイベント
        /// </summary>
        public List<int> History { get; private set; }

        /// <summary>
        ///     休止イベント
        /// </summary>
        public List<int> SleepEvents { get; private set; }

        /// <summary>
        ///     保存日時
        /// </summary>
        public Dictionary<int, GameDate> SaveDates { get; set; }

        /// <summary>
        ///     マップ設定
        /// </summary>
        public ScenarioMap Map { get; set; }

        /// <summary>
        ///     イベントファイル
        /// </summary>
        public List<string> Events { get; private set; }

        /// <summary>
        ///     インクルードファイル
        /// </summary>
        public List<string> Includes { get; private set; }

        /// <summary>
        ///     プロヴィンス情報 (国別incに記載)
        /// </summary>
        public List<ScenarioProvince> CountryProvinces { get; private set; }

        /// <summary>
        ///     プロヴィンス情報 (bases.incに記載)
        /// </summary>
        public List<ScenarioProvince> BasesProvinces { get; private set; }

        /// <summary>
        ///     プロヴィンス情報 (bases_DOD.incに記載)
        /// </summary>
        public List<ScenarioProvince> BasesDodProvinces { get; private set; }

        /// <summary>
        ///     プロヴィンス情報 (vp.incに記載)
        /// </summary>
        public List<ScenarioProvince> VpProvinces { get; private set; }

        /// <summary>
        ///     プロヴィンス情報 (シナリオ.eugに記載)
        /// </summary>
        public List<ScenarioProvince> TopProvinces { get; private set; }

        /// <summary>
        ///     国家情報
        /// </summary>
        public List<ScenarioCountry> Countries { get; private set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public Scenario()
        {
            History = new List<int>();
            SleepEvents = new List<int>();
            Events = new List<string>();
            Includes = new List<string>();
            CountryProvinces = new List<ScenarioProvince>();
            BasesProvinces = new List<ScenarioProvince>();
            BasesDodProvinces = new List<ScenarioProvince>();
            VpProvinces = new List<ScenarioProvince>();
            TopProvinces = new List<ScenarioProvince>();
            Countries = new List<ScenarioCountry>();
        }

        #endregion
    }

    /// <summary>
    ///     シナリオヘッダ
    /// </summary>
    public class ScenarioHeader
    {
        #region 公開プロパティ

        /// <summary>
        ///     シナリオヘッダ名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     開始年
        /// </summary>
        public int StartYear { get; set; }

        /// <summary>
        ///     終了年
        /// </summary>
        public int EndYear { get; set; }

        /// <summary>
        ///     国家の自由選択
        /// </summary>
        public bool Free { get; set; }

        /// <summary>
        ///     ショートシナリオ
        /// </summary>
        public bool Combat { get; set; }

        /// <summary>
        ///     選択可能国家
        /// </summary>
        public List<Country> Selectable { get; private set; }

        /// <summary>
        ///     主要国情報
        /// </summary>
        public List<MajorCountry> Majors { get; private set; }

        /// <summary>
        ///     AIの攻撃性
        /// </summary>
        public int AiAggressive { get; set; }

        /// <summary>
        ///     難易度
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        ///     ゲームスピード
        /// </summary>
        public int GameSpeed { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ScenarioHeader()
        {
            Selectable = new List<Country>();
            Majors = new List<MajorCountry>();
        }

        #endregion
    }

    /// <summary>
    ///     シナリオグローバルデータ
    /// </summary>
    public class ScenarioGlobalData
    {
        #region 公開プロパティ

        /// <summary>
        ///     ルール設定
        /// </summary>
        public ScenarioRules Rules { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     終了日時
        /// </summary>
        public GameDate EndDate { get; set; }

        /// <summary>
        ///     枢軸国
        /// </summary>
        public Alliance Axis { get; set; }

        /// <summary>
        ///     連合国
        /// </summary>
        public Alliance Allies { get; set; }

        /// <summary>
        ///     共産国
        /// </summary>
        public Alliance Comintern { get; set; }

        /// <summary>
        ///     同盟国情報
        /// </summary>
        public List<Alliance> Alliances { get; private set; }

        /// <summary>
        ///     戦争情報
        /// </summary>
        public List<War> Wars { get; private set; }

        /// <summary>
        ///     外交協定情報
        /// </summary>
        public List<Treaty> Treaties { get; private set; }

        /// <summary>
        ///     休止指揮官
        /// </summary>
        public List<int> DormantLeaders { get; private set; }

        /// <summary>
        ///     休止閣僚
        /// </summary>
        public List<int> DormantMinisters { get; private set; }

        /// <summary>
        ///     休止研究機関
        /// </summary>
        public List<int> DormantTeams { get; private set; }

        /// <summary>
        ///     全指揮官を休止
        /// </summary>
        public bool DormantLeadersAll { get; set; }

        /// <summary>
        ///     天候設定
        /// </summary>
        public Weather Weather { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ScenarioGlobalData()
        {
            Alliances = new List<Alliance>();
            Wars = new List<War>();
            Treaties = new List<Treaty>();
            DormantLeaders = new List<int>();
            DormantMinisters = new List<int>();
            DormantTeams = new List<int>();
        }

        #endregion
    }

    /// <summary>
    ///     天候設定
    /// </summary>
    public class Weather
    {
        #region 公開プロパティ

        /// <summary>
        ///     固定設定
        /// </summary>
        public bool Static { get; set; }

        /// <summary>
        ///     天候パターン
        /// </summary>
        public List<WeatherPattern> Patterns { get; private set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public Weather()
        {
            Patterns = new List<WeatherPattern>();
        }

        #endregion
    }

    /// <summary>
    ///     天候パターン
    /// </summary>
    public class WeatherPattern
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     プロヴィンスリスト
        /// </summary>
        public List<int> Provinces { get; private set; }

        /// <summary>
        ///     中央プロヴィンス
        /// </summary>
        public int Centre { get; set; }

        /// <summary>
        ///     速度
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        ///     方向
        /// </summary>
        public string Heading { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public WeatherPattern()
        {
            Provinces = new List<int>();
        }

        #endregion
    }

    /// <summary>
    ///     マップ設定
    /// </summary>
    public class ScenarioMap
    {
        #region 公開プロパティ

        /// <summary>
        ///     全プロヴィンスが有効かどうか
        /// </summary>
        public bool All { get; set; }

        /// <summary>
        ///     有効プロヴィンス
        /// </summary>
        public List<int> Yes { get; private set; }

        /// <summary>
        ///     無効プロヴィンス
        /// </summary>
        public List<int> No { get; private set; }

        /// <summary>
        ///     マップの範囲(左上)
        /// </summary>
        public ScenarioPoint Top { get; set; }

        /// <summary>
        ///     マップの範囲(右下)
        /// </summary>
        public ScenarioPoint Bottom { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ScenarioMap()
        {
            All = true;
            Yes = new List<int>();
            No = new List<int>();
        }

        #endregion
    }

    /// <summary>
    ///     マップの座標
    /// </summary>
    public class ScenarioPoint
    {
        #region 公開プロパティ

        /// <summary>
        ///     X座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     Y座標
        /// </summary>
        public int Y { get; set; }

        #endregion
    }

    /// <summary>
    ///     主要国情報
    /// </summary>
    public class MajorCountry
    {
        #region 公開プロパティ

        /// <summary>
        ///     国タグ
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        ///     説明文
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        ///     プロパガンダ画像名
        /// </summary>
        public string PictureName { get; set; }

        /// <summary>
        ///     右端に配置
        /// </summary>
        public bool Bottom { get; set; }

        #endregion
    }

    /// <summary>
    ///     同盟情報
    /// </summary>
    public class Alliance
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     参加国
        /// </summary>
        public List<Country> Participant { get; private set; }

        /// <summary>
        ///     同盟名
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public Alliance()
        {
            Participant = new List<Country>();
        }

        #endregion
    }

    /// <summary>
    ///     戦争情報
    /// </summary>
    public class War
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     終了日時
        /// </summary>
        public GameDate EndDate { get; set; }

        /// <summary>
        ///     攻撃側参加国
        /// </summary>
        public Alliance Attackers { get; set; }

        /// <summary>
        ///     防御側参加国
        /// </summary>
        public Alliance Defenders { get; set; }

        #endregion
    }

    /// <summary>
    ///     外交協定情報
    /// </summary>
    public class Treaty
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     外交協定の種類
        /// </summary>
        public TreatyType Type { get; set; }

        /// <summary>
        ///     対象国1
        /// </summary>
        public Country Country1 { get; set; }

        /// <summary>
        ///     対象国2
        /// </summary>
        public Country Country2 { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     失効日時
        /// </summary>
        public GameDate ExpiryDate { get; set; }

        /// <summary>
        ///     資金
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     物資
        /// </summary>
        public double Supplies { get; set; }

        /// <summary>
        ///     エネルギー
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        ///     金属
        /// </summary>
        public double Metal { get; set; }

        /// <summary>
        ///     希少資源
        /// </summary>
        public double RareMaterials { get; set; }

        /// <summary>
        ///     石油
        /// </summary>
        public double Oil { get; set; }

        /// <summary>
        ///     取り消し可能かどうか
        /// </summary>
        public bool Cancel { get; set; }

        #endregion
    }

    /// <summary>
    ///     ルール設定
    /// </summary>
    public class ScenarioRules
    {
        #region 公開プロパティ

        /// <summary>
        ///     外交
        /// </summary>
        public bool Diplomacy { get; set; }

        /// <summary>
        ///     生産
        /// </summary>
        public bool Production { get; set; }

        /// <summary>
        ///     技術
        /// </summary>
        public bool Technology { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ScenarioRules()
        {
            Diplomacy = true;
            Production = true;
            Technology = true;
        }

        #endregion
    }

    /// <summary>
    ///     プロヴィンス情報
    /// </summary>
    public class ScenarioProvince
    {
        #region 公開プロパティ

        /// <summary>
        ///     プロヴィンスID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     プロヴィンス名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     工場のサイズ
        /// </summary>
        public BuildingSize Ic { get; set; }

        /// <summary>
        ///     インフラのサイズ
        /// </summary>
        public BuildingSize Infrastructure { get; set; }

        /// <summary>
        ///     陸上要塞のサイズ
        /// </summary>
        public BuildingSize LandFort { get; set; }

        /// <summary>
        ///     沿岸要塞のサイズ
        /// </summary>
        public BuildingSize CoastalFort { get; set; }

        /// <summary>
        ///     対空砲のサイズ
        /// </summary>
        public BuildingSize AntiAir { get; set; }

        /// <summary>
        ///     空軍基地のサイズ
        /// </summary>
        public BuildingSize AirBase { get; set; }

        /// <summary>
        ///     海軍基地のサイズ
        /// </summary>
        public BuildingSize NavalBase { get; set; }

        /// <summary>
        ///     レーダー基地のサイズ
        /// </summary>
        public BuildingSize RadarStation { get; set; }

        /// <summary>
        ///     原子炉のサイズ
        /// </summary>
        public BuildingSize NuclearReactor { get; set; }

        /// <summary>
        ///     ロケット試験場のサイズ
        /// </summary>
        public BuildingSize RocketTest { get; set; }

        /// <summary>
        ///     合成石油工場のサイズ
        /// </summary>
        public BuildingSize SyntheticOil { get; set; }

        /// <summary>
        ///     合成素材工場のサイズ
        /// </summary>
        public BuildingSize SyntheticRares { get; set; }

        /// <summary>
        ///     原子力発電所のサイズ
        /// </summary>
        public BuildingSize NuclearPower { get; set; }

        /// <summary>
        ///     物資の備蓄量
        /// </summary>
        public double SupplyPool { get; set; }

        /// <summary>
        ///     石油の備蓄量
        /// </summary>
        public double OilPool { get; set; }

        /// <summary>
        ///     エネルギーの備蓄量
        /// </summary>
        public double EnergyPool { get; set; }

        /// <summary>
        ///     金属の備蓄量
        /// </summary>
        public double MetalPool { get; set; }

        /// <summary>
        ///     希少資源の備蓄量
        /// </summary>
        public double RareMaterialsPool { get; set; }

        /// <summary>
        ///     エネルギー産出量
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        ///     最大エネルギー産出量
        /// </summary>
        public double MaxEnergy { get; set; }

        /// <summary>
        ///     金属産出量
        /// </summary>
        public double Metal { get; set; }

        /// <summary>
        ///     最大金属産出量
        /// </summary>
        public double MaxMetal { get; set; }

        /// <summary>
        ///     希少資源産出量
        /// </summary>
        public double RareMaterials { get; set; }

        /// <summary>
        ///     最大希少資源産出量
        /// </summary>
        public double MaxRareMaterials { get; set; }

        /// <summary>
        ///     石油産出量
        /// </summary>
        public double Oil { get; set; }

        /// <summary>
        ///     最大石油産出量
        /// </summary>
        public double MaxOil { get; set; }

        /// <summary>
        ///     人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     最大人的資源
        /// </summary>
        public double MaxManpower { get; set; }

        /// <summary>
        ///     勝利ポイント
        /// </summary>
        public int Vp { get; set; }

        /// <summary>
        ///     反乱率
        /// </summary>
        public double RevoltRisk { get; set; }

        /// <summary>
        ///     天候
        /// </summary>
        public WeatherType Weather { get; set; }

        #endregion
    }

    /// <summary>
    ///     建物のサイズ
    /// </summary>
    public class BuildingSize
    {
        #region 公開プロパティ

        /// <summary>
        ///     相対サイズ
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        ///     最大サイズ
        /// </summary>
        public double MaxSize { get; set; }

        /// <summary>
        ///     現在のサイズ
        /// </summary>
        public double CurrentSize { get; set; }

        #endregion
    }

    /// <summary>
    ///     国家情報
    /// </summary>
    public class ScenarioCountry
    {
        #region 公開プロパティ

        /// <summary>
        ///     国タグ
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        ///     国名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     国旗の接尾辞
        /// </summary>
        public string FlagExt { get; set; }

        /// <summary>
        ///     兄弟国
        /// </summary>
        public Country RegularId { get; set; }

        /// <summary>
        ///     独立可能政体
        /// </summary>
        public GovernmentType IntrinsicGovType { get; set; }

        /// <summary>
        ///     宗主国
        /// </summary>
        public Country Master { get; set; }

        /// <summary>
        ///     統帥権取得国
        /// </summary>
        public Country Control { get; set; }

        /// <summary>
        ///     好戦性
        /// </summary>
        public int Belligerence { get; set; }

        /// <summary>
        ///     追加輸送能力
        /// </summary>
        public double ExtraTc { get; set; }

        /// <summary>
        ///     国民不満度
        /// </summary>
        public double Dissent { get; set; }

        /// <summary>
        ///     首都のプロヴィンスID
        /// </summary>
        public int Capital { get; set; }

        /// <summary>
        ///     平時IC補正
        /// </summary>
        public double PeacetimeIcMod { get; set; }

        /// <summary>
        ///     戦時IC補正
        /// </summary>
        public double WartimeIcMod { get; set; }

        /// <summary>
        ///     工業力補正
        /// </summary>
        public double IndustrialModifier { get; set; }

        /// <summary>
        ///     対地防御補正
        /// </summary>
        public double GroundDefEff { get; set; }

        /// <summary>
        ///     AIファイル名
        /// </summary>
        public string Ai { get; set; }

        /// <summary>
        ///     AI設定
        /// </summary>
        public AiSettings AiSettings { get; set; }

        /// <summary>
        ///     人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     人的資源補正値
        /// </summary>
        public double RelativeManpower { get; set; }

        /// <summary>
        ///     エネルギー
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        ///     金属
        /// </summary>
        public double Metal { get; set; }

        /// <summary>
        ///     希少資源
        /// </summary>
        public double RareMaterials { get; set; }

        /// <summary>
        ///     石油
        /// </summary>
        public double Oil { get; set; }

        /// <summary>
        ///     物資
        /// </summary>
        public double Supplies { get; set; }

        /// <summary>
        ///     資金
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     輸送船団
        /// </summary>
        public int Transports { get; set; }

        /// <summary>
        ///     護衛艦
        /// </summary>
        public int Escorts { get; set; }

        /// <summary>
        ///     核兵器
        /// </summary>
        public int Nuke { get; set; }

        /// <summary>
        ///     マップ外資源
        /// </summary>
        public FreeResources Free { get; set; }

        /// <summary>
        ///     外交情報
        /// </summary>
        public List<CountryRelation> Diplomacy { get; private set; }

        /// <summary>
        ///     諜報情報
        /// </summary>
        public List<SpyInfo> Intelligence { get; private set; }

        /// <summary>
        ///     中核プロヴィンス
        /// </summary>
        public List<int> NationalProvinces { get; private set; }

        /// <summary>
        ///     保有プロヴィンス
        /// </summary>
        public List<int> OwnedProvinces { get; private set; }

        /// <summary>
        ///     支配プロヴィンス
        /// </summary>
        public List<int> ControlledProvinces { get; private set; }

        /// <summary>
        ///     領有権主張プロヴィンス
        /// </summary>
        public List<int> ClaimedProvinces { get; private set; }

        /// <summary>
        ///     保有技術
        /// </summary>
        public List<int> TechApps { get; private set; }

        /// <summary>
        ///     青写真
        /// </summary>
        public List<int> BluePrints { get; private set; }

        /// <summary>
        ///     発明イベント
        /// </summary>
        public List<int> Inventions { get; private set; }

        /// <summary>
        ///     政策スライダー
        /// </summary>
        public CountryPolicy Policy { get; set; }

        /// <summary>
        ///     核兵器完成日時
        /// </summary>
        public GameDate NukeDate { get; set; }

        /// <summary>
        ///     国家元首
        /// </summary>
        public TypeId HeadOfState { get; set; }

        /// <summary>
        ///     政府首班
        /// </summary>
        public TypeId HeadOfGovernment { get; set; }

        /// <summary>
        ///     外務大臣
        /// </summary>
        public TypeId ForeignMinister { get; set; }

        /// <summary>
        ///     軍需大臣
        /// </summary>
        public TypeId ArmamentMinister { get; set; }

        /// <summary>
        ///     内務大臣
        /// </summary>
        public TypeId MinisterOfSecurity { get; set; }

        /// <summary>
        ///     情報大臣
        /// </summary>
        public TypeId MinisterOfIntelligence { get; set; }

        /// <summary>
        ///     統合参謀総長
        /// </summary>
        public TypeId ChiefOfStaff { get; set; }

        /// <summary>
        ///     陸軍総司令官
        /// </summary>
        public TypeId ChiefOfArmy { get; set; }

        /// <summary>
        ///     海軍総司令官
        /// </summary>
        public TypeId ChiefOfNavy { get; set; }

        /// <summary>
        ///     空軍総司令官
        /// </summary>
        public TypeId ChiefOfAir { get; set; }

        /// <summary>
        ///     休止指揮官
        /// </summary>
        public List<int> DormantLeaders { get; private set; }

        /// <summary>
        ///     休止閣僚
        /// </summary>
        public List<int> DormantMinisters { get; private set; }

        /// <summary>
        ///     休止研究機関
        /// </summary>
        public List<int> DormantTeams { get; private set; }

        /// <summary>
        ///     抽出指揮官
        /// </summary>
        public List<int> StealLeaders { get; private set; }

        /// <summary>
        ///     輸送船団
        /// </summary>
        public List<Convoy> Convoys { get; private set; }

        /// <summary>
        ///     陸軍ユニット
        /// </summary>
        public List<LandUnit> LandUnits { get; private set; }

        /// <summary>
        ///     海軍ユニット
        /// </summary>
        public List<NavalUnit> NavalUnits { get; private set; }

        /// <summary>
        ///     空軍ユニット
        /// </summary>
        public List<AirUnit> AirUnits { get; private set; }

        /// <summary>
        ///     生産中師団
        /// </summary>
        public List<DivisionDevelopment> DivisionDevelopments { get; private set; }

        /// <summary>
        ///     生産中輸送船団
        /// </summary>
        public List<ConvoyDevelopment> ConvoyDevelopments { get; private set; }

        /// <summary>
        ///     生産中建物
        /// </summary>
        public List<BuildingDevelopment> BuildingDevelopments { get; private set; }

        /// <summary>
        ///     陸軍師団
        /// </summary>
        public List<LandDivision> LandDivisions { get; private set; }

        /// <summary>
        ///     海軍師団
        /// </summary>
        public List<NavalDivision> NavalDivisions { get; private set; }

        /// <summary>
        ///     空軍師団
        /// </summary>
        public List<AirDivision> AirDivisions { get; private set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ScenarioCountry()
        {
            Diplomacy = new List<CountryRelation>();
            Intelligence = new List<SpyInfo>();
            NationalProvinces = new List<int>();
            OwnedProvinces = new List<int>();
            ControlledProvinces = new List<int>();
            ClaimedProvinces = new List<int>();
            TechApps = new List<int>();
            BluePrints = new List<int>();
            Inventions = new List<int>();
            DormantLeaders = new List<int>();
            DormantMinisters = new List<int>();
            DormantTeams = new List<int>();
            StealLeaders = new List<int>();
            Convoys = new List<Convoy>();
            LandUnits = new List<LandUnit>();
            NavalUnits = new List<NavalUnit>();
            AirUnits = new List<AirUnit>();
            DivisionDevelopments = new List<DivisionDevelopment>();
            ConvoyDevelopments = new List<ConvoyDevelopment>();
            BuildingDevelopments = new List<BuildingDevelopment>();
            LandDivisions = new List<LandDivision>();
            NavalDivisions = new List<NavalDivision>();
            AirDivisions = new List<AirDivision>();
        }

        #endregion
    }

    /// <summary>
    ///     AI設定
    /// </summary>
    public class AiSettings
    {
        #region 公開プロパティ

        /// <summary>
        ///     フラグ
        /// </summary>
        public Dictionary<string, string> Flags { get; set; }

        #endregion
    }

    /// <summary>
    ///     マップ外資源情報
    /// </summary>
    public class FreeResources
    {
        #region 公開プロパティ

        /// <summary>
        ///     工業力
        /// </summary>
        public double Ic { get; set; }

        /// <summary>
        ///     人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     エネルギー
        /// </summary>
        public double Energy { get; set; }

        /// <summary>
        ///     金属
        /// </summary>
        public double Metal { get; set; }

        /// <summary>
        ///     希少資源
        /// </summary>
        public double RareMaterials { get; set; }

        /// <summary>
        ///     石油
        /// </summary>
        public double Oil { get; set; }

        /// <summary>
        ///     物資
        /// </summary>
        public double Supplies { get; set; }

        /// <summary>
        ///     資金
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     輸送船団
        /// </summary>
        public int Transports { get; set; }

        /// <summary>
        ///     護衛艦
        /// </summary>
        public int Escorts { get; set; }

        #endregion
    }

    /// <summary>
    ///     国家関係情報
    /// </summary>
    public class CountryRelation
    {
        #region 公開プロパティ

        /// <summary>
        ///     相手国
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        ///     関係値
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     通行許可
        /// </summary>
        public bool Access { get; set; }

        /// <summary>
        ///     独立保障期限
        /// </summary>
        public GameDate Guaranteed { get; set; }

        #endregion
    }

    /// <summary>
    ///     国家諜報情報
    /// </summary>
    public class SpyInfo
    {
        #region 公開プロパティ

        /// <summary>
        ///     相手国
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        ///     スパイの数
        /// </summary>
        public int Spies { get; set; }

        #endregion
    }

    /// <summary>
    ///     政策スライダー
    /// </summary>
    public class CountryPolicy
    {
        #region 公開プロパティ

        /// <summary>
        ///     スライダー移動可能日時
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     民主的 - 独裁的
        /// </summary>
        public int Democratic { get; set; }

        /// <summary>
        ///     政治的左派 - 政治的右派
        /// </summary>
        public int PoliticalLeft { get; set; }

        /// <summary>
        ///     開放社会 - 閉鎖社会
        /// </summary>
        public int Freedom { get; set; }

        /// <summary>
        ///     自由経済 - 中央計画経済
        /// </summary>
        public int FreeMarket { get; set; }

        /// <summary>
        ///     常備軍 - 徴兵軍 (DH Fullでは動員 - 復員)
        /// </summary>
        public int ProfessionalArmy { get; set; }

        /// <summary>
        ///     タカ派 - ハト派
        /// </summary>
        public int DefenseLobby { get; set; }

        /// <summary>
        ///     介入主義 - 孤立主義
        /// </summary>
        public int Interventionism { get; set; }

        #endregion
    }

    /// <summary>
    ///     輸送船団
    /// </summary>
    public class Convoy
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     貿易ID
        /// </summary>
        public TypeId TradeId { get; set; }

        /// <summary>
        ///     貿易用の輸送船団かどうか
        /// </summary>
        public bool IsTrade { get; set; }

        /// <summary>
        ///     輸送船の数
        /// </summary>
        public int Transports { get; set; }

        /// <summary>
        ///     護衛艦の数
        /// </summary>
        public int Escorts { get; set; }

        /// <summary>
        ///     エネルギーの輸送有無
        /// </summary>
        public bool Energy { get; set; }

        /// <summary>
        ///     金属の輸送有無
        /// </summary>
        public bool Metal { get; set; }

        /// <summary>
        ///     希少資源の輸送有無
        /// </summary>
        public bool RareMaterials { get; set; }

        /// <summary>
        ///     石油の輸送有無
        /// </summary>
        public bool Oil { get; set; }

        /// <summary>
        ///     物資の輸送有無
        /// </summary>
        public bool Supplies { get; set; }

        /// <summary>
        ///     航路
        /// </summary>
        public List<int> Path { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public Convoy()
        {
            Path = new List<int>();
        }

        #endregion
    }

    /// <summary>
    ///     陸軍ユニット
    /// </summary>
    public class LandUnit
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     ユニット名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     統帥国
        /// </summary>
        public Country Control { get; set; }

        /// <summary>
        ///     指揮官
        /// </summary>
        public int Leader { get; set; }

        /// <summary>
        ///     現在位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        ///     直前の位置
        /// </summary>
        public int PrevProv { get; set; }

        /// <summary>
        ///     基準位置
        /// </summary>
        public int Home { get; set; }

        /// <summary>
        ///     塹壕レベル
        /// </summary>
        public double DigIn { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public LandMission Mission { get; set; }

        /// <summary>
        ///     指定日時
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     移動完了日時
        /// </summary>
        public GameDate MoveTime { get; set; }

        /// <summary>
        ///     移動経路
        /// </summary>
        public List<int> Movement { get; private set; }

        /// <summary>
        ///     師団
        /// </summary>
        public List<LandDivision> Divisions { get; private set; }

        /// <summary>
        ///     上陸中
        /// </summary>
        public bool Invasion { get; set; }

        /// <summary>
        ///     上陸先
        /// </summary>
        public int Target { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public LandUnit()
        {
            Movement = new List<int>();
            Divisions = new List<LandDivision>();
        }

        #endregion
    }

    /// <summary>
    ///     海軍ユニット
    /// </summary>
    public class NavalUnit
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     ユニット名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     統帥国
        /// </summary>
        public Country Control { get; set; }

        /// <summary>
        ///     指揮官
        /// </summary>
        public int Leader { get; set; }

        /// <summary>
        ///     現在位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        ///     直前の位置
        /// </summary>
        public int PrevProv { get; set; }

        /// <summary>
        ///     基準位置
        /// </summary>
        public int Home { get; set; }

        /// <summary>
        ///     所属基地
        /// </summary>
        public int Base { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public NavalMission Mission { get; set; }

        /// <summary>
        ///     指定日時
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     移動完了日時
        /// </summary>
        public GameDate MoveTime { get; set; }

        /// <summary>
        ///     移動経路
        /// </summary>
        public List<int> Movement { get; private set; }

        /// <summary>
        ///     師団
        /// </summary>
        public List<NavalDivision> Divisions { get; private set; }

        /// <summary>
        ///     搭載ユニット
        /// </summary>
        public List<LandUnit> LandUnits { get; private set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public NavalUnit()
        {
            Movement = new List<int>();
            Divisions = new List<NavalDivision>();
            LandUnits = new List<LandUnit>();
        }

        #endregion
    }

    /// <summary>
    ///     空軍ユニット
    /// </summary>
    public class AirUnit
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     ユニット名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     統帥国
        /// </summary>
        public Country Control { get; set; }

        /// <summary>
        ///     指揮官
        /// </summary>
        public int Leader { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        ///     直前の位置
        /// </summary>
        public int PrevProv { get; set; }

        /// <summary>
        ///     基準位置
        /// </summary>
        public int Home { get; set; }

        /// <summary>
        ///     所属基地
        /// </summary>
        public int Base { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public AirMission Mission { get; set; }

        /// <summary>
        ///     指定日時
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     移動完了日時
        /// </summary>
        public GameDate MoveTime { get; set; }

        /// <summary>
        ///     移動経路
        /// </summary>
        public List<int> Movement { get; private set; }

        /// <summary>
        ///     師団
        /// </summary>
        public List<AirDivision> Divisions { get; private set; }

        /// <summary>
        ///     搭載ユニット
        /// </summary>
        public List<LandUnit> LandUnits { get; private set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public AirUnit()
        {
            Movement = new List<int>();
            Divisions = new List<AirDivision>();
            LandUnits = new List<LandUnit>();
        }

        #endregion
    }

    /// <summary>
    ///     陸軍師団
    /// </summary>
    public class LandDivision
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     師団名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ユニット種類
        /// </summary>
        public UnitType Type { get; set; }

        /// <summary>
        ///     モデル番号
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra1 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra2 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra3 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra4 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra5 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel1 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel2 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel3 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel4 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel5 { get; set; }

        /// <summary>
        ///     最大戦力
        /// </summary>
        public int MaxStrength { get; set; }

        /// <summary>
        ///     戦力
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        ///     指揮統制率
        /// </summary>
        public int Organisation { get; set; }

        /// <summary>
        ///     士気
        /// </summary>
        public int Morale { get; set; }

        /// <summary>
        ///     経験値
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        ///     攻勢開始日時
        /// </summary>
        public GameDate Offensive { get; set; }

        /// <summary>
        ///     休止状態
        /// </summary>
        public bool Dormant { get; set; }

        /// <summary>
        ///     移動不可
        /// </summary>
        public bool Locked { get; set; }

        #endregion

        #region 公開定数

        /// <summary>
        ///     未定義のモデル番号
        /// </summary>
        public const int UndefinedModelNo = -1;

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public LandDivision()
        {
            Model = UndefinedModelNo;
            BrigadeModel = UndefinedModelNo;
        }

        #endregion
    }

    /// <summary>
    ///     海軍師団
    /// </summary>
    public class NavalDivision
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     師団名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ユニット種類
        /// </summary>
        public UnitType Type { get; set; }

        /// <summary>
        ///     モデル番号
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        ///     核兵器搭載
        /// </summary>
        public bool Nuke { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra1 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra2 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra3 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra4 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra5 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel1 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel2 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel3 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel4 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel5 { get; set; }

        /// <summary>
        ///     最大戦力
        /// </summary>
        public int MaxStrength { get; set; }

        /// <summary>
        ///     戦力
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        ///     指揮統制率
        /// </summary>
        public int Organisation { get; set; }

        /// <summary>
        ///     士気
        /// </summary>
        public int Morale { get; set; }

        /// <summary>
        ///     経験値
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        ///     移動速度
        /// </summary>
        public double MaxSpeed { get; set; }

        /// <summary>
        ///     対艦/対潜防御力
        /// </summary>
        public double SeaDefense { get; set; }

        /// <summary>
        ///     対空防御力
        /// </summary>
        public double AirDefence { get; set; }

        /// <summary>
        ///     対艦攻撃力(海軍)
        /// </summary>
        public double SeaAttack { get; set; }

        /// <summary>
        ///     対潜攻撃力
        /// </summary>
        public double SubAttack { get; set; }

        /// <summary>
        ///     通商破壊力
        /// </summary>
        public double ConvoyAttack { get; set; }

        /// <summary>
        ///     湾岸攻撃力
        /// </summary>
        public double ShoreBombardment { get; set; }

        /// <summary>
        ///     対空攻撃力
        /// </summary>
        public double AirAttack { get; set; }

        /// <summary>
        ///     射程距離
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        ///     可視性
        /// </summary>
        public double Visibility { get; set; }

        /// <summary>
        ///     対艦索敵能力
        /// </summary>
        public double SurfaceDetectionCapability { get; set; }

        /// <summary>
        ///     対潜索敵能力
        /// </summary>
        public double SubDetectionCapability { get; set; }

        /// <summary>
        ///     対空索敵能力
        /// </summary>
        public double AirDetectionCapability { get; set; }

        /// <summary>
        ///     休止状態
        /// </summary>
        public bool Dormant { get; set; }

        #endregion

        #region 公開定数

        /// <summary>
        ///     未定義のモデル番号
        /// </summary>
        public const int UndefinedModelNo = -1;

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public NavalDivision()
        {
            Model = UndefinedModelNo;
            BrigadeModel = UndefinedModelNo;
            BrigadeModel1 = UndefinedModelNo;
            BrigadeModel2 = UndefinedModelNo;
            BrigadeModel3 = UndefinedModelNo;
            BrigadeModel4 = UndefinedModelNo;
            BrigadeModel5 = UndefinedModelNo;
        }

        #endregion
    }

    /// <summary>
    ///     空軍師団
    /// </summary>
    public class AirDivision
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     師団名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     ユニット種類
        /// </summary>
        public UnitType Type { get; set; }

        /// <summary>
        ///     モデル番号
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        ///     核兵器搭載
        /// </summary>
        public bool Nuke { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra1 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra2 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra3 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra4 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra5 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel1 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel2 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel3 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel4 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel5 { get; set; }

        /// <summary>
        ///     最大戦力
        /// </summary>
        public int MaxStrength { get; set; }

        /// <summary>
        ///     戦力
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        ///     指揮統制率
        /// </summary>
        public int Organisation { get; set; }

        /// <summary>
        ///     士気
        /// </summary>
        public int Morale { get; set; }

        /// <summary>
        ///     経験値
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        ///     休止状態
        /// </summary>
        public bool Dormant { get; set; }

        #endregion

        #region 公開定数

        /// <summary>
        ///     未定義のモデル番号
        /// </summary>
        public const int UndefinedModelNo = -1;

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public AirDivision()
        {
            Model = UndefinedModelNo;
            BrigadeModel = UndefinedModelNo;
        }

        #endregion
    }

    /// <summary>
    ///     陸軍任務
    /// </summary>
    public class LandMission
    {
        #region 公開プロパティ

        /// <summary>
        ///     任務の種類
        /// </summary>
        public LandMissionType Type { get; set; }

        /// <summary>
        ///     対象プロヴィンス
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        ///     戦力/指揮統制率下限
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        ///     夜間遂行
        /// </summary>
        public bool Night { get; set; }

        /// <summary>
        ///     昼間遂行
        /// </summary>
        public bool Day { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public int Task { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        #endregion
    }

    /// <summary>
    ///     海軍任務
    /// </summary>
    public class NavalMission
    {
        #region 公開プロパティ

        /// <summary>
        ///     任務の種類
        /// </summary>
        public NavalMissionType Type { get; set; }

        /// <summary>
        ///     対象プロヴィンス
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        ///     戦力/指揮統制率下限
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        ///     夜間遂行
        /// </summary>
        public bool Night { get; set; }

        /// <summary>
        ///     昼間遂行
        /// </summary>
        public bool Day { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     終了日時
        /// </summary>
        public GameDate EndDate { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public int Task { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        #endregion
    }

    /// <summary>
    ///     空軍任務
    /// </summary>
    public class AirMission
    {
        #region 公開プロパティ

        /// <summary>
        ///     任務の種類
        /// </summary>
        public AirMissionType Type { get; set; }

        /// <summary>
        ///     対象プロヴィンス
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        ///     戦力/指揮統制率下限
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        ///     夜間遂行
        /// </summary>
        public bool Night { get; set; }

        /// <summary>
        ///     昼間遂行
        /// </summary>
        public bool Day { get; set; }

        /// <summary>
        ///     開始日時
        /// </summary>
        public GameDate StartDate { get; set; }

        /// <summary>
        ///     終了日時
        /// </summary>
        public GameDate EndDate { get; set; }

        /// <summary>
        ///     任務
        /// </summary>
        public int Task { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        #endregion
    }

    /// <summary>
    ///     生産中師団情報
    /// </summary>
    public class DivisionDevelopment
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     師団名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     必要IC
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        ///     必要人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     unitcost (詳細不明)
        /// </summary>
        public bool UnitCost { get; set; }

        /// <summary>
        ///     new_model (詳細不明)
        /// </summary>
        public bool NewModel { get; set; }

        /// <summary>
        ///     完了予定日
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     進捗率増分
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        ///     総進捗率
        /// </summary>
        public double TotalProgress { get; set; }

        /// <summary>
        ///     連続生産ボーナス
        /// </summary>
        public double GearingBonus { get; set; }

        /// <summary>
        ///     総生産数
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     生産完了数
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        ///     完了日数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        ///     1単位の完了日数
        /// </summary>
        public int DaysForFirst { get; set; }

        /// <summary>
        ///     ユニット種類
        /// </summary>
        public UnitType Type { get; set; }

        /// <summary>
        ///     モデル番号
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra1 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra2 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra3 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra4 { get; set; }

        /// <summary>
        ///     付属旅団のユニット種類
        /// </summary>
        public UnitType Extra5 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel1 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel2 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel3 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel4 { get; set; }

        /// <summary>
        ///     付属旅団のモデル番号
        /// </summary>
        public int BrigadeModel5 { get; set; }

        #endregion

        #region 公開定数

        /// <summary>
        ///     未定義のモデル番号
        /// </summary>
        public const int UndefinedModelNo = -1;

        #endregion

        #region 初期化

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public DivisionDevelopment()
        {
            Model = UndefinedModelNo;
            BrigadeModel = UndefinedModelNo;
        }

        #endregion
    }

    /// <summary>
    ///     生産中輸送船団情報
    /// </summary>
    public class ConvoyDevelopment
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     輸送船団の種類
        /// </summary>
        public ConvoyType Type { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        ///     必要IC
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        ///     必要人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     完了予定日
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     進捗率増分
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        ///     総進捗率
        /// </summary>
        public double TotalProgress { get; set; }

        /// <summary>
        ///     連続生産ボーナス
        /// </summary>
        public double GearingBonus { get; set; }

        /// <summary>
        ///     連続生産数
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     生産完了数
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        ///     完了日数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        ///     最初の1単位の完了日数
        /// </summary>
        public int DaysForFirst { get; set; }

        #endregion
    }

    /// <summary>
    ///     生産中建物情報
    /// </summary>
    public class BuildingDevelopment
    {
        #region 公開プロパティ

        /// <summary>
        ///     typeとidの組
        /// </summary>
        public TypeId Id { get; set; }

        /// <summary>
        ///     建物の種類
        /// </summary>
        public BuildingType Type { get; set; }

        /// <summary>
        ///     位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        ///     必要IC
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        ///     必要人的資源
        /// </summary>
        public double Manpower { get; set; }

        /// <summary>
        ///     完了予定日
        /// </summary>
        public GameDate Date { get; set; }

        /// <summary>
        ///     進捗率増分
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        ///     総進捗率
        /// </summary>
        public double TotalProgress { get; set; }

        /// <summary>
        ///     連続生産ボーナス
        /// </summary>
        public double GearingBonus { get; set; }

        /// <summary>
        ///     連続生産数
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     生産完了数
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        ///     完了日数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        ///     最初の1単位の完了日数
        /// </summary>
        public int DaysForFirst { get; set; }

        #endregion
    }

    /// <summary>
    ///     typeとidの組
    /// </summary>
    public class TypeId
    {
        #region 公開プロパティ

        /// <summary>
        ///     id
        /// </summary>
        public int Id;

        /// <summary>
        ///     type
        /// </summary>
        public int Type;

        #endregion
    }

    /// <summary>
    ///     外交協定の種類
    /// </summary>
    public enum TreatyType
    {
        None,
        NonAggression, // 不可侵条約
        Peace, // 休戦協定
        Trade, // 貿易
    }

    /// <summary>
    ///     天候の種類
    /// </summary>
    public enum WeatherType
    {
        None,
        Clear, // 快晴
        Frozen, // 氷点下
        Raining, // 降雨
        Snowing, // 降雪
        Storm, // 暴風雨
        Blizzard, // 吹雪
        Muddy // 泥濘地
    }

    /// <summary>
    ///     政体
    /// </summary>
    public enum GovernmentType
    {
        None,
        Nazi, // 国家社会主義
        Fascist, // ファシズム
        PaternalAutocrat, // 専制独裁
        SocialConservative, // 社会保守派
        MarketLiberal, // 自由経済派
        SocialLiberal, // 社会自由派
        SocialDemocrat, // 社会民主派
        LeftWingRadical, // 急進的左翼
        Leninist, // レーニン主義
        Stalinist // スターリン主義
    }

    /// <summary>
    ///     建物の種類
    /// </summary>
    public enum BuildingType
    {
        None,
        Ic, // 工場
        Infrastructure, // インフラ
        CoastalFort, // 沿岸要塞
        LandFort, // 陸上要塞
        AntiAir, // 対空砲
        AirBase, // 航空基地
        NavalBase, // 海軍基地
        RadarStation, // レーダー基地
        NuclearReactor, // 原子炉
        RocketTest, // ロケット試験場
        SyntheticOil, // 合成石油工場
        SyntheticRares, // 合成素材工場
        NuclearPower // 原子力発電所
    }

    /// <summary>
    ///     陸軍任務の種類
    /// </summary>
    public enum LandMissionType
    {
        None,
        Attack, // 攻撃
        StratRedeploy, // 戦略的再配備
        SupportAttack, // 支援攻撃
        SupportDefense, // 防戦支援
        Reserves, // 待機
        AntiPartisanDuty // パルチザン掃討
    }

    /// <summary>
    ///     海軍任務の種類
    /// </summary>
    public enum NavalMissionType
    {
        None,
        ConvoyRaiding, // 船団襲撃
        Asw, // 対潜作戦
        NavalInterdiction, // 海上阻止
        ShoreBombardment, // 沿岸砲撃
        AmphibiousAssault, // 強襲上陸
        SeaTransport, // 海上輸送
        NavalCombatPatrol, // 海上戦闘哨戒
        NavalPortStrike, // 空母による港湾攻撃
        NavalAirbaseStrike // 空母による航空基地攻撃
    }

    /// <summary>
    ///     空軍任務の種類
    /// </summary>
    public enum AirMissionType
    {
        None,
        AirSuperiority, // 制空権
        GroundAttack, // 地上攻撃
        RunwayCratering, // 空港空爆
        InstallationStrike, // 軍事施設攻撃
        Interdiction, // 地上支援
        NavalStrike, // 艦船攻撃
        PortStrike, // 港湾攻撃
        LogisticalStrike, // 兵站攻撃
        StrategicBombardment, // 戦略爆撃
        AirSupply, // 空輸補給
        AirborneAssault, // 空挺強襲
        ConvoyAirRaiding, // 船団爆撃
        Nuke // 核攻撃
    }

    /// <summary>
    ///     輸送船団の種類
    /// </summary>
    public enum ConvoyType
    {
        None,
        Transports, // 輸送船
        Escorts // 護衛艦
    }
}