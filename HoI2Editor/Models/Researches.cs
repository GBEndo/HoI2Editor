﻿using System.Collections.Generic;

namespace HoI2Editor.Models
{
    /// <summary>
    ///     研究速度データ群
    /// </summary>
    public static class Researches
    {
        #region 公開プロパティ

        /// <summary>
        ///     研究速度リスト
        /// </summary>
        public static readonly List<Research> Items = new List<Research>();

        /// <summary>
        ///     研究速度計算時の基準年度
        /// </summary>
        public static ResearchDateMode DateMode { get; set; }

        /// <summary>
        ///     指定日付
        /// </summary>
        public static GameDate SpecifiedDate { get; set; }

        /// <summary>
        ///     ロケット試験場の規模
        /// </summary>
        public static int RocketTestingSites { get; set; }

        /// <summary>
        ///     原子炉の規模
        /// </summary>
        public static int NuclearReactors { get; set; }

        /// <summary>
        ///     青写真の有無
        /// </summary>
        public static bool Blueprint { get; set; }

        /// <summary>
        ///     研究速度補正
        /// </summary>
        public static double Modifier { get; set; }

        /// <summary>
        ///     研究機関の開始年の考慮
        /// </summary>
        public static bool ConsiderStartYear { get; set; }

        #endregion

        #region 初期化

        /// <summary>
        ///     静的コンストラクタ
        /// </summary>
        static Researches()
        {
            SpecifiedDate = new GameDate();
            Modifier = 1;
        }

        #endregion

        #region 研究速度リスト

        /// <summary>
        ///     研究速度リストを更新する
        /// </summary>
        /// <param name="tech">対象技術</param>
        /// <param name="teams">研究機関</param>
        public static void UpdateResearchList(TechItem tech, IEnumerable<Team> teams)
        {
            Items.Clear();

            // 研究速度を順に登録する
            foreach (Team team in teams)
            {
                Research research = new Research(tech, team);

                // 研究機関の開始年・終了年を考慮する場合
                if( ConsiderStartYear )
                {
                    GameDate date;    
                    if( DateMode == ResearchDateMode.Specified )
                    {
                        date = SpecifiedDate;
                    } else
                    {
                        date = new GameDate(tech.Year);
                    }
                    // 研究機関が終了年を過ぎている
                    if ( team.EndYear <= date.Year )
                    {
                        continue;   /* リストに入れない */
                    }
                }

                Items.Add(research);
            }

            // 研究日数の順にソートする
            Items.Sort((research1, research2) => research1.Days - research2.Days);
        }

        #endregion
    }

    /// <summary>
    ///     研究速度計算時の基準日付モード
    /// </summary>
    public enum ResearchDateMode
    {
        Historical, // 史実年度を使用する
        Specified // 指定日付を使用する
    }
}