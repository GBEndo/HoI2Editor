﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HoI2Editor.Models;
using HoI2Editor.Utilities;

namespace HoI2Editor.Writers
{
    /// <summary>
    ///     シナリオデータのファイル書き込みを担当するクラス
    /// </summary>
    public static class ScenarioWriter
    {
        #region シナリオデータ

        /// <summary>
        ///     シナリオデータをファイルへ書き込む
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void Write(Scenario scenario, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                writer.WriteLine("name       = \"{0}\"", scenario.Name);
                writer.WriteLine("panel      = \"{0}\"", scenario.PanelName);
                WriteHeader(scenario.Header, writer);
                WriteGlobalData(scenario.GlobalData, writer);
                WriteHistoryEvents(scenario, writer);
                WriteSleepEvents(scenario, writer);
                WriteSaveDates(scenario, writer);
                WriteMap(scenario.Map, writer);
                WriteEvents(scenario, writer);
                WriteIncludes(scenario, writer);
            }
        }

        /// <summary>
        ///     イベント履歴リストを書き出す
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteHistoryEvents(Scenario scenario, TextWriter writer)
        {
            if (scenario.HistoryEvents.Count > 0)
            {
                writer.WriteLine();
                writer.Write("history = {");
                WriteIdList(scenario.HistoryEvents, writer);
                writer.WriteLine(" }");
            }
        }

        /// <summary>
        ///     休止イベントリストを書き出す
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteSleepEvents(Scenario scenario, TextWriter writer)
        {
            if (scenario.SleepEvents.Count > 0)
            {
                writer.WriteLine();
                writer.Write("sleepevent = {");
                WriteIdList(scenario.SleepEvents, writer);
                writer.WriteLine(" }");
            }
        }

        /// <summary>
        ///     保存日時リストを書き出す
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteSaveDates(Scenario scenario, TextWriter writer)
        {
            if (scenario.SaveDates == null || scenario.SaveDates.Count == 0)
            {
                return;
            }

            GameDate startDate;
            if (scenario.GlobalData == null || scenario.GlobalData.StartDate == null)
            {
                // シナリオ開始日時が設定されていなければ1936/1/1とみなす
                startDate = new GameDate();
            }
            else
            {
                startDate = scenario.GlobalData.StartDate;
            }

            writer.WriteLine();
            writer.WriteLine("save_date = {");
            foreach (KeyValuePair<int, GameDate> pair in scenario.SaveDates)
            {
                writer.WriteLine("  {0} = {1}", pair.Key, startDate.Difference(pair.Value));
            }
            writer.WriteLine("}");
        }

        /// <summary>
        ///     マップ設定を書き出す
        /// </summary>
        /// <param name="map">マップ設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteMap(MapSettings map, TextWriter writer)
        {
            if (map == null)
            {
                return;
            }
            writer.WriteLine();
            writer.WriteLine("map = {");
            writer.WriteLine("  {0} = all", map.All ? "yes" : "no");
            writer.WriteLine();
            foreach (int id in map.Yes)
            {
                writer.WriteLine("  yes = {0}", id);
            }
            foreach (int id in map.No)
            {
                writer.WriteLine("  no = {0}", id);
            }
            if (map.Top != null || map.Bottom != null)
            {
                writer.WriteLine();
            }
            if (map.Top != null)
            {
                writer.WriteLine("  top = {{ x = {0} y = {1} }}", map.Top.X, map.Top.Y);
            }
            if (map.Bottom != null)
            {
                writer.WriteLine("  bottom = {{ x = {0} y = {1} }}", map.Bottom.X, map.Bottom.Y);
            }
            writer.WriteLine("}");
        }

        /// <summary>
        ///     イベントファイルリストを書き出す
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteEvents(Scenario scenario, TextWriter writer)
        {
            if (scenario.EventFiles.Count == 0)
            {
                return;
            }
            writer.WriteLine("# ###################");
            foreach (string name in scenario.EventFiles)
            {
                writer.WriteLine("event      = \"{0}\"", name);
            }
        }

        /// <summary>
        ///     インクルードファイルリストを書き出す
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteIncludes(Scenario scenario, TextWriter writer)
        {
            if (scenario.IncludeFiles.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (string name in scenario.IncludeFiles)
            {
                writer.WriteLine("include = \"{0}\"", name);
            }
        }

        #endregion

        #region シナリオヘッダ

        /// <summary>
        ///     シナリオヘッダを書き出す
        /// </summary>
        /// <param name="header">シナリオヘッダ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteHeader(ScenarioHeader header, TextWriter writer)
        {
            writer.WriteLine("header = {");
            if (!string.IsNullOrEmpty(header.Name))
            {
                writer.WriteLine("  name       = \"{0}\"", header.Name);
            }
            if (header.StartDate != null)
            {
                writer.WriteLine("  startdate  = {{ year = {0} }}", header.StartDate.Year);
            }
            else
            {
                if (header.StartYear > 0)
                {
                    writer.WriteLine("  startyear  = {0}", header.StartYear);
                }
                if (header.EndYear > 0)
                {
                    writer.WriteLine("  endyear    = {0}", header.EndYear);
                }
            }
            if (!header.IsFreeSelection)
            {
                writer.WriteLine("  free       = no");
            }
            if (header.IsBattleScenario)
            {
                writer.WriteLine("  combat     = yes");
            }
            WriteSelectableCountries(header, writer);
            WriteMajorCountries(header, writer);
            writer.WriteLine("}");
        }

        /// <summary>
        ///     選択可能国リストを書き出す
        /// </summary>
        /// <param name="header">シナリオヘッダ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteSelectableCountries(ScenarioHeader header, TextWriter writer)
        {
            if (header.SelectableCountries.Count == 0)
            {
                return;
            }
            writer.Write("  selectable = {");
            WriteCountryList(header.SelectableCountries, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     主要国設定リストを書き出す
        /// </summary>
        /// <param name="header">シナリオヘッダ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteMajorCountries(ScenarioHeader header, TextWriter writer)
        {
            foreach (MajorCountrySettings major in header.MajorCountries)
            {
                WriteMajorCountry(major, writer);
            }
        }

        /// <summary>
        ///     主要国設定を書き出す
        /// </summary>
        /// <param name="major">主要国設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteMajorCountry(MajorCountrySettings major, TextWriter writer)
        {
            writer.Write("  {0}        = {{", Countries.Strings[(int) major.Country]);
            if (!string.IsNullOrEmpty(major.Desc))
            {
                writer.Write(" desc = \"{0}\"", major.Desc);
            }
            if (major.Bottom)
            {
                writer.Write(" bottom = yes");
            }
            if (!string.IsNullOrEmpty(major.PictureName))
            {
                writer.Write(" picture = \"{0}\"", major.PictureName);
            }
            if (!string.IsNullOrEmpty(major.FlagExt))
            {
                writer.Write(" flag_ext = {0}", major.FlagExt);
            }
            if (!string.IsNullOrEmpty(major.Name))
            {
                writer.Write(" name = {0}", major.Name);
            }
            writer.WriteLine(" }");
        }

        #endregion

        #region シナリオグローバルデータ

        /// <summary>
        ///     シナリオグローバルデータを書き出す
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteGlobalData(ScenarioGlobalData data, TextWriter writer)
        {
            writer.WriteLine("globaldata = {");
            WriteRules(data.Rules, writer);
            WriteScenarioStartDate(data.StartDate, writer);
            WriteAlliances(data, writer);
            WriteWars(data, writer);
            WriteTreaties(data, writer);
            WriteScenarioDormantLeaders(data, writer);
            WriteScenarioDormantMinisters(data, writer);
            WriteScenarioDormantTeams(data, writer);
            WriteScenarioEndDate(data.EndDate, writer);
            writer.WriteLine("}");
        }

        /// <summary>
        ///     シナリオ開始日を書き出す
        /// </summary>
        /// <param name="date">シナリオ開始日</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteScenarioStartDate(GameDate date, TextWriter writer)
        {
            if (date == null)
            {
                return;
            }
            writer.Write("  startdate = ");
            WriteDate(date, writer);
            writer.WriteLine();
        }

        /// <summary>
        ///     シナリオ終了日を書き出す
        /// </summary>
        /// <param name="date">シナリオ終了日</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteScenarioEndDate(GameDate date, TextWriter writer)
        {
            if (date == null)
            {
                return;
            }
            writer.Write("  enddate   = ");
            WriteDate(date, writer);
            writer.WriteLine();
        }

        /// <summary>
        ///     ルール設定を書き出す
        /// </summary>
        /// <param name="rules">ルール設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteRules(ScenarioRules rules, TextWriter writer)
        {
            // 禁止項目がなければセクションを書き出さない
            if (rules.AllowDiplomacy && rules.AllowProduction && rules.AllowTechnology)
            {
                return;
            }

            writer.WriteLine("  rules = {");
            if (!rules.AllowDiplomacy)
            {
                writer.WriteLine("    diplomacy = no");
            }
            if (!rules.AllowProduction)
            {
                writer.WriteLine("    production = no");
            }
            if (!rules.AllowTechnology)
            {
                writer.WriteLine("    technology = no");
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     同盟リストを書き出す
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAlliances(ScenarioGlobalData data, TextWriter writer)
        {
            if (data.Axis != null)
            {
                WriteAlliance(data.Axis, "axis", writer);
            }
            if (data.Allies != null)
            {
                WriteAlliance(data.Allies, "allies", writer);
            }
            if (data.Comintern != null)
            {
                WriteAlliance(data.Comintern, "comintern", writer);
            }
            foreach (Alliance alliance in data.Alliances)
            {
                WriteAlliance(alliance, "alliance", writer);
            }
        }

        /// <summary>
        ///     戦争リストを書き出す
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteWars(ScenarioGlobalData data, TextWriter writer)
        {
            if (data.Wars.Count == 0)
            {
                return;
            }
            foreach (War war in data.Wars)
            {
                WriteWar(war, writer);
            }
        }

        /// <summary>
        ///     外交協定リストを書き出す
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteTreaties(ScenarioGlobalData data, TextWriter writer)
        {
            // 不可侵条約
            foreach (Treaty treaty in data.NonAggressions)
            {
                WriteTreaty(treaty, writer);
            }

            // 講和条約
            foreach (Treaty treaty in data.Peaces)
            {
                WriteTreaty(treaty, writer);
            }

            // 貿易
            foreach (Treaty treaty in data.Trades)
            {
                WriteTreaty(treaty, writer);
            }
        }

        /// <summary>
        ///     同盟情報を書き出す
        /// </summary>
        /// <param name="alliance">同盟情報</param>
        /// <param name="type">同盟の種類</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAlliance(Alliance alliance, string type, TextWriter writer)
        {
            writer.WriteLine("  {0} = {{", type);
            if (alliance.Id != null)
            {
                writer.Write("    id          = ");
                WriteTypeId(alliance.Id, writer);
                writer.WriteLine();
            }
            writer.Write("    participant = {");
            WriteCountryList(alliance.Participant, writer);
            writer.WriteLine(" }");
            if (!string.IsNullOrEmpty(alliance.Name))
            {
                writer.WriteLine("    name = \"{0}\"", alliance.Name);
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     戦争情報を書き出す
        /// </summary>
        /// <param name="war">戦争情報</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteWar(War war, TextWriter writer)
        {
            writer.WriteLine("  war = {");
            if (war.Id != null)
            {
                writer.Write("    id          = ");
                WriteTypeId(war.Id, writer);
                writer.WriteLine();
            }
            if (war.StartDate != null)
            {
                writer.Write("    date        = ");
                WriteDate(war.StartDate, writer);
                writer.WriteLine();
            }
            if (war.EndDate != null)
            {
                writer.Write("    enddate     = ");
                WriteDate(war.EndDate, writer);
                writer.WriteLine();
            }
            writer.WriteLine("    attackers   = {");
            if (war.Attackers.Id != null)
            {
                writer.Write("      id          = ");
                WriteTypeId(war.Attackers.Id, writer);
                writer.WriteLine();
            }
            writer.Write("      participant = {");
            WriteCountryList(war.Attackers.Participant, writer);
            writer.WriteLine(" }");
            writer.WriteLine("    }");
            writer.WriteLine("    defenders   = {");
            if (war.Defenders.Id != null)
            {
                writer.Write("      id          = ");
                WriteTypeId(war.Defenders.Id, writer);
                writer.WriteLine();
            }
            writer.Write("      participant = {");
            WriteCountryList(war.Defenders.Participant, writer);
            writer.WriteLine(" }");
            writer.WriteLine("    }");
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     外交協定情報を書き出す
        /// </summary>
        /// <param name="treaty">外交協定情報</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteTreaty(Treaty treaty, TextWriter writer)
        {
            writer.WriteLine("  treaty = {");
            if (treaty.Id != null)
            {
                writer.Write("    id             = ");
                WriteTypeId(treaty.Id, writer);
                writer.WriteLine();
            }
            writer.WriteLine("    type           = {0}", Scenarios.TreatyStrings[(int) treaty.Type]);
            writer.WriteLine("    country        = \"{0}\"", treaty.Country1);
            writer.WriteLine("    country        = \"{0}\"", treaty.Country2);
            if (treaty.StartDate != null)
            {
                writer.Write("    startdate      = ");
                WriteDate(treaty.StartDate, writer);
                writer.WriteLine();
            }
            if (treaty.EndDate != null)
            {
                writer.Write("    expirydate     = ");
                WriteDate(treaty.EndDate, writer);
                writer.WriteLine();
            }
            if (!DoubleHelper.IsZero(treaty.Energy))
            {
                writer.WriteLine("    energy         = {0}", DoubleHelper.ToString(treaty.Energy));
            }
            if (!DoubleHelper.IsZero(treaty.Metal))
            {
                writer.WriteLine("    metal          = {0}", DoubleHelper.ToString(treaty.Metal));
            }
            if (!DoubleHelper.IsZero(treaty.RareMaterials))
            {
                writer.WriteLine("    rare_materials = {0}", DoubleHelper.ToString(treaty.RareMaterials));
            }
            if (!DoubleHelper.IsZero(treaty.Oil))
            {
                writer.WriteLine("    oil            = {0}", DoubleHelper.ToString(treaty.Oil));
            }
            if (!DoubleHelper.IsZero(treaty.Supplies))
            {
                writer.WriteLine("    supplies       = {0}", DoubleHelper.ToString(treaty.Supplies));
            }
            if (!DoubleHelper.IsZero(treaty.Money))
            {
                writer.WriteLine("    money          = {0}", DoubleHelper.ToString(treaty.Money));
            }
            if (!treaty.Cancel)
            {
                writer.WriteLine("    cancel         = no");
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     休止指揮官リストを書き出す (シナリオグローバルデータ)
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteScenarioDormantLeaders(ScenarioGlobalData data, TextWriter writer)
        {
            if (data.DormantLeadersAll)
            {
                writer.WriteLine("  dormant_leaders   = yes");
                return;
            }

            // 休止指揮官がない場合は何も書き出さない
            if (data.DormantLeaders.Count == 0)
            {
                return;
            }

            writer.Write("  dormant_leaders   = {");
            WriteIdList(data.DormantLeaders, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     休止閣僚リストを書き出す (シナリオグローバルデータ)
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteScenarioDormantMinisters(ScenarioGlobalData data, TextWriter writer)
        {
            // 休止閣僚がない場合は何も書き出さない
            if (data.DormantMinisters.Count == 0)
            {
                return;
            }

            writer.Write("  dormant_ministers = {");
            WriteIdList(data.DormantMinisters, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     休止研究機関リストを書き出す (シナリオグローバルデータ)
        /// </summary>
        /// <param name="data">シナリオグローバルデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteScenarioDormantTeams(ScenarioGlobalData data, TextWriter writer)
        {
            // 休止研究機関がない場合は何も書き出さない
            if (data.DormantTeams.Count == 0)
            {
                return;
            }

            writer.Write("  dormant_teams     = {");
            WriteIdList(data.DormantTeams, writer);
            writer.WriteLine(" }");
        }

        #endregion

        #region プロヴィンス

        /// <summary>
        ///     プロヴィンス設定をbases.incに書き込む
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void WriteBasesInc(Scenario scenario, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                if (Game.IsDhFull())
                {
                    // DH Full
                    WriteBasesIncDhFull(scenario, writer);
                }
                else
                {
                    // HoI2/AoD/DH None
                    WriteBasesIncHoI2(scenario, writer);
                }
            }
        }

        /// <summary>
        ///     プロヴィンス設定をbases.incに書き込む (HoI2/AoD/DH None)
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBasesIncHoI2(Scenario scenario, TextWriter writer)
        {
            foreach (ProvinceSettings settings in scenario.Provinces.Where(settings => settings.NavalBase != null))
            {
                Province province = Provinces.Items[settings.Id];
                writer.Write("province = {{ id = {0} naval_base = ", settings.Id);
                WriteBuilding(settings.NavalBase, writer);
                writer.WriteLine(" }} # {0}", Scenarios.GetProvinceName(province, settings));
            }
            writer.WriteLine();
            foreach (ProvinceSettings settings in scenario.Provinces.Where(settings => settings.AirBase != null))
            {
                Province province = Provinces.Items[settings.Id];
                writer.Write("province = {{ id = {0} air_base = ", settings.Id);
                WriteBuilding(settings.AirBase, writer);
                writer.WriteLine(" }");
                writer.WriteLine(" }} # {0}", Scenarios.GetProvinceName(province, settings));
            }
        }

        /// <summary>
        ///     プロヴィンス設定をbases.incに書き込む (DH Full)
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBasesIncDhFull(Scenario scenario, TextWriter writer)
        {
            foreach (ProvinceSettings settings in scenario.Provinces.Where(p => ExistsBasesIncDataDhFull(p, scenario)))
            {
                Province province = Provinces.Items[settings.Id];
                writer.WriteLine("province = {");
                writer.WriteLine("  id = {0} # {1}", settings.Id, Scenarios.GetProvinceName(province, settings));
                if (!scenario.IsBaseDodProvinceSettings)
                {
                    if (settings.Ic != null)
                    {
                        writer.Write("  ic = ");
                        WriteBuilding(settings.Ic, writer);
                        writer.WriteLine();
                    }
                    if (settings.Infrastructure != null)
                    {
                        writer.Write("  infra = ");
                        WriteBuilding(settings.Infrastructure, writer);
                        writer.WriteLine();
                    }
                }
                if (settings.LandFort != null)
                {
                    writer.Write("  landfort = ");
                    WriteBuilding(settings.LandFort, writer);
                    writer.WriteLine();
                }
                if (settings.CoastalFort != null)
                {
                    writer.Write("  coastalfort = ");
                    WriteBuilding(settings.CoastalFort, writer);
                    writer.WriteLine();
                }
                if (settings.AntiAir != null)
                {
                    writer.Write("  anti_air = ");
                    WriteBuilding(settings.AntiAir, writer);
                    writer.WriteLine();
                }
                if (settings.AirBase != null)
                {
                    writer.Write("  air_base = ");
                    WriteBuilding(settings.AirBase, writer);
                    writer.WriteLine();
                }
                if (settings.NavalBase != null)
                {
                    writer.Write("  naval_base = ");
                    WriteBuilding(settings.NavalBase, writer);
                    writer.WriteLine();
                }
                if (settings.RadarStation != null)
                {
                    writer.Write("  radar_station = ");
                    WriteBuilding(settings.RadarStation, writer);
                    writer.WriteLine();
                }
                if (settings.NuclearReactor != null)
                {
                    writer.Write("  nuclear_reactor = ");
                    WriteBuilding(settings.NuclearReactor, writer);
                    writer.WriteLine();
                }
                if (settings.RocketTest != null)
                {
                    writer.Write("  rocket_test = ");
                    WriteBuilding(settings.RocketTest, writer);
                    writer.WriteLine();
                }
                if (!scenario.IsDepotsProvinceSettings)
                {
                    if (settings.SupplyPool > 0)
                    {
                        writer.WriteLine("  supplypool = {0}", DoubleHelper.ToString(settings.SupplyPool));
                    }
                    if (settings.OilPool > 0)
                    {
                        writer.WriteLine("  oilpool = {0}", DoubleHelper.ToString(settings.OilPool));
                    }
                    if (settings.EnergyPool > 0)
                    {
                        writer.WriteLine("  energypool = {0}", DoubleHelper.ToString(settings.EnergyPool));
                    }
                    if (settings.MetalPool > 0)
                    {
                        writer.WriteLine("  metalpool = {0}", DoubleHelper.ToString(settings.MetalPool));
                    }
                    if (settings.RareMaterialsPool > 0)
                    {
                        writer.WriteLine("  rarematerialspool = {0}", DoubleHelper.ToString(settings.RareMaterialsPool));
                    }
                }
                if (settings.Energy > 0)
                {
                    writer.WriteLine("  energy = {0}", DoubleHelper.ToString(settings.Energy));
                }
                if (settings.MaxEnergy > 0)
                {
                    writer.WriteLine("  max_energy = {0}", DoubleHelper.ToString(settings.MaxEnergy));
                }
                if (settings.Metal > 0)
                {
                    writer.WriteLine("  metal = {0}", DoubleHelper.ToString(settings.Metal));
                }
                if (settings.MaxMetal > 0)
                {
                    writer.WriteLine("  max_metal = {0}", DoubleHelper.ToString(settings.MaxMetal));
                }
                if (settings.RareMaterials > 0)
                {
                    writer.WriteLine("  rare_materials = {0}", DoubleHelper.ToString(settings.RareMaterials));
                }
                if (settings.MaxRareMaterials > 0)
                {
                    writer.WriteLine("  max_rare_materials = {0}", DoubleHelper.ToString(settings.MaxRareMaterials));
                }
                if (settings.Oil > 0)
                {
                    writer.WriteLine("  oil = {0}", DoubleHelper.ToString(settings.Oil));
                }
                if (settings.MaxOil > 0)
                {
                    writer.WriteLine("  max_oil = {0}", DoubleHelper.ToString(settings.MaxOil));
                }
                if (settings.Manpower > 0)
                {
                    writer.WriteLine("  manpower = {0}", DoubleHelper.ToString(settings.Manpower));
                }
                if (settings.MaxManpower > 0)
                {
                    writer.WriteLine("  max_manpower = {0}", DoubleHelper.ToString(settings.MaxManpower));
                }
                if (settings.RevoltRisk > 0)
                {
                    writer.WriteLine("  province_revoltrisk = {0}", DoubleHelper.ToString(settings.RevoltRisk));
                }
                if (!string.IsNullOrEmpty(settings.Name))
                {
                    writer.WriteLine("  name = {0}", settings.Name);
                }
                if (settings.Weather != WeatherType.None)
                {
                    writer.WriteLine("  weather = {0}", Scenarios.WeatherStrings[(int) settings.Weather]);
                }
                writer.WriteLine("}");
                writer.WriteLine();
            }
        }

        /// <summary>
        ///     プロヴィンス設定をbases_DOD.incに書き込む
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void WriteBasesDodInc(Scenario scenario, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                foreach (ProvinceSettings settings in scenario.Provinces.Where(ExistsBasesDodIncData))
                {
                    Province province = Provinces.Items[settings.Id];
                    writer.Write("province = {{ id = {0}", settings.Id);
                    if (settings.Ic != null)
                    {
                        writer.Write(" ic = ");
                        WriteBuilding(settings.Ic, writer);
                    }
                    if (settings.Infrastructure != null)
                    {
                        writer.Write(" infra = ");
                        WriteBuilding(settings.Infrastructure, writer);
                    }
                    writer.WriteLine(" }} # {0}", Scenarios.GetProvinceName(province, settings));
                }
            }
        }

        /// <summary>
        ///     プロヴィンス設定をdepots.incに書き込む
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void WriteDepotsInc(Scenario scenario, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                foreach (ProvinceSettings settings in scenario.Provinces.Where(ExistsDepotsIncData))
                {
                    Province province = Provinces.Items[settings.Id];
                    writer.WriteLine("province = {");
                    writer.WriteLine("  id = {0} # {1}", settings.Id, Scenarios.GetProvinceName(province, settings));
                    if (settings.SupplyPool > 0)
                    {
                        writer.WriteLine("  supplypool = {0}", DoubleHelper.ToString(settings.SupplyPool));
                    }
                    if (settings.OilPool > 0)
                    {
                        writer.WriteLine("  oilpool = {0}", DoubleHelper.ToString(settings.OilPool));
                    }
                    if (settings.EnergyPool > 0)
                    {
                        writer.WriteLine("  energypool = {0}", DoubleHelper.ToString(settings.EnergyPool));
                    }
                    if (settings.MetalPool > 0)
                    {
                        writer.WriteLine("  metalpool = {0}", DoubleHelper.ToString(settings.MetalPool));
                    }
                    if (settings.RareMaterialsPool > 0)
                    {
                        writer.WriteLine("  rarematerialspool = {0}", DoubleHelper.ToString(settings.RareMaterialsPool));
                    }
                    writer.WriteLine("}");
                }
            }
        }

        /// <summary>
        ///     プロヴィンス設定をvp.incに書き込む
        /// </summary>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void WriteVpInc(Scenario scenario, string fileName)
        {
            // vp.incに保存しないならば何もしない
            if (!scenario.IsVpProvinceSettings)
            {
                return;
            }

            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                writer.WriteLine();
                writer.WriteLine("###############################");
                writer.WriteLine("# Victory points distribution #");
                writer.WriteLine("###############################");
                writer.WriteLine();
                foreach (ProvinceSettings settings in scenario.Provinces.Where(settings => settings.Vp != 0))
                {
                    Province province = Provinces.Items[settings.Id];
                    writer.WriteLine("province = {{ id = {0} points = {1} }} # {2}", settings.Id, settings.Vp,
                        Scenarios.GetProvinceName(province, settings));
                }
            }
        }

        /// <summary>
        ///     プロヴィンス設定のリストを国別incに書き込む
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        /// <returns>書き込むデータがあればtrueを返す</returns>
        private static bool WriteCountryProvinces(CountrySettings settings, Scenario scenario,
            TextWriter writer)
        {
            if (!scenario.IsCountryProvinceSettings)
            {
                return false;
            }
            bool exists = false;
            foreach (ProvinceSettings ps in scenario.Provinces.Where(
                ps => settings.ControlledProvinces.Contains(ps.Id) && ExistsCountryIncData(ps, scenario)))
            {
                WriteCountryProvince(ps, scenario, writer);
                exists = true;
            }
            return exists;
        }

        /// <summary>
        ///     プロヴィンス設定を国別incに書き込む
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryProvince(ProvinceSettings settings, Scenario scenario, TextWriter writer)
        {
            if (IsProvinceSingleLine(settings))
            {
                WriteCountryProvinceSingleLine(settings, scenario, writer);
            }
            else
            {
                WriteCountryProvinceMultiLine(settings, scenario, writer);
            }
        }

        /// <summary>
        ///     単一行のプロヴィンス設定を国別incに書き込む
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryProvinceSingleLine(ProvinceSettings settings, Scenario scenario,
            TextWriter writer)
        {
            Province province = Provinces.Items[settings.Id];
            writer.Write("province = {{ id = {0}", settings.Id);
            if (settings.Ic != null)
            {
                writer.Write(" ic = ");
                WriteBuilding(settings.Ic, writer);
            }
            if (settings.Infrastructure != null)
            {
                writer.Write(" infra = ");
                WriteBuilding(settings.Infrastructure, writer);
            }
            if (settings.LandFort != null)
            {
                writer.Write(" landfort = ");
                WriteBuilding(settings.LandFort, writer);
            }
            if (settings.CoastalFort != null)
            {
                writer.Write(" coastalfort = ");
                WriteBuilding(settings.CoastalFort, writer);
            }
            if (settings.AntiAir != null)
            {
                writer.Write(" anti_air = ");
                WriteBuilding(settings.AntiAir, writer);
            }
            if (!scenario.IsBaseProvinceSettings)
            {
                if (settings.AirBase != null)
                {
                    writer.Write(" air_base = ");
                    WriteBuilding(settings.AirBase, writer);
                }
                if (settings.NavalBase != null)
                {
                    writer.Write(" naval_base = ");
                    WriteBuilding(settings.NavalBase, writer);
                }
            }
            if (settings.RadarStation != null)
            {
                writer.Write(" radar_station = ");
                WriteBuilding(settings.RadarStation, writer);
            }
            if (settings.NuclearReactor != null)
            {
                writer.Write(" nuclear_reactor = ");
                WriteBuilding(settings.NuclearReactor, writer);
            }
            if (settings.RocketTest != null)
            {
                writer.Write(" rocket_test = ");
                WriteBuilding(settings.RocketTest, writer);
            }
            if (Game.Type == GameType.ArsenalOfDemocracy)
            {
                if (settings.SyntheticOil != null)
                {
                    writer.Write(" synthetic_oil = ");
                    WriteBuilding(settings.SyntheticOil, writer);
                }
                if (settings.SyntheticRares != null)
                {
                    writer.Write(" synthetic_rares = ");
                    WriteBuilding(settings.SyntheticRares, writer);
                }
                if (settings.NuclearPower != null)
                {
                    writer.Write(" nuclear_power = ");
                    WriteBuilding(settings.NuclearPower, writer);
                }
            }
            if (settings.SupplyPool > 0)
            {
                writer.Write(" supplypool = {0}", ObjectHelper.ToString(settings.SupplyPool));
            }
            if (settings.OilPool > 0)
            {
                writer.Write(" oilpool = {0}", ObjectHelper.ToString(settings.OilPool));
            }
            writer.WriteLine(" }} # {0}", Scenarios.GetProvinceName(province, settings));
        }

        /// <summary>
        ///     複数行のプロヴィンス設定を国別incに書き込む
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryProvinceMultiLine(ProvinceSettings settings, Scenario scenario,
            TextWriter writer)
        {
            Province province = Provinces.Items[settings.Id];
            writer.WriteLine("province = {");
            writer.WriteLine("  id = {0} # {1}", settings.Id, Scenarios.GetProvinceName(province, settings));
            if (settings.Ic != null)
            {
                writer.Write("  ic = ");
                WriteBuilding(settings.Ic, writer);
                writer.WriteLine();
            }
            if (settings.Infrastructure != null)
            {
                writer.Write("  infra = ");
                WriteBuilding(settings.Infrastructure, writer);
                writer.WriteLine();
            }
            if (settings.LandFort != null)
            {
                writer.Write("  landfort = ");
                WriteBuilding(settings.LandFort, writer);
                writer.WriteLine();
            }
            if (settings.CoastalFort != null)
            {
                writer.Write("  coastalfort = ");
                WriteBuilding(settings.CoastalFort, writer);
                writer.WriteLine();
            }
            if (settings.AntiAir != null)
            {
                writer.Write("  anti_air = ");
                WriteBuilding(settings.AntiAir, writer);
                writer.WriteLine();
            }
            if (!scenario.IsBaseProvinceSettings)
            {
                if (settings.AirBase != null)
                {
                    writer.Write("  air_base = ");
                    WriteBuilding(settings.AirBase, writer);
                    writer.WriteLine();
                }
                if (settings.NavalBase != null)
                {
                    writer.Write("  naval_base = ");
                    WriteBuilding(settings.NavalBase, writer);
                    writer.WriteLine();
                }
            }
            if (settings.RadarStation != null)
            {
                writer.Write("  radar_station = ");
                WriteBuilding(settings.RadarStation, writer);
                writer.WriteLine();
            }
            if (settings.NuclearReactor != null)
            {
                writer.Write("  nuclear_reactor = ");
                WriteBuilding(settings.NuclearReactor, writer);
                writer.WriteLine();
            }
            if (settings.RocketTest != null)
            {
                writer.Write("  rocket_test = ");
                WriteBuilding(settings.RocketTest, writer);
                writer.WriteLine();
            }
            if (Game.Type == GameType.ArsenalOfDemocracy)
            {
                if (settings.SyntheticOil != null)
                {
                    writer.Write("  synthetic_oil = ");
                    WriteBuilding(settings.SyntheticOil, writer);
                    writer.WriteLine();
                }
                if (settings.SyntheticRares != null)
                {
                    writer.Write("  synthetic_rares = ");
                    WriteBuilding(settings.SyntheticRares, writer);
                    writer.WriteLine();
                }
                if (settings.NuclearPower != null)
                {
                    writer.Write("  nuclear_power = ");
                    WriteBuilding(settings.NuclearPower, writer);
                    writer.WriteLine();
                }
            }
            if (settings.SupplyPool > 0)
            {
                writer.WriteLine("  supplypool = {0}", ObjectHelper.ToString(settings.SupplyPool));
            }
            if (settings.OilPool > 0)
            {
                writer.WriteLine("  oilpool = {0}", ObjectHelper.ToString(settings.OilPool));
            }
            if (settings.EnergyPool > 0)
            {
                writer.WriteLine("  energypool = {0}", ObjectHelper.ToString(settings.EnergyPool));
            }
            if (settings.MetalPool > 0)
            {
                writer.WriteLine("  metalpool = {0}", ObjectHelper.ToString(settings.MetalPool));
            }
            if (settings.RareMaterialsPool > 0)
            {
                writer.WriteLine("  rarematerialspool = {0}", ObjectHelper.ToString(settings.RareMaterialsPool));
            }
            if (settings.Energy > 0)
            {
                writer.WriteLine("  energy = {0}", ObjectHelper.ToString(settings.Energy));
            }
            if (settings.MaxEnergy > 0)
            {
                writer.WriteLine("  max_energy = {0}", ObjectHelper.ToString(settings.MaxEnergy));
            }
            if (settings.Metal > 0)
            {
                writer.WriteLine("  metal = {0}", ObjectHelper.ToString(settings.Metal));
            }
            if (settings.MaxMetal > 0)
            {
                writer.WriteLine("  max_metal = {0}", ObjectHelper.ToString(settings.MaxMetal));
            }
            if (settings.RareMaterials > 0)
            {
                writer.WriteLine("  rare_materials = {0}", ObjectHelper.ToString(settings.RareMaterials));
            }
            if (settings.MaxRareMaterials > 0)
            {
                writer.WriteLine("  max_rare_materials = {0}", ObjectHelper.ToString(settings.MaxRareMaterials));
            }
            if (settings.Oil > 0)
            {
                writer.WriteLine("  oil = {0}", ObjectHelper.ToString(settings.Oil));
            }
            if (settings.MaxOil > 0)
            {
                writer.WriteLine("  max_oil = {0}", ObjectHelper.ToString(settings.MaxOil));
            }
            if (settings.Manpower > 0)
            {
                writer.WriteLine("  manpower = {0}", ObjectHelper.ToString(settings.Manpower));
            }
            if (settings.MaxManpower > 0)
            {
                writer.WriteLine("  max_manpower = {0}", ObjectHelper.ToString(settings.MaxManpower));
            }
            if (settings.RevoltRisk > 0)
            {
                writer.WriteLine("  province_revoltrisk = {0}", ObjectHelper.ToString(settings.RevoltRisk));
            }
            if (!string.IsNullOrEmpty(settings.Name))
            {
                writer.WriteLine("  name = {0} ", settings.Name);
            }
            if (settings.Weather != WeatherType.None)
            {
                writer.WriteLine("  weather = {0}", Scenarios.WeatherStrings[(int) settings.Weather]);
            }
            writer.WriteLine("}");
        }

        /// <summary>
        ///     プロヴィンス設定が単一行で記載できるかどうかを返す
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <returns>単一行で記載できればtrueを返す</returns>
        private static bool IsProvinceSingleLine(ProvinceSettings settings)
        {
            if ((settings.Ic != null && settings.Ic.MaxSize > 0) ||
                (settings.Infrastructure != null && settings.Infrastructure.MaxSize > 0) ||
                (settings.LandFort != null && settings.LandFort.MaxSize > 0) ||
                (settings.CoastalFort != null && settings.CoastalFort.MaxSize > 0) ||
                (settings.AntiAir != null && settings.AntiAir.MaxSize > 0) ||
                (settings.AirBase != null && settings.AirBase.MaxSize > 0) ||
                (settings.NavalBase != null && settings.NavalBase.MaxSize > 0) ||
                (settings.RadarStation != null && settings.RadarStation.MaxSize > 0) ||
                (settings.NuclearReactor != null && settings.NuclearReactor.MaxSize > 0) ||
                (settings.RocketTest != null && settings.RocketTest.MaxSize > 0) ||
                (settings.SyntheticOil != null && settings.SyntheticOil.MaxSize > 0) ||
                (settings.SyntheticRares != null && settings.SyntheticRares.MaxSize > 0) ||
                (settings.NuclearPower != null && settings.NuclearPower.MaxSize > 0))
            {
                return false;
            }

            if (settings.EnergyPool > 0 ||
                settings.MetalPool > 0 ||
                settings.RareMaterialsPool > 0 ||
                settings.Energy > 0 ||
                settings.MaxEnergy > 0 ||
                settings.Metal > 0 ||
                settings.MaxMetal > 0 ||
                settings.RareMaterials > 0 ||
                settings.MaxRareMaterials > 0 ||
                settings.Oil > 0 ||
                settings.MaxOil > 0 ||
                settings.Manpower > 0 ||
                settings.MaxManpower > 0 ||
                settings.RevoltRisk > 0)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(settings.Name))
            {
                return false;
            }

            if (settings.Weather != WeatherType.None)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     bases.incに保存するプロヴィンスデータが存在するかどうかを返す (DH Full)
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <returns>bases.incに保存するデータが存在すればtrueを返す</returns>
        private static bool ExistsBasesIncDataDhFull(ProvinceSettings settings, Scenario scenario)
        {
            if (!scenario.IsBaseDodProvinceSettings)
            {
                if (settings.Ic != null || settings.Infrastructure != null)
                {
                    return true;
                }
            }

            if (settings.LandFort != null ||
                settings.CoastalFort != null ||
                settings.AntiAir != null ||
                settings.AirBase != null ||
                settings.NavalBase != null ||
                settings.RadarStation != null ||
                settings.NuclearReactor != null ||
                settings.RocketTest != null ||
                settings.SyntheticOil != null ||
                settings.SyntheticRares != null ||
                settings.NuclearPower != null)
            {
                return true;
            }

            if (!scenario.IsDepotsProvinceSettings)
            {
                if (settings.SupplyPool > 0 ||
                    settings.OilPool > 0 ||
                    settings.EnergyPool > 0 ||
                    settings.MetalPool > 0 ||
                    settings.RareMaterialsPool > 0)
                {
                    return true;
                }
            }

            if (settings.RevoltRisk > 0 ||
                settings.Manpower > 0 ||
                settings.MaxManpower > 0 ||
                settings.Energy > 0 ||
                settings.MaxEnergy > 0 ||
                settings.Metal > 0 ||
                settings.MaxMetal > 0 ||
                settings.RareMaterials > 0 ||
                settings.MaxRareMaterials > 0 ||
                settings.Oil > 0 ||
                settings.MaxOil > 0)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(settings.Name))
            {
                return true;
            }

            if (settings.Weather != WeatherType.None)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     bases_DOD.incに保存するプロヴィンスデータが存在するかどうかを返す
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <returns>bases_DOD.incに保存するデータが存在すればtrueを返す</returns>
        private static bool ExistsBasesDodIncData(ProvinceSettings settings)
        {
            return (settings.Ic != null) || (settings.Infrastructure != null);
        }

        /// <summary>
        ///     depots.incに保存するプロヴィンスデータが存在するかどうかを返す
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <returns>depots.incに保存するデータが存在すればtrueを返す</returns>
        private static bool ExistsDepotsIncData(ProvinceSettings settings)
        {
            if (settings.EnergyPool > 0 ||
                settings.MetalPool > 0 ||
                settings.RareMaterialsPool > 0 ||
                settings.OilPool > 0 ||
                settings.SupplyPool > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     国別.incに保存するプロヴィンスデータが存在するかどうかを返す
        /// </summary>
        /// <param name="settings">プロヴィンス設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <returns>bases.incに保存するデータが存在すればtrueを返す</returns>
        private static bool ExistsCountryIncData(ProvinceSettings settings, Scenario scenario)
        {
            if (Game.IsDhFull() && scenario.IsBaseProvinceSettings)
            {
                return false;
            }

            if (!scenario.IsBaseDodProvinceSettings)
            {
                if (settings.Ic != null || settings.Infrastructure != null)
                {
                    return true;
                }
            }

            if (settings.LandFort != null ||
                settings.CoastalFort != null ||
                settings.AntiAir != null ||
                settings.AirBase != null ||
                settings.NavalBase != null ||
                settings.RadarStation != null ||
                settings.NuclearReactor != null ||
                settings.RocketTest != null ||
                settings.SyntheticOil != null ||
                settings.SyntheticRares != null ||
                settings.NuclearPower != null)
            {
                return true;
            }

            if (!scenario.IsDepotsProvinceSettings)
            {
                if (settings.SupplyPool > 0 ||
                    settings.OilPool > 0 ||
                    settings.EnergyPool > 0 ||
                    settings.MetalPool > 0 ||
                    settings.RareMaterialsPool > 0)
                {
                    return true;
                }
            }

            if (settings.RevoltRisk > 0 ||
                settings.Manpower > 0 ||
                settings.MaxManpower > 0 ||
                settings.Energy > 0 ||
                settings.MaxEnergy > 0 ||
                settings.Metal > 0 ||
                settings.MaxMetal > 0 ||
                settings.RareMaterials > 0 ||
                settings.MaxRareMaterials > 0 ||
                settings.Oil > 0 ||
                settings.MaxOil > 0)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(settings.Name))
            {
                return true;
            }

            if (settings.Weather != WeatherType.None)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 建物

        /// <summary>
        ///     生産中建物リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBuildingDevelopments(CountrySettings settings, TextWriter writer)
        {
            if (settings.BuildingDevelopments.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (BuildingDevelopment building in settings.BuildingDevelopments)
            {
                WriteBuildingDevelopment(building, writer);
            }
        }

        /// <summary>
        ///     生産中建物を書き出す
        /// </summary>
        /// <param name="building">生産中建物</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBuildingDevelopment(BuildingDevelopment building, TextWriter writer)
        {
            writer.WriteLine("  province_development = {");
            if (building.Id != null)
            {
                writer.Write("    id             = ");
                WriteTypeId(building.Id, writer);
                writer.WriteLine();
            }
            if (building.Name != null)
            {
                writer.WriteLine("    name           = \"{0}\"", building.Name);
            }
            if (building.Progress > 0)
            {
                writer.WriteLine("    progress       = {0}", DoubleHelper.ToString4(building.Progress));
            }
            if (building.Location > 0)
            {
                writer.WriteLine("    location       = {0}", building.Location);
            }
            if (building.Cost > 0)
            {
                writer.WriteLine("    cost           = {0}", DoubleHelper.ToString4(building.Cost));
            }
            if (building.Date != null)
            {
                writer.Write("    date           = ");
                WriteDate(building.Date, writer);
                writer.WriteLine();
            }
            if (building.Manpower > 0)
            {
                writer.WriteLine("    manpower       = {0}", DoubleHelper.ToString4(building.Manpower));
            }
            if (Game.Type == GameType.ArsenalOfDemocracy)
            {
                if (building.Halted)
                {
                    writer.WriteLine("    halted         = yes");
                }
                writer.WriteLine("    close_when_finished = {0}", BoolHelper.ToString(building.CloseWhenFinished));
                writer.WriteLine("    waitingforclosure = {0}", BoolHelper.ToString(building.WaitingForClosure));
            }
            if (building.TotalProgress > 0)
            {
                writer.WriteLine("    total_progress = {0}", DoubleHelper.ToString4(building.Progress));
            }
            if (building.Size > 0)
            {
                writer.WriteLine("    size           = {0}", building.Size);
            }
            if (building.Done > 0)
            {
                writer.WriteLine("    done           = {0}", building.Done);
            }
            if (building.Days > 0)
            {
                writer.WriteLine("    days           = {0}", building.Days);
            }
            if (building.DaysForFirst > 0)
            {
                writer.WriteLine("    days_for_first = {0}", building.DaysForFirst);
            }
            if (building.GearingBonus > 0)
            {
                writer.WriteLine("    gearing_bonus  = {0}", DoubleHelper.ToString4(building.GearingBonus));
            }
            writer.WriteLine("    type           = {0}", Scenarios.BuildingStrings[(int) building.Type]);
            writer.WriteLine("  }");
        }

        #endregion

        #region 国家設定

        /// <summary>
        ///     国家設定をファイルへ書き込む
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="scenario">シナリオデータ</param>
        /// <param name="fileName">ファイル名</param>
        public static void WriteCountrySettings(CountrySettings settings, Scenario scenario, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding(Game.CodePage)))
            {
                WriteCountryHeader(settings, writer);
                if (WriteCountryProvinces(settings, scenario, writer) || Game.IsDhFull())
                {
                    WriteMainHeader(writer);
                }
                writer.WriteLine();
                writer.WriteLine("country = {");
                WriteCountryInfo(settings, writer);
                WriteCountryResources(settings, writer);
                WriteOffmapResources(settings, writer);
                WriteDiplomacy(settings, writer);
                WriteSpyInfos(settings, writer);
                WriteCountryTerritories(settings, writer);
                WriteTechApps(settings, writer);
                WriteBlueprints(settings, writer);
                WriteInventions(settings, writer);
                WriteCountryPolicy(settings, writer);
                WriteCountryModifiers(settings, writer);
                WriteCabinet(settings, writer);
                WriteIdea(settings, writer);
                WriteCountryDormantLeaders(settings, writer);
                WriteCountryDormantMinisters(settings, writer);
                WriteCountryDormantTeams(settings, writer);
                WriteStealLeaders(settings, writer);
                WriteLandUnits(settings, writer);
                WriteNavalUnits(settings, writer);
                WriteAirUnits(settings, writer);
                WriteDivisionDevelopments(settings, writer);
                WriteBuildingDevelopments(settings, writer);
                WriteConvoyDevelopments(settings, writer);
                WriteDormantLandDivisions(settings, writer);
                writer.WriteLine("}");
            }
        }

        /// <summary>
        ///     国家ヘッダを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryHeader(CountrySettings settings, TextWriter writer)
        {
            writer.WriteLine("##############################");
            writer.WriteLine("# Country definition for {0} #", Countries.Strings[(int) settings.Country]);
            writer.WriteLine("##############################");
        }

        /// <summary>
        ///     メインヘッダを書き出す
        /// </summary>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteMainHeader(StreamWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("#####################");
            writer.WriteLine("# Country main data #");
            writer.WriteLine("#####################");
        }

        /// <summary>
        ///     国家情報を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryInfo(CountrySettings settings, TextWriter writer)
        {
            writer.WriteLine("  tag                = {0}", Countries.Strings[(int) settings.Country]);
            if (settings.RegularId != Country.None)
            {
                writer.WriteLine("  regular_id         = {0}", Countries.Strings[(int) settings.RegularId]);
            }
            if (settings.Master != Country.None)
            {
                writer.WriteLine("  puppet             = {0}", Countries.Strings[(int) settings.Master]);
            }
            if (settings.Control != Country.None)
            {
                writer.WriteLine("  control            = {0}", Countries.Strings[(int) settings.Control]);
            }
            if (!string.IsNullOrEmpty(settings.Name))
            {
                writer.WriteLine("  name               = {0}", settings.Name);
            }
            if (!string.IsNullOrEmpty(settings.FlagExt))
            {
                writer.WriteLine("  flag_ext           = {0}", settings.FlagExt);
            }
            if (!string.IsNullOrEmpty(settings.AiFileName))
            {
                writer.WriteLine("  ai                 = \"{0}\"", settings.AiFileName);
            }
            if (settings.AiSettings != null)
            {
                writer.Write("  ai_settings        = { flags = ");
                WriteAiSettings(settings, writer);
                writer.WriteLine(" }");
            }
            if (settings.IntrinsicGovType != GovernmentType.None)
            {
                writer.WriteLine("  intrinsic_gov_type = {0}",
                    Scenarios.GovernmentStrings[(int) settings.IntrinsicGovType]);
            }
            if (settings.Belligerence > 0)
            {
                writer.WriteLine("  belligerence       = {0}", settings.Belligerence);
            }
            writer.WriteLine("  capital            = {0} # {1}", settings.Capital,
                Scenarios.GetProvinceName(settings.Capital));
            if (settings.Dissent > 0)
            {
                writer.WriteLine("  dissent            = {0}", DoubleHelper.ToString(settings.Dissent));
            }
            if (settings.ExtraTc > 0)
            {
                writer.WriteLine("  extra_tc           = {0}", DoubleHelper.ToString(settings.ExtraTc));
            }
            writer.WriteLine("  manpower           = {0}", DoubleHelper.ToString(settings.Manpower));
            if (!DoubleHelper.IsZero(settings.RelativeManpower))
            {
                writer.WriteLine("  relative_manpower  = {0}", DoubleHelper.ToString(settings.RelativeManpower));
            }
            if (!DoubleHelper.IsZero(settings.GroundDefEff))
            {
                writer.WriteLine("  ground_def_eff     = {0}", DoubleHelper.ToString(settings.GroundDefEff));
            }
        }

        /// <summary>
        ///     AI設定を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAiSettings(CountrySettings settings, TextWriter writer)
        {
            writer.Write("{");
            foreach (KeyValuePair<string, string> pair in settings.AiSettings.Flags)
            {
                writer.Write(" {0} = {1}", pair.Key, pair.Value);
            }
            writer.Write(" }");
        }

        /// <summary>
        ///     資源設定を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryResources(CountrySettings settings, TextWriter writer)
        {
            writer.WriteLine("  # Resource Reserves");
            writer.WriteLine("  energy         = {0}", ObjectHelper.ToString(settings.Energy));
            writer.WriteLine("  metal          = {0}", ObjectHelper.ToString(settings.Metal));
            writer.WriteLine("  rare_materials = {0}", ObjectHelper.ToString(settings.RareMaterials));
            writer.WriteLine("  oil            = {0}", ObjectHelper.ToString(settings.Oil));
            writer.WriteLine("  supplies       = {0}", ObjectHelper.ToString(settings.Supplies));
            writer.WriteLine("  money          = {0}", ObjectHelper.ToString(settings.Money));
            if (settings.Transports != 0)
            {
                writer.WriteLine("  transports     = {0}", settings.Transports);
            }
            if (settings.Escorts != 0)
            {
                writer.WriteLine("  escorts        = {0}", settings.Escorts);
            }
        }

        /// <summary>
        ///     マップ外資源設定を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteOffmapResources(CountrySettings settings, TextWriter writer)
        {
            ResourceSettings offmap = settings.Offmap;
            if (offmap == null)
            {
                return;
            }
            writer.WriteLine("  free = {");
            if (!DoubleHelper.IsZero(offmap.Ic))
            {
                writer.WriteLine("    ic             = {0}", ObjectHelper.ToString(offmap.Ic));
            }
            if (!DoubleHelper.IsZero(offmap.Manpower))
            {
                writer.WriteLine("    manpower       = {0}", ObjectHelper.ToString(offmap.Manpower));
            }
            if (!DoubleHelper.IsZero(offmap.Energy))
            {
                writer.WriteLine("    energy         = {0}", ObjectHelper.ToString(offmap.Energy));
            }
            if (!DoubleHelper.IsZero(offmap.Metal))
            {
                writer.WriteLine("    metal          = {0}", ObjectHelper.ToString(offmap.Metal));
            }
            if (!DoubleHelper.IsZero(offmap.RareMaterials))
            {
                writer.WriteLine("    rare_materials = {0}", ObjectHelper.ToString(offmap.RareMaterials));
            }
            if (!DoubleHelper.IsZero(offmap.Oil))
            {
                writer.WriteLine("    oil            = {0}", ObjectHelper.ToString(offmap.Oil));
            }
            if (!DoubleHelper.IsZero(offmap.Supplies))
            {
                writer.WriteLine("    supplies       = {0}", ObjectHelper.ToString(offmap.Supplies));
            }
            if (!DoubleHelper.IsZero(offmap.Money))
            {
                writer.WriteLine("    money          = {0}", ObjectHelper.ToString(offmap.Money));
            }
            if (offmap.Transports != 0)
            {
                writer.WriteLine("    transports     = {0}", offmap.Transports);
            }
            if (offmap.Escorts != 0)
            {
                writer.WriteLine("    escorts        = {0}", offmap.Escorts);
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     外交設定を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDiplomacy(CountrySettings settings, TextWriter writer)
        {
            if (settings.Relations.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            writer.WriteLine("  diplomacy = {");
            foreach (Relation relation in settings.Relations)
            {
                WriteRelation(relation, writer);
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     国家関係を書き出す
        /// </summary>
        /// <param name="relation">国家関係</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteRelation(Relation relation, TextWriter writer)
        {
            if (relation.Guaranteed == null)
            {
                writer.Write("    relation = {{ tag = {0} value = {1}", Countries.Strings[(int) relation.Country],
                    ObjectHelper.ToString(relation.Value));
                if (relation.Access)
                {
                    writer.Write(" access = yes");
                }
                writer.WriteLine(" }");
            }
            else
            {
                writer.WriteLine("    relation = {");
                writer.WriteLine("      tag        = {0}", Countries.Strings[(int) relation.Country]);
                writer.WriteLine("      value      = {0}", ObjectHelper.ToString(relation.Value));
                if (relation.Access)
                {
                    writer.WriteLine("      access     = yes");
                }
                writer.Write("      guaranteed = ");
                WriteDate(relation.Guaranteed, writer);
                writer.WriteLine();
                writer.WriteLine("    }");
            }
        }

        /// <summary>
        ///     諜報設定リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteSpyInfos(CountrySettings settings, TextWriter writer)
        {
            foreach (SpySettings spy in settings.Intelligence)
            {
                writer.WriteLine("  SpyInfo                = {{ country = \"{0}\" NumberOfSpies = {1} }}",
                    Countries.Strings[(int) spy.Country], spy.Spies);
            }
        }

        /// <summary>
        ///     領土設定を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryTerritories(CountrySettings settings, TextWriter writer)
        {
            writer.WriteLine();
            writer.Write("  nationalprovinces      = {");
            WriteIdList(settings.NationalProvinces, writer);
            writer.WriteLine(" }");
            writer.Write("  ownedprovinces         = {");
            WriteIdList(settings.OwnedProvinces, writer);
            writer.WriteLine(" }");
            writer.Write("  controlledprovinces    = {");
            WriteIdList(settings.ControlledProvinces, writer);
            writer.WriteLine(" }");
            if (settings.ClaimedProvinces.Count > 0)
            {
                writer.Write("  claimedprovinces       = {");
                WriteIdList(settings.ClaimedProvinces, writer);
                writer.WriteLine(" }");
            }
        }

        /// <summary>
        ///     保有技術リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteTechApps(CountrySettings settings, TextWriter writer)
        {
            if (settings.TechApps.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            writer.Write("  techapps               = {");
            WriteIdList(settings.TechApps, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     青写真リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBlueprints(CountrySettings settings, TextWriter writer)
        {
            if (settings.BluePrints.Count == 0)
            {
                return;
            }
            writer.Write("  blueprints             = {");
            WriteIdList(settings.BluePrints, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     発明イベントリストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteInventions(CountrySettings settings, TextWriter writer)
        {
            if (settings.Inventions.Count == 0)
            {
                return;
            }
            writer.Write("  inventions             = {");
            WriteIdList(settings.Inventions, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     政策スライダーを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryPolicy(CountrySettings settings, TextWriter writer)
        {
            CountryPolicy policy = settings.Policy;
            if (policy == null)
            {
                return;
            }
            writer.WriteLine();
            writer.WriteLine("  policy = {");
            if (policy.Date != null)
            {
                writer.Write("    date              = ");
                WriteDate(policy.Date, writer);
                writer.WriteLine();
            }
            writer.WriteLine("    democratic        = {0}", policy.Democratic);
            writer.WriteLine("    political_left    = {0}", policy.PoliticalLeft);
            writer.WriteLine("    freedom           = {0}", policy.Freedom);
            writer.WriteLine("    free_market       = {0}", policy.FreeMarket);
            writer.WriteLine("    professional_army = {0}", policy.ProfessionalArmy);
            writer.WriteLine("    defense_lobby     = {0}", policy.DefenseLobby);
            writer.WriteLine("    interventionism   = {0}", policy.Interventionism);
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     国家補正値を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryModifiers(CountrySettings settings, TextWriter writer)
        {
            if (!DoubleHelper.IsZero(settings.PeacetimeIcModifier))
            {
                writer.WriteLine("  peacetime_ic_mod       = {0}", DoubleHelper.ToString(settings.PeacetimeIcModifier));
            }
            if (!DoubleHelper.IsZero(settings.WartimeIcModifier))
            {
                writer.WriteLine("  wartime_ic_mod         = {0}", DoubleHelper.ToString(settings.WartimeIcModifier));
            }
            if (!DoubleHelper.IsZero(settings.WartimeIcModifier))
            {
                writer.WriteLine("  industrial_modifier    = {0}", DoubleHelper.ToString(settings.WartimeIcModifier));
            }
        }

        /// <summary>
        ///     閣僚リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCabinet(CountrySettings settings, TextWriter writer)
        {
            if (settings.HeadOfState != null)
            {
                writer.Write("  headofstate            = ");
                WriteMinister(settings.HeadOfState, writer);
            }
            if (settings.HeadOfGovernment != null)
            {
                writer.Write("  headofgovernment       = ");
                WriteMinister(settings.HeadOfGovernment, writer);
            }
            if (settings.ForeignMinister != null)
            {
                writer.Write("  foreignminister        = ");
                WriteMinister(settings.ForeignMinister, writer);
            }
            if (settings.ArmamentMinister != null)
            {
                writer.Write("  armamentminister       = ");
                WriteMinister(settings.ArmamentMinister, writer);
            }
            if (settings.MinisterOfSecurity != null)
            {
                writer.Write("  ministerofsecurity     = ");
                WriteMinister(settings.MinisterOfSecurity, writer);
            }
            if (settings.MinisterOfIntelligence != null)
            {
                writer.Write("  ministerofintelligence = ");
                WriteMinister(settings.MinisterOfIntelligence, writer);
            }
            if (settings.ChiefOfStaff != null)
            {
                writer.Write("  chiefofstaff           = ");
                WriteMinister(settings.ChiefOfStaff, writer);
            }
            if (settings.ChiefOfArmy != null)
            {
                writer.Write("  chiefofarmy            = ");
                WriteMinister(settings.ChiefOfArmy, writer);
            }
            if (settings.ChiefOfNavy != null)
            {
                writer.Write("  chiefofnavy            = ");
                WriteMinister(settings.ChiefOfNavy, writer);
            }
            if (settings.ChiefOfAir != null)
            {
                writer.Write("  chiefofair             = ");
                WriteMinister(settings.ChiefOfAir, writer);
            }
        }

        /// <summary>
        ///     閣僚を書き出す
        /// </summary>
        /// <param name="typeId">閣僚のtypeとid</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteMinister(TypeId typeId, TextWriter writer)
        {
            WriteTypeId(typeId, writer);
            Minister minister = Ministers.Items.Find(m => m.Id == typeId.Id);
            if (minister != null)
            {
                writer.WriteLine(" # {0}", minister.Name);
            }
            else
            {
                writer.WriteLine();
            }
        }

        /// <summary>
        ///     国策を書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteIdea(CountrySettings settings, TextWriter writer)
        {
            // 国策をサポートするのはAoDのみ
            if (Game.Type != GameType.ArsenalOfDemocracy)
            {
                return;
            }

            if (!string.IsNullOrEmpty(settings.NationalIdentity))
            {
                writer.WriteLine("  nationalidentity       = \"{0}\"", settings.NationalIdentity);
            }
            if (!string.IsNullOrEmpty(settings.SocialPolicy))
            {
                writer.WriteLine("  socialpolicy           = \"{0}\"", settings.SocialPolicy);
            }
            if (!string.IsNullOrEmpty(settings.NationalCulture))
            {
                writer.WriteLine("  nationalculture        = \"{0}\"", settings.NationalCulture);
            }
        }

        /// <summary>
        ///     休止指揮官リストを書き出す (国家設定)
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryDormantLeaders(CountrySettings settings, TextWriter writer)
        {
            if (settings.DormantLeaders.Count == 0)
            {
                return;
            }
            writer.Write("  dormant_leaders        = {");
            WriteIdList(settings.DormantLeaders, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     休止閣僚リストを書き出す (国家設定)
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryDormantMinisters(CountrySettings settings, TextWriter writer)
        {
            if (settings.DormantMinisters.Count == 0)
            {
                return;
            }
            writer.Write("  dormant_ministers      = {");
            WriteIdList(settings.DormantMinisters, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     休止研究機関リストを書き出す (国家設定)
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryDormantTeams(CountrySettings settings, TextWriter writer)
        {
            if (settings.DormantTeams.Count == 0)
            {
                return;
            }
            writer.Write("  dormant_teams          = {");
            WriteIdList(settings.DormantTeams, writer);
            writer.WriteLine(" }");
        }

        /// <summary>
        ///     抽出指揮官リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteStealLeaders(CountrySettings settings, TextWriter writer)
        {
            if (settings.StealLeaders.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (int id in settings.StealLeaders)
            {
                writer.WriteLine("  steal_leader = {0}", id);
            }
        }

        #endregion

        #region ユニット

        /// <summary>
        ///     陸軍ユニットリストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteLandUnits(CountrySettings settings, TextWriter writer)
        {
            if (settings.LandUnits.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (LandUnit unit in settings.LandUnits)
            {
                WriteLandUnit(unit, writer);
            }
        }

        /// <summary>
        ///     海軍ユニットリストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteNavalUnits(CountrySettings settings, TextWriter writer)
        {
            if (settings.NavalUnits.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (NavalUnit unit in settings.NavalUnits)
            {
                WriteNavalUnit(unit, writer);
            }
        }

        /// <summary>
        ///     空軍ユニットリストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAirUnits(CountrySettings settings, TextWriter writer)
        {
            if (settings.AirUnits.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (AirUnit unit in settings.AirUnits)
            {
                WriteAirUnit(unit, writer);
            }
        }

        /// <summary>
        ///     陸軍ユニットを書き出す
        /// </summary>
        /// <param name="unit">ユニット</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteLandUnit(LandUnit unit, TextWriter writer)
        {
            writer.WriteLine("  landunit = {");
            if (unit.Id != null)
            {
                writer.Write("    id       = ");
                WriteTypeId(unit.Id, writer);
                writer.WriteLine();
            }
            if (!string.IsNullOrEmpty(unit.Name))
            {
                writer.WriteLine("    name     = \"{0}\"", unit.Name);
            }
            if (unit.Control != Country.None)
            {
                writer.WriteLine("    countrol = {0}", Countries.Strings[(int) unit.Control]);
            }
            if (unit.Leader > 0)
            {
                writer.WriteLine("    leader   = {0}", unit.Leader);
            }
            if (unit.Location > 0)
            {
                writer.WriteLine("    location = {0}", unit.Location);
            }
            foreach (LandDivision division in unit.Divisions)
            {
                WriteLandDivision(division, writer);
            }
            if (unit.DigIn > 0)
            {
                writer.WriteLine("    dig_in   = {0}", DoubleHelper.ToString3(unit.DigIn));
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     海軍ユニットを書き出す
        /// </summary>
        /// <param name="unit">ユニット</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteNavalUnit(NavalUnit unit, TextWriter writer)
        {
            writer.WriteLine("  navalunit = {");
            if (unit.Id != null)
            {
                writer.Write("    id       = ");
                WriteTypeId(unit.Id, writer);
                writer.WriteLine();
            }
            if (!string.IsNullOrEmpty(unit.Name))
            {
                writer.WriteLine("    name     = \"{0}\"", unit.Name);
            }
            if (unit.Control != Country.None)
            {
                writer.WriteLine("    countrol = {0}", Countries.Strings[(int) unit.Control]);
            }
            if (unit.Leader > 0)
            {
                writer.WriteLine("    leader   = {0}", unit.Leader);
            }
            if (unit.Location > 0)
            {
                writer.WriteLine("    location = {0}", unit.Location);
            }
            if (unit.Base > 0)
            {
                writer.WriteLine("    base     = {0}", unit.Base);
            }
            foreach (NavalDivision division in unit.Divisions)
            {
                WriteNavalDivision(division, writer);
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     空軍ユニットを書き出す
        /// </summary>
        /// <param name="unit">ユニット</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAirUnit(AirUnit unit, TextWriter writer)
        {
            writer.WriteLine("  airunit = { ");
            if (unit.Id != null)
            {
                writer.Write("    id       = ");
                WriteTypeId(unit.Id, writer);
                writer.WriteLine();
            }
            if (!string.IsNullOrEmpty(unit.Name))
            {
                writer.WriteLine("    name     = \"{0}\"", unit.Name);
            }
            if (unit.Control != Country.None)
            {
                writer.WriteLine("    countrol = {0}", Countries.Strings[(int) unit.Control]);
            }
            if (unit.Leader > 0)
            {
                writer.WriteLine("    leader   = {0}", unit.Leader);
            }
            if (unit.Location > 0)
            {
                writer.WriteLine("    location = {0}", unit.Location);
            }
            if (unit.Base > 0)
            {
                writer.WriteLine("    base     = {0}", unit.Base);
            }
            foreach (AirDivision division in unit.Divisions)
            {
                WriteAirDivision(division, writer);
            }
            writer.WriteLine("  }");
        }

        #endregion

        #region 師団

        /// <summary>
        ///     陸軍師団を書き出す
        /// </summary>
        /// <param name="division">師団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteLandDivision(LandDivision division, TextWriter writer)
        {
            writer.WriteLine("    division = {");
            writer.Write("      id             = ");
            WriteTypeId(division.Id, writer);
            writer.WriteLine();
            if (!string.IsNullOrEmpty(division.Name))
            {
                writer.WriteLine("      name           = \"{0}\"", division.Name);
            }
            writer.WriteLine("      type           = {0}", Units.Strings[(int) division.Type]);
            if (division.Model >= 0)
            {
                writer.WriteLine("      model          = {0}", division.Model);
            }
            if (division.Strength > 0)
            {
                writer.WriteLine("      strength       = {0}", division.Strength);
            }
            if (division.MaxStrength > 0)
            {
                writer.WriteLine("      max_strength   = {0}", division.MaxStrength);
            }
            if (division.Organisation > 0)
            {
                writer.WriteLine("      organisation   = {0}", division.Organisation);
            }
            if (division.Morale > 0)
            {
                writer.WriteLine("      morale         = {0}", division.Morale);
            }
            if (division.Experience > 0)
            {
                writer.WriteLine("      experience     = {0}", division.Experience);
            }
            if (division.Extra >= UnitType.None)
            {
                writer.WriteLine("      extra          = {0}", Units.Strings[(int) division.Extra]);
            }
            if (division.Extra1 >= UnitType.None)
            {
                writer.WriteLine("      extra1         = {0}", Units.Strings[(int) division.Extra1]);
            }
            if (division.Extra2 >= UnitType.None)
            {
                writer.WriteLine("      extra2         = {0}", Units.Strings[(int) division.Extra2]);
            }
            if (division.Extra3 >= UnitType.None)
            {
                writer.WriteLine("      extra3         = {0}", Units.Strings[(int) division.Extra3]);
            }
            if (division.Extra4 >= UnitType.None)
            {
                writer.WriteLine("      extra4         = {0}", Units.Strings[(int) division.Extra4]);
            }
            if (division.Extra5 >= UnitType.None)
            {
                writer.WriteLine("      extra5         = {0}", Units.Strings[(int) division.Extra5]);
            }
            if (division.Extra >= UnitType.None && division.BrigadeModel >= 0)
            {
                writer.WriteLine("      brigade_model  = {0}", division.BrigadeModel);
            }
            if (division.Extra1 >= UnitType.None && division.BrigadeModel1 >= 0)
            {
                writer.WriteLine("      brigade_model1 = {0}", division.BrigadeModel1);
            }
            if (division.Extra2 >= UnitType.None && division.BrigadeModel2 >= 0)
            {
                writer.WriteLine("      brigade_model2 = {0}", division.BrigadeModel2);
            }
            if (division.Extra3 >= UnitType.None && division.BrigadeModel3 >= 0)
            {
                writer.WriteLine("      brigade_model3 = {0}", division.BrigadeModel3);
            }
            if (division.Extra4 >= UnitType.None && division.BrigadeModel4 >= 0)
            {
                writer.WriteLine("      brigade_model4 = {0}", division.BrigadeModel4);
            }
            if (division.Extra5 >= UnitType.None && division.BrigadeModel5 >= 0)
            {
                writer.WriteLine("      brigade_model5 = {0}", division.BrigadeModel5);
            }
            if (division.Offensive != null)
            {
                writer.Write("      offensive = ");
                WriteDate(division.Offensive, writer);
                writer.WriteLine();
            }
            if (division.Dormant)
            {
                writer.WriteLine("      dormant        = yes");
            }
            if (division.Locked)
            {
                writer.WriteLine("      locked         = yes");
            }
            writer.WriteLine("    }");
        }

        /// <summary>
        ///     海軍師団を書き出す
        /// </summary>
        /// <param name="division">師団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteNavalDivision(NavalDivision division, TextWriter writer)
        {
            writer.WriteLine("    division = {");
            writer.Write("      id             = ");
            WriteTypeId(division.Id, writer);
            writer.WriteLine();
            if (!string.IsNullOrEmpty(division.Name))
            {
                writer.WriteLine("      name           = \"{0}\"", division.Name);
            }
            writer.WriteLine("      type           = {0}", Units.Strings[(int) division.Type]);
            if (division.Model >= 0)
            {
                writer.WriteLine("      model          = {0}", division.Model);
            }
            if (division.Nuke)
            {
                writer.WriteLine("      nuke           = yes");
            }
            if (division.Strength > 0)
            {
                writer.WriteLine("      strength       = {0}", division.Strength);
            }
            if (division.MaxStrength > 0)
            {
                writer.WriteLine("      max_strength   = {0}", division.MaxStrength);
            }
            if (division.Organisation > 0)
            {
                writer.WriteLine("      organisation   = {0}", division.Organisation);
            }
            if (division.Morale > 0)
            {
                writer.WriteLine("      morale         = {0}", division.Morale);
            }
            if (division.Experience > 0)
            {
                writer.WriteLine("      experience     = {0}", division.Experience);
            }
            if (division.MaxSpeed > 0)
            {
                writer.WriteLine("      maxspeed       = {0}", division.MaxSpeed);
            }
            if (division.SeaDefense > 0)
            {
                writer.WriteLine("      seadefence     = {0}", division.SeaDefense);
            }
            if (division.AirDefence > 0)
            {
                writer.WriteLine("      airdefence     = {0}", division.AirDefence);
            }
            if (division.SeaAttack > 0)
            {
                writer.WriteLine("      seaattack      = {0}", division.SeaAttack);
            }
            if (division.SubAttack > 0)
            {
                writer.WriteLine("      subattack      = {0}", division.SubAttack);
            }
            if (division.ConvoyAttack > 0)
            {
                writer.WriteLine("      convoyattack   = {0}", division.ConvoyAttack);
            }
            if (division.ShoreBombardment > 0)
            {
                writer.WriteLine("      shorebombardment = {0}", division.ShoreBombardment);
            }
            if (division.AirAttack > 0)
            {
                writer.WriteLine("      airattack      = {0}", division.AirAttack);
            }
            if (division.Distance > 0)
            {
                writer.WriteLine("      distance       = {0}", division.Distance);
            }
            if (division.Visibility > 0)
            {
                writer.WriteLine("      visibility     = {0}", division.Visibility);
            }
            if (division.SurfaceDetection > 0)
            {
                writer.WriteLine("      surfacedetectioncapability = {0}", division.SurfaceDetection);
            }
            if (division.SubDetection > 0)
            {
                writer.WriteLine("      subdetectioncapability = {0}", division.SubDetection);
            }
            if (division.AirDetection > 0)
            {
                writer.WriteLine("      airdetectioncapability = {0}", division.AirDetection);
            }
            if (division.Extra >= UnitType.None)
            {
                writer.WriteLine("      extra          = {0}", Units.Strings[(int) division.Extra]);
            }
            if (division.Extra1 >= UnitType.None)
            {
                writer.WriteLine("      extra1         = {0}", Units.Strings[(int) division.Extra1]);
            }
            if (division.Extra2 >= UnitType.None)
            {
                writer.WriteLine("      extra2         = {0}", Units.Strings[(int) division.Extra2]);
            }
            if (division.Extra3 >= UnitType.None)
            {
                writer.WriteLine("      extra3         = {0}", Units.Strings[(int) division.Extra3]);
            }
            if (division.Extra4 >= UnitType.None)
            {
                writer.WriteLine("      extra4         = {0}", Units.Strings[(int) division.Extra4]);
            }
            if (division.Extra5 >= UnitType.None)
            {
                writer.WriteLine("      extra5         = {0}", Units.Strings[(int) division.Extra5]);
            }
            if (division.Extra >= UnitType.None && division.BrigadeModel >= 0)
            {
                writer.WriteLine("      brigade_model  = {0}", division.BrigadeModel);
            }
            if (division.Extra1 >= UnitType.None && division.BrigadeModel1 >= 0)
            {
                writer.WriteLine("      brigade_model1 = {0}", division.BrigadeModel1);
            }
            if (division.Extra2 >= UnitType.None && division.BrigadeModel2 >= 0)
            {
                writer.WriteLine("      brigade_model2 = {0}", division.BrigadeModel2);
            }
            if (division.Extra3 >= UnitType.None && division.BrigadeModel3 >= 0)
            {
                writer.WriteLine("      brigade_model3 = {0}", division.BrigadeModel3);
            }
            if (division.Extra4 >= UnitType.None && division.BrigadeModel4 >= 0)
            {
                writer.WriteLine("      brigade_model4 = {0}", division.BrigadeModel4);
            }
            if (division.Extra5 >= UnitType.None && division.BrigadeModel5 >= 0)
            {
                writer.WriteLine("      brigade_model5 = {0}", division.BrigadeModel5);
            }
            if (division.Dormant)
            {
                writer.WriteLine("      dormant        = yes");
            }
            writer.WriteLine("    }");
        }

        /// <summary>
        ///     空軍師団を書き出す
        /// </summary>
        /// <param name="division">師団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteAirDivision(AirDivision division, TextWriter writer)
        {
            writer.WriteLine("    division = {");
            writer.Write("      id             = ");
            WriteTypeId(division.Id, writer);
            writer.WriteLine();
            if (!string.IsNullOrEmpty(division.Name))
            {
                writer.WriteLine("      name           = \"{0}\"", division.Name);
            }
            writer.WriteLine("      type           = {0}", Units.Strings[(int) division.Type]);
            if (division.Model >= 0)
            {
                writer.WriteLine("      model          = {0}", division.Model);
            }
            if (division.Nuke)
            {
                writer.WriteLine("      nuke           = yes");
            }
            if (division.Strength > 0)
            {
                writer.WriteLine("      strength       = {0}", division.Strength);
            }
            if (division.MaxStrength > 0)
            {
                writer.WriteLine("      max_strength   = {0}", division.MaxStrength);
            }
            if (division.Organisation > 0)
            {
                writer.WriteLine("      organisation   = {0}", division.Organisation);
            }
            if (division.Morale > 0)
            {
                writer.WriteLine("      morale         = {0}", division.Morale);
            }
            if (division.Experience > 0)
            {
                writer.WriteLine("      experience     = {0}", division.Experience);
            }
            if (division.Extra >= UnitType.None)
            {
                writer.WriteLine("      extra          = {0}", Units.Strings[(int) division.Extra]);
            }
            if (division.Extra1 >= UnitType.None)
            {
                writer.WriteLine("      extra1         = {0}", Units.Strings[(int) division.Extra1]);
            }
            if (division.Extra2 >= UnitType.None)
            {
                writer.WriteLine("      extra2         = {0}", Units.Strings[(int) division.Extra2]);
            }
            if (division.Extra3 >= UnitType.None)
            {
                writer.WriteLine("      extra3         = {0}", Units.Strings[(int) division.Extra3]);
            }
            if (division.Extra4 >= UnitType.None)
            {
                writer.WriteLine("      extra4         = {0}", Units.Strings[(int) division.Extra4]);
            }
            if (division.Extra5 >= UnitType.None)
            {
                writer.WriteLine("      extra5         = {0}", Units.Strings[(int) division.Extra5]);
            }
            if (division.Extra >= UnitType.None && division.BrigadeModel >= 0)
            {
                writer.WriteLine("      brigade_model  = {0}", division.BrigadeModel);
            }
            if (division.Extra1 >= UnitType.None && division.BrigadeModel1 >= 0)
            {
                writer.WriteLine("      brigade_model1 = {0}", division.BrigadeModel1);
            }
            if (division.Extra2 >= UnitType.None && division.BrigadeModel2 >= 0)
            {
                writer.WriteLine("      brigade_model2 = {0}", division.BrigadeModel2);
            }
            if (division.Extra3 >= UnitType.None && division.BrigadeModel3 >= 0)
            {
                writer.WriteLine("      brigade_model3 = {0}", division.BrigadeModel3);
            }
            if (division.Extra4 >= UnitType.None && division.BrigadeModel4 >= 0)
            {
                writer.WriteLine("      brigade_model4 = {0}", division.BrigadeModel4);
            }
            if (division.Extra5 >= UnitType.None && division.BrigadeModel5 >= 0)
            {
                writer.WriteLine("      brigade_model5 = {0}", division.BrigadeModel5);
            }
            if (division.Dormant)
            {
                writer.WriteLine("      dormant        = yes");
            }
            writer.WriteLine("    }");
        }

        /// <summary>
        ///     生産中師団リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDivisionDevelopments(CountrySettings settings, TextWriter writer)
        {
            if (settings.DivisionDevelopments.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (DivisionDevelopment division in settings.DivisionDevelopments)
            {
                WriteDivisionDevelopment(division, writer);
            }
        }

        /// <summary>
        ///     生産中師団を書き出す
        /// </summary>
        /// <param name="division">師団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDivisionDevelopment(DivisionDevelopment division, TextWriter writer)
        {
            writer.WriteLine("  division_development = {");
            writer.Write("    id            = ");
            WriteTypeId(division.Id, writer);
            writer.WriteLine();
            if (!string.IsNullOrEmpty(division.Name))
            {
                writer.WriteLine("    name           = \"{0}\"", division.Name);
            }
            writer.WriteLine("    type           = {0}", Units.Strings[(int) division.Type]);
            if (division.Model >= 0)
            {
                writer.WriteLine("    model          = {0}", division.Model);
            }
            if (division.Cost > 0)
            {
                writer.WriteLine("    cost           = {0}", DoubleHelper.ToString(division.Cost));
            }
            if (!division.UnitCost)
            {
                writer.WriteLine("    unitcost       = no");
            }
            if (!division.NewModel)
            {
                writer.WriteLine("    newmodel       = no");
            }
            if (division.Date != null)
            {
                writer.Write("    date           = ");
                WriteDate(division.Date, writer);
                writer.WriteLine();
            }
            if (division.Manpower > 0)
            {
                writer.WriteLine("    manpower       = {0}", DoubleHelper.ToString(division.Manpower));
            }
            if (Game.Type == GameType.ArsenalOfDemocracy)
            {
                if (division.Halted)
                {
                    writer.WriteLine("    halted         = yes");
                }
                writer.WriteLine("    close_when_finished = {0}", BoolHelper.ToString(division.CloseWhenFinished));
                writer.WriteLine("    waitingforclosure = {0}", BoolHelper.ToString(division.WaitingForClosure));
            }
            if (division.TotalProgress > 0)
            {
                writer.WriteLine("    total_progress = {0}", DoubleHelper.ToString4(division.Progress));
            }
            if (division.Size > 0)
            {
                writer.WriteLine("    size           = {0}", division.Size);
            }
            if (division.Done > 0)
            {
                writer.WriteLine("    done           = {0}", division.Done);
            }
            if (division.Days > 0)
            {
                writer.WriteLine("    days           = {0}", division.Days);
            }
            if (division.DaysForFirst > 0)
            {
                writer.WriteLine("    days_for_first = {0}", division.DaysForFirst);
            }
            if (division.GearingBonus > 0)
            {
                writer.WriteLine("    gearing_bonus  = {0}", DoubleHelper.ToString4(division.GearingBonus));
            }
            if (division.Extra >= UnitType.None)
            {
                writer.WriteLine("    extra          = {0}", Units.Strings[(int) division.Extra]);
            }
            if (division.Extra1 >= UnitType.None)
            {
                writer.WriteLine("    extra1         = {0}", Units.Strings[(int) division.Extra1]);
            }
            if (division.Extra2 >= UnitType.None)
            {
                writer.WriteLine("    extra2         = {0}", Units.Strings[(int) division.Extra2]);
            }
            if (division.Extra3 >= UnitType.None)
            {
                writer.WriteLine("    extra3         = {0}", Units.Strings[(int) division.Extra3]);
            }
            if (division.Extra4 >= UnitType.None)
            {
                writer.WriteLine("    extra4         = {0}", Units.Strings[(int) division.Extra4]);
            }
            if (division.Extra5 >= UnitType.None)
            {
                writer.WriteLine("    extra5         = {0}", Units.Strings[(int) division.Extra5]);
            }
            if (division.Extra >= UnitType.None && division.BrigadeModel >= 0)
            {
                writer.WriteLine("    brigade_model  = {0}", division.BrigadeModel);
            }
            if (division.Extra1 >= UnitType.None && division.BrigadeModel1 >= 0)
            {
                writer.WriteLine("    brigade_model1 = {0}", division.BrigadeModel1);
            }
            if (division.Extra2 >= UnitType.None && division.BrigadeModel2 >= 0)
            {
                writer.WriteLine("    brigade_model2 = {0}", division.BrigadeModel2);
            }
            if (division.Extra3 >= UnitType.None && division.BrigadeModel3 >= 0)
            {
                writer.WriteLine("    brigade_model3 = {0}", division.BrigadeModel3);
            }
            if (division.Extra4 >= UnitType.None && division.BrigadeModel4 >= 0)
            {
                writer.WriteLine("    brigade_model4 = {0}", division.BrigadeModel4);
            }
            if (division.Extra5 >= UnitType.None && division.BrigadeModel5 >= 0)
            {
                writer.WriteLine("    brigade_model5 = {0}", division.BrigadeModel5);
            }
            writer.WriteLine("  }");
        }

        /// <summary>
        ///     休止中陸軍師団リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDormantLandDivisions(CountrySettings settings, TextWriter writer)
        {
            if (settings.LandDivisions.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (LandDivision division in settings.LandDivisions)
            {
                WriteDormantLandDivision(division, writer);
            }
        }

        /// <summary>
        ///     休止中陸軍師団を書き出す
        /// </summary>
        /// <param name="division">師団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDormantLandDivision(LandDivision division, TextWriter writer)
        {
            writer.Write("  landdivision = {");
            if (division.Dormant)
            {
                writer.Write(" dormant = yes");
            }
            if (division.Id != null)
            {
                writer.Write(" id = ");
                WriteTypeId(division.Id, writer);
            }
            if (!string.IsNullOrEmpty(division.Name))
            {
                writer.Write(" name = \"{0}\" ", division.Name);
            }
            writer.Write(" type = {0}", Units.Strings[(int) division.Type]);
            if (division.Model >= 0)
            {
                writer.Write(" model = {0}", division.Model);
            }
            if (division.Strength > 0)
            {
                writer.Write(" strength = {0}", division.Strength);
            }
            if (division.MaxStrength > 0)
            {
                writer.Write(" max_strength = {0}", division.MaxStrength);
            }
            if (division.Organisation > 0)
            {
                writer.Write(" organisation = {0}", division.Organisation);
            }
            if (division.Morale > 0)
            {
                writer.Write(" morale = {0}", division.Morale);
            }
            if (division.Experience > 0)
            {
                writer.Write(" experience = {0}", division.Experience);
            }
            if (division.Extra >= UnitType.None)
            {
                writer.Write(" extra = {0}", Units.Strings[(int) division.Extra]);
            }
            if (division.Extra1 >= UnitType.None)
            {
                writer.Write(" extra1 = {0}", Units.Strings[(int) division.Extra1]);
            }
            if (division.Extra2 >= UnitType.None)
            {
                writer.Write(" extra2 = {0}", Units.Strings[(int) division.Extra2]);
            }
            if (division.Extra3 >= UnitType.None)
            {
                writer.Write(" extra3 = {0}", Units.Strings[(int) division.Extra3]);
            }
            if (division.Extra4 >= UnitType.None)
            {
                writer.Write(" extra4 = {0}", Units.Strings[(int) division.Extra4]);
            }
            if (division.Extra5 >= UnitType.None)
            {
                writer.Write(" extra5 = {0}", Units.Strings[(int) division.Extra5]);
            }
            if (division.Extra >= UnitType.None && division.BrigadeModel >= 0)
            {
                writer.Write(" brigade_model = {0}", division.BrigadeModel);
            }
            if (division.Extra1 >= UnitType.None && division.BrigadeModel1 >= 0)
            {
                writer.Write(" brigade_model1 = {0}", division.BrigadeModel1);
            }
            if (division.Extra2 >= UnitType.None && division.BrigadeModel2 >= 0)
            {
                writer.Write(" brigade_model2 = {0}", division.BrigadeModel2);
            }
            if (division.Extra3 >= UnitType.None && division.BrigadeModel3 >= 0)
            {
                writer.Write(" brigade_model3 = {0}", division.BrigadeModel3);
            }
            if (division.Extra4 >= UnitType.None && division.BrigadeModel4 >= 0)
            {
                writer.Write(" brigade_model4 = {0}", division.BrigadeModel4);
            }
            if (division.Extra5 >= UnitType.None && division.BrigadeModel5 >= 0)
            {
                writer.Write(" brigade_model5 = {0}", division.BrigadeModel5);
            }
            if (division.Locked)
            {
                writer.Write(" locked = yes");
            }
            writer.WriteLine(" } ");
        }

        #endregion

        #region 輸送船団

        /// <summary>
        ///     生産中輸送船団リストを書き出す
        /// </summary>
        /// <param name="settings">国家設定</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteConvoyDevelopments(CountrySettings settings, TextWriter writer)
        {
            if (settings.ConvoyDevelopments.Count == 0)
            {
                return;
            }
            writer.WriteLine();
            foreach (ConvoyDevelopment convoy in settings.ConvoyDevelopments)
            {
                WriteConvoyDevelopment(convoy, writer);
            }
        }

        /// <summary>
        ///     生産中輸送船団を書き出す
        /// </summary>
        /// <param name="convoy">生産中輸送船団</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteConvoyDevelopment(ConvoyDevelopment convoy, TextWriter writer)
        {
            writer.WriteLine("  convoy_development = {");
            if (convoy.Id != null)
            {
                writer.Write("    id             = ");
                WriteTypeId(convoy.Id, writer);
                writer.WriteLine();
            }
            if (convoy.Name != null)
            {
                writer.WriteLine("    name           = \"{0}\"", convoy.Name);
            }
            if (convoy.Progress > 0)
            {
                writer.WriteLine("    progress       = {0}", DoubleHelper.ToString4(convoy.Progress));
            }
            if (convoy.Location > 0)
            {
                writer.WriteLine("    location       = {0}", convoy.Location);
            }
            if (convoy.Cost > 0)
            {
                writer.WriteLine("    cost           = {0}", DoubleHelper.ToString4(convoy.Cost));
            }
            if (convoy.Date != null)
            {
                writer.Write("    date           = ");
                WriteDate(convoy.Date, writer);
                writer.WriteLine();
            }
            if (convoy.Manpower > 0)
            {
                writer.WriteLine("    manpower       = {0}", DoubleHelper.ToString4(convoy.Manpower));
            }
            if (Game.Type == GameType.ArsenalOfDemocracy)
            {
                if (convoy.Halted)
                {
                    writer.WriteLine("    halted         = yes");
                }
                writer.WriteLine("    close_when_finished = {0}", BoolHelper.ToString(convoy.CloseWhenFinished));
                writer.WriteLine("    waitingforclosure = {0}", BoolHelper.ToString(convoy.WaitingForClosure));
            }
            if (convoy.TotalProgress > 0)
            {
                writer.WriteLine("    total_progress = {0}", DoubleHelper.ToString4(convoy.Progress));
            }
            if (convoy.Size > 0)
            {
                writer.WriteLine("    size           = {0}", convoy.Size);
            }
            if (convoy.Done > 0)
            {
                writer.WriteLine("    done           = {0}", convoy.Done);
            }
            if (convoy.Days > 0)
            {
                writer.WriteLine("    days           = {0}", convoy.Days);
            }
            if (convoy.DaysForFirst > 0)
            {
                writer.WriteLine("    days_for_first = {0}", convoy.DaysForFirst);
            }
            if (convoy.GearingBonus > 0)
            {
                writer.WriteLine("    gearing_bonus  = {0}", DoubleHelper.ToString4(convoy.GearingBonus));
            }
            writer.WriteLine("    type           = {0}", Scenarios.ConvoyStrings[(int) convoy.Type]);
            writer.WriteLine("  }");
        }

        #endregion

        #region 汎用

        /// <summary>
        ///     国家リストを書き出す
        /// </summary>
        /// <param name="countries">国家リスト</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteCountryList(IEnumerable<Country> countries, TextWriter writer)
        {
            foreach (Country country in countries)
            {
                writer.Write(" {0}", Countries.Strings[(int) country]);
            }
        }

        /// <summary>
        ///     IDリストを書き出す
        /// </summary>
        /// <param name="ids">IDリスト</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteIdList(IEnumerable<int> ids, TextWriter writer)
        {
            foreach (int id in ids)
            {
                writer.Write(" {0}", id);
            }
        }

        /// <summary>
        ///     建物のサイズを書き出す
        /// </summary>
        /// <param name="building">建物のサイズ</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteBuilding(BuildingSize building, TextWriter writer)
        {
            if (DoubleHelper.IsZero(building.Size))
            {
                writer.Write("{{ size = {0} current_size = {1} }}", ObjectHelper.ToString(building.MaxSize),
                    ObjectHelper.ToString(building.CurrentSize));
            }
            else
            {
                writer.Write(ObjectHelper.ToString(building.Size));
            }
        }

        /// <summary>
        ///     日時を書き出す
        /// </summary>
        /// <param name="date">日時</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteDate(GameDate date, TextWriter writer)
        {
            if (date.Hour == 0)
            {
                writer.Write("{{ year = {0} month = {1} day = {2} }}", date.Year, Scenarios.MonthStrings[date.Month - 1],
                    date.Day - 1);
            }
            else
            {
                writer.Write("{{ year = {0} month = {1} day = {2} hour = {3} }}", date.Year,
                    Scenarios.MonthStrings[date.Month - 1], date.Day - 1, date.Hour);
            }
        }

        /// <summary>
        ///     typeとidの組を書き出す
        /// </summary>
        /// <param name="id">typeとidの組</param>
        /// <param name="writer">ファイル書き込み用</param>
        private static void WriteTypeId(TypeId id, TextWriter writer)
        {
            writer.Write("{{ type = {0} id = {1} }}", id.Type, id.Id);
        }

        #endregion
    }
}